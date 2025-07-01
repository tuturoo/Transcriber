using System.Text.Json.Serialization;

namespace Api.Watson.Models.Requests
{
    public sealed class StartRecognize
    {
        [JsonPropertyName("action")]
        public string Action { get; set; } = null!;

        [JsonPropertyName("content-type")]
        public string ContentType { get; set; } = null!;
    }
}
