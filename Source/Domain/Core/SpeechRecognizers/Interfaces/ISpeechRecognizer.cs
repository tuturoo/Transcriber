using Core.Shared.Abstractions;

namespace Core.SpeechRecognizers.Interfaces
{
    /// <summary>
    /// Интерфейс транскрибирования аудио
    /// </summary>
    public interface ISpeechRecognizer
    {
        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="token">Токен</param>
        /// <returns>Задача</returns>
        public Task InitAsync(CancellationToken token);

        /// <summary>
        /// Транскрибирует записанную речь в потоке
        /// </summary>
        /// <param name="audio">Аудиопоток</param>
        /// <param name="token">Токен</param>
        /// <returns>Транскрибированная речь</returns>
        public Task<string> TranscribeAsync(AudioStream audio, CancellationToken token);
    }
}
