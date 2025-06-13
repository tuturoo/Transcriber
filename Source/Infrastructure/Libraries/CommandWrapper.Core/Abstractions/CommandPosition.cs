using CommandWrapper.Core.Exceptions;

namespace CommandWrapper.Core.Abstractions;

/// <summary>
///     Позиция команды
/// </summary>
public abstract class CommandPosition : IComparable<CommandPosition>
{
    /// <summary>
    ///     Приоритет
    /// </summary>
    protected readonly int Priority;

    /// <summary>
    ///     Конструктор
    /// </summary>
    /// <param name="priority">Приоритет</param>
    protected CommandPosition(int priority)
    {
        Priority = priority;
    }

    /// <summary>
    ///     Все аргументы на данной позиции
    /// </summary>
    protected abstract IEnumerable<CommandArgument>? Arguments { get; }

    protected internal bool HasChangesArguments
    {
        get
        {
            ArgumentNullException.ThrowIfNull(Arguments, nameof(Arguments));

            return Arguments.Any(arg => arg.HasChanges);
        }
    }

    public int CompareTo(CommandPosition? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Priority.CompareTo(other.Priority);
    }

    /// <summary>
    ///     Форматирование всех аргументов
    /// </summary>
    /// <returns>Строковое представление позиции с форматированными аргументами</returns>
    protected internal virtual string Format()
    {
        ArgumentNullException.ThrowIfNull(Arguments, nameof(Arguments));

        var requiredArgumentExceptions = Arguments
            .Where(arg => arg is { IsRequired: true, HasChanges: false })
            .Select(arg => new RequiredArgumentException { Argument = arg })
            .ToArray();

        if (requiredArgumentExceptions.Length > 0)
            throw new AggregateException(requiredArgumentExceptions as IEnumerable<Exception>);

        var formattedArguments = Arguments
            .Where(arg => arg.HasChanges)
            .Select(arg => arg.Format());

        return string.Join(" ", formattedArguments);
    }
}