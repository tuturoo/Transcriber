using CommandWrapper.Core.Abstractions;
using CommandWrapper.Core.Constants;
using CommandWrapper.Core.Exceptions;

namespace CommandWrapper.Sox.Positions.FormatOptions.Arguments
{
    public sealed class Depth : CommandArgument
    {
        private ulong? _bits = null;

        public ulong? Bits
        {
            get => _bits;
            set => SetValue(ref _bits, value);
        }

        internal Depth() : base(ArgumentFormatters.Linux.ShortArgument)
        { }

        protected override string? Name => "b";

        protected override string? Value
        {
            get
            {
                if (Bits is null or 0)
                    throw new NotValidArgumentException("...", nameof(Bits));

                return Bits.ToString();
            }
        }
    }
}
