using Spectre.Console;

namespace Mapgen.FileGenerators;

public class TextFileGenerator(int[,] imageArr) : FileGeneratorBase(imageArr, "./grid.txt")
{
	protected override Func<Task> GenerateMethod => async () =>
	{
		await File.WriteAllTextAsync(DefaultFile, ImageArr.ToStringGrid());
		AnsiConsole.MarkupLineInterpolated($"File saved in [green bold]{DefaultFile}[/]");
	};
}
