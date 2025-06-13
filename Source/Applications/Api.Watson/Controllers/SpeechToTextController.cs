using Api.Watson.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Api.Watson.Controllers
{
    [ApiController, Route("/speech-to-text/api/v1/")]
    public sealed class SpeechToTextController : ControllerBase
    {
        private readonly ILogger<SpeechToTextController> _logger;
        private readonly TranscriptionWebSocketHandler _speechToTextHandler;

        public SpeechToTextController(ILogger<SpeechToTextController> logger, TranscriptionWebSocketHandler speechToTextHandler)
        {
            _logger = logger;
            _speechToTextHandler = speechToTextHandler;
        }

        [HttpGet("recognize")]
        public async Task RecognizeAsync(CancellationToken token)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
                HttpContext.Response.StatusCode = 405;

            Guid requestId = Guid.NewGuid();

            try
            {
                using (var scope = _logger.BeginScope<Guid>(requestId))
                {
                    _logger.LogInformation($"Принят запрос на транскрипцию: {requestId}");

                    using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                    await _speechToTextHandler.HandleWebsocketAsync(socket, token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обработке запроса: {requestId}");

                throw;
            }
        }
    }
}
