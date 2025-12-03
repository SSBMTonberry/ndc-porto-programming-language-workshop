using Douro.Values;

namespace Douro.Statements;

public class Assign(string name, Expr expr) : ExprStatement(expr)
{
    public string Name => name;
}