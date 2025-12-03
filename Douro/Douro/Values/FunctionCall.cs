using System.Text;

namespace Douro.Values;

public class FunctionCall(string name, List<Expr> args) : Expr
{
    public string Name => name;
    public List<Expr> Args => args;

    public override string ToString(int depth)
    {
        var sb = new StringBuilder();
        var indent = String.Empty.PadLeft(depth);
        sb.AppendLine(indent + $"call: {name}");
        foreach (var arg in args)
        {
            sb.AppendLine(arg.ToString(depth + 1));
        }

        return sb.ToString();
    }
}