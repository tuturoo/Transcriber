using System;
using System.Buffers;
using System.Net.WebSockets;
using Api.Watson.Extensions;
using Api.Watson.Models.Requests;
using Api.Watson.Models.Responses;
using Core.Pipelines.Interfaces;
using Core.Shared.Interfaces;
using Core.Shared.Models;

namespace Api.Watson.Handlers
{
    public sealed class TranscriptionWebSocketHandler
    {
        private const int DefaultBufferSize = 4_194_304; // 4 МБ

        private readonly ILogger<TranscriptionWebSocketHandler> _logger;
        private readonly IFactory<IStreamingPipeline> _pipelineFactory;

        public TranscriptionWebSocketHandler(ILogger<TranscriptionWebSocketHandler> logger, IFactory<IStreamingPipeline> factory)
        {
            _logger = logger;
            _pipelineFactory = factory;
        }

        public async Task HandleWebsocketAsync(WebSocket webSocket, CancellationToken token)
        {
            var inputFormat = new AudioFormat(default!, default!, default!, default!);

            await HandshakeAsync(webSocket, toUpdate: inputFormat, token);
        
            // Проверка на interimresult

            using (var pipeline = await _pipelineFactory.CreateAsync(inputFormat, token))
            {
                await pipeline.StartAsync();

                await Task.WhenAll
                (
                    ReaderWorker(webSocket, pipeline, token), 
                    InterimResultsWriterWorker(webSocket, pipeline, token)
                );
            }
        }

        private async Task HandshakeAsync(WebSocket webSocket, AudioFormat toUpdate, CancellationToken token)
        {
            await webSocket.SendJsonAsync(StateResponse.Listening, true, token);

            byte[] buffer = ArrayPool<byte>.Shared.Rent(DefaultBufferSize);

            try
            {
                var message = await webSocket.ReceiveAsync(buffer, token);
            
                var startRecognize = message.ReadJson<StartRecognize>(buffer, token);

                // Из content type можно все вытянуть, но для клауда берем как AUDIO/l16 (aiff)

                toUpdate.Type = "raw";
                toUpdate.BitsPerFrame = 16;
                toUpdate.Channels = 1;

                if (!startRecognize.TryGetRateFromContentType(out toUpdate.SamplingFrequency))
                    throw new ArgumentException($"Не удалось вытащить значение частоты дискретизации {startRecognize.ContentType}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при первичном обмене информацией");
            
                await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, $"{ex.Message}", token);

                throw;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private async Task ReaderWorker(WebSocket webSocket, IStreamingPipeline pipeline, CancellationToken token)
        {
            while (webSocket.State == WebSocketState.Open)
            {
                byte[] buffer = ArrayPool<byte>.Shared.Rent(DefaultBufferSize);

                try
                {
                    var message = await webSocket.ReceiveAsync(buffer, token);

                    switch (message.MessageType)
                    {
                        // Вторым текстовым пакетом ожидается Action=stop
                        case WebSocketMessageType.Text:
                            _logger.LogInformation($"Получен текстовый пакет размером {message.Count}");

                            await pipeline.StopAsync();
                            return;

                        case WebSocketMessageType.Binary:
                            _logger.LogInformation($"Получен бинарный пакет размером {message.Count}");

                            var toSend = buffer.AsSpan(0, message.Count).ToArray();
                            await pipeline.WriteAudioDataAsync(toSend, token);
                            break;

                        case WebSocketMessageType.Close:
                            await pipeline.StopAsync();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Возникла ошибка при получении данных");

                    await pipeline.StopAsync();
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        private async Task InterimResultsWriterWorker(WebSocket webSocket, IStreamingPipeline streamingPipeline, CancellationToken token)
        {
            int totalSended = 0;

            await foreach (var transcribedSpeech in streamingPipeline.ReadTranscribedSpeechAsync(token))
            {
                var model = transcribedSpeech.ToRecognitionResults(totalSended++);

                await webSocket.SendJsonAsync(model, true, token);
            }

            await webSocket.SendJsonAsync(StateResponse.Listening, true, token);

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Ended", token);
        }
    }
}
