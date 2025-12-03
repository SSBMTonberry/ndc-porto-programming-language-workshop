using System.Text;
using Douro.Values;

namespace Douro.Statements;

public class Lookup(string name) : Expr
{
    public string Name => name;

    public override string ToString(int depth = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("lookup:");
        sb.AppendLine($"{String.Empty.PadLeft(depth + 1)}name: {name}");
        sb.AppendLine(name);
        return sb.ToString();
    }
}