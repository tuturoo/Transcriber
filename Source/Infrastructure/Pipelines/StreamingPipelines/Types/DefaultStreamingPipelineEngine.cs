using Core.AudioTransformers.Interfaces;
using Core.Pipelines.Interfaces;
using Core.Pipelines.Models;
using Core.Shared.Abstractions;
using Core.SpeechRecognizers.Interfaces;
using Core.VoiceActivityDetectors.Interfaces;
using Microsoft.Extensions.Logging;

namespace StreamingPipelines.Types
{
    public sealed class DefaultStreamingPipelineEngine : IStreamingPipelineEngine
    {
        private readonly ILogger<DefaultStreamingPipelineEngine> _logger;
        private readonly IVoiceActivityDetector _voiceDetector;
        private readonly ISpeechRecognizer _speechRecognizer;
        private readonly IAudioTransformer _audioTransformer;

        public DefaultStreamingPipelineEngine
        (
            ILogger<DefaultStreamingPipelineEngine> logger,
            IVoiceActivityDetector voiceDetector,
            ISpeechRecognizer speechRecognizer,
            IAudioTransformer audioTransformer
        )
        {
            _logger = logger;
            _voiceDetector = voiceDetector;
            _speechRecognizer = speechRecognizer;
            _audioTransformer = audioTransformer;
        }

        public async Task ProcessAudioAsync(AudioStream source, CancellationToken token)
        {
            try
            {
                await _audioTransformer.TransformAsync(source, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Обработка аудио завершилось ошибкой");

                throw;
            }
        }

        public async Task<SegmentationState> SegmentationAsync(SegmentationContext context, AudioStream source, AudioStream speech, CancellationToken token)
        {
            try
            {
                _logger.LogInformation($"Определение наличия голоса на фрагменте длительностью {source.TotalTime.TotalSeconds} секунд");

                context.HasVoiceActivity = await _voiceDetector.ContainsVoiceAsync(source, token);

                _logger.LogInformation($"Наличие голоса на фрагменте - {context.HasVoiceActivity}, текущее состояние - {context.CurrentState}");

                context.TotalTime += source.TotalTime;

                switch (context.CurrentState)
                {
                    case SegmentationState.Started:
                        await source.CopyToAsync(speech, token);
                        break;

                    case SegmentationState.Processing:
                        await source.CopyToAsync(speech, token);
                        break;

                    case SegmentationState.Ended:
                        return context.CurrentState;
                }

                source.Clear();

                return context.CurrentState;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сегментации модели");

                throw;
            }
        }

        public async Task<TranscribedSpeech> TranscribeAsync(SegmentationContext context, AudioStream source, CancellationToken token)
        {
            try
            {
                var text = await _speechRecognizer.TranscribeAsync(source, token);

                return new TranscribedSpeech(text, context.Start, context.End);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при транскрибации аудиопотока");

                throw;
            }
        }

        public Task<TranscribedSpeech> ProcessTextAsync(TranscribedSpeech speech, CancellationToken token)
            => throw new NotImplementedException();

        public void Dispose()
        {
            if (_voiceDetector is IDisposable detectorDisposable)
                detectorDisposable.Dispose();

            if (_audioTransformer is IDisposable audioTransformerDisposable)
                audioTransformerDisposable.Dispose();

            if (_speechRecognizer is IDisposable recognizerDisposable)
                recognizerDisposable.Dispose();
        }
    }
}
