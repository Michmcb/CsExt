namespace MichMcb.CsExt.Dates
{
	using System;
	/// <summary>
	/// Extensions methods for <see cref="DateTime"/>.
	/// </summary>
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Calculates an hour/minute/second/millisecond given <paramref name="dt"/>.
		/// </summary>
		public static void GetTimeParts(this DateTime dt, out int hour, out int minute, out int second, out int millis, out int remainder)
		{
			hour = dt.Hour;
			minute = dt.Minute;
			second = dt.Second;
			millis = dt.Millisecond;
			remainder = (int)(dt.Ticks % TimeSpan.TicksPerMillisecond);
		}
		/// <summary>
		/// Assigns the values of the year/month/day properties to <paramref name="year"/>, <paramref name="month"/>, and <paramref name="day"/>.
		/// </summary>
		public static void GetDateParts(this DateTime dt, out int year, out int month, out int day)
		{
			year = dt.Year;
			month = dt.Month;
			day = dt.Day;
		}
		/// <summary>
		/// Calculates a year/month/day/hour/minute/second/millisecond given <paramref name="dt"/>.
		/// </summary>
		public static void Deconstruct(this DateTime dt, out int year, out int month, out int day, out int hour, out int minute, out int second, out int millis, out int remainder)
		{
			GetDateParts(dt, out year, out month, out day);
			GetTimeParts(dt, out hour, out minute, out second, out millis, out remainder);
		}
		/// <summary>
		/// Converts a <see cref="DateTime"/> to seconds that have elapsed since 1970-01-01 00:00:00.
		/// The provided <see cref="DateTime"/> is converted to Utc (using <see cref="DateTime.ToUniversalTime()"/>) if its Kind is Local or Unspecified.
		/// </summary>
		/// <returns>The number of seconds</returns>
		public static long ToUnixTimeSeconds(this DateTime dt)
		{
			return dt.Kind == DateTimeKind.Utc
				? ((dt.Ticks - DotNetTime.UnixEpochTicks) / TimeSpan.TicksPerSecond)
				: ((dt.ToUniversalTime().Ticks - DotNetTime.UnixEpochTicks) / TimeSpan.TicksPerSecond);
		}
		/// <summary>
		/// Converts a <see cref="DateTime"/> to milliseconds that have elapsed since 1970-01-01 00:00:00.
		/// The provided <see cref="DateTime"/> is converted to Utc (using <see cref="DateTime.ToUniversalTime()"/>) if its Kind is Local or Unspecified.
		/// </summary>
		/// <returns>The number of milliseconds</returns>
		public static long ToUnixTimeMilliseconds(this DateTime dt)
		{
			return dt.Kind == DateTimeKind.Utc
				? ((dt.Ticks - DotNetTime.UnixEpochTicks) / TimeSpan.TicksPerMillisecond)
				: ((dt.ToUniversalTime().Ticks - DotNetTime.UnixEpochTicks) / TimeSpan.TicksPerMillisecond);
		}
		/// <summary>
		/// Returns a truncated instance so that it is only accurate to the part specified by <paramref name="truncateTo"/>.
		/// For example, if <paramref name="truncateTo"/> is Minute, then Seconds and Milliseconds are set to zero.
		/// Truncating days or months will cause them to be truncated to 1.
		/// </summary>
		public static DateTime Truncate(this DateTime dt, DateTimePart truncateTo)
		{
			return truncateTo switch
			{
				DateTimePart.Year => new(dt.Year, 1, 1, 0, 0, 0, dt.Kind),
				DateTimePart.Month => new(dt.Year, dt.Month, 1, 0, 0, 0, dt.Kind),
				DateTimePart.Day => dt.AddTicks(-dt.Ticks % TimeSpan.TicksPerDay),
				DateTimePart.Hour => dt.AddTicks(-dt.Ticks % TimeSpan.TicksPerHour),
				DateTimePart.Minute => dt.AddTicks(-dt.Ticks % TimeSpan.TicksPerMinute),
				DateTimePart.Second => dt.AddTicks(-dt.Ticks % TimeSpan.TicksPerSecond),
				DateTimePart.Millisecond => dt.AddTicks(-dt.Ticks % TimeSpan.TicksPerMillisecond),
				_ => throw new ArgumentOutOfRangeException(nameof(truncateTo), "Parameter was not a valid value for DateTimePart"),
			};
		}
	}
}
