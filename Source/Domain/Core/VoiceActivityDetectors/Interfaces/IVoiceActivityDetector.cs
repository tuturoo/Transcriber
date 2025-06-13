using Core.Shared.Abstractions;

namespace Core.VoiceActivityDetectors.Interfaces
{
    /// <summary>
    /// Интерфейс фильтра голоса
    /// </summary>
    public interface IVoiceActivityDetector
    {
        /// <summary>
        /// Инициализация фильтра
        /// </summary>
        /// <param name="token">Токен</param>
        /// <returns>Задача</returns>
        public Task InitAsync(CancellationToken token);

        /// <summary>
        /// Фильтрация голоса
        /// </summary>
        /// <param name="audio">Аудиопоток</param>
        /// <param name="token">Токен</param>
        /// <returns>True в случае наличия голоса; false в случае отсутствия</returns>
        public Task<bool> ContainsVoiceAsync(AudioStream audio, CancellationToken token);
    }
}
