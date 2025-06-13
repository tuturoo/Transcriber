using Core.Shared.Abstractions;

namespace Core.AudioTransformers.Interfaces
{
    /// <summary>
    /// Интерфейс для предобработки аудио (нормализация, выравнивание звука и т. п.)
    /// </summary>
    public interface IAudioTransformer
    {
        /// <summary>
        /// Инициализирует обработчик
        /// </summary>
        /// <param name="token">Токен</param>
        /// <returns>Задача</returns>
        public Task InitAsync(CancellationToken token);

        /// <summary>
        /// Выполняет преобразование аудиопотока с записью результата в этот поток
        /// </summary>
        /// <param name="audio">Аудиопоток</param>
        /// <param name="token">Токен</param>
        /// <returns>Задача</returns>
        public Task TransformAsync(AudioStream audio, CancellationToken token);
    }
}
