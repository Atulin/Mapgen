using System.Text;

namespace Mapgen;

public static class ArrayHelpers
{
	public static unsafe void Fill<T>(this T[,] arr, T value) where T : unmanaged
	{
		fixed (T* start = &arr[0, 0])
		{
			T* s = start;
			var span = new Span<T>(s, arr.Length);
			span.Fill(value);
		}
	}

	private static readonly (int dW, int dH)[] Deltas =
	[
		(+0, +1),
		(+0, -1),
		(+1, +0),
		(-1, +0)
	];

	public static bool AnyNeighborEquals<T>(this T[,] arr, int w, int h, T value) where T : IEquatable<T>
	{
		var wMax = arr.GetLength(0) - 1;
		var hMax = arr.GetLength(1) - 1;

		foreach (var (dW, dH) in Deltas)
		{
			var resultW = (w + dW);
			if (resultW > wMax || resultW < 0) continue;
			
			var resultH = (h + dH);
			if (resultH > hMax || resultH < 0) continue;

			if (arr[resultW.Clamp(0, wMax), resultH.Clamp(0, hMax)].Equals(value)) return true;
		}
		return false;
	}

	public static bool TryGetRandomNeighbor<T>(this T[,] arr, int w, int h, out (int w, int h) neighbor)
	{
		var wMax = arr.GetLength(0) - 1;
		var hMax = arr.GetLength(1) - 1;

		var validDeltas = new List<(int w, int h)>();
		
		foreach (var (dW, dH) in Deltas)
		{
			var resultW = (w + dW);
			if (resultW > wMax || resultW < 0) continue;
	
			var resultH = (h + dH);
			if (resultH > hMax || resultH < 0) continue;

			validDeltas.Add((dW, dH));
		}

		if (validDeltas.Count > 0)
		{
			var (dW, dH) = validDeltas[Random.Shared.Next(0, validDeltas.Count)];
			neighbor = (w + dW, h + dH);
			return true;
		}
		
		neighbor = (-1, -1);
		return false;
	}

	public static string ToStringGrid<T>(this T[,] arr, bool pretty = false) where T : IEquatable<T>
	{
		var sb = new StringBuilder();
		for (var w = 0; w < arr.GetLength(0); w++)
		{
			for (var h = 0; h < arr.GetLength(1); h++)
			{
				if (pretty)
				{
					sb.Append(arr[w, h].Equals(0) ? ' ' : '\u2588');
				}
				else
				{
					sb.Append(arr[w, h]);
				}
			}
			sb.AppendLine();
		}
		return sb.ToString();
	}
}
