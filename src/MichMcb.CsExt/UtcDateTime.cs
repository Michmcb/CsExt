#if !NETSTANDARD2_0
namespace MichMcb.CsExt
{
	using MichMcb.CsExt.Strings;
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;

	/*
TODO notes for a ZonedDateTime are written here
MaxMillis is this in hex: 0001 1EFA E44C B3FF
3 hex digits (12 bits, 4096 possible states) is just enough to cram in TimeZone, but only if the highest resolution for it is minutes, and only if it's ± 24 hours at max. (or more realistically, 23:59)
60 mins * 24 hrs gives us 0x0D80, so we can just barely fit that into the upper 3 hex digits of our TotalMilliseconds. Of course, doing this means we can't be any more precise than 1 millisecond. But that's fine I think.*/

	/// <summary>
	/// Represents a UTC Date Time, as milliseconds since 0001-01-01 00:00:00.000.
	/// Unlike <see cref="DateTime"/> and <see cref="DateTimeOffset"/>, this is only ever UTC, which can help if you want to differentiate by type.
	/// Also, this has less accuracy; it's only accurate to the millisecond, instead of 100-nanosecond ticks. In practice this shouldn't be a problem since
	/// DateTime.Now isn't that precise, typically it's only precise to 1~35 milliseconds.
	/// This can be explicitly cast to and from <see cref="DateTime"/> (unless Kind is Unspecified), and <see cref="DateTimeOffset"/>.
	/// </summary>
	public readonly struct UtcDateTime : IEquatable<UtcDateTime>, IComparable<UtcDateTime>
	{
		/// <summary>
		/// 1970-01-01 00:00:00
		/// </summary>
		public static readonly UtcDateTime UnixEpoch = new UtcDateTime(Dates.UnixEpochMillis);
		/// <summary>
		/// 0001-01-01 00:00:00
		/// </summary>
		public static readonly UtcDateTime MinValue = new UtcDateTime(0);
		/// <summary>
		/// 9999-12-31 23:59:59.999
		/// </summary>
		public static readonly UtcDateTime MaxValue = new UtcDateTime(Dates.MaxMillis);
		/// <summary>
		/// Creates a new instance, as milliseconds elapsed since 0001-01-01 00:00:00
		/// </summary>
		/// <param name="millis">Milliseconds elapsed since 0001-01-01 00:00:00</param>
		public UtcDateTime(long millis)
		{
			if (millis < 0 || millis > Dates.MaxMillis)
			{
				throw new ArgumentOutOfRangeException(nameof(millis), "Milliseconds must be at least 0 and at most " + Dates.MaxMillis.ToString());
			}
			TotalMilliseconds = millis;
		}
		/// <summary>
		/// Creates a new instance, with the hours, minutes, seconds, and milliseconds parts set to 0
		/// </summary>
		public UtcDateTime(int year, int month, int day)
		{
			var ex = Dates.MillisFromParts(year, month, day, 0, 0, 0, 0, 0, 0, out long ms);
			if (ex != null)
			{
				throw ex;
			}
			TotalMilliseconds = ms;
		}
		/// <summary>
		/// Creates a new UtcDateTime instance, with the millisecond part set to 0
		/// </summary>
		public UtcDateTime(int year, int month, int day, int hour, int minute, int second) : this(year, month, day, hour, minute, second, 0) { }
		public UtcDateTime(int year, int month, int day, int hour, int minute, int second, int millis)
		{
			var ex = Dates.MillisFromParts(year, month, day, hour, minute, second, millis, 0, 0, out long ms);
			if (ex != null)
			{
				throw ex;
			}
			TotalMilliseconds = ms;
		}
		/// <summary>
		/// Returns an instance representing the current UTC time
		/// </summary>
		public static UtcDateTime Now => new UtcDateTime(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);
		/// <summary>
		/// Returns the Years part of this instance
		/// </summary>
		public int Year
		{
			get
			{
				GetDateParts(out int year, out _, out _);
				return year;
			}
		}
		/// <summary>
		/// Returns the Months part of this instance
		/// </summary>
		public int Month
		{
			get
			{
				GetDateParts(out _, out int month, out _);
				return month;
			}
		}
		/// <summary>
		/// Returns the Days part of this instance
		/// </summary>
		public int Day
		{
			get
			{
				GetDateParts(out _, out _, out int day);
				return day;
			}
		}
		/// <summary>
		/// Returns the Hours part of this instance
		/// </summary>
		public int Hour => (int)(TotalMilliseconds / Dates.MillisPerHour % 24);
		/// <summary>
		/// Returns the minutes part of this instance
		/// </summary>
		public int Minute => (int)(TotalMilliseconds / Dates.MillisPerMinute % 60);
		/// <summary>
		/// Returns the seconds part of this instance
		/// </summary>
		public int Second => (int)(TotalMilliseconds / Dates.MillisPerSecond % 60);
		/// <summary>
		/// Returns the milliseconds part of this instance
		/// </summary>
		public int Millisecond => (int)(TotalMilliseconds % Dates.MillisPerSecond);
		/// <summary>
		/// Returns the total number of days since 0001-01-01 represented by this instance
		/// </summary>
		public int TotalDays => (int)(TotalMilliseconds / Dates.MillisPerDay);
		/// <summary>
		/// Returns the Day of the year, from 1 to 366
		/// </summary>
		public int DayOfYear
		{
			get
			{
				int totalDays = TotalDays;
				int y400 = totalDays / Dates.DaysPer400Years;
				totalDays -= Dates.DaysPer400Years * y400;
				int y100 = totalDays / Dates.DaysPer100Years;
				if (y100 == 4)
				{
					y100 = 3; // Adjustment
				}
				totalDays -= Dates.DaysPer100Years * y100;
				int y4 = totalDays / Dates.DaysPer4Years;
				totalDays -= Dates.DaysPer4Years * y4;
				int y1 = totalDays / 365;
				if (y1 == 4)
				{
					y1 = 3; // Adjustment
				}

				return (totalDays -= y1 * 365) + 1;
			}
		}
		/// <summary>
		/// Gets the Day of Week represented by this instance
		/// </summary>
		public DayOfWeek DayOfWeek => (DayOfWeek)(TotalDays % 7); // 0001-01-01 is a Monday, this 0001-01-00 would be a Sunday. Therefore, all we need to do is return TotalDays % 7 (Sun = 0, Sat = 6)
		/// <summary>
		/// Returns the Time of Day as a TimeSpan
		/// </summary>
		public TimeSpan TimeOfDay => new TimeSpan(TotalMilliseconds * TimeSpan.TicksPerMillisecond);
		/// <summary>
		/// The number of milliseconds elapsed since 0001-01-01 00:00:00 represented by this instance
		/// </summary>
		public long TotalMilliseconds { get; }
		/// <summary>
		/// Returns true if this instance represents the 29th of February
		/// </summary>
		public bool Is29thFeb
		{
			get
			{
				GetDateParts(out _, out int month, out int day);
				return month == 2 && day == 29;
			}
		}
		/// <summary>
		/// Gets the year, month, and day parts of this instance
		/// </summary>
		public void GetDateParts(out int year, out int month, out int day)
		{
			Deconstruct(out year, out month, out day, out _, out _, out _, out _);
		}
		/// <summary>
		/// Gets all parts of this instance
		/// </summary>
		public void Deconstruct(out int year, out int month, out int day, out int hour, out int minute, out int second, out int millis)
		{
			Dates.CalcDateTimeParts(TotalMilliseconds, out year, out month, out day, out hour, out minute, out second, out millis);
		}
		/// <summary>
		/// Adds the specified number of years to this instance.
		/// If this instance represents the 29th of February, and the result is not a leap year, the result will be 28th February.
		/// The reason is so the month doesn't change from February to March suddenly, which is weird.
		/// </summary>
		public UtcDateTime AddYears(int years)
		{
			if (years == 0)
			{
				return this;
			}
			Deconstruct(out int year, out int month, out int day, out int hour, out int minute, out int second, out int millis);
			// If it's not 29th february, we don't need any sort of special handling.
			// if it is 29th February, we're still okay so long as the resultant year is also a leap year.
			// If not, then we make it the 28th of February.
			int newYear = year + years;
			return !(month == 2 && day == 29) || DateTime.IsLeapYear(newYear)
				? new UtcDateTime(newYear, month, day, hour, minute, second, millis)
				: new UtcDateTime(newYear, 2, 28, hour, minute, second, millis);
		}
		/// <summary>
		/// Adds the specified number of months to this instance.
		/// If is instance represents a day of month that is too large for the resultant month, then the day of the resultant instance will be the last day of the resultant month.
		/// </summary>
		public UtcDateTime AddMonths(int months)
		{
			if (months == 0)
			{
				return this;
			}
			Deconstruct(out int year, out int month, out int day, out int hour, out int minute, out int second, out int millis);

			int m = month - 1 + months;
			int newMonth;
			int newYear;
			if (m > 0)
			{
				newMonth = m % 12 + 1;
				newYear = year + (m / 12);
			}
			else
			{
				newMonth = 12 + (m + 1) % 12;
				newYear = year + (m - 11) / 12;
			}
			int newDay = Math.Min(day, DateTime.DaysInMonth(newYear, newMonth));
			return new UtcDateTime(newYear, newMonth, newDay, hour, minute, second, millis);
		}
		/// <summary>
		/// Adds <paramref name="days"/> to this instance
		/// </summary>
		public UtcDateTime AddDays(int days)
		{
			return new UtcDateTime(TotalMilliseconds + days * Dates.MillisPerDay);
		}
		/// <summary>
		/// Adds <paramref name="hours"/> to this instance
		/// </summary>
		public UtcDateTime AddHours(int hours)
		{
			return new UtcDateTime(TotalMilliseconds + hours * Dates.MillisPerHour);
		}
		/// <summary>
		/// Adds <paramref name="minutes"/> to this instance
		/// </summary>
		public UtcDateTime AddMinutes(int minutes)
		{
			return new UtcDateTime(TotalMilliseconds + minutes * Dates.MillisPerMinute);
		}
		/// <summary>
		/// Adds <paramref name="seconds"/> to this instance
		/// </summary>
		public UtcDateTime AddSeconds(int seconds)
		{
			return new UtcDateTime(TotalMilliseconds + seconds * Dates.MillisPerSecond);
		}
		/// <summary>
		/// Adds <paramref name="millis"/> to this instance
		/// </summary>
		public UtcDateTime AddMilliseconds(int millis)
		{
			return new UtcDateTime(TotalMilliseconds + millis);
		}
		// /// <summary>
		// /// Adds <paramref name="workingDays"/> to this instance, assuming Monday, Tuesday, Wednesday, Thursday, and Friday are working days.
		// /// Assumes no holidays.
		// /// </summary>
		//public UtcDateTime AddWorkingDays(int workingDays)
		//{
		// TODO implement AddWorkingDays - allow specifying the working days of weeks too.
		//	throw new NotImplementedException("Working days not done yet");
		/*
	To add working days:
Normalize input to the Friday
Add the weeks (days / 5 * 7)
Get the remainder (days % 5) (can probably use DivRem for this)
Add remainder, adding 2 if it would end on a weekend day
*/
		//}
		/// <summary>
		/// Returns a truncated instance so that it is only accurate to the part specified by <paramref name="truncateTo"/>.
		/// For example, if <paramref name="truncateTo"/> is Minute, then Seconds and Milliseconds are set to zero.
		/// Truncating days or months will cause them to be truncated to 1.
		/// </summary>
		public UtcDateTime Truncate(DateTimePart truncateTo)
		{
			// TruncateTo means that the part in question is the smallest part that should not be truncated
#pragma warning disable IDE0066 // Convert switch statement to expression
			switch (truncateTo)
#pragma warning restore IDE0066 // Convert switch statement to expression
			{
				case DateTimePart.Year: return new UtcDateTime(Year, 1, 1);
				case DateTimePart.Month: return new UtcDateTime(TotalMilliseconds - (Day * Dates.MillisPerDay) + Dates.MillisPerDay - (Hour * Dates.MillisPerHour) - (Minute * Dates.MillisPerMinute) - (Second * Dates.MillisPerSecond) - Millisecond);
				case DateTimePart.Day: return new UtcDateTime(TotalMilliseconds - (Hour * Dates.MillisPerHour) - (Minute * Dates.MillisPerMinute) - (Second * Dates.MillisPerSecond) - Millisecond);
				case DateTimePart.Hour: return new UtcDateTime(TotalMilliseconds - (Minute * Dates.MillisPerMinute) - (Second * Dates.MillisPerSecond) - Millisecond);
				case DateTimePart.Minute: return new UtcDateTime(TotalMilliseconds - Second * Dates.MillisPerSecond - Millisecond);
				case DateTimePart.Second: return new UtcDateTime(TotalMilliseconds - Millisecond);
				case DateTimePart.Millisecond: return this;
				default:
					throw new ArgumentOutOfRangeException(nameof(truncateTo), "Parameter was not a valid value for DateTimePart");
			}
		}
		/// <summary>
		/// Creates a new instance, as the number of <paramref name="days"/> elapsed since 0001-01-01 00:00:00.
		/// Optionally allows specifying the hour/minute/second/millisecond
		/// </summary>
		public static UtcDateTime FromDays(int days, int hour = 0, int minute = 0, int second = 0, int millis = 0)
		{
			var ex = Dates.CheckTimeParts(hour, minute, second, millis, 0, 0);
			if (ex != null)
			{
				throw ex;
			}
			return new UtcDateTime(days * Dates.MillisPerDay + hour * Dates.MillisPerHour + minute * Dates.MillisPerMinute + second * Dates.MillisPerSecond + millis);
		}
		/// <summary>
		/// Creates a new instance from the provided seconds, interpreted as seconds since the Unix Epoch (1970-01-01 00:00:00).
		/// Negative values are allowed.
		/// </summary>
		public static UtcDateTime FromUnixEpochSeconds(long seconds)
		{
			return new UtcDateTime(seconds * Dates.MillisPerSecond + Dates.UnixEpochMillis);
		}
		/// <summary>
		/// Creates a new instance from the provided seconds, interpreted as milliseconds since the Unix Epoch (1970-01-01 00:00:00).
		/// Negative values are allowed.
		/// </summary>
		public static UtcDateTime FromUnixEpochMilliseconds(long milliseconds)
		{
			return new UtcDateTime(milliseconds + Dates.UnixEpochMillis);
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string using Extended Format with UTC as the Timezone Designator. i.e. 2010-12-30T13:30:20.123Z
		/// </summary>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public override string ToString()
		{
			return ToIso8601String(TimeSpan.Zero, Iso8601Parts.Format_ExtendedFormatUtc);
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string, from the perspective of UTC, according to the rules specified by <paramref name="format"/>.
		/// Note that if you omit the Time, this may cause data loss; when read again, time is assumed to be 00:00 of whatever timezone the string is interpreted as.
		/// </summary>
		/// <param name="format">How to format the string. By default, this is ISO-8601 extended, with UTC timezone designator</param>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public string ToIso8601StringUtc(Iso8601Parts format = Iso8601Parts.Format_ExtendedFormatUtc)
		{
			return ToIso8601String(TimeSpan.Zero, format);
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string, from the perspective of the local timezone, according to the rules specified by <paramref name="format"/>.
		/// Note that if you omit the Time, this may cause data loss; when read again, time is assumed to be 00:00 of whatever timezone the string is interpreted as.
		/// </summary>
		/// <param name="format">How to format the string. By default, this is ISO-8601 extended, with full timzone designator</param>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public string ToIso8601StringLocal(Iso8601Parts format = Iso8601Parts.Format_ExtendedFormatFullTz)
		{
			return ToIso8601String(TimeZoneInfo.Local.BaseUtcOffset, format);
		}
		/// <summary>
		/// Formats this UtcDateTime as an ISO-8601 string, according to the rules specified by <paramref name="format"/>.
		/// The provided <paramref name="timezone"/> specifies the timezone designator to use and then writes the string according to the <paramref name="format"/>.
		/// Note that if <paramref name="timezone"/> is provided and  <paramref name="format"/> specifies a UTC Timezone designator or no Timezone designator (Local) this doesn't have any effect; use Tz_Hour or Tz_HourMinute.
		/// </summary>
		/// <param name="format">How to format the string. By default, this is ISO-8601 extended (Everything, with separators, and UTC timezone)</param>
		/// <param name="timezone">If <paramref name="format"/> specifies hours/minutes for the Timezone designator, it use this timezone</param>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public string ToIso8601String(TimeSpan timezone, Iso8601Parts format = Iso8601Parts.Format_ExtendedFormatUtc)
		{
			var err = ValidateAsFormat(format);
			if (err != null)
			{
				throw new ArgumentException(err, nameof(format));
			}
			bool seps = (format & Iso8601Parts.Separator_Date) == Iso8601Parts.Separator_Date;

			Iso8601Parts ftz = format & Iso8601Parts.Mask_Tz;
			long tzOffsetMs;
			// TODO When omitting Time, treat this instance as midnight in whatever timezone they pass; or local or UTC, whatever. 
			// When we aren't writing the time, and timezone designator is absent, we still offset to the local timezone...
			// which can cause the date to change. The thing is, when we write just the Date that technically causes data loss.
			// So when we write the date, I guess what we're saying is that we mean this date in our local timezone. In that case, we should not offset it at all
			switch (ftz)
			{
				default:
				case 0:
					// No timezone designator; that means we need to write it as local time.
					tzOffsetMs = TimeZoneInfo.Local.BaseUtcOffset.Ticks / TimeSpan.TicksPerMillisecond;
					break;
				case Iso8601Parts.Tz_Hour:
				case Iso8601Parts.Tz_HourMinute:
					// Write a timezone designator; use the timezone we were given
					tzOffsetMs = timezone.Ticks / TimeSpan.TicksPerMillisecond;
					break;
				case Iso8601Parts.Tz_Utc: // UTC timezone, 0 ms offset
					tzOffsetMs = 0;
					break;
			}
			Dates.CalcDateTimeParts(TotalMilliseconds + tzOffsetMs, out int year, out int month, out int day, out int hour, out int minute, out int second, out int ms);

			// The longest possible string is 2010-12-30T13:30:20.123+10:00, which is 29 characters long
			Span<char> str = stackalloc char[29];
			// Part of the reason we use our own custom implementation instead of int.TryFormat is because we want to target 2.0
			// and 2.0 doesn't have int.TryFormat
			int i = 0;
			int written = 0;
			if ((format & Iso8601Parts.Year) == Iso8601Parts.Year)
			{
				year.TryFormat(str, out written, format: "0000", CultureInfo.InvariantCulture);
				i += 4;
				if (seps)
				{
					str[i++] = '-';
				}
			}
			else
			{
				// If no year, write --. This is primarily used for VCF format for birthdays, when the year is unknown (--MM-dd or --MMdd)
				str[i++] = '-';
				str[i++] = '-';
			}
			if ((format & Iso8601Parts.Month) == 0 && ((format & Iso8601Parts.Day) == Iso8601Parts.Day))
			{
				// Month and no Day is the ordinal format; we need to turn months into days and add that together with day to get the number to write
				int[] totalDaysFromStartYearToMonth = DateTime.IsLeapYear(year) ? Dates.TotalDaysFromStartLeapYearToMonth : Dates.TotalDaysFromStartYearToMonth;
				(totalDaysFromStartYearToMonth[month - 1] + day).TryFormat(str.Slice(i), out written, format: "000", CultureInfo.InvariantCulture);
				i += 3;
			}
			else
			{
				if ((format & Iso8601Parts.Month) == Iso8601Parts.Month)
				{
					month.TryFormat(str.Slice(i), out written, format: "00", CultureInfo.InvariantCulture);
					i += 2;
				}
				if ((format & Iso8601Parts.Day) == Iso8601Parts.Day)
				{
					if (seps)
					{
						str[i++] = '-';
					}
					day.TryFormat(str.Slice(i), out written, format: "00", CultureInfo.InvariantCulture);
					i += 2;
				}
			}

			if ((format & Iso8601Parts.Mask_Time) != 0)
			{
				seps = (format & Iso8601Parts.Separator_Time) == Iso8601Parts.Separator_Time;
				str[i++] = 'T';
				if ((format & Iso8601Parts.Hour) == Iso8601Parts.Hour)
				{
					hour.TryFormat(str.Slice(i), out written, format: "00", CultureInfo.InvariantCulture);
					i += 2;
				}
				if ((format & Iso8601Parts.Minute) == Iso8601Parts.Minute)
				{
					if (seps)
					{
						str[i++] = ':';
					}
					minute.TryFormat(str.Slice(i), out written, format: "00", CultureInfo.InvariantCulture);
					i += 2;
				}
				if ((format & Iso8601Parts.Second) == Iso8601Parts.Second)
				{
					if (seps)
					{
						str[i++] = ':';
					}
					second.TryFormat(str.Slice(i), out written, format: "00", CultureInfo.InvariantCulture);
					i += 2;
				}
				if ((format & Iso8601Parts.Millis) == Iso8601Parts.Millis)
				{
					str[i++] = '.';
					ms.TryFormat(str.Slice(i), out written, format: "000", CultureInfo.InvariantCulture);
					i += 3;
				}
			}

			switch (ftz)
			{
				case Iso8601Parts.Tz_Utc:
					str[i++] = 'Z';
					break;
				case Iso8601Parts.Tz_Hour:
					str[i++] = tzOffsetMs >= 0 ? '+' : '-';
					int tz = Math.Abs(timezone.Hours);
					tz.TryFormat(str.Slice(i), out written, format: "00", CultureInfo.InvariantCulture);
					i += 2;
					break;
				case Iso8601Parts.Tz_HourMinute:
					str[i++] = tzOffsetMs >= 0 ? '+' : '-';
					tz = Math.Abs(timezone.Hours);
					tz.TryFormat(str.Slice(i), out written, format: "00", CultureInfo.InvariantCulture);
					i += 2;
					if ((format & Iso8601Parts.Separator_Tz) == Iso8601Parts.Separator_Tz)
					{
						str[i++] = ':';
					}
					tz = Math.Abs(timezone.Minutes);
					tz.TryFormat(str.Slice(i), out written, format: "00", CultureInfo.InvariantCulture);
					i += 2;
					break;
			}
			return new string(str.Slice(0, i));
		}
		/// <summary>
		/// Calls <see cref="Parse.Iso8601StringAsUtcDateTime(in ReadOnlySpan{char}, TimeSpan?)"/>
		/// </summary>
		/// <param name="assumeMissingTimeZoneAs">If the string is missing a timezone designator, then it uses this. If null, local time is used if a timezone designator is missing.</param>
		/// <returns>A UtcDateTime if parsing was successful, or an error message otherwise.</returns>
		public static Maybe<UtcDateTime, string> TryParseIso8601String(in ReadOnlySpan<char> str, TimeSpan? assumeMissingTimeZoneAs = null)
		{
			return Parse.Iso8601StringAsUtcDateTime(str, assumeMissingTimeZoneAs);
		}
		[return: MaybeNull]
		private static string ValidateAsFormat(Iso8601Parts format)
		{
			switch (format & Iso8601Parts.Mask_Date)
			{
				case Iso8601Parts.None:
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
				default:
					return "The provided format for the date portion is not valid";
			}
			switch (format & Iso8601Parts.Mask_Time)
			{
				case Iso8601Parts.None:
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
		public override bool Equals([AllowNull]object obj)
		{
			return obj is UtcDateTime time && Equals(time);
		}
		public override int GetHashCode()
		{
			return TotalMilliseconds.GetHashCode();
		}
		/// <summary>
		/// Returns true if this instance and <paramref name="other"/> refer to the same point in time.
		/// </summary>
		public bool Equals(UtcDateTime other)
		{
			return TotalMilliseconds == other.TotalMilliseconds;
		}
		/// <summary>
		/// Returns true if this instance and <paramref name="other"/> are close enough.
		/// The parameter <paramref name="millisDifference"/> specifies the maximum amount of milliseconds they can differ by.
		/// </summary>
		public bool Equals(UtcDateTime other, long millisDifference)
		{
			return Math.Abs(TotalMilliseconds - other.TotalMilliseconds) < millisDifference;
		}
		/// <summary>
		/// Returns true if this instance and <paramref name="other"/> are close enough.
		/// The parameter <paramref name="delta"/> specifies the maximum amount of milliseconds they can differ.
		/// Any nanoseconds that <paramref name="delta"/> has are ignored.
		/// </summary>
		public bool Equals(UtcDateTime other, TimeSpan delta)
		{
			return Equals(other, delta.Ticks / TimeSpan.TicksPerMillisecond);
		}
		/// <summary>
		/// If this instance is later than <paramref name="other"/>, returns 1.
		/// If this instance is earlier, returns -1.
		/// If they are the same point in time, returns 0.
		/// </summary>
		public int CompareTo(UtcDateTime other)
		{
			if (TotalMilliseconds > other.TotalMilliseconds)
			{
				return 1;
			}
			if (TotalMilliseconds < other.TotalMilliseconds)
			{
				return -1;
			}
			return 0;
		}
		public static bool operator ==(in UtcDateTime left, in UtcDateTime right)
		{
			return left.TotalMilliseconds == right.TotalMilliseconds;
		}
		public static bool operator !=(in UtcDateTime left, in UtcDateTime right)
		{
			return left.TotalMilliseconds != right.TotalMilliseconds;
		}
		public static bool operator <(UtcDateTime left, UtcDateTime right)
		{
			return left.CompareTo(right) < 0;
		}
		public static bool operator <=(UtcDateTime left, UtcDateTime right)
		{
			return left.CompareTo(right) <= 0;
		}
		public static bool operator >(UtcDateTime left, UtcDateTime right)
		{
			return left.CompareTo(right) > 0;
		}
		public static bool operator >=(UtcDateTime left, UtcDateTime right)
		{
			return left.CompareTo(right) >= 0;
		}
		public static UtcDateTime operator +(UtcDateTime left, TimeSpan right)
		{
			return new UtcDateTime(left.TotalMilliseconds + right.Ticks / TimeSpan.TicksPerMillisecond);
		}
		public static TimeSpan operator -(UtcDateTime left, UtcDateTime right)
		{
			return new TimeSpan((left.TotalMilliseconds - right.TotalMilliseconds) * TimeSpan.TicksPerMillisecond);
		}
		/// <summary>
		/// Converts this UtcDateTime instance to a DateTime instance, with a DateTimeKind of Utc
		/// </summary>
		public static explicit operator DateTime(UtcDateTime utcDateTime)
		{
			return new DateTime(utcDateTime.TotalMilliseconds * TimeSpan.TicksPerMillisecond, DateTimeKind.Utc);
		}
		/// <summary>
		/// Converts this UtcDateTime instance to a DateTimeOffset instance, with an Offset of TimeSpan.Zero
		/// </summary>
		public static explicit operator DateTimeOffset(UtcDateTime utcDateTime)
		{
			return new DateTimeOffset(utcDateTime.TotalMilliseconds * TimeSpan.TicksPerMillisecond, TimeSpan.Zero);
		}
		/// <summary>
		/// Creates a new instance from the provided <paramref name="dateTime"/>. If <paramref name="dateTime"/>.DateTimeKind is Unspecified, an InvalidCastException is thrown.
		/// </summary>
		public static explicit operator UtcDateTime(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Unspecified)
			{
				throw new InvalidCastException("Provided DateTime has an Unspecified kind; it must be either Utc or Local. Use DateTime.SpecifyKind() to fix this if you know what the kind should be.");
			}
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return new UtcDateTime(dateTime.Ticks / TimeSpan.TicksPerMillisecond);
			}
			else
			{
				return new UtcDateTime(dateTime.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond);
			}
		}
		/// <summary>
		/// Creates a new instance from the provided <paramref name="dateTimeOffset"/>, interpreting <paramref name="dateTimeOffset"/> as if its Offset were Zero.
		/// To be specific, <paramref name="dateTimeOffset"/>.Offset is subtracted from <paramref name="dateTimeOffset"/> to make the offset Zero, and then that is converted to a UtcDateTime.
		/// </summary>
		public static explicit operator UtcDateTime(DateTimeOffset dateTimeOffset)
		{
			return new UtcDateTime((dateTimeOffset.Ticks - dateTimeOffset.Offset.Ticks) / TimeSpan.TicksPerMillisecond);
		}
	}
}
#endif