namespace Douro.Statements;

public abstract class Statement
{
    public abstract string ToString(int depth);
    public override string ToString() => this.ToString(0);
}