using Core.AudioTransformers.Interfaces;
using Core.Pipelines.Interfaces;
using Core.Shared.Extensions;
using Core.Shared.Interfaces;
using Core.Shared.Models;
using Core.Shared.Types;
using Core.SpeechRecognizers.Interfaces;
using Core.VoiceActivityDetectors.Interfaces;
using Microsoft.Extensions.Logging;

namespace StreamingPipelines.Types
{
    public sealed class ChunkedStreamingPipelineFactory : IFactory<IStreamingPipeline>
    {
        private readonly ILogger<ChunkedStreamingPipeline> _logger;
        private readonly ILogger<DefaultStreamingPipelineEngine> _engineLogger;
        private readonly TimeSpan _chunkDuration;
        
        private readonly IFactory<IVoiceActivityDetector> _voiceActivityDetectorFactory;
        private readonly IFactory<ISpeechRecognizer> _speechRecognizerFactory;
        private readonly IFactory<IAudioTransformer> _audioTransformerFactory;
        
        public ChunkedStreamingPipelineFactory
        (
            ILogger<ChunkedStreamingPipeline> logger,
            ILogger<DefaultStreamingPipelineEngine> engineLogger,
            IFactory<IAudioTransformer> audioTransformer,
            IFactory<IVoiceActivityDetector> voiceActivityDetector,
            IFactory<ISpeechRecognizer> speechRecognizerFactory,
            TimeSpan chunkDuration
        )
        {
            _audioTransformerFactory = audioTransformer;
            _voiceActivityDetectorFactory = voiceActivityDetector;
            _speechRecognizerFactory = speechRecognizerFactory;

            _logger = logger;
            _engineLogger = engineLogger;
            _chunkDuration = chunkDuration;
        }

        public async Task<IStreamingPipeline> CreateAsync(AudioFormat format, CancellationToken token)
        {
            if (format.Channels != 1 || format.BytesPerFrame != 2)
                throw new NotSupportedException("Поддерживается только один канал с глубиной 16 бит");

            int analyzingFrameCount = format.CalculateFrameCount(_chunkDuration);

            var source = new AiffAudioStream(format);
            var buffer = new AiffAudioStream(format);
            var speech = new AiffAudioStream(format);

            var engine = new DefaultStreamingPipelineEngine
            (
                _engineLogger,
                await _voiceActivityDetectorFactory.CreateAsync(format, token),
                await _speechRecognizerFactory.CreateAsync(format, token),
                await _audioTransformerFactory.CreateAsync(format, token)
            );

            return new ChunkedStreamingPipeline
            (
                _logger,
                engine,
                source,
                speech,
                buffer,
                analyzingFrameCount
            );
        }
    }
}
