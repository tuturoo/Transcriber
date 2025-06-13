using Core.Pipelines.Models;
using Core.Shared.Abstractions;

namespace Core.Pipelines.Interfaces
{
    /// <summary>
    /// Интерфейс выполнения транскрибации речи без состояния
    /// </summary>
    public interface IStreamingPipelineEngine : IDisposable
    {
        /// <summary>
        /// Выполняет обработку аудио
        /// </summary>
        /// <param name="source">Аудиопоток</param>
        /// <param name="token">Токен</param>
        /// <returns>Задача</returns>
        public Task ProcessAudioAsync(AudioStream source, CancellationToken token);

        /// <summary>
        /// Сегментирует аудиопоток. В случае окончания речи возвращает <see cref="SegmentationState.Ended"/>, иначе - текущее состояние
        /// </summary>
        /// <param name="context">Контекст сегментации</param>
        /// <param name="source">Аудиопоток</param>
        /// <param name="speech">Аудиопоток для записи речи</param>
        /// <param name="token">Токен</param>
        /// <returns>Текущее состояние сегментации</returns>
        public Task<SegmentationState> SegmentationAsync
        (
            SegmentationContext context,
            AudioStream source,
            AudioStream speech,
            CancellationToken token
        );

        /// <summary>
        /// Выполняет транскрипцию аудиопотока
        /// </summary>
        /// <param name="context">Контекст сегментации</param>
        /// <param name="source">Аудиопоток</param>
        /// <param name="token">Токен</param>
        /// <returns>Транскрибированная речь</returns>
        public Task<TranscribedSpeech> TranscribeAsync(SegmentationContext context, AudioStream source, CancellationToken token);

        /// <summary>
        /// Выполняет обработку текста
        /// </summary>
        /// <param name="speech">Транскрибированная речь</param>
        /// <param name="token">Токен</param>
        /// <returns>Обработанная речь</returns>
        public Task<TranscribedSpeech> ProcessTextAsync(TranscribedSpeech speech, CancellationToken token);
    }
}
