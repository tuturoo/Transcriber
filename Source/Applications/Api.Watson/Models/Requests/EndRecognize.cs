using System.Text.Json.Serialization;

namespace Api.Watson.Models.Requests
{
    public sealed class EndRecognize
    {
        [JsonPropertyName("action")]
        public string Action { get; set; } = null!;
    }
}
