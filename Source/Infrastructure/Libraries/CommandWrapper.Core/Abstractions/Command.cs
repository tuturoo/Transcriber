
namespace CommandWrapper.Core.Abstractions;

/// <summary>
///     Класс, описывающий команду и ее аргументы
/// </summary>
public abstract class Command<TProcess> where TProcess: CommandProcess
{
    /// <summary>
    /// Строка с аргументами
    /// </summary>
    private string? _arguments;
    
    /// <summary>
    ///     Путь к исполняемому файлу
    /// </summary>
    protected readonly string ExecutablePath;

    /// <summary>
    ///     Конструктор
    /// </summary>
    /// <param name="executablePath">Путь к исполняемому файлу</param>
    protected Command(string executablePath)
    {
        ExecutablePath = executablePath;
    }

    /// <summary>
    ///     Текущие аргументы команды
    /// </summary>
    public virtual string Arguments => _arguments ??= FormatCommandArguments();

    /// <summary>
    ///     Позиции команды
    /// </summary>
    protected abstract SortedSet<CommandPosition>? Positions { get; }

    /// <summary>
    ///     Создает процесс команды
    /// </summary>
    /// <returns>Процесс команды</returns>
    public abstract TProcess Run();

    #region Вспомогательные функции

    /// <summary>
    ///     Форматирует аргументы команды
    /// </summary>
    /// <returns>Форматированные аргументы по позициям</returns>
    private string FormatCommandArguments()
    {
        ArgumentNullException.ThrowIfNull(Positions);

        var formattedPositions = Positions
            .Where(pos => pos.HasChangesArguments)
            .Select(pos => pos.Format());

        return string.Join(" ", formattedPositions);
    }

    #endregion
}