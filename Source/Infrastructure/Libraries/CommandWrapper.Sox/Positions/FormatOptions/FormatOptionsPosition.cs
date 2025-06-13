using CommandWrapper.Core.Abstractions;
using CommandWrapper.Sox.Positions.FormatOptions.Arguments;
using Type = CommandWrapper.Sox.Positions.FormatOptions.Arguments.Type;

namespace CommandWrapper.Sox.Positions.FormatOptions;

public sealed class FormatOptionsPosition : CommandPosition
{
    public readonly Channels Channels = new Channels();

    public readonly Depth Depth = new Depth();

    public Encoding Encoding = new Encoding();

    public readonly Rate Rate = new Rate();

    public readonly Type Type = new Type();
    
    internal FormatOptionsPosition(Constants.Priorities priority) : base((int) priority)
    {
    }

    protected override IEnumerable<CommandArgument>? Arguments =>
    [
        Rate,
        Type,
        Channels,
        Depth,
        Encoding,
    ];
}