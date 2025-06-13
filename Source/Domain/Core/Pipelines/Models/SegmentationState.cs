
namespace Core.Pipelines.Models
{
    /// <summary>
    /// Стадия сегментации для аудио фрагмента
    /// </summary>
    public enum SegmentationState : byte
    {
        /// <summary>
        /// Начало
        /// </summary>
        None,

        /// <summary>
        /// Тишина
        /// </summary>
        Silence,

        /// <summary>
        /// Начало речи
        /// </summary>
        Started,

        /// <summary>
        /// Конец речи
        /// </summary>
        Ended,

        /// <summary>
        /// Речь продолжается
        /// </summary>
        Processing
    }
}
