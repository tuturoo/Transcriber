using CommandWrapper.Core.Implementations.Formatters;
using CommandWrapper.Core.Implementations.Formatters.LinuxLike;

namespace CommandWrapper.Core.Constants;

/// <summary>
///     Статический класс с существующими реализациями форматирования
/// </summary>
public static class ArgumentFormatters
{
    /// <summary>
    ///     Стандартный
    /// </summary>
    public static readonly DefaultCommandArgumentFormatter Default = new();

    /// <summary>
    ///     Linux-like
    /// </summary>
    public static class Linux
    {
        /// <summary>
        ///     Аргумент в формате `--NAME value`
        /// </summary>
        public static readonly LongArgumentFormatter LongArgument = new();

        /// <summary>
        ///     Аргумент в формате `-NAME value`
        /// </summary>
        public static readonly ShortArgumentFormatter ShortArgument = new();
    }
}