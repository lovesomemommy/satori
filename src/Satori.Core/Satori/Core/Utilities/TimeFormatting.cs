using System;

namespace Satori.Core.Utilities;

public static class TimeFormatting
{
	public static string FormatCountdown(TimeSpan remaining)
	{
		int num = Math.Max(0, (int)Math.Ceiling(remaining.TotalSeconds));
		int value = num / 60;
		int value2 = num % 60;
		return $"{value}:{value2:00}";
	}
}
