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
		public static void GetTimeParts(this DateTime dt, out int hour, out int minute, out int second, out int millis)
		{
			UtcDateTime.TimePartsFromTotalMilliseconds(dt.Ticks / TimeSpan.TicksPerMillisecond, out hour, out minute, out second, out millis);
		}
		/// <summary>
		/// Calculates a year/month/day given <paramref name="dt"/>.
		/// </summary>
		public static void GetDateParts(this DateTime dt, out int year, out int month, out int day)
		{
			UtcDateTime.DatePartsFromTotalDays((int)(dt.Ticks / TimeSpan.TicksPerDay), out year, out month, out day);
		}
		/// <summary>
		/// Calculates a year/month/day/hour/minute/second/millisecond given <paramref name="dt"/>.
		/// </summary>
		public static void Deconstruct(this DateTime dt, out int year, out int month, out int day, out int hour, out int minute, out int second, out int millis)
		{
			UtcDateTime.DateTimePartsFromTotalMilliseconds(dt.Ticks / TimeSpan.TicksPerMillisecond, out year, out month, out day, out hour, out minute, out second, out millis);
		}
		/// <summary>
		/// Converts a <see cref="DateTime"/> to seconds that have elapsed since 1970-01-01 00:00:00.
		/// The provided <see cref="DateTime"/> is converted to Utc (using <see cref="DateTime.ToUniversalTime()"/>) if its Kind is Local or Unspecified.
		/// </summary>
		/// <returns>The number of seconds</returns>
		public static long ToUnixTimeSeconds(this DateTime dt)
		{
			return dt.Kind == DateTimeKind.Utc
				? ((dt.Ticks - UtcDateTime.UnixEpochTicks) / TimeSpan.TicksPerSecond)
				: ((dt.ToUniversalTime().Ticks - UtcDateTime.UnixEpochTicks) / TimeSpan.TicksPerSecond);
		}
		/// <summary>
		/// Converts a <see cref="DateTime"/> to milliseconds that have elapsed since 1970-01-01 00:00:00.
		/// The provided <see cref="DateTime"/> is converted to Utc (using <see cref="DateTime.ToUniversalTime()"/>) if its Kind is Local or Unspecified.
		/// </summary>
		/// <returns>The number of milliseconds</returns>
		public static long ToUnixTimeMilliseconds(this DateTime dt)
		{
			return dt.Kind == DateTimeKind.Utc
				? ((dt.Ticks - UtcDateTime.UnixEpochTicks) / TimeSpan.TicksPerMillisecond)
				: ((dt.ToUniversalTime().Ticks - UtcDateTime.UnixEpochTicks) / TimeSpan.TicksPerMillisecond);
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
