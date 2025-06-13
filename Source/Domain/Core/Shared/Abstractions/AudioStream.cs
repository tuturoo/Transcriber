using Core.Shared.Models;

namespace Core.Shared.Abstractions
{
    /// <summary>
    /// Аудиопоток для работы с сэмплами
    /// </summary>
    public abstract class AudioStream : MemoryStream
    {
        /// <summary>
        /// Формат аудио
        /// </summary>
        public readonly AudioFormat Format;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="format">Формат аудио</param>
        /// <param name="capacity">Размер буфера</param>
        public AudioStream(AudioFormat format, int capacity) : base(capacity)
        {
            Format = format;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="format">Формат аудио</param>
        /// <param name="buffer">Буфер</param>
        public AudioStream(AudioFormat format, byte[] buffer) : base(buffer)
        {
            Format = format;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="format">Формат аудио</param>
        public AudioStream(AudioFormat format) : base()
        {
            Format = format;
        }

        /// <summary>
        /// Количество прочитанных сэмплов на текущей длине
        /// </summary>
        public abstract long CurrentFrames { get; }

        /// <summary>
        /// Записанное число сэмплов
        /// </summary>
        public abstract long TotalFrames { get; }

        /// <summary>
        /// Время на текущей позиции
        /// </summary>
        public abstract TimeSpan CurrentTime { get; }

        /// <summary>
        /// Общая продолжительность аудио
        /// </summary>
        public abstract TimeSpan TotalTime { get; }

        /// <summary>
        /// Запись сэмплов
        /// </summary>
        /// <param name="buffer">Буфер с сэмплами</param>
        /// <param name="token">Токен</param>
        /// <returns>Задача</returns>
        public abstract Task WriteFramesAsync(float[] buffer, CancellationToken token);

        /// <summary>
        /// Чтение сэмплов в буфер
        /// </summary>
        /// <param name="buffer">Буфер для заполнения</param>
        /// <param name="offset">Смещение</param>
        /// <param name="count">Количество сэмплов для чтения</param>
        /// <param name="token">Токен</param>
        /// <returns>Задача</returns>
        public abstract Task<int> ReadFramesAsync(float[] buffer, int offset, int count, CancellationToken token);

        /// <summary>
        /// Устанавливает значение позиции в начало
        /// </summary>
        public virtual void Clear()
        {
            SetLength(0);
            Seek(0, SeekOrigin.Begin);
        }
    }
}
