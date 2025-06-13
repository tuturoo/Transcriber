using CommandWrapper.Core.Abstractions;
using CommandWrapper.Sox.Positions.Target.Arguments;

namespace CommandWrapper.Sox.Positions.Target;

public sealed class TargetPosition : CommandPosition
{
    public readonly TargetArgument Target;
    
    internal TargetPosition(Constants.Priorities priority) : base((int) priority)
    {
        Target = new TargetArgument();
    }
    
    protected override IEnumerable<CommandArgument>? Arguments => [ Target ];
}