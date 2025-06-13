using CommandWrapper.Core.Abstractions;

namespace CommandWrapper.Core.Implementations.Formatters.LinuxLike;

/// <summary>
///     Форматирование в виде короткого аргумента (`-NAME value`)
/// </summary>
public sealed class ShortArgumentFormatter : DefaultCommandArgumentFormatter
{
    public ShortArgumentFormatter()
    {
        Start = "-";
        Between = " ";
        Quote = "\"";
    }

    protected override bool UseQuotes(CommandArgument argument)
    {
        return argument.Value!.Contains(" ");
    }
}