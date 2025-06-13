using Core.Shared.Models;

namespace Core.Shared.Extensions
{
    public static class AudioFormatExtensions
    {
        /// <summary>
        /// Высчитывает размер аудио по формату и времени
        /// </summary>
        /// <param name="format">Формат</param>
        /// <param name="duration">Продолжительность аудио</param>
        /// <returns>Размер в байтах</returns>
        public static int CalculateSize(this AudioFormat format, TimeSpan duration)
        {
            var total = (double)duration.TotalSeconds * format.BytesPerFrame * format.SamplingFrequency;

            return (int)total;
        }

        /// <summary>
        /// Считает количество сэмплов аудио по его формату и размеру 
        /// </summary>
        /// <param name="format">Формат</param>
        /// <param name="size">Размер аудио</param>
        /// <returns>Количество сэмплов</returns>
        public static int CalculateFrameCount(this AudioFormat format, int size) => size / format.BytesPerFrame;

        /// <summary>
        /// Высчитает количество сэмплов по его формату и продолжительности аудио
        /// </summary>
        /// <param name="format">Формат</param>
        /// <param name="duration">Продолжительность аудио</param>
        /// <returns>Количество сэмплов</returns>
        public static int CalculateFrameCount(this AudioFormat format, TimeSpan duration)
        {
            return (int) (format.SamplingFrequency * duration.TotalSeconds);
        }

        /// <summary>
        /// Высчитывает продолжительность аудио по его формату и размеру
        /// </summary>
        /// <param name="format">Формат</param>
        /// <param name="size">Размер</param>
        /// <returns>Продолжительность аудио</returns>
        public static TimeSpan CalculateDuration(this AudioFormat format, long size)
        {
            var seconds = size / (format.BytesPerFrame * format.SamplingFrequency);

            return TimeSpan.FromSeconds(seconds);
        }
    }
}
