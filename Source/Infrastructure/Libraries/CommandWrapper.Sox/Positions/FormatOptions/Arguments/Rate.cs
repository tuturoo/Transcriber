using CommandWrapper.Core.Abstractions;
using CommandWrapper.Core.Constants;
using CommandWrapper.Core.Exceptions;

namespace CommandWrapper.Sox.Positions.FormatOptions.Arguments;

public sealed class Rate : CommandArgument
{
    private ulong? _frequency = null;

    public ulong? Frequency
    {
        get => _frequency;
        set => SetValue(ref _frequency, value);
    }

    internal Rate() : base(ArgumentFormatters.Linux.ShortArgument)
    { }

    protected override string? Name => "r";

    protected override string? Value
    {
        get
        {
            if (Frequency is null or 0)
                throw new NotValidArgumentException("...", nameof(Frequency));

            return Frequency.ToString();
        }
    }
}