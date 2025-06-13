using System.Text.Json.Serialization;

namespace Api.Watson.Models.Requests
{
    public class SpeechRecognitionResult
    {
        public class EndOfUtteranceEnumValue
        {
            public const string END_OF_DATA = "end_of_data";

            public const string FULL_STOP = "full_stop";
            
            public const string RESET = "reset";
            
            public const string SILENCE = "silence";

        }

        [JsonPropertyName("end_of_utterance")]
        public string? EndOfUtterance { get; set; }

        [JsonPropertyName("final")]
        public bool? Final { get; set; }


        [JsonPropertyName("alternatives")]
        public List<SpeechRecognitionAlternative>? Alternatives { get; set; }
    }

}
