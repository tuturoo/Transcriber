using System.Buffers;
using Core.Shared.Abstractions;
using Core.Shared.Models;

namespace Core.Shared.Types
{
    /// <summary>
    /// Аудиопоток с сырыми данными
    /// </summary>
    public sealed class AiffAudioStream : AudioStream
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="format">Формат аудио</param>
        /// <exception cref="NotSupportedException">Поддерживается только один канал с глубиной 16 бит</exception>
        public AiffAudioStream(AudioFormat format) : base(format)
        {
            if (format.Channels != 1 || format.BytesPerFrame != 2)
                throw new NotSupportedException("Поддерживается только один канал с глубиной 16 бит");
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="format">Формат</param>
        /// <param name="capacity">Размер буфера</param>
        /// <exception cref="NotSupportedException">Поддерживается только один канал с глубиной 16 бит</exception>
        public AiffAudioStream(AudioFormat format, int capacity) : base(format, capacity)
        {
            if (format.Channels != 1 || format.BytesPerFrame != 2)
                throw new NotSupportedException(nameof(format));
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="format">Формат</param>
        /// <param name="buffer">Буфер</param>
        /// <exception cref="NotSupportedException">Поддерживается только один канал с глубиной 16 бит</exception>
        public AiffAudioStream(AudioFormat format, byte[] buffer) : base(format, buffer)
        {
            if (format.Channels != 1 || format.BytesPerFrame != 2)
                throw new NotSupportedException(nameof(format));
        }

        public override long CurrentFrames => Position / Format.BytesPerFrame;

        public override long TotalFrames => Length / Format.BytesPerFrame;

        public override TimeSpan CurrentTime => TimeSpan.FromSeconds((double)Position / Format.AverageBytesPerSecond);

        public override TimeSpan TotalTime => TimeSpan.FromSeconds((double)Length / Format.AverageBytesPerSecond);

        public override async Task<int> ReadFramesAsync(float[] buffer, int offset, int count, CancellationToken token)
        {
            int sourceBytesRequired = count * Format.BytesPerFrame;
            var sourceBuffer = ArrayPool<byte>.Shared.Rent(sourceBytesRequired);

            try
            {
                int bytesRead = await ReadAsync(sourceBuffer, 0, sourceBytesRequired, token);
                int outIndex = offset;

                for (int n = 0; n < bytesRead; n += 2)
                {
                    buffer[outIndex++] = BitConverter.ToInt16(sourceBuffer, n) / 32768f;
                }

                return bytesRead / 2;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sourceBuffer);
            }
        }

        public override async Task WriteFramesAsync(float[] buffer, CancellationToken token)
        {
            for (int i = 0; i < buffer.Length; ++i)
            {
                var toWrite = BitConverter.GetBytes((short)(buffer[i] * 32768f));

                await WriteAsync(toWrite, token);
            }
        }
    }
}
