using Api.Watson.Options;
using AudioTransformers.Sox.Extensions;
using SpeechRecognizers.Whisper.Extensions;
using StreamingPipelines.Extensions;
using VoiceActivityDetectors.Silero.Extensions;

namespace Api.Watson
{
    internal static partial class Program
    {
        public static void AddTranscriber
        (
            this IServiceCollection services, 
            SoxOptions? soxOptions, 
            SileroOptions? sileroOptions, 
            WhisperOptions? whisperOptions,
            ChunkedStreamingPipelineOptions? pipelineOptions
        )
        {
            if (soxOptions is null)
                throw new ArgumentNullException(nameof(soxOptions), "Нет настроек для SoxAudioTransformer");

            if (sileroOptions is null)
                throw new ArgumentNullException(nameof(sileroOptions), "Нет настроек для SileroVoiceActivityDetector");

            if (whisperOptions is null)
                throw new ArgumentNullException(nameof(whisperOptions), "Нет настроек для WhisperSpeechRecognizer");

            if (pipelineOptions is null)
                throw new ArgumentNullException(nameof(pipelineOptions), "Нет настроек для ChunkedStreamingPipelineOptions");

            services.AddSoxAudioTransformerFactory
            (
                soxOptions.ExecutablePath,
                command =>
                {
                    command.OutputFormat.Rate.Frequency = (ulong?) soxOptions.OutputRate;
                    command.OutputFormat.Type.AudioFormat = soxOptions.OutputAudioFormat;
                    command.OutputFormat.Depth.Bits = (ulong?) soxOptions.OutputDepth;
                    command.OutputFormat.Channels.Count = (ulong?) soxOptions.OutputChannelCount;
                    command.OutputFormat.Encoding.Type = soxOptions.OutputEncodingType;

                    if (soxOptions.Normalize is not null)
                        command.Effects.Normalize.DecibelLevel = soxOptions.Normalize.DecibelLevel;

                    if (soxOptions.Compand is not null)
                    {
                        if (soxOptions.Compand.Gain is not null)
                            command.Effects.Compand.Gain = soxOptions.Compand.Gain.Value;

                        if (soxOptions.Compand.InitialLevel is not null)
                            command.Effects.Compand.InitialLevel = (double)soxOptions.Compand.InitialLevel.Value;

                        if (soxOptions.Compand.Delay is not null)
                            command.Effects.Compand.Delay = (double)soxOptions.Compand.Delay.Value;

                        if (soxOptions.Compand.TransferFunction is SoxOptions.TransferFunction transferFunction)
                            command.Effects.Compand.AddTransferFunction(transferFunction.Attack, transferFunction.Decay, transferFunction.DecibelTable);
                    }
                }
            );

            services.AddSileroVoiceActivityDetectorFactory
            (
                sileroOptions.ModelPath,
                factory =>
                {
                    factory.WithThreshold(sileroOptions.Threshold);
                    factory.WithMinimalSpeechDuration(sileroOptions.MinimalSpeechDurationMilliseconds);
                    factory.WithMinimalSilenceDuration(sileroOptions.MinimalSilenceDurationMilliseconds);
                    factory.WithPad(sileroOptions.PadMilliseconds);
                }
            );

            services.AddWhisperSpeechRecognizerFactory
            (
                whisperOptions.ModelPath,
                settings =>
                {
                    settings.Language = whisperOptions.Language;
                    settings.Threads = whisperOptions.ThreadCount;
                }
            );

            services.AddChunkedStreamingPipelineFactory(TimeSpan.FromMilliseconds(pipelineOptions.ChunkSizeMilliseconds));
        }
    }
}
