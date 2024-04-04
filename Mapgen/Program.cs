using System.Diagnostics;
using System.Text;
using Mapgen;
using Mapgen.FileGenerators;
using Spectre.Console;

Console.OutputEncoding = Encoding.UTF8;

AnsiConsole.MarkupLine("Mapgen");

var stopwatch = new Stopwatch();
stopwatch.Start();

var width = AnsiConsole.Ask<int>("Map [bold]width[/] in pixels:");
var height = AnsiConsole.Ask<int>("Map [bold]height[/] in pixels:");
var verbose = AnsiConsole.Confirm("Verbose output? [gray](Will slow down generation)[/]", false);

var imageArr = new int[width, height];
imageArr.Fill(0);

// Place first pixel
var startW = Random.Shared.Next((width / 5) * 2, (width / 5) * 4);
var startH = Random.Shared.Next((height / 5) * 2, (height / 5) * 4);

startW = AnsiConsole.Ask("Start location (width):", startW);
startH = AnsiConsole.Ask("Start location (height):", startH);

imageArr[startW, startH] = 1;
AnsiConsole.MarkupLineInterpolated($"Placing the starting pixel at [bold]w: [red]{startW}[/], h: [blue]{startH}[/][/]");

var maxPixels = imageArr.Length / 3;
var maxDigits = maxPixels.Digits();

AnsiConsole.Progress()
	.Columns([
		new SpinnerColumn(),
		new TaskDescriptionColumn(),
		new ProgressBarColumn(),
		new PercentageColumn(),
		new ElapsedTimeColumn(),
	])
	.Start(ctx =>
	{
		var task = ctx.AddTask("Generating map");

		var pixels = 1;
		while (pixels < maxPixels && !ctx.IsFinished)
		{ 
			int w, h;
			do
			{
				w = Random.Shared.Next(0, width);
				h = Random.Shared.Next(0, height);
			} while (imageArr[w, h] != 0);

			AnsiConsole.MarkupLineInterpolated($"[green]:play_button:[/] Starting pixel [bold green]{pixels.Pad(maxDigits)}[/] at [bold]w: [red]{w.Pad(width.Digits())}[/], h: [blue]{h.Pad(height.Digits())}[/][/]");

			imageArr[w, h] = 1;

			while (!imageArr.AnyNeighborEquals(w, h, 1))
			{
				if (!imageArr.TryGetRandomNeighbor(w, h, out var neighbor)) break;

				imageArr[w, h] = 0;
				(w, h) = neighbor;
				imageArr[w, h] = 1;

				if (verbose)
				{
					AnsiConsole.MarkupLineInterpolated($"[yellow]:clockwise_vertical_arrows:[/] Moving pixel [bold green]{pixels.Pad(maxDigits)}[/] to [bold]w: [red]{w.Pad(width.Digits())}[/], h: [blue]{h.Pad(height.Digits())}[/][/]");
				}
			}

			AnsiConsole.MarkupLineInterpolated($"[red]:stop_button:[/] Stopping pixel [bold green]{pixels.Pad(maxDigits)}[/] at [bold]w: [red]{w.Pad(width.Digits())}[/], h: [blue]{h.Pad(height.Digits())}[/][/]");
			pixels++;
			task.Increment(pixels / (double)maxPixels);
		}
	});

stopwatch.Stop();

AnsiConsole.MarkupLineInterpolated($"Generation took [green bold]{stopwatch.Elapsed}[/]");

var generators = new Dictionary<string, FileGeneratorBase>
{
	["text file"] = new TextFileGenerator(imageArr),
	["pretty text file"] = new PrettyTextFileGenerator(imageArr),
};

var fileTypes = AnsiConsole.Prompt(new MultiSelectionPrompt<string>()
	.Title("Select file types to generate")
	.PageSize(generators.Count < 3 ? 3 : generators.Count)
	.AddChoices(generators.Keys)
);

foreach (var fileType in fileTypes)
{
	await generators[fileType].Generate();
}
