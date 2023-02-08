namespace MichMcb.CsExt.Dates
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Represents a UTC Date Time, as ticks since 0001-01-01 00:00:00.000.
	/// Unlike <see cref="System.DateTime"/> and <see cref="DateTimeOffset"/>, this is only ever UTC, which can help if you want to differentiate by type.
	/// This can be cast to and from <see cref="System.DateTime"/> (unless Kind is Unspecified), and <see cref="DateTimeOffset"/>.
	/// </summary>
	public readonly partial struct UtcDateTime
		: IEquatable<UtcDateTime>
		, IComparable<UtcDateTime>
#if NET7_0_OR_GREATER
		, ISpanParsable<UtcDateTime>
#endif
	{
		private readonly DateTime _dt;
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
		private UtcDateTime(DateTime dt)
		{
			_dt = dt;
		}
		/// <summary>
		/// Creates a new instance, as ticks elapsed since 0001-01-01 00:00:00
		/// </summary>
		/// <param name="ticks">Ticks elapsed since 0001-01-01 00:00:00</param>
		public UtcDateTime(long ticks)
		{
			_dt = new(ticks, DateTimeKind.Utc);
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
			_dt = new(year, month, day, hour, minute, second, millis, DateTimeKind.Utc);
		}
		/// <summary>
		/// Returns an instance representing the current UTC time
		/// </summary>
		public static UtcDateTime Now => new(DateTime.UtcNow.Ticks);
		/// <summary>
		/// Identical to calling <see cref="Truncate(DateTimePart)"/> with <see cref="DateTimePart.Day"/>.
		/// </summary>
		public UtcDateTime Date => Truncate(DateTimePart.Day);
		/// <summary>
		/// Returns a <see cref="System.DateTime"/> whose kind is <see cref="DateTimeKind.Utc"/>.
		/// </summary>
		public DateTime DateTime => _dt;
		/// <summary>
		/// Returns a <see cref="System.DateTime"/> whose kind is <see cref="DateTimeKind.Local"/>.
		/// Equivalent to calling <see cref="DateTime.ToLocalTime"/> on <see cref="DateTime"/>.
		/// </summary>
		public DateTime Local => _dt.ToLocalTime();
		/// <summary>
		/// Returns the Years part of this instance
		/// </summary>
		public int Year => _dt.Year;
		/// <summary>
		/// Returns the Months part of this instance
		/// </summary>
		public int Month => _dt.Month;
		/// <summary>
		/// Returns the Days part of this instance
		/// </summary>
		public int Day => _dt.Day;
		/// <summary>
		/// Returns the Hours part of this instance
		/// </summary>
		public int Hour => _dt.Hour;
		/// <summary>
		/// Returns the minutes part of this instance
		/// </summary>
		public int Minute => _dt.Minute;
		/// <summary>
		/// Returns the seconds part of this instance
		/// </summary>
		public int Second => _dt.Second;
		/// <summary>
		/// Returns the milliseconds part of this instance
		/// </summary>
		public int Millisecond => _dt.Millisecond;
#if NET7_0_OR_GREATER
		/// <summary>
		/// The microseconds component, expressed as a value between 0 and 999.
		/// </summary>
		public int Microsecond => _dt.Microsecond;
		/// <summary>
		/// The nanoseconds component, expressed as a value between 0 and 900 (in increments of 100 nanoseconds).
		/// </summary>
		public int Nanosecond => _dt.Nanosecond;
#endif
		/// <summary>
		/// Returns the total number of days since 0001-01-01 represented by this instance
		/// </summary>
		public int TotalDays => (int)(Ticks / TimeSpan.TicksPerDay);
		/// <summary>
		/// Returns the Day of the year, from 1 to 366
		/// </summary>
		public int DayOfYear => _dt.DayOfYear;
		/// <summary>
		/// Gets the <see cref="System.DayOfWeek"/> represented by this instance
		/// </summary>
		public DayOfWeek DayOfWeek => _dt.DayOfWeek; 
		/// <summary>
		/// Gets the <see cref="Dates.IsoDayOfWeek"/> Day of Week represented by this instance
		/// </summary>
		public IsoDayOfWeek IsoDayOfWeek => (IsoDayOfWeek)(((TotalDays + 7) % 7) + 1);
		/*
		 * 0001-01-00 is Sunday, but TotalDays = 0 is 0001-01-01. So by adding 1, we "align" ourselves with the DayOfWeek enum values.
		 * ISO day of week goes 1-7, not 0-6, and starts on Monday instead of Sunday. So, we add 7 to align, and adjust up by 1 after the mod.
		 */
		/// <summary>
		/// Returns the Time of Day as a <see cref="TimeSpan"/>.
		/// </summary>
		public TimeSpan TimeOfDay => _dt.TimeOfDay;
#if NET6_0_OR_GREATER
		/// <summary>
		/// Returns the Time of Day as a <see cref="TimeOnly"/>.
		/// </summary>
		public TimeOnly Time => TimeOnly.FromTimeSpan(_dt.TimeOfDay);
#endif
		/// <summary>
		/// The number of 100-nanosecond intervals elapsed since 0001-01-01 00:00:00 represented by this instance
		/// </summary>
		public long Ticks => _dt.Ticks;
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
		/// Adds the specified <paramref name="timeSpan"/> to this instance.
		/// </summary>
		public UtcDateTime Add(TimeSpan timeSpan)
		{
			return new(_dt.Add(timeSpan));
		}
		/// <summary>
		/// Subtracts the specified <paramref name="timeSpan"/> from this instance.
		/// </summary>
		public UtcDateTime Subtract(TimeSpan timeSpan)
		{
			return new(_dt.Subtract(timeSpan));
		}
		/// <summary>
		/// Adds the specified number of years to this instance.
		/// If this instance represents the 29th of February, and the result is not a leap year, the result will be 28th February.
		/// The reason is so the month doesn't change from February to March suddenly, which is weird.
		/// </summary>
		public UtcDateTime AddYears(int years)
		{
			return new(_dt.AddYears(years));
		}
		/// <summary>
		/// Adds the specified number of months to this instance.
		/// If is instance represents a day of month that is too large for the resultant month, then the day will be the last day of the resultant month.
		/// For example: 31st of January plus 1 Month is 28th of Februray (or 29th, if a leap year).
		/// </summary>
		public UtcDateTime AddMonths(int months)
		{
			return new(_dt.AddMonths(months));
		}
		/// <summary>
		/// Adds <paramref name="days"/> to this instance
		/// </summary>
		public UtcDateTime AddDays(int days)
		{
			return new(_dt.AddDays(days));
		}
		/// <summary>
		/// Adds <paramref name="hours"/> to this instance
		/// </summary>
		public UtcDateTime AddHours(int hours)
		{
			return new(_dt.AddHours(hours));
		}
		/// <summary>
		/// Adds <paramref name="minutes"/> to this instance
		/// </summary>
		public UtcDateTime AddMinutes(int minutes)
		{
			return new(_dt.AddMinutes(minutes));
		}
		/// <summary>
		/// Adds <paramref name="seconds"/> to this instance
		/// </summary>
		public UtcDateTime AddSeconds(int seconds)
		{
			return new(_dt.AddSeconds(seconds));
		}
		/// <summary>
		/// Adds <paramref name="millis"/> to this instance
		/// </summary>
		public UtcDateTime AddMilliseconds(long millis)
		{
			return new(_dt.AddMilliseconds(millis));
		}
		/// <summary>
		/// Adds <paramref name="ticks"/> to this instance.
		/// </summary>
		public UtcDateTime AddTicks(long ticks)
		{
			return new(_dt.AddTicks(ticks));
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
		/// Returns a <see cref="System.DateTime"/> with the provided <paramref name="kind"/>.
		/// Equivalent to <see cref="DateTime"/> and <see cref="Local"/>.
		/// If <paramref name="kind"/> is <see cref="DateTimeKind.Unspecified"/> or an undefined value, throws an <see cref="ArgumentOutOfRangeException"/>.
		/// </summary>
		public DateTime ToDateTime(DateTimeKind kind = DateTimeKind.Utc)
		{
			return kind switch
			{
				DateTimeKind.Utc => _dt,
				DateTimeKind.Local => Local,
				_ => throw new ArgumentOutOfRangeException(nameof(kind), "Provided DateTimeKind must be Utc or Local, but it was " + kind.ToString()),
			};
		}
		/// <summary>
		/// Creates a new instance from the provided <paramref name="dateTime"/>. If <paramref name="dateTime"/>.DateTimeKind is Unspecified, throws <see cref="ArgumentException"/>, unless <paramref name="treatUnspecifiedAsUtc"/> is true.
		/// </summary>
		/// <param name="dateTime">The <see cref="System.DateTime"/> to convert.</param>
		/// <param name="treatUnspecifiedAsUtc">If true, and the kind of <paramref name="dateTime"/> is <see cref="DateTimeKind.Unspecified"/>, then treats <paramref name="dateTime"/> as if it were UTC.</param>
		/// <returns>A <see cref="UtcDateTime"/>.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static UtcDateTime FromDateTime(DateTime dateTime, bool treatUnspecifiedAsUtc = false)
		{
			return dateTime.Kind switch
			{
				DateTimeKind.Utc => new(dateTime),
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
		/// Adds <paramref name="right"/> to <paramref name="left"/>.
		/// </summary>
		public static UtcDateTime operator +(UtcDateTime left, TimeSpan right) => new(left.Ticks + right.Ticks);
		/// <summary>
		/// Adds <paramref name="right"/> to <paramref name="left"/>.
		/// </summary>
		public static UtcDateTime operator -(UtcDateTime left, TimeSpan right) => new(left.Ticks - right.Ticks);
#if NET6_0_OR_GREATER
		/// <summary>
		/// Adds <paramref name="right"/> to <paramref name="left"/>.
		/// </summary>
		public static UtcDateTime operator +(UtcDateTime left, TimeOnly right) => new(left.Ticks + right.Ticks);
		/// <summary>
		/// Adds <paramref name="right"/> to <paramref name="left"/>.
		/// </summary>
		public static UtcDateTime operator -(UtcDateTime left, TimeOnly right) => new(left.Ticks - right.Ticks);
#endif
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
