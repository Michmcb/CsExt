namespace MichMcb.CsExt.Dates
{
	using System;

	/// <summary>
	/// Has various constants to help convert between Unix Time and .NET Time,
	/// and also various constants 
	/// </summary>
	public static class DotNetTime
	{
		/// <summary>
		/// Days in a non-leap year
		/// </summary>
		public const int DaysPerYear = 365;
		/// <summary>
		/// Every 4 years, there's 1 leap year.
		/// </summary>
		public const int DaysPer4Years = 365 * 3 + 366;
		/// <summary>
		/// In 100 years, there's 24 leap years and 76 non-leap years (a year divisible by 100 is NOT a leap year)
		/// </summary>
		public const int DaysPer100Years = (365 * 76) + (366 * 24);
		/// <summary>
		/// In 400 years, there's 97 leap years and 303 non-leap years (a year divisible by 400 IS a leap year)
		/// </summary>
		public const int DaysPer400Years = DaysPer100Years * 4 + 1;
		/// <summary>
		/// 9999-12-31 23:59:59.9999999, represented as ticks elapsed since 0001-01-01 00:00:00
		/// </summary>
		public const long MaxTicks = 3155378975999999999;
		/// <summary>
		/// 1970-01-01 00:00:00, represented as days elapsed since 0001-01-01 00:00:00
		/// </summary>
		public const int UnixEpochDays = DaysPer400Years * 4 + DaysPer100Years * 3 + DaysPer4Years * 17 + DaysPerYear;
		/// <summary>
		/// 1970-01-01 00:00:00, represented as ticks elapsed since 0001-01-01 00:00:00
		/// </summary>
		public const long UnixEpochTicks = TimeSpan.TicksPerDay * UnixEpochDays;
		/// <summary>
		/// 1970-01-01 00:00:00, represented as seconds elapsed since 0001-01-01 00:00:00
		/// </summary>
		public const long UnixEpochSeconds = UnixEpochDays * 24L * 60L * 60L; // Days -> Hours -> Minutes -> Seconds
		/// <summary>
		/// 0001-01-01 00:00:00, represented as seconds elapsed since 1970-01-01 00:00:00
		/// </summary>
		public const long MinSecondsAsUnixTime = -UnixEpochSeconds;
		/// <summary>
		/// 9999-12-31 23:59:59, represented as seconds elapsed since 1970-01-01 00:00:00
		/// </summary>
		public const long MaxSecondsAsUnixTime = 315537897599 - UnixEpochSeconds;
		/// <summary>
		/// Returns the number of seconds elapsed since 1970-01-01 00:00:00.
		/// </summary>
		/// <param name="ticks">The number of ticks elapsed since 0000-01-01 00:00:00</param>
		public static long TicksToUnixTimeSeconds(long ticks)
		{
			return (ticks - UnixEpochTicks) / TimeSpan.TicksPerSecond;
		}
		/// <summary>
		/// Returns the number of ticks elapsed since 0000-01-01 00:00:00.
		/// </summary>
		/// <param name="seconds">The number of seconds elapsed since 1970-01-01 00:00:00</param>
		public static long UnixTimeSecondsToTicks(long seconds)
		{
			return (seconds * TimeSpan.TicksPerSecond) + UnixEpochTicks;
		}
	}
}
