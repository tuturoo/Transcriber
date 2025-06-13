using Api.Watson.Models.Requests;
using Core.Pipelines.Models;

namespace Api.Watson.Extensions
{
    public static class ModelsExtensions
    {
        public static SpeechRecognitionResults ToRecognitionResults(this TranscribedSpeech transcribedSpeech, int index)
        {
            return new SpeechRecognitionResults()
            {
                ResultIndex = index,
                Results = new List<SpeechRecognitionResult>()
                {
                    new SpeechRecognitionResult()
                    {
                        Final = true,
                        Alternatives = new List<SpeechRecognitionAlternative>()
                        {
                            new SpeechRecognitionAlternative()
                            {
                                Confidence = 1.0,
                                Transcript = transcribedSpeech.Text,
                                Timestamps = new List<List<string>>()
                                {
                                    new List<string>()
                                    {
                                        transcribedSpeech.Text,
                                        transcribedSpeech.Start.TotalSeconds.ToString("0.00"),
                                        transcribedSpeech.End.TotalSeconds.ToString("0.00")
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        public static bool TryGetRateFromContentType(this StartRecognize startRecognize, out int rate)
        {
            rate = default;
            
            if (startRecognize is null || startRecognize.ContentType is null || startRecognize.ContentType.Length == 0)
                return false;

            int index = startRecognize.ContentType.IndexOf("rate=") + "rate=".Length;

            if (index < 0) 
                return false;
            
            int indexEnd = startRecognize.ContentType.IndexOf(';', index);

            if (indexEnd < 0)
                return false;

            var value = startRecognize.ContentType.Substring(index, indexEnd - index);

            return int.TryParse(value, out rate);
        }
    }
}
