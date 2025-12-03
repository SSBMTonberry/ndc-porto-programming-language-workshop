using System.Globalization;

namespace Douro.Values;

public class Number(decimal value) : Value
{
    public decimal Value => value;
    public Number(string digits) : this(Decimal.Parse(digits, CultureInfo.InvariantCulture))
    {
    }
    
    public override string ToString()
        => value.ToString(CultureInfo.InvariantCulture);

    public override string ToString(int depth)
        => $"{String.Empty.PadLeft(depth)}number: {Value}";
}