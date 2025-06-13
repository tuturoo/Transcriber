using System.Diagnostics;

namespace CommandWrapper.Core.Abstractions;

/// <summary>
///     Процесс команды
/// </summary>
public abstract class CommandProcess : IDisposable
{
    /// <summary>
    ///     Используемые аргументы
    /// </summary>
    public readonly string Arguments;

    /// <summary>
    ///     Порожденный процесс
    /// </summary>
    protected readonly Process CreatedProcess;

    /// <summary>
    ///     Путь к исполняемому файлу
    /// </summary>
    public readonly string ExecutePath;

    /// <summary>
    ///     Конструктор
    /// </summary>
    /// <param name="executePath">Путь к файлу</param>
    /// <param name="arguments">Аргументы</param>
    /// <exception cref="ArgumentNullException">Не удалось создать процесс</exception>
    protected internal CommandProcess(string executePath, string arguments)
    {
        ExecutePath = executePath;
        Arguments = arguments;

        var startInfo = new ProcessStartInfo
        {
            FileName = executePath,
            Arguments = arguments,
            RedirectStandardInput = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        };

        CreatedProcess = Process.Start(startInfo) ?? throw new ArgumentNullException(nameof(CreatedProcess));
    }

    /// <summary>
    ///     Полное описание команды
    /// </summary>
    public virtual string FullCommand => $"{ExecutePath} {Arguments}";

    /// <summary>
    ///     Освобождение ресурсов
    /// </summary>
    public virtual void Dispose()
    {
        CreatedProcess?.Dispose();
    }
}