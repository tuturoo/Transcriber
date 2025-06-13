using VoiceActivityDetectorSilero.Models;

namespace VoiceActivityDetectorSilero.Types
{
    public class SileroDetector
    {
        private readonly SileroModel _model;
        private readonly float _threshold;
        private readonly float _negThreshold;
        private readonly int _samplingRate;
        private readonly int _windowSizeSample;
        private readonly float _minSpeechSamples;
        private readonly float _speechPadSamples;
        private readonly float _maxSpeechSamples;
        private readonly float _minSilenceSamples;
        private readonly float _minSilenceSamplesAtMaxSpeech;
        private int _audioLengthSamples;
        private const float THRESHOLD_GAP = 0.15f;
        // ReSharper disable once InconsistentNaming
        private const int SAMPLING_RATE_8K = 8000;
        // ReSharper disable once InconsistentNaming
        private const int SAMPLING_RATE_16K = 16000;

        public SileroDetector(SileroModel model, float threshold, int samplingRate,
            int minSpeechDurationMs, float maxSpeechDurationSeconds,
            int minSilenceDurationMs, int speechPadMs)
        {
            if (samplingRate != SAMPLING_RATE_8K && samplingRate != SAMPLING_RATE_16K)
            {
                throw new ArgumentException("Sampling rate not support, only available for [8000, 16000]");
            }

            _model = model;
            _samplingRate = samplingRate;
            _threshold = threshold;
            _negThreshold = threshold - THRESHOLD_GAP;
            _windowSizeSample = samplingRate == SAMPLING_RATE_16K ? 512 : 256;
            _minSpeechSamples = samplingRate * minSpeechDurationMs / 1000f;
            _speechPadSamples = samplingRate * speechPadMs / 1000f;
            _maxSpeechSamples = samplingRate * maxSpeechDurationSeconds - _windowSizeSample - 2 * _speechPadSamples;
            _minSilenceSamples = samplingRate * minSilenceDurationMs / 1000f;
            _minSilenceSamplesAtMaxSpeech = samplingRate * 98 / 1000f;
            Reset();
        }

        public void Reset()
        {
            _model.ResetStates();
        }

        public List<SpeechSegment> GetSpeechSegmentList(float[] audioFrames)
        {
            Reset();

            List<float> speechProbList = new List<float>();
            _audioLengthSamples = audioFrames.Length / 2;
            float[] buffer = new float[_windowSizeSample];

            int startIndex = 0;
            int count = 0;

            while (startIndex < audioFrames.Length)
            {
                if (startIndex + _windowSizeSample >= audioFrames.Length)
                    count = audioFrames.Length - startIndex;
                else
                    count = _windowSizeSample;

                audioFrames.AsSpan(startIndex, count).CopyTo(buffer.AsSpan());
                startIndex += _windowSizeSample;

                float speechProb = _model.Call(new[] { buffer }, _samplingRate)[0];
                speechProbList.Add(speechProb);
            }

            return CalculateProb(speechProbList);
        }

        private List<SpeechSegment> CalculateProb(List<float> speechProbList)
        {
            List<SpeechSegment> result = new List<SpeechSegment>();
            bool triggered = false;
            int tempEnd = 0, prevEnd = 0, nextStart = 0;
            SpeechSegment segment = new SpeechSegment();

            for (int i = 0; i < speechProbList.Count; i++)
            {
                float speechProb = speechProbList[i];
                if (speechProb >= _threshold && tempEnd != 0)
                {
                    tempEnd = 0;
                    if (nextStart < prevEnd)
                    {
                        nextStart = _windowSizeSample * i;
                    }
                }

                if (speechProb >= _threshold && !triggered)
                {
                    triggered = true;
                    segment.StartOffset = _windowSizeSample * i;
                    continue;
                }

                if (triggered && _windowSizeSample * i - segment.StartOffset > _maxSpeechSamples)
                {
                    if (prevEnd != 0)
                    {
                        segment.EndOffset = prevEnd;
                        result.Add(segment);
                        segment = new SpeechSegment();
                        if (nextStart < prevEnd)
                        {
                            triggered = false;
                        }
                        else
                        {
                            segment.StartOffset = nextStart;
                        }

                        prevEnd = 0;
                        nextStart = 0;
                        tempEnd = 0;
                    }
                    else
                    {
                        segment.EndOffset = _windowSizeSample * i;
                        result.Add(segment);
                        segment = new SpeechSegment();
                        prevEnd = 0;
                        nextStart = 0;
                        tempEnd = 0;
                        triggered = false;
                        continue;
                    }
                }

                if (speechProb < _negThreshold && triggered)
                {
                    if (tempEnd == 0)
                    {
                        tempEnd = _windowSizeSample * i;
                    }

                    if (_windowSizeSample * i - tempEnd > _minSilenceSamplesAtMaxSpeech)
                    {
                        prevEnd = tempEnd;
                    }

                    if (_windowSizeSample * i - tempEnd < _minSilenceSamples)
                    {
                        continue;
                    }
                    else
                    {
                        segment.EndOffset = tempEnd;
                        if (segment.EndOffset - segment.StartOffset > _minSpeechSamples)
                        {
                            result.Add(segment);
                        }

                        segment = new SpeechSegment();
                        prevEnd = 0;
                        nextStart = 0;
                        tempEnd = 0;
                        triggered = false;
                        continue;
                    }
                }
            }

            if (segment.StartOffset != null && _audioLengthSamples - segment.StartOffset > _minSpeechSamples)
            {
                segment.EndOffset = _audioLengthSamples;
                result.Add(segment);
            }

            for (int i = 0; i < result.Count; i++)
            {
                SpeechSegment item = result[i];
                if (i == 0)
                {
                    item.StartOffset = (int)Math.Max(0, item.StartOffset.Value - _speechPadSamples);
                }

                if (i != result.Count - 1)
                {
                    SpeechSegment nextItem = result[i + 1];
                    int silenceDuration = nextItem.StartOffset.Value - item.EndOffset.Value;
                    if (silenceDuration < 2 * _speechPadSamples)
                    {
                        item.EndOffset = item.EndOffset + silenceDuration / 2;
                        nextItem.StartOffset = Math.Max(0, nextItem.StartOffset.Value - silenceDuration / 2);
                    }
                    else
                    {
                        item.EndOffset = (int)Math.Min(_audioLengthSamples, item.EndOffset.Value + _speechPadSamples);
                        nextItem.StartOffset = (int)Math.Max(0, nextItem.StartOffset.Value - _speechPadSamples);
                    }
                }
                else
                {
                    item.EndOffset = (int)Math.Min(_audioLengthSamples, item.EndOffset.Value + _speechPadSamples);
                }
            }

            return MergeListAndCalculateSecond(result, _samplingRate);
        }

        private List<SpeechSegment> MergeListAndCalculateSecond(List<SpeechSegment> original, int samplingRate)
        {
            List<SpeechSegment> result = new List<SpeechSegment>();
            if (original == null || original.Count == 0)
            {
                return result;
            }

            int left = original[0].StartOffset.Value;
            int right = original[0].EndOffset.Value;
            if (original.Count > 1)
            {
                original.Sort((a, b) => a.StartOffset.Value.CompareTo(b.StartOffset.Value));
                for (int i = 1; i < original.Count; i++)
                {
                    SpeechSegment segment = original[i];

                    if (segment.StartOffset > right)
                    {
                        result.Add(new SpeechSegment(left, right,
                            CalculateSecondByOffset(left, samplingRate), CalculateSecondByOffset(right, samplingRate)));
                        left = segment.StartOffset.Value;
                        right = segment.EndOffset.Value;
                    }
                    else
                    {
                        right = Math.Max(right, segment.EndOffset.Value);
                    }
                }

                result.Add(new SpeechSegment(left, right,
                    CalculateSecondByOffset(left, samplingRate), CalculateSecondByOffset(right, samplingRate)));
            }
            else
            {
                result.Add(new SpeechSegment(left, right,
                    CalculateSecondByOffset(left, samplingRate), CalculateSecondByOffset(right, samplingRate)));
            }

            return result;
        }

        private float CalculateSecondByOffset(int offset, int samplingRate)
        {
            float secondValue = offset * 1.0f / samplingRate;
            return (float)Math.Floor(secondValue * 1000.0f) / 1000.0f;
        }
    }
}
