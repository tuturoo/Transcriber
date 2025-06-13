using System.Text.Json.Serialization;

namespace Api.Watson.Models.Requests
{
    public class SpeechRecognitionResults
    {
        [JsonPropertyName("results")]
        public List<SpeechRecognitionResult> Results { get; set; } = null!;

        [JsonPropertyName("result_index")]
        public long? ResultIndex { get; set; }
    }
}
