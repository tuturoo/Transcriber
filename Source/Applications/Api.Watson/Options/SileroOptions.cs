namespace Api.Watson.Options
{
    public sealed class SileroOptions
    {
        public string ModelPath { get; set; } = null!;

        public float Threshold { get; set; }

        public int MinimalSpeechDurationMilliseconds { get; set; }
        
        public int MinimalSilenceDurationMilliseconds { get; set; }
        
        public int PadMilliseconds { get; set; }
    }
}
