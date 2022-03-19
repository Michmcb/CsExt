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
		public static readonly UtcDateTime UnixEpoch = new(UnixEpochMillis);
		/// <summary>
		/// 0001-01-01 00:00:00
		/// </summary>
		public static readonly UtcDateTime MinValue = new(0);
		/// <summary>
		/// 9999-12-31 23:59:59.999
		/// </summary>
		public static readonly UtcDateTime MaxValue = new(MaxMillis);
		/// <summary>
		/// Creates a new instance, as milliseconds elapsed since 0001-01-01 00:00:00
		/// </summary>
		/// <param name="millis">Milliseconds elapsed since 0001-01-01 00:00:00</param>
		public UtcDateTime(long millis)
		{
			if (millis < 0 || millis > MaxMillis)
			{
				throw new ArgumentOutOfRangeException(nameof(millis), "Milliseconds must be at least 0 and at most " + MaxMillis.ToString());
			}
			TotalMilliseconds = millis;
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
			if (!MillisFromHourMinuteSecondMillisTimezoneOffset(hour, minute, second, millis, 0).Success(out long tms, out string err))
			{
				throw new ArgumentOutOfRangeException(null, err);
			}
			if (!MillisFromYearMonthDay(year, month, day).Success(out long dms, out err))
			{
				throw new ArgumentOutOfRangeException(null, err);
			}
			TotalMilliseconds = tms + dms;
		}
		/// <summary>
		/// Returns an instance representing the current UTC time
		/// </summary>
		public static UtcDateTime Now => new(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);
		/// <summary>
		/// Returns the Years part of this instance
		/// </summary>
		public int Year
		{
			get
			{
				DatePartsFromTotalDays((int)(TotalMilliseconds / MillisPerDay), out int year, out _, out _);
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
				DatePartsFromTotalDays((int)(TotalMilliseconds / MillisPerDay), out _, out int month, out _);
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
				DatePartsFromTotalDays((int)(TotalMilliseconds / MillisPerDay), out _, out _, out int day);
				return day;
			}
		}
		/// <summary>
		/// Returns the Hours part of this instance
		/// </summary>
		public int Hour => (int)(TotalMilliseconds / MillisPerHour % 24);
		/// <summary>
		/// Returns the minutes part of this instance
		/// </summary>
		public int Minute => (int)(TotalMilliseconds / MillisPerMinute % 60);
		/// <summary>
		/// Returns the seconds part of this instance
		/// </summary>
		public int Second => (int)(TotalMilliseconds / MillisPerSecond % 60);
		/// <summary>
		/// Returns the milliseconds part of this instance
		/// </summary>
		public int Millisecond => (int)(TotalMilliseconds % MillisPerSecond);
		/// <summary>
		/// Returns the total number of days since 0001-01-01 represented by this instance
		/// </summary>
		public int TotalDays => (int)(TotalMilliseconds / MillisPerDay);
		/// <summary>
		/// Returns the Day of the year, from 1 to 366
		/// </summary>
		public int DayOfYear
		{
			get
			{
				int totalDays = TotalDays;
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
		public TimeSpan TimeOfDay => new(TotalMilliseconds % MillisPerDay * TimeSpan.TicksPerMillisecond);
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
			DateTimePartsFromTotalMilliseconds(TotalMilliseconds, out year, out month, out day, out hour, out minute, out second, out millis);
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
			// We don't call Is29thFeb here, because that would call Deconstruct a 2nd time.
			return !(month == 2 && day == 29) || DateTime.IsLeapYear(newYear)
				? new UtcDateTime(newYear, month, day, hour, minute, second, millis)
				: new UtcDateTime(newYear, 2, 28, hour, minute, second, millis);
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
			return days == 0 ? this : new UtcDateTime(TotalMilliseconds + days * MillisPerDay);
		}
		/// <summary>
		/// Adds <paramref name="hours"/> to this instance
		/// </summary>
		public UtcDateTime AddHours(int hours)
		{
			return new UtcDateTime(TotalMilliseconds + (hours * MillisPerHour));
		}
		/// <summary>
		/// Adds <paramref name="minutes"/> to this instance
		/// </summary>
		public UtcDateTime AddMinutes(int minutes)
		{
			return new UtcDateTime(TotalMilliseconds + minutes * MillisPerMinute);
		}
		/// <summary>
		/// Adds <paramref name="seconds"/> to this instance
		/// </summary>
		public UtcDateTime AddSeconds(int seconds)
		{
			return new UtcDateTime(TotalMilliseconds + seconds * MillisPerSecond);
		}
		/// <summary>
		/// Adds <paramref name="millis"/> to this instance
		/// </summary>
		public UtcDateTime AddMilliseconds(long millis)
		{
			return new UtcDateTime(TotalMilliseconds + millis);
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
				// then remove the days (the Day property calculates year/month/day)
				DateTimePart.Month => new(TotalMilliseconds - (TotalMilliseconds % MillisPerDay) - (Day * MillisPerDay) + MillisPerDay),
				DateTimePart.Day => new UtcDateTime(TotalMilliseconds - (TotalMilliseconds % MillisPerDay)),
				DateTimePart.Hour => new UtcDateTime(TotalMilliseconds - (TotalMilliseconds % MillisPerHour)),
				DateTimePart.Minute => new UtcDateTime(TotalMilliseconds - (TotalMilliseconds % MillisPerMinute)),
				DateTimePart.Second => new UtcDateTime(TotalMilliseconds - Millisecond),
				DateTimePart.Millisecond => this,
				_ => throw new ArgumentOutOfRangeException(nameof(truncateTo), "Parameter was not a valid value for DateTimePart"),
			};
		}
		/// <summary>
		/// Returns the number of seconds elapsed since 1970-01-01 00:00:00.
		/// </summary>
		public long ToUnixTimeSeconds()
		{
			return (TotalMilliseconds - UnixEpochMillis) / MillisPerSecond;
		}
		/// <summary>
		/// Returns the number of milliseconds elapsed since 1970-01-01 00:00:00.
		/// </summary>
		public long ToUnixTimeMilliseconds()
		{
			return TotalMilliseconds - UnixEpochMillis;
		}
		/// <summary>
		/// Creates a new instance from the provided seconds, interpreted as seconds since the Unix Epoch (1970-01-01 00:00:00).
		/// The value of (<paramref name="seconds"/> * <see cref="MillisPerSecond"/>) must be within the range of <see cref="MinMillisAsUnixTime"/> and <see cref="MaxMillisAsUnixTime"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UtcDateTime FromUnixTimeSeconds(long seconds)
		{
			return FromUnixTimeMilliseconds(seconds * MillisPerSecond);
		}
		/// <summary>
		/// Creates a new instance from the provided seconds, interpreted as milliseconds since the Unix Epoch (1970-01-01 00:00:00).
		/// <paramref name="milliseconds"/> must be within the range of <see cref="MinMillisAsUnixTime"/> and <see cref="MaxMillisAsUnixTime"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UtcDateTime FromUnixTimeMilliseconds(long milliseconds)
		{
			return milliseconds < MinMillisAsUnixTime || milliseconds > MaxMillisAsUnixTime
				? throw new ArgumentOutOfRangeException(nameof(milliseconds), "Unix Epoch milliseconds must be at least " + MinMillisAsUnixTime + " and at most " + MaxMillisAsUnixTime)
				: new UtcDateTime(milliseconds + UnixEpochMillis);
		}
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
		/// Calls <see cref="GetHashCode"/> on <see cref="TotalMilliseconds"/>.
		/// </summary>
		/// <returns>A hashcode.</returns>
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
		public bool Equals(UtcDateTime other, ulong millisDifference)
		{
			return AbsDifferenceMillis(other) <= millisDifference;
		}
		/// <summary>
		/// Returns the absolute difference, in milliseconds, between this instance and <paramref name="other"/>.
		/// </summary>
		public ulong AbsDifferenceMillis(UtcDateTime other)
		{
			return TotalMilliseconds > other.TotalMilliseconds
				? (ulong)TotalMilliseconds - (ulong)other.TotalMilliseconds
				: (ulong)other.TotalMilliseconds - (ulong)TotalMilliseconds;
		}
		/// <summary>
		/// If this instance is later than <paramref name="other"/>, returns 1.
		/// If this instance is earlier than <paramref name="other"/>, returns -1.
		/// If they are the same point in time, returns 0.
		/// </summary>
		public int CompareTo(UtcDateTime other)
		{
			return TotalMilliseconds > other.TotalMilliseconds ? 1 : TotalMilliseconds < other.TotalMilliseconds ? -1 : 0;
		}
		/// <summary>
		/// Returns true if <paramref name="left"/> and <paramref name="right"/> refer to the same point in time, false otherwise.
		/// </summary>
		public static bool operator ==(UtcDateTime left, UtcDateTime right) => left.TotalMilliseconds == right.TotalMilliseconds;
		/// <summary>
		/// Returns true if <paramref name="left"/> and <paramref name="right"/> refer to different point in time, false otherwise.
		/// </summary>
		public static bool operator !=(UtcDateTime left, UtcDateTime right) => left.TotalMilliseconds != right.TotalMilliseconds;
		/// <summary>
		/// Returns true if <paramref name="left"/> is earlier than <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator <(UtcDateTime left, UtcDateTime right) => left.TotalMilliseconds < right.TotalMilliseconds;
		/// <summary>
		/// Returns true if <paramref name="left"/> is earlier than or the same point in time as <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator <=(UtcDateTime left, UtcDateTime right) => left.TotalMilliseconds <= right.TotalMilliseconds;
		/// <summary>
		/// Returns true if <paramref name="left"/> is later than <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator >(UtcDateTime left, UtcDateTime right) => left.TotalMilliseconds > right.TotalMilliseconds;
		/// <summary>
		/// Returns true if <paramref name="left"/> is later than or the same point in time as <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator >=(UtcDateTime left, UtcDateTime right) => left.TotalMilliseconds >= right.TotalMilliseconds;
		/// <summary>
		/// Adds <paramref name="right"/> to <paramref name="left"/>. Any sub-millisecond precision of <paramref name="right"/> is truncated.
		/// </summary>
		public static UtcDateTime operator +(UtcDateTime left, TimeSpan right) => new(left.TotalMilliseconds + right.Ticks / TimeSpan.TicksPerMillisecond);
		/// <summary>
		/// Adds <paramref name="right"/> to <paramref name="left"/>. Any sub-millisecond precision of <paramref name="right"/> is truncated.
		/// </summary>
		public static UtcDateTime operator -(UtcDateTime left, TimeSpan right) => new(left.TotalMilliseconds - right.Ticks / TimeSpan.TicksPerMillisecond);
		/// <summary>
		/// Returns the difference between <paramref name="left"/> and <paramref name="right"/>.
		/// </summary>
		public static TimeSpan operator -(UtcDateTime left, UtcDateTime right) => new((left.TotalMilliseconds - right.TotalMilliseconds) * TimeSpan.TicksPerMillisecond);
		/// <summary>
		/// Converts this UtcDateTime instance to a DateTime instance, with a DateTimeKind of Utc
		/// </summary>
		public static implicit operator DateTime(UtcDateTime utcDateTime) => new(utcDateTime.TotalMilliseconds * TimeSpan.TicksPerMillisecond, DateTimeKind.Utc);
		/// <summary>
		/// Converts this UtcDateTime instance to a DateTimeOffset instance, with an Offset of TimeSpan.Zero
		/// </summary>
		public static implicit operator DateTimeOffset(UtcDateTime utcDateTime) => new(utcDateTime.TotalMilliseconds * TimeSpan.TicksPerMillisecond, TimeSpan.Zero);
		/// <summary>
		/// Creates a new instance from the provided <paramref name="dateTime"/>. If <paramref name="dateTime"/>.DateTimeKind is Unspecified, an <see cref="ArgumentException"/> is thrown.
		/// </summary>
		public static explicit operator UtcDateTime(DateTime dateTime)
		{
			return dateTime.Kind switch
			{
				DateTimeKind.Utc => new UtcDateTime(dateTime.Ticks / TimeSpan.TicksPerMillisecond),
				DateTimeKind.Local => new UtcDateTime(dateTime.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond),
				_ => throw new ArgumentException("Provided DateTime has an Unspecified kind; it must be either Utc or Local. Use DateTime.SpecifyKind() or DateTime.ToUniversalTime() to fix this if you know what the kind should be.", nameof(dateTime)),
			};
		}
		/// <summary>
		/// Creates a new instance from the provided <paramref name="dateTimeOffset"/>, using <see cref="DateTimeOffset.UtcTicks"/>.
		/// </summary>
		public static explicit operator UtcDateTime(DateTimeOffset dateTimeOffset) => new(dateTimeOffset.UtcTicks / TimeSpan.TicksPerMillisecond);
	}
}
