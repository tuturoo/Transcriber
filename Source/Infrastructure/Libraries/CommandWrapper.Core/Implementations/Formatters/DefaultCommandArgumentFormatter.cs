using CommandWrapper.Core.Abstractions;
using CommandWrapper.Core.Interfaces;

namespace CommandWrapper.Core.Implementations.Formatters;

/// <summary>
///     Стандартное форматирование команды
/// </summary>
public class DefaultCommandArgumentFormatter : ICommandArgumentFormatter
{
    /// <summary>
    ///     Знак между именем и значением
    /// </summary>
    protected string Between = " ";

    /// <summary>
    ///     Знак после значения
    /// </summary>
    protected string End = string.Empty;

    /// <summary>
    ///     Символ кавычек
    /// </summary>
    protected string Quote = string.Empty;

    /// <summary>
    ///     Знак перед именем
    /// </summary>
    protected string Start = string.Empty;

    public string Format(CommandArgument argument)
    {
        if (argument.Name is null)
            return argument.Value!;

        if (argument.Value is null)
            return $"{Start}{argument.Name}{End}";

        var quote = UseQuotes(argument) ? Quote : string.Empty;

        return $"{Start}{argument.Name}{Between}{quote}{argument.Value}{quote}{End}";
    }

    /// <summary>
    ///     Определяет, должны ли использоваться кавычки
    /// </summary>
    /// <param name="argument">Аргумент</param>
    /// <returns>Результат проверки</returns>
    protected virtual bool UseQuotes(CommandArgument argument)
    {
        return false;
    }
}