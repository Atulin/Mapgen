namespace Mapgen;

public static class NumberHelpers
{
	public static T Clamp<T>(this T number, T min, T max) where T : IComparable<T>
	{
		if (number.CompareTo(min) < 0) return min;
		if (number.CompareTo(max) > 0) return max;
		return number;
	}

	public static int Digits(this int number)
	{
		return ((number == 0) ? 1 : ((int)Math.Floor(Math.Log10(Math.Abs(number))) + 1));
	}

	public static string Pad(this int number, int length)
	{
		return number.ToString().PadLeft(length, ' ');
	}
}
