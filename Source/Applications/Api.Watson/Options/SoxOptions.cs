
namespace Api.Watson.Options
{
    public sealed class SoxOptions
    {
        public string ExecutablePath { get; set; } = null!;

        public int OutputRate { get; set; }

        public string OutputAudioFormat { get; set; } = null!;

        public int OutputDepth { get; set; }

        public int OutputChannelCount { get; set; }

        public string OutputEncodingType { get; set; } = null!;

        public Normalization? Normalize { get; set; }

        public Companding? Compand { get; set; }

        public sealed class Normalization
        {
            public double DecibelLevel { get; set; }
        }

        public sealed class Companding
        {
            public TransferFunction? TransferFunction { get; set; }
        
            public double? Gain { get; set; }
        
            public double? InitialLevel { get; set; }

            public double? Delay { get; set; }
        }

        public sealed class TransferFunction
        {
            public double Attack { get; set; }
            
            public double Decay { get; set; }

            public double[] DecibelTable { get; set; } = null!;

        }
    }
}
