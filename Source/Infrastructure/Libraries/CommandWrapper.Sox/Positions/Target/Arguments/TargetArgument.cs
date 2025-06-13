using CommandWrapper.Core.Abstractions;
using CommandWrapper.Core.Constants;

namespace CommandWrapper.Sox.Positions.Target.Arguments;

public sealed class TargetArgument : CommandArgument
{
    internal TargetArgument() : base(ArgumentFormatters.Default)
    {
        HasChanges = true;
    }

    protected override string? Name => null;

    protected override string? Value => "-";
}