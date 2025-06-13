using CommandWrapper.Core.Abstractions;
using CommandWrapper.Core.Constants;
using CommandWrapper.Core.Exceptions;

namespace CommandWrapper.Sox.Positions.FormatOptions.Arguments
{
    public sealed class Channels : CommandArgument
    {
        private ulong? _count = null;

        public ulong? Count
        {
            get => _count;
            set => SetValue(ref _count, value);
        }

        internal Channels() : base(ArgumentFormatters.Linux.ShortArgument)
        { }

        protected override string? Name => "c";

        protected override string? Value
        {
            get
            {
                if (Count is null or 0)
                    throw new NotValidArgumentException("...", nameof(Count));

                return Count.ToString();
            }
        }
    }
}
