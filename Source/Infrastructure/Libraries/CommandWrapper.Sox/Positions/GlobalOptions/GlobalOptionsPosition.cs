using CommandWrapper.Core.Abstractions;
using CommandWrapper.Sox.Positions.GlobalOptions.Arguments;

namespace CommandWrapper.Sox.Positions.GlobalOptions;

public sealed class GlobalOptionsPosition : CommandPosition
{
    public readonly BufferArgument Buffer = new BufferArgument();
    
    internal GlobalOptionsPosition() : base((int) Constants.Priorities.GlobalOptions)
    {
        
    }

    protected override IEnumerable<CommandArgument>? Arguments =>
    [
        Buffer
    ];
}