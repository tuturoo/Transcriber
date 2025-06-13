using System.Globalization;
using CommandWrapper.Core.Abstractions;
using CommandWrapper.Core.Constants;

namespace CommandWrapper.Sox.Positions.Effects.Arguments;

public sealed class NormalizeArgument : CommandArgument
{
    private double _decibelLevel;

    public double DecibelLevel
    {
        get => _decibelLevel;
        set => SetValue(ref _decibelLevel, value);
    }
    
    internal NormalizeArgument() : base(ArgumentFormatters.Default)
    {
    }

    protected override string? Name => "norm";

    protected override string? Value => DecibelLevel.ToString(CultureInfo.InvariantCulture);
}