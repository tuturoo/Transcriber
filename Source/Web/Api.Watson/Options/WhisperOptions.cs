namespace Api.Watson.Options
{
    public sealed class WhisperOptions
    {
        public string ModelPath { get; set; } = null!;

        public int ThreadCount { get; set; } = 1;

        public string Language { get; set; } = "auto";
    }
}
