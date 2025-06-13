using CommandWrapper.Core.Abstractions;
using CommandWrapper.Sox.Positions.Effects.Arguments;

namespace CommandWrapper.Sox.Positions.Effects;

public sealed class EffectsPosition : CommandPosition
{
    public readonly NormalizeArgument Normalize = new NormalizeArgument();

    public readonly CompandArgument Compand = new CompandArgument();
    
    internal EffectsPosition() : base((int) Constants.Priorities.Effects)
    { }

    protected override IEnumerable<CommandArgument>? Arguments =>
    [
        Normalize,
        Compand
    ];
}