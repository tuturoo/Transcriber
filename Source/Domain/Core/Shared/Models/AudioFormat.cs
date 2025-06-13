namespace Core.Shared.Models
{
    /// <summary>
    /// Входящий аудиоформат
    /// </summary>
    public class AudioFormat
    {
        /// <summary>
        /// Тип (WAV, AIFF, RAW и т.п.)
        /// </summary>
        public string Type;

        /// <summary>
        /// Количество каналов
        /// </summary>
        public int Channels;

        /// <summary>
        /// Частота дискретизации
        /// </summary>
        public int SamplingFrequency;

        /// <summary>
        /// Глубина
        /// </summary>
        public int BitsPerFrame;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="type">Тип аудиоданных (WAV, AIFF, и т.п.)</param>
        /// <param name="channels">Количество каналов</param>
        /// <param name="samplingFrequency">Частота дискретизации</param>
        /// <param name="bitsPerSample">Глубина</param>
        public AudioFormat(string type, int channels, int samplingFrequency, int bitsPerSample)
        {
            Type = type;
            Channels = channels;
            SamplingFrequency = samplingFrequency;
            BitsPerFrame = bitsPerSample;
        }

        /// <summary>
        /// Среднее количество байт на секунду аудио
        /// </summary>
        public int AverageBytesPerSecond => SamplingFrequency * BytesPerFrame * Channels;

        /// <summary>
        /// Размер одного сэмпла в байтах
        /// </summary>
        public int BytesPerFrame => BitsPerFrame / 8;
    }
}