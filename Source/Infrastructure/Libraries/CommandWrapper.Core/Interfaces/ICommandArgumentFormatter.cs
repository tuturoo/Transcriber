using CommandWrapper.Core.Abstractions;

namespace CommandWrapper.Core.Interfaces;

/// <summary>
///     Интерфейс форматирования команды
/// </summary>
public interface ICommandArgumentFormatter
{
    /// <summary>
    ///     Форматирует команду
    /// </summary>
    /// <param name="argument">Аргумент</param>
    /// <returns>Форматированное представление аргумента</returns>
    public string Format(CommandArgument argument);
}