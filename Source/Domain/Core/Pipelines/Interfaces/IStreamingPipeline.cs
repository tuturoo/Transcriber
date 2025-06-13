using Core.Pipelines.Models;

namespace Core.Pipelines.Interfaces
{
    /// <summary>
    /// Внешний интерфейс транскрибирования речи 
    /// </summary>
    public interface IStreamingPipeline : IDisposable
    {
        /// <summary>
        /// Записывает аудиофрагмент для транскрибирования
        /// </summary>
        /// <param name="data">Аудиофрагмент</param>
        /// <param name="token">Токен</param>
        /// <returns>Задача</returns>
        public Task WriteAudioDataAsync(byte[] data, CancellationToken token);
        
        /// <summary>
        /// Читает готовую транскрибированную речь
        /// </summary>
        /// <param name="token">Токен</param>
        /// <returns>Асинхронный итератор с транскрибированной речью</returns>
        public IAsyncEnumerable<TranscribedSpeech> ReadTranscribedSpeechAsync(CancellationToken token);

        /// <summary>
        /// Помечает, что данных больше не будет
        /// </summary>
        /// <returns>Задача</returns>
        public Task StopAsync();

        /// <summary>
        /// Запускает Pipeline
        /// </summary>
        /// <returns>Задача</returns>
        public Task StartAsync();

        /// <summary>
        /// Отменяет транскрибцию
        /// </summary>
        /// <returns>Задача</returns>
        public Task CancelAsync();
    }
}
