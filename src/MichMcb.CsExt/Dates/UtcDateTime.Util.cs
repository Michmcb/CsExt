namespace MichMcb.CsExt.Dates
{
	using System;
	public readonly partial struct UtcDateTime
	{
		internal const int DaysToUnixEpoch = DaysPer400Years * 4 + DaysPer100Years * 3 + DaysPer4Years * 17 + DaysPerYear;
		internal const long UnixEpochTicks = TimeSpan.TicksPerDay * DaysToUnixEpoch;
		/// <summary>
		/// 1970-01-01 00:00:00, represented as milliseconds elapsed since 0001-01-01 00:00:00
		/// </summary>
		public const long UnixEpochMillis = 62135596800000;
		/// <summary>
		/// 0001-01-01 00:00:00, represented as milliseconds elapsed since 1970-01-01 00:00:00
		/// </summary>
		public const long MinMillisUnixEpoch = -UnixEpochMillis;
		/// <summary>
		/// 9999-12-31 23:59:59.999, represented as milliseconds elapsed since 1970-01-01 00:00:00
		/// </summary>
		public const long MaxMillisUnixEpoch = MaxMillis - UnixEpochMillis;
		/// <summary>
		/// 9999-12-31 23:59:59.999, represented as milliseconds elapsed since 0001-01-01 00:00:00
		/// </summary>
		public const long MaxMillis = 315537897599999;
		/// <summary>
		/// Milliseconds in a second
		/// </summary>
		public const long MillisPerSecond = 1000;
		/// <summary>
		/// Milliseconds in a minute
		/// </summary>
		public const long MillisPerMinute = MillisPerSecond * 60;
		/// <summary>
		/// Milliseconds in an hour
		/// </summary>
		public const long MillisPerHour = MillisPerMinute * 60;
		/// <summary>
		/// Milliseconds in an hour
		/// </summary>
		public const long MillisPerDay = MillisPerHour * 24;
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
		/// The total number of days in all months, no leap years
		/// </summary>
		internal static readonly int[] TotalDaysFromStartYearToMonth = new int[] { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
		internal static readonly int[] TotalDaysFromStartLeapYearToMonth = new int[] { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };
		/// <summary>
		/// Calculates a day/month/year given <paramref name="totalDays"/>, which is interpreted as the number of days elapsed since 0001-01-01.
		/// </summary>
		public static void DatePartsFromTotalDays(int totalDays, out int year, out int month, out int day)
		{
			// Most of this code is written by myself. However, the two Adjustments and the optimization of (days >> 5) + 1 comes from the .NET Source for DateTime

			// The way we'll do this is first, get the number of groups of 400 years, then deduct that from our totalDays.
			int y400 = totalDays / DaysPer400Years;
			totalDays -= DaysPer400Years * y400;
			int y100 = totalDays / DaysPer100Years;
			if (y100 == 4)
			{
				y100 = 3; // Adjustment
			}
			totalDays -= DaysPer100Years * y100;
			int y4 = totalDays / DaysPer4Years;
			totalDays -= DaysPer4Years * y4;
			int y1 = totalDays / 365;
			if (y1 == 4)
			{
				y1 = 3; // Adjustment 
			}

			totalDays -= y1 * 365;

			year = y400 * 400 + y100 * 100 + y4 * 4 + y1 + 1;

			// Here, our years are relative to year 1, not year 0, so we need to use a special leap year calculation
			bool leapYear = y1 == 3 && (y4 != 24 || y100 == 3);
			int[] totalDaysFromStartYearToMonth = leapYear ? TotalDaysFromStartLeapYearToMonth : TotalDaysFromStartYearToMonth;

			// Bitshifting right 5 bytes, because all months have less than 32 days.
			// It saves us a bit of checking in the loop
			month = (totalDays >> 5) + 1;
			while (totalDays >= totalDaysFromStartYearToMonth[month])
			{
				month++;
			}
			day = totalDays - totalDaysFromStartYearToMonth[month - 1] + 1;
		}
		/// <summary>
		/// Calculates an hour/minute/second/millisecond given <paramref name="totalMilliseconds"/>, which is interpreted as the number of milliseconds elapsed since 0001-01-01.
		/// </summary>
		public static void TimePartsFromTotalMilliseconds(long totalMilliseconds, out int hour, out int minute, out int second, out int millis)
		{
			hour = (int)(totalMilliseconds / MillisPerHour % 24);
			minute = (int)(totalMilliseconds / MillisPerMinute % 60);
			second = (int)(totalMilliseconds / MillisPerSecond % 60);
			millis = (int)(totalMilliseconds % MillisPerSecond);
		}
		/// <summary>
		/// Calculates a year/month/day/hour/minute/second/millisecond given <paramref name="totalMilliseconds"/>, which is interpreted as the number of milliseconds elapsed since 0001-01-01.
		/// </summary>
		public static void DateTimePartsFromTotalMilliseconds(long totalMilliseconds, out int year, out int month, out int day, out int hour, out int minute, out int second, out int millis)
		{
			TimePartsFromTotalMilliseconds(totalMilliseconds, out hour, out minute, out second, out millis);

			// We know the total number of days easily.
			DatePartsFromTotalDays((int)(totalMilliseconds / MillisPerDay), out year, out month, out day);
		}
		/// <summary>
		/// Returns the total number of days elapsed since 0001-01-01, as of <paramref name="year"/>.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <returns>Total days elapsed since 0001-01-01.</returns>
		public static Maybe<int, string> TotalDaysFromYear(int year)
		{
			--year;
			return year >= 0 && year <= 9998
				// Add extra leap year days; a leap year is divisible by 4, but not by 100, unless also divisible by 400.
				? (year * 365) + year / 4 - year / 100 + year / 400
				: "Year must be at least 1 and at most 9999";
		}
		/// <summary>
		/// Returns the total number of days elapsed since 0001-01-01, as of <paramref name="year"/> and <paramref name="month"/>, or an error message if the fall out of the expected range.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <param name="month">The month.</param>
		/// <returns>Total days elapsed since 0001-01-01.</returns>
		public static Maybe<int, string> TotalDaysFromYearMonth(int year, int month)
		{
			return month >= 1 && month <= 12
				? TotalDaysFromYear(year).Success(out int yd, out string err)
					? (yd + (DateTime.IsLeapYear(year) ? TotalDaysFromStartLeapYearToMonth : TotalDaysFromStartYearToMonth)[month - 1])
					: err
				: "Month must be at least 1 and at most 12";
		}
		/// <summary>
		/// Returns the total number of days elapsed since 0001-01-01, as of <paramref name="year"/> and <paramref name="month"/>, or an error message if the fall out of the expected range.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <param name="month">The month.</param>
		/// <param name="day">The day.</param>
		/// <returns>Total days elapsed since 0001-01-01.</returns>
		public static Maybe<int, string> TotalDaysFromYearMonthDay(int year, int month, int day)
		{
			return TotalDaysFromYearMonth(year, month).Success(out int totalDays, out string err)
				? day >= 1 && day <= DateTime.DaysInMonth(year, month)
					? (totalDays + (day - 1))
					: string.Concat("Day must be at least 1 and, for the provided month (", month.ToString(), "), at most ", DateTime.DaysInMonth(year, month).ToString())
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
			if (TotalDaysFromYear(year).Failure(out int totalDays, out string err))
			{
				return err;
			}
			if (week < 1 || week > 53)
			{
				return "Week must be at least 1 and at most 53";
			}
			int iwd = (int)isoWeekDay;
			if (iwd < 1 || iwd > 7)
			{
				return "Week Day must be at least 1 and at most 7";
			}

			// Thanks to https://en.wikipedia.org/wiki/ISO_week_date#Calculating_an_ordinal_or_month_date_from_a_week_date

			// totalDays also happens to be January the 1st, so we can simply add 3 onto it to be able to get January the 4th.
			int jan4TotalDays = totalDays + 3;
			// Under ISO-8601, 1 is Monday, 7 is Sunday
			int weekdayOf4thJan = (jan4TotalDays % 7) + 1;

			// 1. Multiply week number by 7
			// 2. Add weekday number
			// 3. From this sum, subtract the correction for the year (Get the weekday of 4th january. Add 3)
			// 4. The result is the ordinal date which can be converted into a calendar date.
			int ordinalDate = week * 7 + iwd - (weekdayOf4thJan + 3);

			// Now that we know the specific day that year/week refers to, it's a simple matter to just add on days.
			return totalDays + ordinalDate - 1;
		}
		/// <summary>
		/// Calculates milliseconds elapsed since 0001-01-01 for a year, month, and day.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <param name="month">The month.</param>
		/// <param name="day">The day.</param>
		/// <returns>The total milliseconds elapsed since 0001-01-01, or an error message.</returns>
		public static Maybe<long, string> MillisFromYearMonthDay(int year, int month, int day)
		{
			return TotalDaysFromYearMonthDay(year, month, day).Success(out int totalDays, out string errMsg) ? totalDays * MillisPerDay : errMsg;
		}
		/// <summary>
		/// Calculates milliseconds elapsed since 0001-01-01 for a year and a day of year.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <param name="dayOfYear">The day of the year.</param>
		/// <returns>The total milliseconds elapsed since 0001-01-01, or an error message.</returns>
		public static Maybe<long, string> MillisFromYearOrdinalDays(int year, int dayOfYear)
		{
			return TotalDaysFromYear(year).Success(out int totalDays, out string err)
				? dayOfYear >= 1 && dayOfYear <= (DateTime.IsLeapYear(year) ? 366 : 365)
					? ((totalDays + (dayOfYear - 1)) * MillisPerDay)
					: string.Concat("Day must be at least 1 and, for the provided year (", year.ToString(), "), at most ", (DateTime.IsLeapYear(year) ? 366 : 365).ToString())
				: err;
		}
		/// <summary>
		/// Calculates milliseconds elapsed since 0001-01-01 for a year, week of the year, and weekday, according to ISO-8601.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <param name="week">The ISO-8601 week of the year.</param>
		/// <param name="isoWeekDay">The day of the week. You can use <see cref="DayOfWeekExtensions.IsoDayOfWeek(DayOfWeek)"/> to convert.</param>
		/// <returns>The total milliseconds elapsed since 0001-01-01, or an error message.</returns>
		public static Maybe<long, string> MillisFromYearWeekDay(int year, int week, IsoDayOfWeek isoWeekDay)
		{
			return TotalDaysFromYearWeekDay(year, week, isoWeekDay).Success(out int totalDays, out string err) ? totalDays * MillisPerDay : err;
		}
		/// <summary>
		/// Calcualtes the total milliseconds in the provided hour, minute, second, milliseconds, and timezone offset.
		/// </summary>
		/// <param name="hour">The hour.</param>
		/// <param name="minute">The minute.</param>
		/// <param name="second">The second.</param>
		/// <param name="millis">The milliseconds.</param>
		/// <param name="tzTotalMins">The timezone offset, in minutes.</param>
		/// <returns>The total milliseconds, or an error message.</returns>
		public static Maybe<long, string> MillisFromHourMinuteSecondMillisTimezoneOffset(int hour, int minute, int second, int millis, int tzTotalMins)
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
			if (tzTotalMins < -840 || tzTotalMins > 840)
			{
				return "Timezone Total Minutes must be at least -840 (-14:00) and at most 840 (+14:00)";
			}

			return (hour * MillisPerHour) + ((minute - tzTotalMins) * MillisPerMinute) + (second * MillisPerSecond) + millis;
		}
		/// <summary>
		/// Creates a new instance from the provided <paramref name="dateTime"/>. If <paramref name="dateTime"/>.DateTimeKind is Unspecified, throws <see cref="ArgumentException"/>, unless <paramref name="treatUnspecifiedAsUtc"/> is true.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime"/> to convert.</param>
		/// <param name="treatUnspecifiedAsUtc">If true, and the kind of <paramref name="dateTime"/> is <see cref="DateTimeKind.Unspecified"/>, then treats <paramref name="dateTime"/> as if it were UTC.</param>
		/// <returns>A <see cref="UtcDateTime"/>.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static UtcDateTime FromDateTime(DateTime dateTime, bool treatUnspecifiedAsUtc)
		{
			return dateTime.Kind switch
			{
				DateTimeKind.Utc => new UtcDateTime(dateTime.Ticks / TimeSpan.TicksPerMillisecond),
				DateTimeKind.Local => new UtcDateTime(dateTime.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond),
				_ => treatUnspecifiedAsUtc
					? new UtcDateTime(dateTime.Ticks / TimeSpan.TicksPerMillisecond)
					: throw new ArgumentException("Provided DateTime has an Unspecified kind; it must be either Utc or Local. Use DateTime.SpecifyKind() or DateTime.ToUniversalTime() to fix this if you know what the kind should be.", nameof(dateTime)),
			};
		}
	}
}
