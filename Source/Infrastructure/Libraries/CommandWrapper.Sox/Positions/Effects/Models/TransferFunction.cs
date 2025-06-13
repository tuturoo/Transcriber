using System.Globalization;
using System.Text;

namespace CommandWrapper.Sox.Positions.Effects.Models;

internal sealed class TransferFunction(double attack, double decay, IEnumerable<double> decibelTable)
{
    public double Attack = attack;

    public double Decay = decay;

    public IEnumerable<double> DecibelTable = decibelTable;

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.Append(Attack.ToString(CultureInfo.InvariantCulture) + ",");
        builder.Append(Decay.ToString(CultureInfo.InvariantCulture) + " ");

        builder.AppendJoin(",", decibelTable.Select(d => d.ToString(CultureInfo.InvariantCulture)));
        
        return builder.ToString();
    }
}