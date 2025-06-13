using CommandWrapper.Core.Abstractions;
using CommandWrapper.Core.Constants;
using CommandWrapper.Core.Exceptions;

namespace CommandWrapper.Sox.Positions.FormatOptions.Arguments;

public sealed class Type : CommandArgument
{
    private string? _audioFormat = null;

    public string? AudioFormat
    {
        get => _audioFormat;
        set => SetValue(ref _audioFormat, value);
    }

    internal Type() : base(ArgumentFormatters.Linux.ShortArgument)
    { }

    protected override string? Name => "t";

    protected override string? Value
    {
        get
        {
            if (AudioFormat is null)
                throw new NotValidArgumentException("...", nameof(Format));

            return AudioFormat.ToString();
        }
    }
}