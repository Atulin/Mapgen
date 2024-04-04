using Spectre.Console;

namespace Mapgen.FileGenerators;

public abstract class FileGeneratorBase(int[,] imageArr, string defaultFile)
{
	protected readonly string DefaultFile = defaultFile;
	protected readonly int[,] ImageArr = imageArr;
	protected abstract Func<Task> GenerateMethod { get; }
	
	public async Task Generate()
	{
		var path = AnsiConsole.Ask("Output file path:", DefaultFile);

		await GenerateMethod();

		if (AnsiConsole.Confirm("Open file?"))
		{
			FileHelpers.OpenWithDefaultApp(path);
		}
	}
}
