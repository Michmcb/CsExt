namespace MichMcb.CsExt.Dates
{
	using System;
	/// <summary>
	/// A utility class for Dates. Also contains extension methods for <see cref="DateTime"/>.
	/// </summary>
	public static class DateUtil
	{
		/// <summary>
		/// Converts a Unix time expressed as the number of <paramref name="seconds"/> that have elapsed since 1970-01-01 00:00:00 UTC.
		/// </summary>
		/// <param name="seconds">The seconds.</param>
		/// <returns>A DateTime with a Kind of Utc.</returns>
		public static DateTime DateTimeFromUnixTimeSeconds(long seconds)
		{
			return new DateTime(seconds * TimeSpan.TicksPerSecond + UtcDateTime.UnixEpochTicks, DateTimeKind.Utc);
		}
		/// <summary>
		/// Converts a Unix time expressed as the number of <paramref name="milliseconds"/> that have elapsed since 1970-01-01 00:00:00 UTC.
		/// </summary>
		/// <param name="milliseconds">The milliseconds</param>
		/// <returns>A DateTime with a Kind of Utc</returns>
		public static DateTime DateTimeFromUnixTimeMilliseconds(long milliseconds)
		{
			return new DateTime(milliseconds * TimeSpan.TicksPerMillisecond + UtcDateTime.UnixEpochTicks, DateTimeKind.Utc);
		}
	}
}
