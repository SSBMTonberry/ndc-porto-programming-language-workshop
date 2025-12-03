using System.Diagnostics;

namespace Douro.Values;

public abstract class Value : Expr
{
    public static Value operator +(Value lhs, Value rhs)
    {
        if (lhs is Number lNum && rhs is Number rNum)
            return new Number(lNum.Value + rNum.Value);
        throw new InvalidOperationException($"Cannot add {lhs.GetType()} and {rhs.GetType()}");
    }

    public static Value operator -(Value lhs, Value rhs)
    {
        if (lhs is Number lNum && rhs is Number rNum)
            return new Number(lNum.Value - rNum.Value);
        throw new InvalidOperationException($"Cannot subtract {lhs.GetType()} and {rhs.GetType()}");
    }

    public static Value operator *(Value lhs, Value rhs)
    {
        if (lhs is Number lNum && rhs is Number rNum)
            return new Number(lNum.Value * rNum.Value);
        throw new InvalidOperationException($"Cannot multiply {lhs.GetType()} and {rhs.GetType()}");
    }

    public static Value operator /(Value lhs, Value rhs)
    {
        if (lhs is Number lNum && rhs is Number rNum)
            return new Number(lNum.Value / rNum.Value);
        throw new InvalidOperationException($"Cannot divide {lhs.GetType()} and {rhs.GetType()}");
    }
}