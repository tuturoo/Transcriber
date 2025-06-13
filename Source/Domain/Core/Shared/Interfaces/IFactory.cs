using Core.Shared.Models;

namespace Core.Shared.Interfaces
{
    /// <summary>
    /// Базовый интерфейс фабрики сложных объектов
    /// </summary>
    /// <typeparam name="T">Порождаемый тип</typeparam>
    public interface IFactory<T> where T : class
    {
        /// <summary>
        /// Создание объекта
        /// </summary>
        /// <param name="audioFormat">Аудиоформат</param>
        /// <param name="token">Токен</param>
        /// <returns>Объект нужного типа</returns>
        public Task<T> CreateAsync(AudioFormat audioFormat, CancellationToken token);
    }
}
