using CommandWrapper.Sox.Types;
using Core.AudioTransformers.Interfaces;
using Core.Shared.Interfaces;
using Core.Shared.Models;
using Microsoft.Extensions.Logging;

namespace AudioTransformers.Sox.Types
{
    public sealed class SoxAudioTransformerFactory : IFactory<IAudioTransformer>
    {
        private readonly ILogger<SoxAudioTransformer> _logger;
        private readonly string _executablePath;

        private Action<SoxCommand>? _configure;

        public SoxAudioTransformerFactory(ILogger<SoxAudioTransformer> logger, string executablePath)
        {
            _logger = logger;
            _executablePath = executablePath;
        }

        public void Configure(Action<SoxCommand> action)
        {
            _configure = action;
        }

        public async Task<IAudioTransformer> CreateAsync(AudioFormat audioFormat, CancellationToken token)
        {
            if (_logger is null || _executablePath is null ||  _configure is null)
                throw new ArgumentNullException($"Не был вызван конфигурационный метод SoxAudioTransformerFactory.Configure");

            var command = new SoxCommand(_executablePath);

            command.InputFormat.Rate.Frequency = (ulong?)audioFormat.SamplingFrequency;
            command.InputFormat.Type.AudioFormat = audioFormat.Type;
            command.InputFormat.Depth.Bits = (ulong?)audioFormat.BitsPerFrame;
            command.InputFormat.Channels.Count = (ulong?)audioFormat.Channels;
            command.InputFormat.Encoding.Type = "signed-integer";

            _configure(command);

            var transformer = new SoxAudioTransformer(_logger, command);

            await transformer.InitAsync(token);

            return transformer;
        }
    }
}
