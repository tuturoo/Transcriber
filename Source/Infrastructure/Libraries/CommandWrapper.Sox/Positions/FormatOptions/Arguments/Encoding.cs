using CommandWrapper.Core.Abstractions;
using CommandWrapper.Core.Constants;
using CommandWrapper.Core.Exceptions;

namespace CommandWrapper.Sox.Positions.FormatOptions.Arguments
{
    public sealed class Encoding : CommandArgument
    {
        private string? _type = null;

        public string? Type
        {
            get => _type;
            set => SetValue(ref _type, value);
        }

        internal Encoding() : base(ArgumentFormatters.Linux.ShortArgument)
        { }

        protected override string? Name => "e";

        protected override string? Value
        {
            get
            {
                if (Type is null)
                    throw new NotValidArgumentException("...", nameof(Type));

                return Type;
            }
        }
    }
}
