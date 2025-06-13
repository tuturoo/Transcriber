using Core.AudioTransformers.Interfaces;
using Core.Pipelines.Interfaces;
using Core.Shared.Interfaces;
using Core.SpeechRecognizers.Interfaces;
using Core.VoiceActivityDetectors.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StreamingPipelines.Types;

namespace StreamingPipelines.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChunkedStreamingPipelineFactory
        (
            this IServiceCollection services, 
            TimeSpan chunkDuration
        )
        {
            return services.AddSingleton<IFactory<IStreamingPipeline>>
            (
                provider =>
                {
                    var factory = new ChunkedStreamingPipelineFactory
                    (
                        provider.GetRequiredService<ILogger<ChunkedStreamingPipeline>>(),
                        provider.GetRequiredService<ILogger<DefaultStreamingPipelineEngine>>(),
                        provider.GetRequiredService<IFactory<IAudioTransformer>>(),
                        provider.GetRequiredService<IFactory<IVoiceActivityDetector>>(),
                        provider.GetRequiredService<IFactory<ISpeechRecognizer>>(),
                        chunkDuration
                    );

                    return factory;
                }
            );
        }
    }
}
