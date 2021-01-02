namespace MichMcb.CsExt.Dates
{
	using System;
	using System.Diagnostics.CodeAnalysis;
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
	/// This can be cast to and from <see cref="DateTime"/> (unless Kind is Unspecified), and <see cref="DateTimeOffset"/>.
	/// </summary>
	public readonly partial struct UtcDateTime : IEquatable<UtcDateTime>, IComparable<UtcDateTime>
	{
		/// <summary>
		/// 1970-01-01 00:00:00
		/// </summary>
		public static readonly UtcDateTime UnixEpoch = new UtcDateTime(DateUtil.UnixEpochMillis);
		/// <summary>
		/// 0001-01-01 00:00:00
		/// </summary>
		public static readonly UtcDateTime MinValue = new UtcDateTime(0);
		/// <summary>
		/// 9999-12-31 23:59:59.999
		/// </summary>
		public static readonly UtcDateTime MaxValue = new UtcDateTime(DateUtil.MaxMillis);
		/// <summary>
		/// Creates a new instance, as milliseconds elapsed since 0001-01-01 00:00:00
		/// </summary>
		/// <param name="millis">Milliseconds elapsed since 0001-01-01 00:00:00</param>
		public UtcDateTime(long millis)
		{
			if (millis < 0 || millis > DateUtil.MaxMillis)
			{
				throw new ArgumentOutOfRangeException(nameof(millis), "Milliseconds must be at least 0 and at most " + DateUtil.MaxMillis.ToString());
			}
			TotalMilliseconds = millis;
		}
		/// <summary>
		/// Creates a new instance, with the hours, minutes, seconds, and milliseconds parts set to 0
		/// </summary>
		public UtcDateTime(int year, int month, int day)
		{
			var ex = DateUtil.MillisFromParts(year, month, day, 0, 0, 0, 0, 0, 0, out long ms);
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
		/// <summary>
		/// Creates a new instance with the lot.
		/// </summary>
		public UtcDateTime(int year, int month, int day, int hour, int minute, int second, int millis)
		{
			var ex = DateUtil.MillisFromParts(year, month, day, hour, minute, second, millis, 0, 0, out long ms);
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
		public int Hour => (int)(TotalMilliseconds / DateUtil.MillisPerHour % 24);
		/// <summary>
		/// Returns the minutes part of this instance
		/// </summary>
		public int Minute => (int)(TotalMilliseconds / DateUtil.MillisPerMinute % 60);
		/// <summary>
		/// Returns the seconds part of this instance
		/// </summary>
		public int Second => (int)(TotalMilliseconds / DateUtil.MillisPerSecond % 60);
		/// <summary>
		/// Returns the milliseconds part of this instance
		/// </summary>
		public int Millisecond => (int)(TotalMilliseconds % DateUtil.MillisPerSecond);
		/// <summary>
		/// Returns the total number of days since 0001-01-01 represented by this instance
		/// </summary>
		public int TotalDays => (int)(TotalMilliseconds / DateUtil.MillisPerDay);
		/// <summary>
		/// Returns the Day of the year, from 1 to 366
		/// </summary>
		public int DayOfYear
		{
			get
			{
				int totalDays = TotalDays;
				int y400 = totalDays / DateUtil.DaysPer400Years;
				totalDays -= DateUtil.DaysPer400Years * y400;
				int y100 = totalDays / DateUtil.DaysPer100Years;
				if (y100 == 4)
				{
					y100 = 3; // Adjustment
				}
				totalDays -= DateUtil.DaysPer100Years * y100;
				int y4 = totalDays / DateUtil.DaysPer4Years;
				totalDays -= DateUtil.DaysPer4Years * y4;
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
			DateUtil.CalcDateTimeParts(TotalMilliseconds, out year, out month, out day, out hour, out minute, out second, out millis);
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
			return new UtcDateTime(TotalMilliseconds + days * DateUtil.MillisPerDay);
		}
		/// <summary>
		/// Adds <paramref name="hours"/> to this instance
		/// </summary>
		public UtcDateTime AddHours(int hours)
		{
			return new UtcDateTime(TotalMilliseconds + hours * DateUtil.MillisPerHour);
		}
		/// <summary>
		/// Adds <paramref name="minutes"/> to this instance
		/// </summary>
		public UtcDateTime AddMinutes(int minutes)
		{
			return new UtcDateTime(TotalMilliseconds + minutes * DateUtil.MillisPerMinute);
		}
		/// <summary>
		/// Adds <paramref name="seconds"/> to this instance
		/// </summary>
		public UtcDateTime AddSeconds(int seconds)
		{
			return new UtcDateTime(TotalMilliseconds + seconds * DateUtil.MillisPerSecond);
		}
		/// <summary>
		/// Adds <paramref name="millis"/> to this instance
		/// </summary>
		public UtcDateTime AddMilliseconds(int millis)
		{
			return new UtcDateTime(TotalMilliseconds + millis);
		}
		//}
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
				DateTimePart.Month => new UtcDateTime(TotalMilliseconds - (Day * DateUtil.MillisPerDay) + DateUtil.MillisPerDay - (Hour * DateUtil.MillisPerHour) - (Minute * DateUtil.MillisPerMinute) - (Second * DateUtil.MillisPerSecond) - Millisecond),
				DateTimePart.Day => new UtcDateTime(TotalMilliseconds - (Hour * DateUtil.MillisPerHour) - (Minute * DateUtil.MillisPerMinute) - (Second * DateUtil.MillisPerSecond) - Millisecond),
				DateTimePart.Hour => new UtcDateTime(TotalMilliseconds - (Minute * DateUtil.MillisPerMinute) - (Second * DateUtil.MillisPerSecond) - Millisecond),
				DateTimePart.Minute => new UtcDateTime(TotalMilliseconds - Second * DateUtil.MillisPerSecond - Millisecond),
				DateTimePart.Second => new UtcDateTime(TotalMilliseconds - Millisecond),
				DateTimePart.Millisecond => this,
				_ => throw new ArgumentOutOfRangeException(nameof(truncateTo), "Parameter was not a valid value for DateTimePart"),
			};
		}
		/// <summary>
		/// Creates a new instance, as the number of <paramref name="days"/> elapsed since 0001-01-01 00:00:00.
		/// Optionally allows specifying the hour/minute/second/millisecond
		/// </summary>
		public static UtcDateTime FromDays(int days, int hour = 0, int minute = 0, int second = 0, int millis = 0)
		{
			ArgumentOutOfRangeException? ex = DateUtil.CheckTimeParts(hour, minute, second, millis, 0, 0);
			return ex != null
				? throw ex
				: new UtcDateTime(days * DateUtil.MillisPerDay + hour * DateUtil.MillisPerHour + minute * DateUtil.MillisPerMinute + second * DateUtil.MillisPerSecond + millis);
		}
		/// <summary>
		/// Creates a new instance from the provided seconds, interpreted as seconds since the Unix Epoch (1970-01-01 00:00:00).
		/// Negative values are allowed.
		/// </summary>
		public static UtcDateTime FromUnixEpochSeconds(long seconds)
		{
			return new UtcDateTime(seconds * DateUtil.MillisPerSecond + DateUtil.UnixEpochMillis);
		}
		/// <summary>
		/// Creates a new instance from the provided seconds, interpreted as milliseconds since the Unix Epoch (1970-01-01 00:00:00).
		/// Negative values are allowed.
		/// </summary>
		public static UtcDateTime FromUnixEpochMilliseconds(long milliseconds)
		{
			return new UtcDateTime(milliseconds + DateUtil.UnixEpochMillis);
		}
		/// <summary>
		/// Returns true of <paramref name="obj"/> is a <see cref="UtcDateTime"/> and they refer to the same point in time.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>True if equal, false if not.</returns>
		public override bool Equals([AllowNull]object obj)
		{
			return obj is UtcDateTime time && Equals(time);
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
		public static bool operator ==(in UtcDateTime left, in UtcDateTime right) => left.TotalMilliseconds == right.TotalMilliseconds;
		/// <summary>
		/// Returns true if <paramref name="left"/> and <paramref name="right"/> refer to different point in time, false otherwise.
		/// </summary>
		public static bool operator !=(in UtcDateTime left, in UtcDateTime right) => left.TotalMilliseconds != right.TotalMilliseconds;
		/// <summary>
		/// Returns true if <paramref name="left"/> is earlier than <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator <(UtcDateTime left, UtcDateTime right) => left.CompareTo(right) < 0;
		/// <summary>
		/// Returns true if <paramref name="left"/> is earlier than or the same point in time as <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator <=(UtcDateTime left, UtcDateTime right) => left.CompareTo(right) <= 0;
		/// <summary>
		/// Returns true if <paramref name="left"/> is later than <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator >(UtcDateTime left, UtcDateTime right) => left.CompareTo(right) > 0;
		/// <summary>
		/// Returns true if <paramref name="left"/> is later than or the same point in time as <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator >=(UtcDateTime left, UtcDateTime right) => left.CompareTo(right) >= 0;
		/// <summary>
		/// Adds <paramref name="right"/> to <paramref name="left"/>. Any sub-millisecond precision of <paramref name="right"/> is truncated.
		/// </summary>
		public static UtcDateTime operator +(UtcDateTime left, TimeSpan right) => new UtcDateTime(left.TotalMilliseconds + right.Ticks / TimeSpan.TicksPerMillisecond);
		/// <summary>
		/// Returns the difference between <paramref name="left"/> and <paramref name="right"/>.
		/// </summary>
		public static TimeSpan operator -(UtcDateTime left, UtcDateTime right) => new TimeSpan((left.TotalMilliseconds - right.TotalMilliseconds) * TimeSpan.TicksPerMillisecond);
		/// <summary>
		/// Converts this UtcDateTime instance to a DateTime instance, with a DateTimeKind of Utc
		/// </summary>
		public static implicit operator DateTime(UtcDateTime utcDateTime) => new DateTime(utcDateTime.TotalMilliseconds * TimeSpan.TicksPerMillisecond, DateTimeKind.Utc);
		/// <summary>
		/// Converts this UtcDateTime instance to a DateTimeOffset instance, with an Offset of TimeSpan.Zero
		/// </summary>
		public static implicit operator DateTimeOffset(UtcDateTime utcDateTime) => new DateTimeOffset(utcDateTime.TotalMilliseconds * TimeSpan.TicksPerMillisecond, TimeSpan.Zero);
		/// <summary>
		/// Creates a new instance from the provided <paramref name="dateTime"/>. If <paramref name="dateTime"/>.DateTimeKind is Unspecified, an <see cref="ArgumentException"/> is thrown.
		/// </summary>
		public static explicit operator UtcDateTime(DateTime dateTime)
		{
			return dateTime.Kind == DateTimeKind.Unspecified
				? throw new ArgumentException("Provided DateTime has an Unspecified kind; it must be either Utc or Local. Use DateTime.SpecifyKind() to fix this if you know what the kind should be.", nameof(dateTime))
				: dateTime.Kind == DateTimeKind.Utc
					? new UtcDateTime(dateTime.Ticks / TimeSpan.TicksPerMillisecond)
					: new UtcDateTime(dateTime.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond);
		}
		/// <summary>
		/// Creates a new instance from the provided <paramref name="dateTimeOffset"/>, interpreting <paramref name="dateTimeOffset"/> as if its Offset were Zero.
		/// To be specific, <paramref name="dateTimeOffset"/>.Offset is subtracted from <paramref name="dateTimeOffset"/> to make the offset Zero, and then that is converted to a UtcDateTime.
		/// </summary>
		public static explicit operator UtcDateTime(DateTimeOffset dateTimeOffset) => new UtcDateTime((dateTimeOffset.Ticks - dateTimeOffset.Offset.Ticks) / TimeSpan.TicksPerMillisecond);
	}
}
