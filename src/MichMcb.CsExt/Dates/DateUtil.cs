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
		/// <summary>
		/// Parses <paramref name="str"/> as yyyy-MM-dd or yyyyMMdd. String must be exactly 10 or 8 chars long.
		/// </summary>
		/// <param name="str">The string to parse.</param>
		/// <returns>The parsed Date, or an error message.</returns>
#if NET6_0_OR_GREATER
		public static Maybe<DateOnly, string> ParseYearMonthDay(in ReadOnlySpan<char> str)
#else
		public static Maybe<DateTime, string> ParseYearMonthDay(in ReadOnlySpan<char> str)
#endif
		{
			return str.Length switch
			{
				// yyyy-MM-dd
				// 0123456789
				10 => Parse.LatinInt(str.Slice(0, 4)).Success(out int y, out string err) && Parse.LatinInt(str.Slice(5, 2)).Success(out int m, out err) && Parse.LatinInt(str.Slice(8)).Success(out int d, out err)
					? UtcDateTime.TotalDaysFromYearMonthDay(y, m, d).Success(out int totalDays, out err)
#if NET6_0_OR_GREATER
						? DateOnly.FromDayNumber(totalDays)
#else
						? new DateTime(TimeSpan.TicksPerDay * totalDays)
#endif
						: err
					: err,
				// yyyyMMdd
				// 01234567
				8 => Parse.LatinInt(str.Slice(0, 4)).Success(out int y, out string err) && Parse.LatinInt(str.Slice(4, 2)).Success(out int m, out err) && Parse.LatinInt(str.Slice(6)).Success(out int d, out err)
					? UtcDateTime.TotalDaysFromYearMonthDay(y, m, d).Success(out int totalDays, out err)
#if NET6_0_OR_GREATER
						? DateOnly.FromDayNumber(totalDays)
#else
						? new DateTime(TimeSpan.TicksPerDay * totalDays)
#endif
						: err
					: err,
				_ => Compat.StringConcat("String was not either 10 or 8 chars long: ".AsSpan(), str),
			};
		}
	}
}
