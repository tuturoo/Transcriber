using System.Text.Json.Serialization;

namespace Api.Watson.Models.Requests
{
    public class SpeechRecognitionAlternative
    {
        [JsonPropertyName("transcript")]
        public string Transcript { get; set; } = null!;
        
        [JsonPropertyName("confidence")]
        public double? Confidence { get; set; }
        
        [JsonPropertyName("timestamps")]
        public List<List<string>>? Timestamps { get; set; }

        [JsonPropertyName("word_confidence")]
        public List<List<string>>? WordConfidence { get; set; }
    }
}
