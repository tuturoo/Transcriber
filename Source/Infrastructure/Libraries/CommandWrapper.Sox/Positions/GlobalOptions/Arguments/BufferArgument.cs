using CommandWrapper.Core.Abstractions;
using CommandWrapper.Core.Constants;
using CommandWrapper.Core.Exceptions;

namespace CommandWrapper.Sox.Positions.GlobalOptions.Arguments;

public sealed class BufferArgument : CommandArgument
{
    private ulong _size = default;

    public ulong Size
    {
        get => _size;
        set => SetValue(ref _size, value);
    }

    internal BufferArgument() : base(ArgumentFormatters.Linux.LongArgument)
    {
        
    }

    protected override string? Name => "buffer";

    protected override string? Value
    {
        get
        {
            if (Size == 0)
                throw new NotValidArgumentException("...", nameof(Size));

            return Size.ToString();
        } 
    }
}