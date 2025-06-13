using Core.Shared.Interfaces;
using Core.VoiceActivityDetectors.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VoiceActivityDetectors.Silero.Types;

namespace VoiceActivityDetectors.Silero.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSileroVoiceActivityDetectorFactory
        (
            this IServiceCollection services, 
            string modelPath, 
            Action<SileroVoiceActivityDetectorFactory> configure
        )
        {
            return services.AddSingleton<IFactory<IVoiceActivityDetector>>
            (
                provider =>
                {
                    var logger = provider.GetRequiredService<ILogger<SileroVoiceActivityDetector>>();

                    var factory = new SileroVoiceActivityDetectorFactory(logger, modelPath);

                    configure(factory);

                    return factory;
                }
            );
        }
    }
}
