using System.Diagnostics;
using System.Text;
using Mapgen;
using SkiaSharp;
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
AnsiConsole.MarkupLineInterpolated($"Placing pixel [bold green]0[/] at [bold]w: [red]{startW}[/], h: [blue]{startH}[/][/]");

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

var files = AnsiConsole.Prompt(new MultiSelectionPrompt<string>()
	.Title("Select files to generate")
	.PageSize(10)
	.AddChoices(["image", "blurred image", "text", "pretty text"])
);

if (files.Contains("image"))
{
	AnsiConsole.MarkupLine("[purple bold]Generate image[/]");
	var path = AnsiConsole.Ask<string>("Output file:", "./grid.png");
	
	var img = CreateImage(imageArr, 10);
	await File.WriteAllBytesAsync(path, img);
	
	AnsiConsole.MarkupLineInterpolated($"File saved in [green bold]{path}[/]");
	if (AnsiConsole.Confirm("Open file?"))
	{
		FileHelpers.OpenWithDefaultApp(path);
	}
}

if (files.Contains("blurred image"))
{
	AnsiConsole.MarkupLine("[purple bold]Generate blurred image[/]");
	var path = AnsiConsole.Ask<string>("Output file:", "./grid-blurred.png");
	
	var img = CreateImage(imageArr, 10, 4);
	await File.WriteAllBytesAsync(path, img);
	
	AnsiConsole.MarkupLineInterpolated($"File saved in [green bold]{path}[/]");
	if (AnsiConsole.Confirm("Open file?"))
	{
		FileHelpers.OpenWithDefaultApp(path);
	}
}

if (files.Contains("text"))
{
	AnsiConsole.MarkupLine("[purple bold]Generate text file[/]");
	var path = AnsiConsole.Ask<string>("Output file:", "./grid.txt");
	
	await File.WriteAllTextAsync(path, imageArr.ToStringGrid());
	
	AnsiConsole.MarkupLineInterpolated($"File saved in [green bold]{path}[/]");
	if (AnsiConsole.Confirm("Open file?"))
	{
		FileHelpers.OpenWithDefaultApp(path);
	}
}

if (files.Contains("pretty text"))
{
	AnsiConsole.MarkupLine("[purple bold]Generate pretty-printed text file[/]");
	var path = AnsiConsole.Ask<string>("Output file:", "./grid-pretty.txt");
	
	await File.WriteAllTextAsync(path, imageArr.ToStringGrid(true));
	
	AnsiConsole.MarkupLineInterpolated($"File saved in [green bold]{path}[/]");
	if (AnsiConsole.Confirm("Open file?"))
	{
		FileHelpers.OpenWithDefaultApp(path);
	}
}
return;

byte[] CreateImage(int[,] arr, float? scale = null, float? blur = null)
{
	var wSize = arr.GetLength(0);
	var hSize = arr.GetLength(1);
	
	var bmp = new SKBitmap(wSize, hSize);
	using (var canvas = new SKCanvas(bmp))
	{
		for (var w = 0; w < wSize; w++)
		{
			for (var h = 0; h < hSize; h++)
			{
				canvas.DrawPoint(w, h, arr[w, h] == 0 ? SKColors.White : SKColors.Black);
			}
		}

		canvas.Save();
		if (scale is {} s)
		{
			canvas.Scale(s);
		}
	}

	if (blur is {} b)
	{
		var newBmp = new SKBitmap((int)(wSize * scale ?? 1), (int)(hSize * scale ?? 1));
		using var canvas = new SKCanvas(newBmp);
		using var paint = new SKPaint();
		
		paint.ImageFilter = SKImageFilter.CreateBlur(b, b);
		canvas.DrawImage(SKImage.FromBitmap(bmp), 0, 0, paint);
		
		using var blurredImage = SKImage.FromBitmap(newBmp);
		return blurredImage.Encode().ToArray();
	}

	using var image = SKImage.FromBitmap(bmp);
	return image.Encode().ToArray();
}
