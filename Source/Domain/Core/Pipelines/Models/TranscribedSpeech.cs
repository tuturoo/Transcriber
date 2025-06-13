namespace Core.Pipelines.Models
{
    /// <summary>
    /// Результат транскрибированной речи
    /// </summary>
    public sealed class TranscribedSpeech
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="text">Содержание</param>
        /// <param name="start">Начало речи</param>
        /// <param name="end">Конец речи</param>
        public TranscribedSpeech(string text, TimeSpan start, TimeSpan end)
        {
            Text = text;
            Start = start;
            End = end;
        }

        /// <summary>
        /// Содержание речи
        /// </summary>
        public string Text { get; }

        public TimeSpan Start { get; }

        public TimeSpan End { get; }
    }
}
