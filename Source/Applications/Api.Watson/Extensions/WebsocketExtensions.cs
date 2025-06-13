using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Api.Watson.Extensions
{
    public static class WebsocketExtensions
    {
        public static T ReadJson<T>(this WebSocketReceiveResult? message, byte[] buffer, CancellationToken token) where T : class
        {
            if (message is null) 
                throw new ArgumentNullException(nameof(message));

            if (!message.EndOfMessage)
                throw new NotSupportedException(nameof(message.EndOfMessage));

            var decodedContent = Encoding.UTF8.GetString(buffer, 0, message.Count);

            return JsonSerializer.Deserialize<T>(decodedContent) ??
                throw new ArgumentNullException($"При десериализации возникла ошибка - {decodedContent}");
        }

        public static async Task SendJsonAsync<T>(this WebSocket webSocket, T message, bool endOfMessage, CancellationToken token)
        {
            var encodedContent = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            await webSocket.SendAsync(encodedContent, WebSocketMessageType.Text, endOfMessage, token);
        }
    }
}
