using System.Diagnostics;

namespace Mapgen;

public static class FileHelpers
{
	public static void OpenWithDefaultApp(string path)
	{
		var process = new Process
		{
			StartInfo = new ProcessStartInfo(Path.Combine(Environment.CurrentDirectory, path))
			{
				UseShellExecute = true
			}
		};
		process.Start();
	}
}
