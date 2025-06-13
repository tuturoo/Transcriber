using Core.AudioTransformers.Interfaces;
using Core.Shared.Abstractions;
using Core.Shared.Models;
using Core.SpeechRecognizers.Interfaces;
using Microsoft.Extensions.Logging;
using SpeechRecognizers.Whisper.Models;

namespace SpeechRecognizers.Whisper.Types
{
    public sealed class WhisperSpeechRecognizerFactory : CachedFactory<ISpeechRecognizer>
    {
        private readonly ILogger<WhisperSpeechRecognizer> _logger;
        private readonly WhisperSettings _whisperSettings;

        public WhisperSpeechRecognizerFactory(ILogger<WhisperSpeechRecognizer> logger, string modelPath)
        {
            _logger = logger;
            _whisperSettings = new WhisperSettings()
            {
                ModelPath = modelPath
            };
        }

        public void Configure(Action<WhisperSettings> action)
            => action(_whisperSettings);
        
        protected override async Task<ISpeechRecognizer> InternalCreateAsync(AudioFormat audioFormat, CancellationToken token)
        {
            if (_whisperSettings is null || _logger is null)
                throw new ArgumentNullException(nameof(_whisperSettings), "Не был вызван метод конфигурации WhisperSpeechRecognizerFactory.Configure");

            var recognizer = new WhisperSpeechRecognizer(_logger, _whisperSettings);

            await recognizer.InitAsync(token);

            return recognizer;
        }
    }
}
