using System.Text;

namespace Douro.Values;

public class PrintExpr(Expr expr) : Expr
{
    public Expr Expr => expr;

    public override string ToString(int depth)
    {
        var sb = new StringBuilder();
        var indent = String.Empty.PadLeft(depth);
        sb.AppendLine(indent + "print:");
        sb.AppendLine(expr.ToString(depth + 1));
        return sb.ToString();
    }
}