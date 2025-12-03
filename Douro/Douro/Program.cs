using Douro;
using Douro.Statements;

if (args.Length < 1) {
	Console.WriteLine("Usage: Douro <source-file>");
	Environment.Exit(1);
}

string filename = args[0];
string source = File.ReadAllText(filename);

DouroEnvironment env = new();
DouroParser parser = new();
DouroEngine engine = new(env);

Console.WriteLine(source);
DouroProgram? program = parser.Parse(source);
Console.WriteLine(program);
Console.WriteLine("============");
engine.Run(program);
