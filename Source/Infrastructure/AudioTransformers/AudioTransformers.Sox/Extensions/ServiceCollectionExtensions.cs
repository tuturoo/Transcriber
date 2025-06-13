using AudioTransformers.Sox.Types;
using CommandWrapper.Sox.Types;
using Core.AudioTransformers.Interfaces;
using Core.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AudioTransformers.Sox.Extensions
{
    /// <summary>
    /// Методы расширения SoxAudioTransformer
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSoxAudioTransformerFactory
        (
            this IServiceCollection services, 
            string executablePath, 
            Action<SoxCommand> configure
        )
        {
            return services.AddSingleton<IFactory<IAudioTransformer>>
            (
                provider =>
                {
                    var logger = provider.GetRequiredService<ILogger<SoxAudioTransformer>>();

                    var factory = new SoxAudioTransformerFactory(logger, executablePath);

                    factory.Configure(configure);

                    return factory;
                }
            );
        }
    }
}
