using Api.Watson.Handlers;
using Api.Watson.Options;
using Api.Watson.Services;
using AspNetCore.Authentication.Basic;
using AudioTransformers.Sox.Types;
using Microsoft.AspNetCore.WebSockets;
using SpeechRecognizers.Whisper.Types;
using StreamingPipelines.Types;
using VoiceActivityDetectors.Silero.Types;

namespace Api.Watson
{
    internal static partial class Program
    {
        public static void Configure(WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile("pipelinesettings.json");
            builder.Configuration.AddJsonFile("users.json");

            var soxOptions = builder.Configuration.GetSection(nameof(SoxAudioTransformer)).Get<SoxOptions>();
            var sileroOptions = builder.Configuration.GetSection(nameof(SileroVoiceActivityDetector)).Get<SileroOptions>();
            var whisperOptions = builder.Configuration.GetSection(nameof(WhisperSpeechRecognizer)).Get<WhisperOptions>();
            var pipelineOptions = builder.Configuration.GetSection(nameof(ChunkedStreamingPipeline)).Get<ChunkedStreamingPipelineOptions>();
            
            var users = builder.Configuration.GetSection("users").Get<Dictionary<string, string>>() 
                ?? throw new ArgumentNullException("users", $"Неустановлены логины и пароли для пользователей");

            builder.Services.AddControllers();
            builder.Services.AddHttpLogging(x => { });
            builder.Services.AddAuthentication(BasicDefaults.AuthenticationScheme);
            builder.Services.AddTranscriber(soxOptions, sileroOptions, whisperOptions, pipelineOptions);
            builder.Services.AddSingleton<IBasicUserValidationService, DictionaryBasicValidationService>(_ => new DictionaryBasicValidationService(users));
            builder.Services.AddSingleton<TranscriptionWebSocketHandler>();

            builder.Services.AddWebSockets
            (
                configure =>
                { 
                    configure.KeepAliveInterval = TimeSpan.FromSeconds(30); 
                }
            );
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Configure(builder);

            var app = builder.Build();

            app.UseHttpLogging();
            app.UseAuthorization();
            app.UseWebSockets();
            app.MapControllers();

            app.Run();
        }
    }
}
