using System;
using System.Diagnostics.CodeAnalysis;

namespace MichMcb.CsExt
{
	public static class Dates
	{
		private const int DaysToUnixEpoch = DaysPer400Years * 4 + DaysPer100Years * 3 + DaysPer4Years * 17 + DaysPerYear;
		private const long UnixEpochTicks = TimeSpan.TicksPerDay * DaysToUnixEpoch;
		/// <summary>
		/// 1970-01-01 00:00:00, represented as milliseconds elapsed since 0001-01-01 00:00:00
		/// </summary>
		public const long UnixEpochMillis = 62135596800000;
		/// <summary>
		/// 9999-12-31 23:59:59.999, represented as milliseconds elapsed since 0001-01-01 00:00:00
		/// </summary>
		public const long MaxMillis = 315537897599999;
		public const long MillisPerSecond = 1000;
		public const long MillisPerMinute = MillisPerSecond * 60;
		public const long MillisPerHour = MillisPerMinute * 60;
		public const long MillisPerDay = MillisPerHour * 24;
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

		public static void CalcTimeParts(long ms, out int hour, out int minute, out int second, out int millis)
		{
			// Number of milliseconds elapsed since 0001-01-01
			hour = (int)(ms / MillisPerHour % 24);
			minute = (int)(ms / MillisPerMinute % 60);
			second = (int)(ms / MillisPerSecond % 60);
			millis = (int)(ms % MillisPerSecond);
		}
		public static void CalcDateTimeParts(long ms, out int year, out int month, out int day, out int hour, out int minute, out int second, out int millis)
		{
			CalcTimeParts(ms, out hour, out minute, out second, out millis);
			// Credit where it's due; most of this code is written by myself.
			// However, the two Adjusments and the optimization of (totalDays >> 5) + 1 comes from the .NET Source for DateTime

			// We know the total number of days easily.
			// totalDays is the number of days since 0001-01-01
			int totalDays = (int)(ms / MillisPerDay);
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
		internal static int DaysFromYear(int year)
		{
			// Add extra leap year days; a leap year is divisible by 4, but not by 100, unless also divisible by 400.
			--year;
			return (year * 365) + year / 4 - year / 100 + year / 400;
		}
		[return: MaybeNull]
		internal static ArgumentOutOfRangeException MillisFromParts_OrdinalDays(int year, int days, int hour, int minute, int second, int millis, int tzHours, int tzMinutes, out long totalMs)
		{
			totalMs = 0;
			if (year < 1 || year > 9999)
			{
				return new ArgumentOutOfRangeException(nameof(year), "Year must be at least 1 and at most 9999");
			}
			if (days < 1 || days > (DateTime.IsLeapYear(year) ? 366 : 365))
			{
				return new ArgumentOutOfRangeException(nameof(year), string.Concat("Day must be at least 1 and, for the provided year (", year.ToString(), "), at most ", (DateTime.IsLeapYear(year) ? 366 : 365).ToString()));
			}
			var ex = CheckTimeParts(hour, minute, second, millis, tzHours, tzMinutes);
			if (ex != null)
			{
				return ex;
			}

			totalMs = ((DaysFromYear(year) + (days - 1)) * MillisPerDay) +
				((hour + tzHours) * MillisPerHour) + ((minute + tzMinutes) * MillisPerMinute) + (second * MillisPerSecond) + millis;
			if (totalMs < 0 || totalMs > MaxMillis)
			{
				return new ArgumentOutOfRangeException(string.Concat("The provided date parts (Year ", year.ToString(), " Ordinal Day ", days.ToString(), " Hour ",
					hour.ToString(), " Minute ", minute.ToString(), " Second ", second.ToString(), " Millis ", millis.ToString(), " Timezone Hours ", tzHours.ToString(), " Timezone Minutes ", tzMinutes.ToString(),
					") resulted in a UtcDateTime that is outside the range of representable values."));
			}
			return null;
		}
		[return: MaybeNull]
		internal static ArgumentOutOfRangeException MillisFromParts(int year, int month, int day, int hour, int minute, int second, int millis, int tzHours, int tzMinutes, out long totalMs)
		{
			totalMs = 0;
			if (year < 1 || year > 9999)
			{
				return new ArgumentOutOfRangeException(nameof(year), "Year must be at least 1 and at most 9999");
			}
			if (month < 1 || month > 12)
			{
				return new ArgumentOutOfRangeException(nameof(month), "Month must be at least 1 and at most 12");
			}
			if (day < 1 || day > DateTime.DaysInMonth(year, month))
			{
				return new ArgumentOutOfRangeException(nameof(day), string.Concat("Day must be at least 1 and, for the provided month (", month.ToString(), "), at most ", DateTime.DaysInMonth(year, month).ToString()));
			}
			var ex = CheckTimeParts(hour, minute, second, millis, tzHours, tzMinutes);
			if (ex != null)
			{
				return ex;
			}

			int[] totalDaysFromStartYearToMonth = DateTime.IsLeapYear(year) ? TotalDaysFromStartLeapYearToMonth : TotalDaysFromStartYearToMonth;

			totalMs = ((DaysFromYear(year) +
				// Add number of days from the months already passed
				totalDaysFromStartYearToMonth[month - 1] + (day - 1)) * MillisPerDay) +
				// Time portion is easy, just hours, minutes, seconds, milliseconds
				((hour + tzHours) * MillisPerHour) + ((minute + tzMinutes) * MillisPerMinute) + (second * MillisPerSecond) + millis;
			if (totalMs < 0 || totalMs > MaxMillis)
			{
				return new ArgumentOutOfRangeException(string.Concat("The provided date parts (Year ", year.ToString(), " Month ", month.ToString(), " Day ", day.ToString(), " Hour ",
					hour.ToString(), " Minute ", minute.ToString(), " Second ", second.ToString(), " Millis ", millis.ToString(), " Timezone Hours ", tzHours.ToString(), " Timezone Minutes ", tzMinutes.ToString(),
					") resulted in a UtcDateTime that is outside the range of representable values."));
			}
			return null;
		}
		[return: MaybeNull]
		internal static ArgumentOutOfRangeException CheckTimeParts(int hour, int minute, int second, int millis, int tzHours, int tzMinutes)
		{
			if (hour < 0 || hour > 23)
			{
				return new ArgumentOutOfRangeException(nameof(hour), "Hour must be at least 0 and at most 23");
			}
			if (minute < 0 || minute > 59)
			{
				return new ArgumentOutOfRangeException(nameof(minute), "Minute must be at least 0 and at most 59");
			}
			if (second < 0 || second > 59)
			{
				return new ArgumentOutOfRangeException(nameof(second), "Second must be at least 0 and at most 59");
			}
			if (millis < 0 || millis > 999)
			{
				return new ArgumentOutOfRangeException(nameof(millis), "Millisecond must be at least 0 and at most 999");
			}
			if (tzHours < -12 || tzHours > 14)
			{
				return new ArgumentOutOfRangeException(nameof(tzHours), "Timezone Hours must be at least -12 and at most 14");
			}
			if (tzMinutes < -59 || tzMinutes > 59)
			{
				return new ArgumentOutOfRangeException(nameof(tzMinutes), "Timezone Minutes must be at least -59 and at most 59");
			}
			return null;
		}
		/// <summary>
		/// Converts a Unix time expressed as the number of <paramref name="seconds"/> that have elapsed since 1970-01-01 00:00:00
		/// </summary>
		/// <param name="seconds">The seconds</param>
		public static DateTime DateTimeFromUnixTimeSeconds(long seconds)
		{
			return new DateTime(seconds * TimeSpan.TicksPerSecond + UnixEpochTicks, DateTimeKind.Utc);
		}
		/// <summary>
		/// Converts a DateTime to seconds that have elapsed since 1970-01-01 00:00:00
		/// </summary>
		public static long ToUnixTimeSeconds(this in DateTime dt)
		{
			return dt.Kind == DateTimeKind.Utc
				? ((dt.Ticks - UnixEpochTicks) / TimeSpan.TicksPerSecond)
				: ((dt.ToUniversalTime().Ticks - UnixEpochTicks) / TimeSpan.TicksPerSecond);
		}
		/// <summary>
		/// Converts a Unix time expressed as the number of <paramref name="milliseconds"/> that have elapsed since 1970-01-01 00:00:00
		/// </summary>
		/// <param name="milliseconds">The seconds</param>
		public static DateTime DateTimeFromUnixTimeMilliseconds(long milliseconds)
		{
			return new DateTime(milliseconds * TimeSpan.TicksPerMillisecond + UnixEpochTicks, DateTimeKind.Utc);
		}
		/// <summary>
		/// Converts a DateTime to milliseconds that have elapsed since 1970-01-01 00:00:00
		/// </summary>
		public static long ToUnixTimeMilliseconds(this in DateTime dt)
		{
			return dt.Kind == DateTimeKind.Utc
				? ((dt.Ticks - UnixEpochTicks) / TimeSpan.TicksPerMillisecond)
				: ((dt.ToUniversalTime().Ticks - UnixEpochTicks) / TimeSpan.TicksPerMillisecond);
		}
	}
}
