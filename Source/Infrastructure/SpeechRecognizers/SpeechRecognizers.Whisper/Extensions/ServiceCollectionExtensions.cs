using Core.Shared.Interfaces;
using Core.SpeechRecognizers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpeechRecognizers.Whisper.Models;
using SpeechRecognizers.Whisper.Types;

namespace SpeechRecognizers.Whisper.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWhisperSpeechRecognizerFactory
        (
            this IServiceCollection services, 
            string modelPath, 
            Action<WhisperSettings> configure
        )
        {
            return services.AddSingleton<IFactory<ISpeechRecognizer>>
            (
                provider =>
                {
                    var logger = provider.GetRequiredService<ILogger<WhisperSpeechRecognizer>>();

                    var factory = new WhisperSpeechRecognizerFactory(logger, modelPath);

                    factory.Configure(configure);

                    return factory;
                }
            );
        }
    }
}
