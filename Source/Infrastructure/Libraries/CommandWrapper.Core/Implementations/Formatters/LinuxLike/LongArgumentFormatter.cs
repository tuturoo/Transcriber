using CommandWrapper.Core.Abstractions;

namespace CommandWrapper.Core.Implementations.Formatters.LinuxLike;

/// <summary>
///     Форматирование в виде длинного аргумента (`--NAME value`)
/// </summary>
public sealed class LongArgumentFormatter : DefaultCommandArgumentFormatter
{
    public LongArgumentFormatter()
    {
        Start = "--";
        Between = " ";
        Quote = "\"";
    }

    protected override bool UseQuotes(CommandArgument argument)
    {
        return argument.Value!.Contains(" ");
    }
}