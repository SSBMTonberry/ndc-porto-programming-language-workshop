using Douro.Values;

namespace Douro.Statements;

public class Return(Expr expr) : ExprStatement(expr)
{
}