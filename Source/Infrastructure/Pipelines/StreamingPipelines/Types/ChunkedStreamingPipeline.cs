using Core.Pipelines.Interfaces;
using Core.Pipelines.Models;
using Core.Shared.Abstractions;
using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Threading.Channels;

namespace StreamingPipelines.Types
{
    /// <summary>
    /// StreamingPipeline, транскрибирующий чанки аудиоданных одной длины
    /// </summary>
    public sealed class ChunkedStreamingPipeline : IStreamingPipeline
    {
        private readonly ILogger<ChunkedStreamingPipeline> _logger;
        private readonly SegmentationContext _segmentationContext;

        private readonly CancellationTokenSource _tokenSource;
        private readonly Channel<TranscribedSpeech> _workerOutputChannel;
        private readonly Channel<byte[]> _workerInputChannel;
        private readonly Task _worker;


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="engine">Основной метод транскрипции</param>
        /// <param name="source">Аудиопоток для записи аудио</param>
        /// <param name="speech">Аудиопоток для записи речи</param>
        /// <param name="buffer"></param>
        public ChunkedStreamingPipeline
        (
            ILogger<ChunkedStreamingPipeline> logger,
            IStreamingPipelineEngine engine,
            AudioStream source,
            AudioStream speech,
            AudioStream buffer,
            int analyzingFrameChunkSize
        )
        {
            _logger = logger;

            _segmentationContext = new SegmentationContext();

            _workerOutputChannel = Channel.CreateUnbounded<TranscribedSpeech>();
            _workerInputChannel = Channel.CreateUnbounded<byte[]>();
            _tokenSource = new CancellationTokenSource();

            _worker = new Task
            (
                async () => await Worker(_logger, (engine, _segmentationContext), (source, speech, buffer), _workerOutputChannel.Writer, _workerInputChannel.Reader, analyzingFrameChunkSize, _tokenSource.Token)
            );
        }

        public IAsyncEnumerable<TranscribedSpeech> ReadTranscribedSpeechAsync(CancellationToken token)
            => _workerOutputChannel.Reader.ReadAllAsync(token);

        public async Task WriteAudioDataAsync(byte[] data, CancellationToken token)
        {
            _logger.LogInformation($"Запись аудиосегмента в канал, размер - {data.Length} байт");

            await _workerInputChannel.Writer.WriteAsync(data, token);
        }

        public Task StopAsync()
        {
            _logger.LogInformation($"Отправлен сигнал окончания поступающих данных");

            _workerInputChannel.Writer.Complete();

            return Task.CompletedTask;
        }

        public Task StartAsync()
        {
            _logger.LogInformation($"Запущен поток для выполнения транскрибции");

            _worker.Start();

            return Task.CompletedTask;
        }

        public async Task CancelAsync()
        {
            if (!_tokenSource.IsCancellationRequested)
            {
                _logger.LogInformation($"Транскрибирование отменено");
                
                _workerInputChannel.Writer.Complete();

                await _tokenSource.CancelAsync();
            }
        }

        public void Dispose()
        {
            _tokenSource.Dispose();
        }

        #region Worker

        /// <summary>
        /// Тело работы потока
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="transcriber"></param>
        /// <param name="streams"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <param name="analyzingFrameCount"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private static async Task Worker
        (
            ILogger<ChunkedStreamingPipeline> logger,
            (IStreamingPipelineEngine Engine, SegmentationContext Context) transcriber,
            (AudioStream Source, AudioStream Speech, AudioStream Buffer) streams,
            ChannelWriter<TranscribedSpeech> output,
            ChannelReader<byte[]> input,
            int analyzingFrameCount,
            CancellationToken token
        )
        {
            var source = streams.Source;
            var buffer = streams.Buffer;
            var speech = streams.Speech;

            int transcribedPosition = 0;
            int readed = 0;

            int analyzingChunkSize = analyzingFrameCount * streams.Source.Format.BytesPerFrame;

            byte[] framesData = ArrayPool<byte>.Shared.Rent(analyzingChunkSize);

            try
            {
                await foreach (var audioBuffer in input.ReadAllAsync(token))
                {
                    await source.WriteAsync(audioBuffer, 0, audioBuffer.Length, token);

                    if (source.Length - transcribedPosition < analyzingChunkSize)
                    {
                        logger.LogInformation($"Недостаточно данных для обработки - {source.Length - transcribedPosition}/{analyzingChunkSize}");

                        continue;
                    }
                    
                    source.Position = transcribedPosition;
                    buffer.Clear();

                    readed = await source.ReadAsync(framesData, 0, analyzingChunkSize, token);

                    transcribedPosition += readed;

                    await buffer.WriteAsync(framesData, 0, readed, token);

                    await HandleAudioSegmentAsync
                    (
                        logger,
                        output,
                        transcriber.Engine,
                        transcriber.Context,
                        buffer,
                        speech,
                        token
                    );

                    source.Seek(0, SeekOrigin.End);
                }

                source.Position = transcribedPosition;

                // Довыполняем транскрипцию в случае если имеются необработанные данные

                while ((readed = await source.ReadAsync(framesData, 0, analyzingChunkSize, token)) != 0)
                {
                    await buffer.WriteAsync(framesData, 0, readed, token);

                    await HandleAudioSegmentAsync
                    (
                        logger,
                        output,
                        transcriber.Engine,
                        transcriber.Context,
                        buffer,
                        speech,
                        token
                    );

                    buffer.Clear();
                }

                if (speech.Length != 0)
                {
                    speech.Seek(0, SeekOrigin.Begin);

                    transcriber.Context.TotalTime += speech.TotalTime;
                    transcriber.Context.HasVoiceActivity = false;

                    await HandleSpeechAsync(logger, output, transcriber.Engine, transcriber.Context, speech, token);
                }

                logger.LogInformation($"Транскрипция завершена");

                output.Complete();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Возникла ошибка при транскрибировании");

                throw;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(framesData);

                source.Dispose();
                speech.Dispose();
                buffer.Dispose();
            }
        }

        private static async Task HandleAudioSegmentAsync
        (
            ILogger<ChunkedStreamingPipeline> logger,
            ChannelWriter<TranscribedSpeech> output,
            IStreamingPipelineEngine engine,
            SegmentationContext context,
            AudioStream buffer,
            AudioStream speech,
            CancellationToken token
        )
        {
            await engine.ProcessAudioAsync(buffer, token);

            switch (await engine.SegmentationAsync(context, buffer, speech, token))
            {
                case SegmentationState.Started:
                    buffer.Seek(0, SeekOrigin.Begin);
                    await buffer.CopyToAsync(speech, token);
                    break;

                case SegmentationState.Ended:
                    speech.Seek(0, SeekOrigin.Begin);
                    
                    await HandleSpeechAsync
                    (
                        logger,
                        output,
                        engine,
                        context,
                        speech,
                        token
                    );

                    speech.Clear();

                    break;

                case SegmentationState.Processing:
                    buffer.Seek(0, SeekOrigin.Begin);
                    await buffer.CopyToAsync(speech, token);
                    break;
            }
        }

        private static async Task HandleSpeechAsync
        (
            ILogger<ChunkedStreamingPipeline> logger,
            ChannelWriter<TranscribedSpeech> output,
            IStreamingPipelineEngine engine,
            SegmentationContext context,
            AudioStream speech,
            CancellationToken token
        )
        {
            var transcribedSpeech = await engine.TranscribeAsync(context, speech, token);

            logger.LogInformation($"Транскрибирована речь [{transcribedSpeech.Start} -> {transcribedSpeech.End}]: {transcribedSpeech.Text}");

            await output.WriteAsync(transcribedSpeech, token);
        }

        #endregion
    }
}
