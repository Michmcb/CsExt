namespace MichMcb.CsExt.Dates
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Represents a UTC Date Time, as milliseconds since 0001-01-01 00:00:00.000.
	/// Unlike <see cref="DateTime"/> and <see cref="DateTimeOffset"/>, this is only ever UTC, which can help if you want to differentiate by type.
	/// Also, this has less accuracy; it's only accurate to the millisecond, instead of 100-nanosecond ticks. In practice this shouldn't be a problem since
	/// DateTime.Now isn't that precise, typically it's only precise to 1~35 milliseconds.
	/// This can be cast to and from <see cref="DateTime"/> (unless Kind is Unspecified), and <see cref="DateTimeOffset"/>.
	/// </summary>
	public readonly partial struct UtcDateTime : IEquatable<UtcDateTime>, IComparable<UtcDateTime>
	{
		/// <summary>
		/// 1970-01-01 00:00:00
		/// </summary>
		public static readonly UtcDateTime UnixEpoch = new(DotNetTime.UnixEpochTicks);
		/// <summary>
		/// 0001-01-01 00:00:00
		/// </summary>
		public static readonly UtcDateTime MinValue = default;
		/// <summary>
		/// 9999-12-31 23:59:59.9999999
		/// </summary>
		public static readonly UtcDateTime MaxValue = new(DotNetTime.MaxTicks);
		/// <summary>
		/// Creates a new instance, as ticks elapsed since 0001-01-01 00:00:00
		/// </summary>
		/// <param name="ticks">Ticks elapsed since 0001-01-01 00:00:00</param>
		public UtcDateTime(long ticks)
		{
			if (ticks < 0 || ticks > DotNetTime.MaxTicks)
			{
				throw new ArgumentOutOfRangeException(nameof(ticks), "Ticks must be at least 0 and at most " + DotNetTime.MaxTicks.ToString());
			}
			Ticks = ticks;
		}
		/// <summary>
		/// Creates a new instance, with the hours, minutes, seconds, and milliseconds parts set to 0
		/// </summary>
		public UtcDateTime(int year, int month, int day) : this(year, month, day, 0, 0, 0, 0) { }
		/// <summary>
		/// Creates a new UtcDateTime instance, with the millisecond part set to 0
		/// </summary>
		public UtcDateTime(int year, int month, int day, int hour, int minute, int second) : this(year, month, day, hour, minute, second, 0) { }
		/// <summary>
		/// Creates a new instance with every part specified.
		/// </summary>
		public UtcDateTime(int year, int month, int day, int hour, int minute, int second, int millis)
		{
			if (!TicksFromHourMinuteSecondMillisTimezoneOffset(hour, minute, second, millis, default).Success(out long tms, out string err))
			{
				throw new ArgumentOutOfRangeException(null, err);
			}
			if (!TicksFromYearMonthDay(year, month, day).Success(out long dms, out err))
			{
				throw new ArgumentOutOfRangeException(null, err);
			}
			Ticks = tms + dms;
		}
		/// <summary>
		/// Returns an instance representing the current UTC time
		/// </summary>
		public static UtcDateTime Now => new(DateTime.UtcNow.Ticks);
		/// <summary>
		/// Returns the Years part of this instance
		/// </summary>
		public int Year
		{
			get
			{
				DatePartsFromTotalDays((int)(Ticks / TimeSpan.TicksPerDay), out int year, out _, out _);
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
				DatePartsFromTotalDays((int)(Ticks / TimeSpan.TicksPerDay), out _, out int month, out _);
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
				DatePartsFromTotalDays((int)(Ticks / TimeSpan.TicksPerDay), out _, out _, out int day);
				return day;
			}
		}
		/// <summary>
		/// Returns the Hours part of this instance
		/// </summary>
		public int Hour => (int)(Ticks / TimeSpan.TicksPerHour % 24);
		/// <summary>
		/// Returns the minutes part of this instance
		/// </summary>
		public int Minute => (int)(Ticks / TimeSpan.TicksPerMinute % 60);
		/// <summary>
		/// Returns the seconds part of this instance
		/// </summary>
		public int Second => (int)(Ticks / TimeSpan.TicksPerSecond % 60);
		/// <summary>
		/// Returns the milliseconds part of this instance
		/// </summary>
		public int Millisecond => (int)(Ticks / TimeSpan.TicksPerMillisecond % 1000);
		/// <summary>
		/// Returns the total number of days since 0001-01-01 represented by this instance
		/// </summary>
		public int TotalDays => (int)(Ticks / TimeSpan.TicksPerDay);
		/// <summary>
		/// Returns the Day of the year, from 1 to 366
		/// </summary>
		public int DayOfYear
		{
			get
			{
				int totalDays = TotalDays;
				int y400 = totalDays / DotNetTime.DaysPer400Years;
				totalDays -= DotNetTime.DaysPer400Years * y400;
				int y100 = totalDays / DotNetTime.DaysPer100Years;
				if (y100 == 4)
				{
					y100 = 3; // Adjustment
				}
				totalDays -= DotNetTime.DaysPer100Years * y100;
				int y4 = totalDays / DotNetTime.DaysPer4Years;
				totalDays -= DotNetTime.DaysPer4Years * y4;
				int y1 = totalDays / 365;
				if (y1 == 4)
				{
					y1 = 3; // Adjustment
				}

				return (totalDays -= y1 * 365) + 1;
			}
		}
		/// <summary>
		/// Gets the <see cref="System.DayOfWeek"/> represented by this instance
		/// </summary>
		public DayOfWeek DayOfWeek => (DayOfWeek)((TotalDays + 1) % 7); // 0001-01-00 is Sunday, but TotalDays = 0 is 0001-01-01. So by adding 1, we "align" ourselves with the DayOfWeek enum values.
		/// <summary>
		/// Gets the <see cref="Dates.IsoDayOfWeek"/> Day of Week represented by this instance
		/// </summary>
		public IsoDayOfWeek IsoDayOfWeek => (IsoDayOfWeek)(((TotalDays + 7) % 7) + 1); // Similar to the above, but ISO day of week goes 1-7, not 0-6, and starts on Monday instead of Sunday. So, we add 7 to align, and adjust up by 1 after the mod.
		/// <summary>
		/// Returns the Time of Day as a TimeSpan
		/// </summary>
		public TimeSpan TimeOfDay => new(Ticks % TimeSpan.TicksPerDay);
		/// <summary>
		/// The number of 100-nanosecond intervals elapsed since 0001-01-01 00:00:00 represented by this instance
		/// </summary>
		public long Ticks { get; }
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
			Deconstruct(out year, out month, out day, out _, out _, out _, out _, out _);
		}
		/// <summary>
		/// Gets all parts of this instance
		/// </summary>
		public void Deconstruct(out int year, out int month, out int day, out int hour, out int minute, out int second, out int millis, out int remainder)
		{
			DateTimePartsFromTicks(Ticks, out year, out month, out day, out hour, out minute, out second, out millis, out remainder);
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
			Deconstruct(out int year, out int month, out int day, out int hour, out int minute, out int second, out int millis, out int remainder);
			// If it's not 29th february, we don't need any sort of special handling.
			// if it is 29th February, we're still okay so long as the resultant year is also a leap year.
			// If not, then we make it the 28th of February.
			int newYear = year + years;
			// We don't call Is29thFeb here, because that would call Deconstruct a 2nd time.

			return !(month == 2 && day == 29) || DateTime.IsLeapYear(newYear)
				? new(TicksFromAll(newYear, month, day, hour, minute, second, millis, remainder).ValueOrException())
				: new(TicksFromAll(newYear, 2, 28, hour, minute, second, millis, remainder).ValueOrException());
		}
		/// <summary>
		/// Adds the specified number of months to this instance.
		/// If is instance represents a day of month that is too large for the resultant month, then the day will be the last day of the resultant month.
		/// For example: 31st of January plus 1 Month is 28th of Februray (or 29th, if a leap year).
		/// </summary>
		public UtcDateTime AddMonths(int months)
		{
			if (months == 0)
			{
				return this;
			}
			Deconstruct(out int year, out int month, out int day, out int hour, out int minute, out int second, out int millis, out int remainder);

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
			return new UtcDateTime(TicksFromAll(newYear, newMonth, newDay, hour, minute, second, millis, remainder).ValueOrException());
		}
		/// <summary>
		/// Adds <paramref name="days"/> to this instance
		/// </summary>
		public UtcDateTime AddDays(int days)
		{
			return new(Ticks + TimeSpan.TicksPerDay * days);
		}
		/// <summary>
		/// Adds <paramref name="hours"/> to this instance
		/// </summary>
		public UtcDateTime AddHours(int hours)
		{
			return new(Ticks + TimeSpan.TicksPerHour * hours);
		}
		/// <summary>
		/// Adds <paramref name="minutes"/> to this instance
		/// </summary>
		public UtcDateTime AddMinutes(int minutes)
		{
			return new(Ticks + TimeSpan.TicksPerMinute * minutes);
		}
		/// <summary>
		/// Adds <paramref name="seconds"/> to this instance
		/// </summary>
		public UtcDateTime AddSeconds(int seconds)
		{
			return new(Ticks + TimeSpan.TicksPerSecond * seconds);
		}
		/// <summary>
		/// Adds <paramref name="millis"/> to this instance
		/// </summary>
		public UtcDateTime AddMilliseconds(long millis)
		{
			return new(Ticks + TimeSpan.TicksPerMillisecond * millis);
		}
		/// <summary>
		/// Adds <paramref name="ticks"/> to this instance.
		/// </summary>
		public UtcDateTime AddTicks(long ticks)
		{
			return new(Ticks + ticks);
		}
		/// <summary>
		/// Returns a truncated instance so that it is only accurate to the part specified by <paramref name="truncateTo"/>.
		/// For example, if <paramref name="truncateTo"/> is Minute, then Seconds and Milliseconds are set to zero.
		/// Truncating days or months will cause them to be truncated to 1.
		/// </summary>
		public UtcDateTime Truncate(DateTimePart truncateTo)
		{
			// TruncateTo means that the part in question is the smallest part that should not be truncated
			return truncateTo switch
			{
				DateTimePart.Year => new UtcDateTime(Year, 1, 1),
				// It's slow to calculate year/month/day and then additionally recalculate the milliseconds from that, so we truncate to days first,
				// then remove the days (the Day property calculates year/month/day), then add 1 day so it's the 1st of the month
				DateTimePart.Month => new(Ticks - (Ticks % TimeSpan.TicksPerDay) - (Day * TimeSpan.TicksPerDay) + TimeSpan.TicksPerDay),
				DateTimePart.Day => new(Ticks - (Ticks % TimeSpan.TicksPerDay)),
				DateTimePart.Hour => new(Ticks - (Ticks % TimeSpan.TicksPerHour)),
				DateTimePart.Minute => new(Ticks - (Ticks % TimeSpan.TicksPerMinute)),
				DateTimePart.Second => new(Ticks - (Ticks % TimeSpan.TicksPerSecond)),
				DateTimePart.Millisecond => new(Ticks - (Ticks % TimeSpan.TicksPerMillisecond)),
				_ => throw new ArgumentOutOfRangeException(nameof(truncateTo), "Parameter was not a valid value for DateTimePart"),
			};
		}
		/// <summary>
		/// Returns the number of seconds elapsed since 1970-01-01 00:00:00.
		/// Equivalent to <see cref="DotNetTime.TicksToUnixTimeSeconds(long)"/>.
		/// </summary>
		public long ToUnixTimeSeconds()
		{
			return DotNetTime.TicksToUnixTimeSeconds(Ticks);
		}
		/// <summary>
		/// Creates a new instance from the provided seconds, interpreted as seconds since the Unix Epoch (1970-01-01 00:00:00).
		/// Equivalent to <see cref="DotNetTime.UnixTimeSecondsToTicks(long)"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UtcDateTime FromUnixTimeSeconds(long seconds)
		{
			return new UtcDateTime(DotNetTime.UnixTimeSecondsToTicks(seconds));
		}
		/// <summary>
		/// Returns a <see cref="DateTime"/> with the provided <paramref name="kind"/>.
		/// If <paramref name="kind"/> is <see cref="DateTimeKind.Unspecified"/> or an undefined value, throws an <see cref="ArgumentOutOfRangeException"/>.
		/// </summary>
		public DateTime ToDateTime(DateTimeKind kind = DateTimeKind.Utc)
		{
			return kind switch
			{
				DateTimeKind.Utc => new(Ticks, DateTimeKind.Utc),
				DateTimeKind.Local => new DateTime(Ticks, DateTimeKind.Utc).ToLocalTime(),
				_ => throw new ArgumentOutOfRangeException(nameof(kind), "Provided DateTimeKind must be Utc or Local, but it was " + kind.ToString()),
			};
		}
		/// <summary>
		/// Creates a new instance from the provided <paramref name="dateTime"/>. If <paramref name="dateTime"/>.DateTimeKind is Unspecified, throws <see cref="ArgumentException"/>, unless <paramref name="treatUnspecifiedAsUtc"/> is true.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime"/> to convert.</param>
		/// <param name="treatUnspecifiedAsUtc">If true, and the kind of <paramref name="dateTime"/> is <see cref="DateTimeKind.Unspecified"/>, then treats <paramref name="dateTime"/> as if it were UTC.</param>
		/// <returns>A <see cref="UtcDateTime"/>.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static UtcDateTime FromDateTime(DateTime dateTime, bool treatUnspecifiedAsUtc = false)
		{
			return dateTime.Kind switch
			{
				DateTimeKind.Utc => new(dateTime.Ticks),
				DateTimeKind.Local => new(dateTime.ToUniversalTime().Ticks),
				_ => treatUnspecifiedAsUtc
					? new(dateTime.Ticks)
					: throw new ArgumentException("Provided DateTime has an Unspecified kind; it must be either Utc or Local. Use DateTime.SpecifyKind() or DateTime.ToUniversalTime() to fix this if you know what the kind should be.", nameof(dateTime)),
			};
		}
		/// <summary>
		/// Returns a <see cref="DateTimeOffset"/>, with an offset of <see cref="TimeSpan.Zero"/>.
		/// </summary>
		public DateTimeOffset ToDateTimeOffset()
		{
			return new(Ticks, TimeSpan.Zero);
		}
		/// <summary>
		/// Returns a <see cref="UtcDateTime"/> from the provided <paramref name="dateTimeOffset"/>. Uses <see cref="DateTimeOffset.UtcTicks"/> to do so.
		/// </summary>
		public static UtcDateTime FromDateTimeOffset(DateTimeOffset dateTimeOffset)
		{
			return new(dateTimeOffset.UtcTicks);
		}
#if NET6_0_OR_GREATER
		/// <summary>
		/// Returns a <see cref="DateOnly"/>.
		/// </summary>
		public DateOnly ToDateOnly()
		{
			return DateOnly.FromDayNumber(TotalDays);
		}
		/// <summary>
		/// Returns a <see cref="UtcDateTime"/> from the provided <paramref name="dateOnly"/> and <paramref name="timeOnly"/>.
		/// </summary>
		public static UtcDateTime FromDateOnly(DateOnly dateOnly, TimeOnly timeOnly)
		{
			return new((TimeSpan.TicksPerDay * dateOnly.DayNumber) + timeOnly.Ticks);
		}
#endif
		/// <summary>
		/// Returns true of <paramref name="obj"/> is a <see cref="UtcDateTime"/> and they refer to the same point in time.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>True if equal, false if not.</returns>
		public override bool Equals([AllowNull] object obj)
		{
			return obj is UtcDateTime udt && Equals(udt);
		}
		/// <summary>
		/// Calls <see cref="GetHashCode"/> on <see cref="Ticks"/>.
		/// </summary>
		/// <returns>A hashcode.</returns>
		public override int GetHashCode()
		{
			return Ticks.GetHashCode();
		}
		/// <summary>
		/// Returns true if this instance and <paramref name="other"/> refer to the same point in time.
		/// </summary>
		public bool Equals(UtcDateTime other)
		{
			return Ticks == other.Ticks;
		}
		/// <summary>
		/// Returns true if this instance and <paramref name="other"/> are close enough.
		/// The parameter <paramref name="maxDifference"/> specifies the maximum amount of time they can differ by.
		/// </summary>
		public bool Equals(UtcDateTime other, TimeSpan maxDifference)
		{
			return AbsDifference(other) <= maxDifference;
		}
		/// <summary>
		/// Returns the absolute difference between this instance and <paramref name="other"/>.
		/// </summary>
		public TimeSpan AbsDifference(UtcDateTime other)
		{
			return Ticks > other.Ticks
				? this - other
				: other - this;
		}
		/// <summary>
		/// If this instance is later than <paramref name="other"/>, returns 1.
		/// If this instance is earlier than <paramref name="other"/>, returns -1.
		/// If they are the same point in time, returns 0.
		/// </summary>
		public int CompareTo(UtcDateTime other)
		{
			return Ticks > other.Ticks ? 1 : Ticks < other.Ticks ? -1 : 0;
		}
		/// <summary>
		/// Returns true if <paramref name="left"/> and <paramref name="right"/> refer to the same point in time, false otherwise.
		/// </summary>
		public static bool operator ==(UtcDateTime left, UtcDateTime right) => left.Ticks == right.Ticks;
		/// <summary>
		/// Returns true if <paramref name="left"/> and <paramref name="right"/> refer to different point in time, false otherwise.
		/// </summary>
		public static bool operator !=(UtcDateTime left, UtcDateTime right) => left.Ticks != right.Ticks;
		/// <summary>
		/// Returns true if <paramref name="left"/> is earlier than <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator <(UtcDateTime left, UtcDateTime right) => left.Ticks < right.Ticks;
		/// <summary>
		/// Returns true if <paramref name="left"/> is earlier than or the same point in time as <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator <=(UtcDateTime left, UtcDateTime right) => left.Ticks <= right.Ticks;
		/// <summary>
		/// Returns true if <paramref name="left"/> is later than <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator >(UtcDateTime left, UtcDateTime right) => left.Ticks > right.Ticks;
		/// <summary>
		/// Returns true if <paramref name="left"/> is later than or the same point in time as <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator >=(UtcDateTime left, UtcDateTime right) => left.Ticks >= right.Ticks;
		/// <summary>
		/// Adds <paramref name="right"/> to <paramref name="left"/>. Any sub-millisecond precision of <paramref name="right"/> is truncated.
		/// </summary>
		public static UtcDateTime operator +(UtcDateTime left, TimeSpan right) => new(left.Ticks + right.Ticks);
		/// <summary>
		/// Adds <paramref name="right"/> to <paramref name="left"/>. Any sub-millisecond precision of <paramref name="right"/> is truncated.
		/// </summary>
		public static UtcDateTime operator -(UtcDateTime left, TimeSpan right) => new(left.Ticks - right.Ticks);
		/// <summary>
		/// Returns the difference between <paramref name="left"/> and <paramref name="right"/>.
		/// </summary>
		public static TimeSpan operator -(UtcDateTime left, UtcDateTime right) => new(left.Ticks - right.Ticks);
		/// <summary>
		/// Creates a new instance from the provided <paramref name="dateTime"/>. If <paramref name="dateTime"/>.DateTimeKind is Unspecified, an <see cref="ArgumentException"/> is thrown.
		/// </summary>
		public static explicit operator UtcDateTime(DateTime dateTime) => FromDateTime(dateTime, treatUnspecifiedAsUtc: false);
		/// <summary>
		/// Creates a new instance from the provided <paramref name="dateTimeOffset"/>, using <see cref="DateTimeOffset.UtcTicks"/>.
		/// </summary>
		public static explicit operator UtcDateTime(DateTimeOffset dateTimeOffset) => FromDateTimeOffset(dateTimeOffset);
	}
}
