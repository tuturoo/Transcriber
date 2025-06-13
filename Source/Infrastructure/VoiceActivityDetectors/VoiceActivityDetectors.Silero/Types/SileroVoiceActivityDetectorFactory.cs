using Core.Shared.Abstractions;
using Core.Shared.Models;
using Core.VoiceActivityDetectors.Interfaces;
using Microsoft.Extensions.Logging;

namespace VoiceActivityDetectors.Silero.Types
{
    public sealed class SileroVoiceActivityDetectorFactory : CachedFactory<IVoiceActivityDetector>
    {
        private readonly ILogger<SileroVoiceActivityDetector>? _logger;
        private readonly string? _modelPath;
        
        private float _threshold = 0.5f;
        private TimeSpan _minimalSpeechDuration = TimeSpan.FromMilliseconds(150);
        private TimeSpan _minimalSilenceDuration = TimeSpan.FromMilliseconds(50);
        private TimeSpan _pad = TimeSpan.FromMilliseconds(25);

        public SileroVoiceActivityDetectorFactory(ILogger<SileroVoiceActivityDetector> logger, string modelPath)
        {
            _logger = logger;
            _modelPath = modelPath;
        }

        public SileroVoiceActivityDetectorFactory WithThreshold(float threshold)
        {
            _threshold = threshold;
            
            return this;
        }

        public SileroVoiceActivityDetectorFactory WithMinimalSpeechDuration(int milliseconds)
        {
            _minimalSpeechDuration = TimeSpan.FromMilliseconds(milliseconds);

            return this;
        }

        public SileroVoiceActivityDetectorFactory WithMinimalSilenceDuration(int milliseconds)
        {
            _minimalSilenceDuration = TimeSpan.FromMilliseconds(milliseconds);
            return this;
        }

        public SileroVoiceActivityDetectorFactory WithPad(int milliseconds)
        {
            _pad = TimeSpan.FromMilliseconds(milliseconds);
            return this;
        }

        protected override async Task<IVoiceActivityDetector> InternalCreateAsync(AudioFormat audioFormat, CancellationToken token)
        {
            if (_modelPath is null || _logger is null)
                throw new ArgumentNullException("Не был вызван конфигурационный метод SileroVoiceActivityDetectorFactory.Configure");

            var detector = new SileroVoiceActivityDetector(_logger, _modelPath, _threshold, _minimalSpeechDuration, _minimalSilenceDuration, _pad);

            await detector.InitAsync(token);

            return detector;
        }
    }
}
