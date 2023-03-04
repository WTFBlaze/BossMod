namespace CheetoClient;

using System;

public static class Randomizer
{
	private static readonly Random _random = new();

	/// <summary>
	/// Returns a non negative byte that is less than the specified maximum
	/// </summary>
	/// <param name="max"></param>
	/// <returns></returns>
	public static byte Byte(byte max)
	{
		return Convert.ToByte(_random.Next(max));
	}

	public static float Float()
	{
		Random rand = new();
		const double range = (double) float.MaxValue - float.MinValue;
		double sample = rand.NextDouble();
		double scaled = (sample * range) + float.MinValue;
		return (float) scaled;
	}
}