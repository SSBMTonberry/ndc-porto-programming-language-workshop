using System.Text;
using Douro.Values;

namespace Douro.Statements;

public abstract class ExprStatement(Expr expr) : Statement
{
    public Expr Expr => expr;

    public override string ToString(int depth = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{this.GetType().Name.ToLowerInvariant()}:");
        sb.AppendLine(expr.ToString(depth + 1));
        return sb.ToString();
    }
}