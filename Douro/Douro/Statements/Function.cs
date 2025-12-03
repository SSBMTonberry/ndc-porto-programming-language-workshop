using System.Text;
using Douro.Values;

namespace Douro.Statements;

public class Function(List<string> argumentNames, List<Statement> body) : Value
{
    public List<string> ArgumentNames => argumentNames;
    public List<Statement> Body => body;

    public override string ToString(int depth)
    {
        var sb = new StringBuilder();
        var indent = String.Empty.PadLeft(depth);
        sb.AppendLine(indent + $"function ({String.Join(",", argumentNames.ToArray())}) => ");
        foreach (var statement in body)
        {
            sb.AppendLine(statement.ToString(depth + 1));
        }

        return sb.ToString();
    }
}