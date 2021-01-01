namespace MichMcb.CsExt.Dates
{
	using System;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// A utility class for Dates.
	/// </summary>
	public static class DateUtil
	{
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_ExtendedFormat_UtcTz"/>
		/// </summary>
		public const int Length_Format_ExtendedFormat_UtcTz = 24;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_ExtendedFormat_FullTz"/>
		/// </summary>
		public const int Length_Format_ExtendedFormat_FullTz = 29;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_ExtendedFormat_LocalTz"/>
		/// </summary>
		public const int Length_Format_ExtendedFormat_LocalTz = 23;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_ExtendedFormat_NoMillis_UtcTz"/>
		/// </summary>
		public const int Length_Format_ExtendedFormat_NoMillis_UtcTz = 20;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_ExtendedFormat_NoMillis_FullTz"/>
		/// </summary>
		public const int Length_Format_ExtendedFormat_NoMillis_FullTz = 25;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_ExtendedFormat_NoMillis_LocalTz"/>
		/// </summary>
		public const int Length_Format_ExtendedFormat_NoMillis_LocalTz = 19;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_BasicFormat_UtcTz"/>
		/// </summary>
		public const int Length_Format_BasicFormat_UtcTz = 20;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_BasicFormat_FullTz"/>
		/// </summary>
		public const int Length_Format_BasicFormat_FullTz = 24;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_BasicFormat_LocalTz"/>
		/// </summary>
		public const int Length_Format_BasicFormat_LocalTz = 19;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_BasicFormat_NoMillis_UtcTz"/>
		/// </summary>
		public const int Length_Format_BasicFormat_NoMillis_UtcTz = 16;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_BasicFormat_NoMillis_FullTz"/>
		/// </summary>
		public const int Length_Format_BasicFormat_NoMillis_FullTz = 20;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_BasicFormat_NoMillis_LocalTz"/>
		/// </summary>
		public const int Length_Format_BasicFormat_NoMillis_LocalTz = 15;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_DateOnly"/>
		/// </summary>
		public const int Length_Format_DateOnly = 10;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_DateOnlyWithoutSeparators"/>
		/// </summary>
		public const int Length_Format_DateOnlyWithoutSeparators = 8;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_DateOrdinal"/>
		/// </summary>
		public const int Length_Format_DateOrdinal = 8;
		/// <summary>
		/// Length of a string produced by <see cref="Iso8601Parts.Format_VcfUnknownYear"/>
		/// </summary>
		public const int Length_Format_VcfUnknownYear = 7;

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
		/// Returns the length of the string that will be created if a <see cref="UtcDateTime"/> is formatted using <paramref name="format"/>.
		/// Or if <paramref name="format"/> is not a valid format (i.e. <see cref="ValidateAsFormat(Iso8601Parts)"/> returns non-null), returns that error message.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <returns>Length on success, or an error message on failure.</returns>
		public static int LengthRequired(Iso8601Parts format)
		{
			// Commonly used formats, known to be valid
			switch (format)
			{
				case Iso8601Parts.Format_ExtendedFormat_UtcTz: return Length_Format_ExtendedFormat_UtcTz;
				case Iso8601Parts.Format_ExtendedFormat_FullTz: return Length_Format_ExtendedFormat_FullTz;
				case Iso8601Parts.Format_ExtendedFormat_LocalTz: return Length_Format_ExtendedFormat_LocalTz;
				case Iso8601Parts.Format_ExtendedFormat_NoMillis_UtcTz: return Length_Format_ExtendedFormat_NoMillis_UtcTz;
				case Iso8601Parts.Format_ExtendedFormat_NoMillis_FullTz: return Length_Format_ExtendedFormat_NoMillis_FullTz;
				case Iso8601Parts.Format_ExtendedFormat_NoMillis_LocalTz: return Length_Format_ExtendedFormat_NoMillis_LocalTz;
				case Iso8601Parts.Format_BasicFormat_UtcTz: return Length_Format_BasicFormat_UtcTz;
				case Iso8601Parts.Format_BasicFormat_FullTz: return Length_Format_BasicFormat_FullTz;
				case Iso8601Parts.Format_BasicFormat_LocalTz: return Length_Format_BasicFormat_LocalTz;
				case Iso8601Parts.Format_BasicFormat_NoMillis_UtcTz: return Length_Format_BasicFormat_NoMillis_UtcTz;
				case Iso8601Parts.Format_BasicFormat_NoMillis_FullTz: return Length_Format_BasicFormat_NoMillis_FullTz;
				case Iso8601Parts.Format_BasicFormat_NoMillis_LocalTz: return Length_Format_BasicFormat_NoMillis_LocalTz;
				case Iso8601Parts.Format_DateOnly: return Length_Format_DateOnly;
				case Iso8601Parts.Format_DateOnlyWithoutSeparators: return Length_Format_DateOnlyWithoutSeparators;
				case Iso8601Parts.Format_DateOrdinal: return Length_Format_DateOrdinal;
				case Iso8601Parts.Format_VcfUnknownYear: return Length_Format_VcfUnknownYear;
				default:
					break;
			}
			string? errMsg = ValidateAsFormat(format);
			if (errMsg != null)
			{
				return -1;
			}

			Iso8601Parts part = format & Iso8601Parts.Mask_Date;
			int length = 0;
			if (part != Iso8601Parts.None)
			{
				/* Date portion can be one of these, not counting Week dates (not implemented yet)
				yyyyMMdd
				yyyy-MM-dd
				yyyy-MM
				yyyyddd
				yyyy-ddd
				--MM-dd
				--MMdd
				*/

				bool year = (part & Iso8601Parts.Year) == Iso8601Parts.Year;
				// Omitting the year means it's always replaced with 2 dashes; hence adding 2
				length += year ? 4 : 2;

				bool month = (part & Iso8601Parts.Month) == Iso8601Parts.Month;
				bool day = (part & Iso8601Parts.Day) == Iso8601Parts.Day;
				if (month && day)
				{
					length += 4; // MMdd
				}
				else if (month && !day)
				{
					length += 2; // MM
				}
				else if (!month && day)
				{
					length += 3; // ddd
				}
				// If there are separators defined for the date, then it's either 1 or 2 more length
				if ((format & Iso8601Parts.Separator_Date) == Iso8601Parts.Separator_Date)
				{
					// Only 2 separators if we have defined year, month, and day
					// Otherwise, possibilities are omitting one of year/month/day. Can't omit more than 1 so we'll at least be adding 1 separator.
					length += (year && month && day) ? 2 : 1;
				}
			}
			{
				// Time is simple; it's just 2 for every part specified (4 for milliseconds)

				// If length is 0 so far that means Date wasn't specified. If we did have a date we need to add an extra 1 for the 'T' separating Date/Time.
				int lengthForT = length != 0 ? 1 : 0;
				bool separators = (format & Iso8601Parts.Separator_Time) == Iso8601Parts.Separator_Time;
				switch (format & Iso8601Parts.Mask_Time)
				{
					case Iso8601Parts.Hour: // +2
						length += 2 + lengthForT;
						break;
					case Iso8601Parts.HourMinute: // +4 (+1 if separators)
						length += 4 + lengthForT + (separators ? 1 : 0);
						break;
					case Iso8601Parts.HourMinuteSecond: // +6 (+2 if separators)
						length += 6 + lengthForT + (separators ? 2 : 0);
						break;
					case Iso8601Parts.HourMinuteSecondMillis: // +10 (+2 if separators. The decimal place is not a separator, it's always required)
						length += 10 + lengthForT + (separators ? 2 : 0);
						break;
					case Iso8601Parts.None: // +0
					default:
						break;
				}
			}
			{
				// Timzone is very simple. Tz_Utc is a 'Z', Local is nothing, Tz_Hour is 3 (+HH), and Tz_HourMinute is either 5 or 6 (+HHmm or +HH:mm)
				switch (format & Iso8601Parts.Mask_Tz)
				{
					case Iso8601Parts.None: // +0
					default:
						break;
					case Iso8601Parts.Tz_Utc: // +1
						length++;
						break;
					case Iso8601Parts.Tz_Hour: // +3
						length += 3;
						break;
					case Iso8601Parts.Tz_HourMinute: // +6 with the : separator, +5 without
						length += (format & Iso8601Parts.Separator_Tz) == Iso8601Parts.Separator_Tz ? 6 : 5;
						break;
				}
			}
			
			return length;
		}
		/// <summary>
		/// If the provided <paramref name="format"/> is valid, returns null. Otherwise, returns a string indicating what's wrong with it.
		/// </summary>
		/// <param name="format">The format to validate.</param>
		/// <returns>null if valid, error message otherwise.</returns>
		[return: MaybeNull]
		public static string? ValidateAsFormat(Iso8601Parts format)
		{
			if (format == Iso8601Parts.None)
			{
				return "Nothing was provided for the format";
			}
			bool noDate = false;
			switch (format & Iso8601Parts.Mask_Date)
			{
				case Iso8601Parts.None:
					noDate = true;
					break;
				case Iso8601Parts.YearDay:
				case Iso8601Parts.YearMonthDay:
				case Iso8601Parts.MonthDay:
					break;
				case Iso8601Parts.YearMonth:
					if ((format & Iso8601Parts.Separator_Date) == 0)
					{
						return "Writing year and month only without separators is not valid; it can be confused with yyMMdd";
					}
					break;
				case Iso8601Parts.Year:
					return "The provided format for the date portion needs to specify more than just a year";
				default:
					return "The provided format for the date portion is not valid";
			}
			switch (format & Iso8601Parts.Mask_Time)
			{
				case Iso8601Parts.None:
					if (noDate)
					{
						return "At least a date or time has to be specified";
					}
					break;
				case Iso8601Parts.Hour:
				case Iso8601Parts.HourMinute:
				case Iso8601Parts.HourMinuteSecond:
				case Iso8601Parts.HourMinuteSecondMillis:
					break;
				default:
					return "The provided format for the time portion is not valid";
			}
			switch (format & Iso8601Parts.Mask_Tz)
			{
				case Iso8601Parts.None:
				case Iso8601Parts.Tz_Utc:
				case Iso8601Parts.Tz_Hour:
				case Iso8601Parts.Tz_HourMinute:
					break;
				case Iso8601Parts.Tz_Minute:
					return "Timezone designator can't be just minutes; it needs hours and minutes";
				default:
					return "The provided format for the timezone designator is not valid";
			}
			return null;
		}
		/// <summary>
		/// Calculates an hour/minute/second/millisecond given <paramref name="ms"/>, which is interpreted as the number of milliseconds elapsed since 0001-01-01.
		/// </summary>
		public static void CalcTimeParts(long ms, out int hour, out int minute, out int second, out int millis)
		{
			hour = (int)(ms / MillisPerHour % 24);
			minute = (int)(ms / MillisPerMinute % 60);
			second = (int)(ms / MillisPerSecond % 60);
			millis = (int)(ms % MillisPerSecond);
		}
		/// <summary>
		/// Calculates an year/month/dayhour/minute/second/millisecond given <paramref name="ms"/>, which is interpreted as the number of milliseconds elapsed since 0001-01-01.
		/// </summary>
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
		internal static ArgumentOutOfRangeException? MillisFromParts_OrdinalDays(int year, int days, int hour, int minute, int second, int millis, int tzHours, int tzMinutes, out long totalMs)
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
			return totalMs < 0 || totalMs > MaxMillis
				? new ArgumentOutOfRangeException(string.Concat("The provided date parts (Year ", year.ToString(), " Ordinal Day ", days.ToString(), " Hour ",
					hour.ToString(), " Minute ", minute.ToString(), " Second ", second.ToString(), " Millis ", millis.ToString(), " Timezone Hours ", tzHours.ToString(), " Timezone Minutes ", tzMinutes.ToString(),
					") resulted in a UtcDateTime that is outside the range of representable values."))
				: null;
		}
		internal static ArgumentOutOfRangeException? MillisFromParts(int year, int month, int day, int hour, int minute, int second, int millis, int tzHours, int tzMinutes, out long totalMs)
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
			return totalMs < 0 || totalMs > MaxMillis
				? new ArgumentOutOfRangeException(string.Concat("The provided date parts (Year ", year.ToString(), " Month ", month.ToString(), " Day ", day.ToString(), " Hour ",
					hour.ToString(), " Minute ", minute.ToString(), " Second ", second.ToString(), " Millis ", millis.ToString(), " Timezone Hours ", tzHours.ToString(), " Timezone Minutes ", tzMinutes.ToString(),
					") resulted in a UtcDateTime that is outside the range of representable values."))
				: null;
		}
		internal static ArgumentOutOfRangeException? CheckTimeParts(int hour, int minute, int second, int millis, int tzHours, int tzMinutes)
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
		/// Converts a Unix time expressed as the number of <paramref name="seconds"/> that have elapsed since 1970-01-01 00:00:00 UTC.
		/// </summary>
		/// <param name="seconds">The seconds.</param>
		/// <returns>A DateTime with a Kind of Utc.</returns>
		public static DateTime DateTimeFromUnixTimeSeconds(long seconds)
		{
			return new DateTime(seconds * TimeSpan.TicksPerSecond + UnixEpochTicks, DateTimeKind.Utc);
		}
		/// <summary>
		/// Converts a DateTime to seconds that have elapsed since 1970-01-01 00:00:00. The provided DateTime is converted to Utc if its Kind is Local or Unspecified.
		/// </summary>
		/// <returns>The number of seconds</returns>
		public static long ToUnixTimeSeconds(this in DateTime dt)
		{
			return dt.Kind == DateTimeKind.Utc
				? ((dt.Ticks - UnixEpochTicks) / TimeSpan.TicksPerSecond)
				: ((dt.ToUniversalTime().Ticks - UnixEpochTicks) / TimeSpan.TicksPerSecond);
		}
		/// <summary>
		/// Converts a Unix time expressed as the number of <paramref name="milliseconds"/> that have elapsed since 1970-01-01 00:00:00 UTC.
		/// </summary>
		/// <param name="milliseconds">The milliseconds</param>
		/// <returns>A DateTime with a Kind of Utc</returns>
		public static DateTime DateTimeFromUnixTimeMilliseconds(long milliseconds)
		{
			return new DateTime(milliseconds * TimeSpan.TicksPerMillisecond + UnixEpochTicks, DateTimeKind.Utc);
		}
		/// <summary>
		/// Converts a DateTime to milliseconds that have elapsed since 1970-01-01 00:00:00. The provided DateTime is converted to Utc if its Kind is Local or Unspecified.
		/// </summary>
		/// <returns>The number of milliseconds</returns>
		public static long ToUnixTimeMilliseconds(this in DateTime dt)
		{
			return dt.Kind == DateTimeKind.Utc
				? ((dt.Ticks - UnixEpochTicks) / TimeSpan.TicksPerMillisecond)
				: ((dt.ToUniversalTime().Ticks - UnixEpochTicks) / TimeSpan.TicksPerMillisecond);
		}
	}
}
