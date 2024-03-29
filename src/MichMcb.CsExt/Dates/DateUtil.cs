﻿namespace MichMcb.CsExt.Dates
{
	using System;
	/// <summary>
	/// A utility class for Dates. Also contains extension methods for <see cref="DateTime"/>.
	/// </summary>
	public static class DateUtil
	{
		/// <summary>
		/// Checks to make sure <paramref name="year"/> and <paramref name="dayOfYear"/> are within the legal range.
		/// </summary>
		/// <returns>Null on success, erorr message otherwise.</returns>
		public static string? ValidateOrdinalDays(int year, int dayOfYear)
		{
			if (year < 1 || year > 9999)
			{
				return "Year must be at least 1 and at most 9999";
			}
			int d = DateTime.IsLeapYear(year) ? 366 : 365;
			if (dayOfYear < 1 || dayOfYear > d)
			{
				return "Day must be at least 1 and at most " + d;
			}
			return null;
		}
		/// <summary>
		/// Checks to make sure <paramref name="year"/>, <paramref name="week"/>, and <paramref name="day"/> are within the legal range.
		/// </summary>
		/// <returns>Null on success, erorr message otherwise.</returns>
		public static string? ValidateWeekDate(int year, int week, IsoDayOfWeek day)
		{
			if (year < 1 || year > 9999)
			{
				return "Year must be at least 1 and at most 9999";
			}
			if (week < 1 || week > 53)
			{
				return "Week must be at least 1 and at most 53";
			}
			if (day < IsoDayOfWeek.Monday || day > IsoDayOfWeek.Sunday)
			{
				return "Week Day must be at least 1 and at most 7";
			}
			return null;
		}
		/// <summary>
		/// Checks to make sure <paramref name="year"/>, <paramref name="month"/>, and <paramref name="day"/> are within the legal range.
		/// </summary>
		/// <returns>Null on success, erorr message otherwise.</returns>
		public static string? ValidateDate(int year, int month, int day)
		{
			if (year < 1 || year > 9999)
			{
				return "Year must be at least 1 and at most 9999";
			}
			if (month < 1 || month > 12)
			{
				return "Month must be at least 1 and at most 12";
			}
			if (day < 1 || day > DateTime.DaysInMonth(year, month))
			{
				return string.Concat("Day must be at least 1 and, for the provided month (", month, "), at most ", DateTime.DaysInMonth(year, month));
			}
			return null;
		}
		/// <summary>
		/// Checks to make sure <paramref name="hour"/>, <paramref name="minute"/>, <paramref name="second"/>, and <paramref name="millis"/> are within the legal range.
		/// </summary>
		/// <returns>Null on success, erorr message otherwise.</returns>
		public static string? ValidateTime(int hour, int minute, int second, int millis)
		{
			if (hour < 0 || hour > 23)
			{
				return "Hour must be at least 0 and at most 23";
			}
			if (minute < 0 || minute > 59)
			{
				return "Minute must be at least 0 and at most 59";
			}
			if (second < 0 || second > 59)
			{
				return "Second must be at least 0 and at most 59";
			}
			if (millis < 0 || millis > 999)
			{
				return "Millisecond must be at least 0 and at most 999";
			}
			return null;
		}
		/// <summary>
		/// Converts a Unix time expressed as the number of <paramref name="seconds"/> that have elapsed since 1970-01-01 00:00:00 UTC.
		/// </summary>
		/// <param name="seconds">The seconds.</param>
		/// <returns>A DateTime with a Kind of Utc.</returns>
		public static DateTime DateTimeFromUnixTimeSeconds(long seconds)
		{
			return new DateTime(seconds * TimeSpan.TicksPerSecond + DotNetTime.UnixEpochTicks, DateTimeKind.Utc);
		}
		/// <summary>
		/// Returns a string, either yyyy-MM-dd or yyyyMMdd.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <param name="dashes">Whether or not to include dashes.</param>
		/// <returns>A string in the form of yyyy-MM-dd if <paramref name="dashes"/> is true or yyyyMMdd if <paramref name="dashes"/> is false.</returns>
		public static string YearMonthDayToString(DateTime date, bool dashes)
		{
			return YearMonthDayToString(date.Year, date.Month, date.Day, dashes);
		}
#if NET6_0_OR_GREATER
		/// <summary>
		/// Returns a string, either yyyy-MM-dd or yyyyMMdd.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <param name="dashes">Whether or not to include dashes.</param>
		/// <returns>A string in the form of yyyy-MM-dd if <paramref name="dashes"/> is true or yyyyMMdd if <paramref name="dashes"/> is false.</returns>
		public static string YearMonthDayToString(DateOnly date, bool dashes)
		{
			return YearMonthDayToString(date.Year, date.Month, date.Day, dashes);
		}
#endif
		/// <summary>
		/// Returns a string, either yyyy-MM-dd or yyyyMMdd.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <param name="month">The month.</param>
		/// <param name="day">The day.</param>
		/// <param name="dashes">Whether or not to include dashes.</param>
		/// <returns>A string in the form of yyyy-MM-dd if <paramref name="dashes"/> is true or yyyyMMdd if <paramref name="dashes"/> is false.</returns>
		public static string YearMonthDayToString(int year, int month, int day, bool dashes)
		{
#if NETSTANDARD2_0
			char[] str = new char[dashes ? 10 : 8];
			WriteYearMonthDay(str, year, month, day, dashes);
			return new string(str);
#else
			return string.Create(dashes ? 10 : 8, (year, month, day, dashes), (dest, ymd) => WriteYearMonthDay(dest, ymd.year, ymd.month, ymd.day, ymd.dashes));
#endif
		}
		/// <summary>
		/// Writes as yyyy-MM-dd or yyyyMMdd to <paramref name="str"/>, which must be able to hold 8 or 10 chars.
		/// </summary>
		/// <param name="str">The span into which to write the date.</param>
		/// <param name="year">The year.</param>
		/// <param name="month">The month.</param>
		/// <param name="day">The day.</param>
		/// <param name="dashes">Whether or not to include dashes.</param>
		/// <returns>The number of chars written, or the length required as a negative number on failure.</returns>
		public static int WriteYearMonthDay(Span<char> str, int year, int month, int day, bool dashes)
		{
			if (dashes)
			{
				if (str.Length < 10)
				{
					return -10;
				}
				// 0123456789
				// yyyy-MM-dd
				str[4] = '-';
				str[7] = '-';
				Formatting.Write4Digits((uint)year, str, 0);
				Formatting.Write2Digits((uint)month, str, 5);
				Formatting.Write2Digits((uint)day, str, 8);
				return 10;
			}
			else
			{
				if (str.Length < 8)
				{
					return -8;
				}
				// 01234567
				// yyyyMMdd
				Formatting.Write4Digits((uint)year, str, 0);
				Formatting.Write2Digits((uint)month, str, 4);
				Formatting.Write2Digits((uint)day, str, 6);
				return 8;
			}
		}
		/// <summary>
		/// Parses <paramref name="str"/> as yyyy-MM-dd or yyyyMMdd. String must be exactly 10 or 8 chars long.
		/// </summary>
		/// <param name="str">The string to parse.</param>
		/// <returns>The parsed Date, or an error message.</returns>
#if NET6_0_OR_GREATER
		public static Maybe<DateOnly, string> ParseYearMonthDay(ReadOnlySpan<char> str)
#else
		public static Maybe<DateTime, string> ParseYearMonthDay(ReadOnlySpan<char> str)
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
