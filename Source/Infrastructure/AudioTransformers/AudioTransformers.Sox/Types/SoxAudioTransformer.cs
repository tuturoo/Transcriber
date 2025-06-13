using CommandWrapper.Sox.Types;
using Core.AudioTransformers.Interfaces;
using Core.Shared.Abstractions;
using Microsoft.Extensions.Logging;

namespace AudioTransformers.Sox.Types
{
    /// <summary>
    /// Преобразователь аудио через биндинг к SOX
    /// </summary>
    public sealed class SoxAudioTransformer : IAudioTransformer
    {
        private readonly ILogger<SoxAudioTransformer> _logger;

        /// <summary>
        /// Команда для выполнения
        /// </summary>
        private readonly SoxCommand _soxCommand;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="soxCommand">Команда для выполнения</param>
        public SoxAudioTransformer(ILogger<SoxAudioTransformer> logger, SoxCommand soxCommand)
        {
            _logger = logger;

            _soxCommand = soxCommand;
        }

        public Task InitAsync(CancellationToken token) => Task.CompletedTask;

        public async Task TransformAsync(AudioStream audio, CancellationToken token)
        {
            using var sox = _soxCommand.Run();

            _logger.LogInformation($"Запущена обработка аудио: {sox.FullCommand}");

            audio.Seek(0, SeekOrigin.Begin);

            await sox.ProcessAudioAsync(audio, token);

            _logger.LogInformation($"Окончена обработка аудио");

            if (_soxCommand.OutputFormat.Depth.Bits is not null)
                audio.Format.BitsPerFrame = (int) _soxCommand.OutputFormat.Depth.Bits.Value;

            if (_soxCommand.OutputFormat.Rate.Frequency is not null)
                audio.Format.SamplingFrequency = (int)_soxCommand.OutputFormat.Rate.Frequency.Value;
            
            audio.Seek(0, SeekOrigin.Begin);
        }
    }
}
