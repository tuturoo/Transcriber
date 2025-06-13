using Core.Shared.Abstractions;
using Core.VoiceActivityDetectors.Interfaces;
using Microsoft.Extensions.Logging;
using VoiceActivityDetectorSilero.Types;

namespace VoiceActivityDetectors.Silero.Types
{
    public sealed class SileroVoiceActivityDetector : IVoiceActivityDetector
    {
        private readonly ILogger<SileroVoiceActivityDetector> _logger;

        private readonly float _threshold;
        private readonly TimeSpan _minimalSpeechDuration;
        private readonly TimeSpan _minimalSilenceDuration;
        private readonly TimeSpan _pad;
        
        private readonly string _modelPath;

        private byte[]? _model;

        public SileroVoiceActivityDetector(ILogger<SileroVoiceActivityDetector> logger, string modelPath, float threshold, TimeSpan minimalSpeechDuration, TimeSpan minimalSilenceDuration, TimeSpan pad)
        {
            _logger = logger;

            _modelPath = modelPath;
            _threshold = threshold;
            _minimalSpeechDuration = minimalSpeechDuration;
            _minimalSilenceDuration = minimalSilenceDuration;
            _pad = pad;
        }

        public async Task InitAsync(CancellationToken token)
        {
            _logger.LogInformation($"Загрузка SileroVoiceAcitivityDetector модели: {_modelPath}");

            _model = await File.ReadAllBytesAsync(_modelPath, token);

            _logger.LogInformation($"Загрузка завершена");
        }

        public async Task<bool> ContainsVoiceAsync(AudioStream audio, CancellationToken token)
        {
            if (_model is null)
                throw new ArgumentNullException(nameof(_model), $"Фильтр не был инициализирован");

            var model = new SileroModel(_model);

            try
            {
                var detector = new SileroDetector
                (
                    model,
                    _threshold,
                    audio.Format.SamplingFrequency,
                    _minimalSpeechDuration.Milliseconds,
                    float.PositiveInfinity,
                    _minimalSilenceDuration.Milliseconds,
                    _pad.Milliseconds
                );

                audio.Seek(0, SeekOrigin.Begin);

                float[] buffer = new float[audio.TotalFrames];

                await audio.ReadFramesAsync(buffer, 0, (int)audio.TotalFrames, token);

                _logger.LogInformation($"Начата фильтрация звука");

                var segments = detector.GetSpeechSegmentList(buffer);

                _logger.LogInformation($"Отфильтрованные речевые сегменты: {string.Join("\n", segments.Select(x => $"{x.StartSecond} -> {x.EndSecond}"))}");

                audio.Seek(0, SeekOrigin.Begin);

                return segments.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при фильтре звука");

                throw;
            }
            finally
            {
                model.Dispose();          
            }
        }
    }
}
