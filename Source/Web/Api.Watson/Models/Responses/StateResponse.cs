using System.Text.Json.Serialization;

namespace Api.Watson.Models.Responses
{
    public sealed class StateResponse
    {
        [JsonPropertyName("state")]
        public string State { get; set; } = null!;

        public static StateResponse Listening => new() { State = "listening" };
    }
}
