#if !NETSTANDARD2_0
namespace MichMcb.CsExt.Strings
{
	using System;

	internal readonly struct LexedIso8601
	{
		internal readonly Range Year;
		internal readonly Range Month;
		internal readonly Range Day;
		internal readonly Range Hour;
		internal readonly Range Minute;
		internal readonly Range Second;
		internal readonly Range Millis;
		internal readonly char TimezoneChar;
		internal readonly Range TimezoneHours;
		internal readonly Range TimezoneMinutes;
		/// <summary>
		/// The parts found in the lexed string. Note that if a portion is too small to have separators, it's detected as not having them.
		/// </summary>
		internal readonly Iso8601Parts PartsFound;
		internal LexedIso8601(Range year, Range month, Range day, Range hour, Range minute, Range second, Range millis, char timezoneChar, Range timezoneHours, Range timezoneMinutes, Iso8601Parts partsFound)
		{
			if (timezoneChar != '\0' && timezoneChar != 'Z' && timezoneChar != '+' && timezoneChar != '-')
			{
				throw new ArgumentOutOfRangeException(nameof(timezoneChar), "Timezone Character must be either Z, +, -, or \0 (null)");
			}
			Year = year;
			Month = month;
			Day = day;
			Hour = hour;
			Minute = minute;
			Second = second;
			Millis = millis;
			TimezoneChar = timezoneChar;
			TimezoneHours = timezoneHours;
			TimezoneMinutes = timezoneMinutes;
			PartsFound = partsFound;
		}
	}
}
#endif