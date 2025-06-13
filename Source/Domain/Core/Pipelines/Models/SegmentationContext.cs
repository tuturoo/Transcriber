namespace Core.Pipelines.Models
{
    /// <summary>
    /// Контекст для сегментирования речи
    /// </summary>
    public sealed class SegmentationContext
    {
        /// <summary>
        /// Текущая стадия
        /// </summary>
        private SegmentationState _currentState;

        /// <summary>
        /// Начало сегментируемого отрезка
        /// </summary>
        private TimeSpan _start = TimeSpan.Zero;

        /// <summary>
        /// Конец сегментируемого отрезка
        /// </summary>
        private TimeSpan _end = TimeSpan.Zero;

        /// <summary>
        /// Флаг о начавшемся сегменте
        /// </summary>
        private bool _isSegmentRecording;

        /// <summary>
        /// Флаг наличия голоса на фрагменте
        /// </summary>
        private bool _hasVoiceActivity;

        /// <summary>
        /// Начало речи
        /// </summary>
        public TimeSpan Start => _start;

        /// <summary>
        /// Конец речи
        /// </summary>
        public TimeSpan End => _end;

        /// <summary>
        /// Общая продолжительность
        /// </summary>
        public TimeSpan TotalTime { get; set; }

        /// <summary>
        /// Текущая стадия
        /// </summary>
        public SegmentationState CurrentState => _currentState;

        /// <summary>
        /// Наличие голосовой активности на фрагменте <br/>
        /// Установка значения этому полю меняет состояние <see cref="CurrentState"/>, а также <br/><br></br>
        /// <see cref="Start"/> - при <see cref="CurrentState"/> равным <see cref="SegmentationState.Started"/> <br></br>
        /// <see cref="End"/> - при <see cref="CurrentState"/> равным <see cref="SegmentationState.Ended"/>
        /// </summary>
        public bool HasVoiceActivity
        {
            get => _hasVoiceActivity;
            set
            {
                _hasVoiceActivity = value;
                
                if (_isSegmentRecording && HasVoiceActivity)
                {
                    _currentState = SegmentationState.Processing;
                } else if (!_isSegmentRecording && !HasVoiceActivity)
                {
                    _currentState = SegmentationState.Silence;
                } else if (!_isSegmentRecording && HasVoiceActivity)
                {
                    _start = TotalTime;
                    _isSegmentRecording = true;
                    _currentState = SegmentationState.Started;
                } else
                {
                    _end = TotalTime;
                    _isSegmentRecording = false;
                    _currentState = SegmentationState.Ended;
                }
            }
        }
    }
}
