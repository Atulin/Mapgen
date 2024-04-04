using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using Spectre.Console;

namespace Mapgen.FileGenerators;

public class ImageFileGenerator(int[,] imageArr) : FileGeneratorBase(imageArr, "./grid.png")
{
	protected override Func<Task> GenerateMethod => async () =>
	{
		var wSize = ImageArr.GetLength(0);
		var hSize = ImageArr.GetLength(1);
		
		using var image = new Image<L16>(wSize, hSize);

		for (var w = 0; w < wSize; w++)
		{
			for (var h = 0; h < hSize; h++)
			{
				image[w, h] = new L16(ImageArr[w, h] == 1 ? ushort.MaxValue : ushort.MinValue);
			}
		}

		var scale = AnsiConsole.Ask("Image scale", 1);
		if (scale != 1)
		{
			image.Mutate(m => m.Resize(wSize * scale, hSize * scale, new NearestNeighborResampler()));
		}
		
		await image.SaveAsPngAsync(DefaultFile);
		
		AnsiConsole.MarkupLineInterpolated($"File saved in [green bold]{DefaultFile}[/]");
	};
}
