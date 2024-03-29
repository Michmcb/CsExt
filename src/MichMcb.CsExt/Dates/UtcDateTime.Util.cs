﻿namespace MichMcb.CsExt.Dates
{
	using System;

	public readonly partial struct UtcDateTime
	{
		/// <summary>
		/// The total number of days in all months, no leap years
		/// </summary>
		internal static readonly int[] TotalDaysFromStartYearToMonth = new int[13] { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
		internal static readonly int[] TotalDaysFromStartLeapYearToMonth = new int[13] { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };
		#region Obsolete Stuff
		/// <summary>
		/// Calculates a day/month/year given <paramref name="totalDays"/>, which is interpreted as the number of days elapsed since 0001-01-01.
		/// </summary>
		[Obsolete("Just use .NET DateTime/DateOnly properties")]
		public static void DatePartsFromTotalDays(int totalDays, out int year, out int month, out int day)
		{
#if NET6_0_OR_GREATER
			DateOnly.FromDayNumber(totalDays).Deconstruct(out year, out month, out day);
#else
			new DateTime(totalDays * TimeSpan.TicksPerDay).GetDateParts(out year, out month, out day);
#endif
		}
		/// <summary>
		/// Calculates a year/month/day given <paramref name="ticks"/>, which is interpreted as the number of ticks elapsed since 0001-01-01.
		/// </summary>
		[Obsolete("Just use .NET DateTime properties")]
		public static void DatePartsFromTicks(long ticks, out int year, out int month, out int day)
		{
			DatePartsFromTotalDays((int)(ticks / TimeSpan.TicksPerDay), out year, out month, out day);
		}
		/// <summary>
		/// Calculates an hour/minute/second/millis/remainder given <paramref name="ticks"/>, which is interpreted as the number of ticks elapsed since 0001-01-01.
		/// </summary>
		[Obsolete("Just use .NET DateTime properties")]
		public static void TimePartsFromTicks(long ticks, out int hour, out int minute, out int second, out int millis, out int remainder)
		{
			hour = (int)(ticks / TimeSpan.TicksPerHour % 24);
			minute = (int)(ticks / TimeSpan.TicksPerMinute % 60);
			second = (int)(ticks / TimeSpan.TicksPerSecond % 60);
			millis = (int)(ticks / TimeSpan.TicksPerMillisecond % 1000);
			remainder = (int)(ticks % TimeSpan.TicksPerMillisecond);
		}
		/// <summary>
		/// Calculates an hour/minute/second/remainder given <paramref name="ticks"/>, which is interpreted as the number of ticks elapsed since 0001-01-01.
		/// </summary>
		[Obsolete("Just use .NET DateTime properties")]
		public static void TimePartsNoMillisFromTicks(long ticks, out int hour, out int minute, out int second, out int remainder)
		{
			hour = (int)(ticks / TimeSpan.TicksPerHour % 24);
			minute = (int)(ticks / TimeSpan.TicksPerMinute % 60);
			second = (int)(ticks / TimeSpan.TicksPerSecond % 60);
			remainder = (int)(ticks % TimeSpan.TicksPerSecond);
		}
		/// <summary>
		/// Calculates a year/month/day/hour/minute/second/millis/remainder given <paramref name="ticks"/>, which is interpreted as the number of ticks elapsed since 0001-01-01.
		/// </summary>
		[Obsolete("Just use .NET DateTime properties")]
		public static void DateTimePartsFromTicks(long ticks, out int year, out int month, out int day, out int hour, out int minute, out int second, out int millis, out int remainder)
		{
			TimePartsFromTicks(ticks, out hour, out minute, out second, out millis, out remainder);
			DatePartsFromTicks(ticks, out year, out month, out day);
		}
		/// <summary>
		/// Calculates a year/month/day/hour/minute/second/remainder given <paramref name="ticks"/>, which is interpreted as the number of ticks elapsed since 0001-01-01.
		/// </summary>
		[Obsolete("Just use .NET DateTime properties")]
		public static void DateTimePartsNoMillisFromTicks(long ticks, out int year, out int month, out int day, out int hour, out int minute, out int second, out int remainder)
		{
			TimePartsNoMillisFromTicks(ticks, out hour, out minute, out second, out remainder);
			DatePartsFromTicks(ticks, out year, out month, out day);
		}
		/// <summary>
		/// Returns the total number of days elapsed since 0001-01-01, as of <paramref name="year"/> and <paramref name="month"/>, or an error message if the fall out of the expected range.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <param name="month">The month.</param>
		/// <returns>Total days elapsed since 0001-01-01.</returns>
		[Obsolete("Use either TotalDaysFromYearMonthDay or TotalDaysFromYearOrdinalDays or TotalDaysFromYearWeekDay")]
		public static Maybe<int, string> TotalDaysFromYearMonth(int year, int month)
		{
			return month >= 1 && month <= 12
				? TotalDaysFromYear(year).Success(out int yd, out string err)
					? (yd + (DateTime.IsLeapYear(year) ? TotalDaysFromStartLeapYearToMonth : TotalDaysFromStartYearToMonth)[month - 1])
					: err
				: "Month must be at least 1 and at most 12";
		}
		#endregion
		/// <summary>
		/// Returns the total number of days elapsed since 0001-01-01, as of <paramref name="year"/>.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <returns>Total days elapsed since 0001-01-01.</returns>
		public static Maybe<int, string> TotalDaysFromYear(int year)
		{
			return year >= 1 && year <= 9999
				? UnsafeDate.TotalDaysFromYear(year)
				: "Year must be at least 1 and at most 9999";
		}
		/// <summary>
		/// Returns the total number of days elapsed since 0001-01-01, as of <paramref name="year"/>, <paramref name="month"/>, and <paramref name="day"/>, or an error message if any fall out of the expected range.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <param name="month">The month.</param>
		/// <param name="day">The day.</param>
		/// <returns>Total days elapsed since 0001-01-01, or an error message.</returns>
		public static Maybe<int, string> TotalDaysFromYearMonthDay(int year, int month, int day)
		{
			string? e = DateUtil.ValidateDate(year, month, day);
			if (e != null) return e;
			return TotalDaysFromYear(year).Success(out int yearDays, out string err)
				? (day - 1 + yearDays + (DateTime.IsLeapYear(year) ? TotalDaysFromStartLeapYearToMonth : TotalDaysFromStartYearToMonth)[month - 1])
				: err;
		}
		/// <summary>
		/// Calculates days elapsed since 0001-01-01 for a year and a day of year.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <param name="dayOfYear">The day of the year.</param>
		/// <returns>The total days elapsed since 0001-01-01, or an error message.</returns>
		public static Maybe<int, string> TotalDaysFromYearOrdinalDays(int year, int dayOfYear)
		{
			string? e = DateUtil.ValidateOrdinalDays(year, dayOfYear);
			if (e != null) return e;
			return TotalDaysFromYear(year).Success(out int yearDays, out string err)
				? (yearDays + (dayOfYear - 1))
				: err;
		}
		/// <summary>
		/// Calculates days elapsed since 0001-01-01 for a year, week of the year, and weekday, according to ISO-8601.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <param name="week">The ISO-8601 week of the year.</param>
		/// <param name="isoWeekDay">The day of the week. You can use <see cref="DayOfWeekExtensions.IsoDayOfWeek(DayOfWeek)"/> to convert.</param>
		/// <returns>The total milliseconds elapsed since 0001-01-01, or an error message.</returns>
		public static Maybe<int, string> TotalDaysFromYearWeekDay(int year, int week, IsoDayOfWeek isoWeekDay)
		{
			string? e = DateUtil.ValidateWeekDate(year, week, isoWeekDay);
			if (e != null) return e;
			if (TotalDaysFromYear(year).Failure(out int yearDays, out string err))
			{
				return err;
			}
			int iwd = (int)isoWeekDay;

			// Thanks to https://en.wikipedia.org/wiki/ISO_week_date#Calculating_an_ordinal_or_month_date_from_a_week_date

			// totalDays also happens to be January the 1st, so we can simply add 3 onto it to be able to get January the 4th.
			int jan4TotalDays = yearDays + 3;
			// Under ISO-8601, 1 is Monday, 7 is Sunday
			int weekdayOf4thJan = (jan4TotalDays % 7) + 1;

			// 1. Multiply week number by 7
			// 2. Add weekday number
			// 3. From this sum, subtract the correction for the year (Get the weekday of 4th january. Add 3)
			// 4. The result is the ordinal date which can be converted into a calendar date.
			int ordinalDate = week * 7 + iwd - (weekdayOf4thJan + 3);

			// Now that we know the specific day that year/week refers to, it's a simple matter to just add on days.
			return yearDays + ordinalDate - 1;
		}
		/// <summary>
		/// Calculates ticks elapsed since 0001-01-01 for a year, month, and day.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <param name="month">The month.</param>
		/// <param name="day">The day.</param>
		/// <returns>The total ticks elapsed since 0001-01-01, or an error message.</returns>
		public static Maybe<long, string> TicksFromYearMonthDay(int year, int month, int day)
		{
			return TotalDaysFromYearMonthDay(year, month, day).Success(out int totalDays, out string errMsg) ? totalDays * TimeSpan.TicksPerDay : errMsg;
		}
		/// <summary>
		/// Calculates ticks elapsed since 0001-01-01 for a year and a day of year.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <param name="dayOfYear">The day of the year.</param>
		/// <returns>The total ticks elapsed since 0001-01-01, or an error message.</returns>
		public static Maybe<long, string> TicksFromYearOrdinalDays(int year, int dayOfYear)
		{
			return TotalDaysFromYearOrdinalDays(year, dayOfYear).Success(out int totalDays, out string errMsg) ? totalDays * TimeSpan.TicksPerDay : errMsg;
		}
		/// <summary>
		/// Calculates ticks elapsed since 0001-01-01 for a year, week of the year, and weekday, according to ISO-8601.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <param name="week">The ISO-8601 week of the year.</param>
		/// <param name="isoWeekDay">The day of the week. You can use <see cref="DayOfWeekExtensions.IsoDayOfWeek(DayOfWeek)"/> to convert.</param>
		/// <returns>The total ticks elapsed since 0001-01-01, or an error message.</returns>
		public static Maybe<long, string> TicksFromYearWeekDay(int year, int week, IsoDayOfWeek isoWeekDay)
		{
			return TotalDaysFromYearWeekDay(year, week, isoWeekDay).Success(out int totalDays, out string err) ? totalDays * TimeSpan.TicksPerDay : err;
		}
		/// <summary>
		/// Calculates the total ticks in the provided hour, minute, second, milliseconds, and timezone offset.
		/// </summary>
		/// <param name="hour">The hour.</param>
		/// <param name="minute">The minute.</param>
		/// <param name="second">The second.</param>
		/// <param name="millis">The milliseconds.</param>
		/// <param name="tz">The timezone offset.</param>
		/// <returns>The total ticks, or an error message.</returns>
		public static Maybe<long, string> TicksFromHourMinuteSecondMillisTimezoneOffset(int hour, int minute, int second, int millis, Tz tz)
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

			return (hour * TimeSpan.TicksPerHour) + (minute * TimeSpan.TicksPerMinute) + (second * TimeSpan.TicksPerSecond) + (millis * TimeSpan.TicksPerMillisecond) - tz.Ticks;
		}
		/// <summary>
		/// Calculates the total ticks from all of the provided parts. <paramref name="extraTicks"/> is intended for sub-millisecond accuracy.
		/// </summary>
		/// <returns>The total ticks, or an error message.</returns>
		public static Maybe<long, string> TicksFromAll(int year, int month, int day, int hour, int minute, int second, int millis, int extraTicks)
		{
			return TicksFromYearMonthDay(year, month, day).Success(out long dTicks, out string errMsg)
				&& TicksFromHourMinuteSecondMillisTimezoneOffset(hour, minute, second, millis, default).Success(out long tTicks, out errMsg)
				? dTicks + tTicks + extraTicks
				: errMsg;
		}
	}
}
