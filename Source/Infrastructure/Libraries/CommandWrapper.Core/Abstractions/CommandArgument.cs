using CommandWrapper.Core.Interfaces;

namespace CommandWrapper.Core.Abstractions;

/// <summary>
///     Аргумент команды
/// </summary>
public abstract class CommandArgument
{
    /// <summary>
    ///     Используемый стиль форматирования аргумента
    /// </summary>
    protected readonly ICommandArgumentFormatter Formatter;

    /// <summary>
    ///     Условие обязательного использования аргумента
    /// </summary>
    protected internal readonly bool IsRequired;

    /// <summary>
    ///     Создает аргумент команды
    /// </summary>
    /// <param name="formatter">Используемый формат</param>
    protected CommandArgument(ICommandArgumentFormatter formatter)
    {
        IsRequired = false;
        Formatter = formatter;
    }

    /// <summary>
    ///     Наличие изменений внутренних полей аргумента
    /// </summary>
    protected internal bool HasChanges { get; protected set; }

    /// <summary>
    ///     Имя аргумента
    /// </summary>
    protected internal abstract string? Name { get; }

    /// <summary>
    ///     Высчитываемое значение аргумента
    /// </summary>
    protected internal abstract string? Value { get; }

    /// <summary>
    ///     Возвращает форматированное значение аргумента
    /// </summary>
    /// <returns>Форматированное представление аргумента</returns>
    protected internal virtual string Format()
    {
        if (Name is null && Value is null)
            ArgumentNullException.ThrowIfNull("Name and value is null");

        return Formatter.Format(this);
    }

    #region Вспомогательные функции

    /// <summary>
    ///     Устанавливает значения переменной и ставит <see cref="HasChanges" /> на true
    /// </summary>
    /// <param name="value">Изменяемая переменная</param>
    /// <param name="newValue">Новое значение</param>
    /// <typeparam name="T">Целевой тип</typeparam>
    protected virtual void SetValue<T>(ref T value, T newValue)
    {
        HasChanges = true;
        value = newValue;
    }

    #endregion
}