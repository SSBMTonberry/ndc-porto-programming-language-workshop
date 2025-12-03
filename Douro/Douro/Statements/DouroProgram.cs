using System.Text;

namespace Douro.Statements;

public class DouroProgram
{
    private readonly List<Statement> _statements = [];
    public List<Statement> Statements => _statements;

    public DouroProgram Insert(Statement statement)
    {
        _statements.Insert(0, statement);
        return this;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var s in Statements) sb.AppendLine(s.ToString());
        return sb.ToString();
    }
}