using System.Globalization;
using System.Text;
using CommandWrapper.Core.Abstractions;
using CommandWrapper.Core.Constants;
using CommandWrapper.Core.Exceptions;
using CommandWrapper.Sox.Positions.Effects.Models;

namespace CommandWrapper.Sox.Positions.Effects.Arguments;

public sealed class CompandArgument : CommandArgument
{
    private double _gain;

    private double? _initialLevel;

    private double? _delay;
    
    private readonly List<TransferFunction> _transferFunctions = [];

    public CompandArgument AddTransferFunction(double attack, double decay, params double[] decibelTable)
    {
        HasChanges = true;
        
        var function = new TransferFunction(attack, decay, decibelTable);
        
        _transferFunctions.Add(function);
        
        return this;
    }

    public double Gain
    {
        get => _gain;
        set => SetValue(ref _gain, value);
    }

    public double? InitialLevel
    {
        get => _initialLevel;
        set => SetValue(ref _initialLevel, value);
    }

    public double? Delay
    {
        get => _delay;
        set => SetValue(ref _delay, value);
    }
    
    internal CompandArgument() : base(ArgumentFormatters.Default)
    { }

    protected override string? Name => "compand";

    protected override string? Value
    {
        get
        {
            if (_transferFunctions.Count == 0)
                throw new NotValidArgumentException("...", nameof(_transferFunctions));
            
            var builder = new StringBuilder();

            builder.AppendJoin(" ", _transferFunctions);

            builder.Append(" " + Gain.ToString(CultureInfo.InvariantCulture));

            if (InitialLevel is not null)
                builder.Append(" " + InitialLevel.Value.ToString(CultureInfo.InvariantCulture));
            
            if (Delay is not null)
                builder.Append(" " + Delay.Value.ToString(CultureInfo.InvariantCulture));

            return builder.ToString();
        }
    }
}