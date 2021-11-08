namespace MichMcb.CsExt.Dates
{
	using System;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// Represents a Date without a Time.
	/// Use with care, as this is inherently ambiguous. It's useful in situations where you are only given a Date, but then you can apply a Time to it.
	/// </summary>
	public readonly partial struct Date
	{
		/// <summary>
		/// 1970-01-01
		/// </summary>
		public static readonly Date UnixEpoch = new(DateUtil.UnixEpochDays);
		/// <summary>
		/// 0001-01-01
		/// </summary>
		public static readonly Date MinValue = new(0);
		/// <summary>
		/// 9999-12-31
		/// </summary>
		public static readonly Date MaxValue = new(DateUtil.MaxDays);
		/// <summary>
		/// Creates a new instance, as days elapsed since 0001-01-01
		/// </summary>
		/// <param name="totalDays">Days elapsed since 0001-01-01</param>
		public Date(int totalDays)
		{
			if (totalDays < 0 || totalDays > DateUtil.MaxDays)
			{
				throw new ArgumentOutOfRangeException(nameof(totalDays), "Total Days is out of the range of representable values.");
			}
			TotalDays = totalDays;
		}
		/// <summary>
		/// Creates a new instance with the provided <paramref name="year"/>, <paramref name="month"/>, <paramref name="day"/>.
		/// </summary>
		public Date(int year, int month, int day)
		{
			ArgumentOutOfRangeException? ex = DateUtil.TotalDaysFromParts(year, month, day, out int totalDays);
			if (ex != null)
			{
				throw ex;
			}
			TotalDays = totalDays;
		}
		/// <summary>
		/// The number of days elapsed since 0001-01-01 represented by this instance
		/// </summary>
		public int TotalDays { get; }
		/// <summary>
		/// Returns the Years part of this instance
		/// </summary>
		public int Year
		{
			get
			{
				DateUtil.CalcDateParts(TotalDays, out int year, out _, out _);
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
				DateUtil.CalcDateParts(TotalDays, out _, out int month, out _);
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
				DateUtil.CalcDateParts(TotalDays, out _, out _, out int day);
				return day;
			}
		}
		/// <summary>
		/// Gets the Day of Week represented by this instance
		/// </summary>
		public DayOfWeek DayOfWeek => (DayOfWeek)((TotalDays + 1) % 7);
		/// <summary>
		/// Returns the Day of the year, from 1 to 366
		/// </summary>
		public int DayOfYear => DateUtil.DayOfYear(TotalDays);
		/// <summary>
		/// Returns true if this instance represents the 29th of February
		/// </summary>
		public bool Is29thFeb
		{
			get
			{
				DateUtil.CalcDateParts(TotalDays, out _, out int month, out int day);
				return month == 2 && day == 29;
			}
		}
		/// <summary>
		/// Gets the year, month, and day parts of this instance
		/// </summary>
		public void Deconstruct(out int year, out int month, out int day)
		{
			DateUtil.CalcDateParts(TotalDays, out year, out month, out day);
		}
		/// <summary>
		/// Adds the specified number of years to this instance.
		/// If this instance represents the 29th of February, and the result is not a leap year, the result will be 28th February.
		/// The reason is so the month doesn't change from February to March suddenly, which is weird.
		/// </summary>
		public Date AddYears(int years)
		{
			if (years == 0)
			{
				return this;
			}
			Deconstruct(out int year, out int month, out int day);
			// If it's not 29th february, we don't need any sort of special handling.
			// if it is 29th February, we're still okay so long as the resultant year is also a leap year.
			// If not, then we make it the 28th of February.
			int newYear = year + years;
			// We don't call Is29thFeb here, because that would call Deconstruct a 2nd time.
			return !(month == 2 && day == 29) || DateTime.IsLeapYear(newYear)
				? new Date(newYear, month, day)
				: new Date(newYear, 2, 28);
		}
		/// <summary>
		/// Adds the specified number of months to this instance.
		/// If is instance represents a day of month that is too large for the resultant month, then the day will be the last day of the resultant month.
		/// For example: 31st of January plus 1 Month is 28th of Februray (or 29th, if a leap year).
		/// </summary>
		public Date AddMonths(int months)
		{
			if (months == 0)
			{
				return this;
			}
			Deconstruct(out int year, out int month, out int day);

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
			return new Date(newYear, newMonth, newDay);
		}
		/// <summary>
		/// Adds <paramref name="days"/> to this instance
		/// </summary>
		public Date AddDays(int days)
		{
			return days == 0 ? this : new Date(TotalDays + days);
		}
		/// <summary>
		/// Returns a truncated instance so that it is only accurate to the part specified by <paramref name="truncateTo"/>.
		/// Truncating days or months will cause them to be truncated to 1.
		/// </summary>
		public Date Truncate(DateTimePart truncateTo)
		{
			// TruncateTo means that the part in question is the smallest part that should not be truncated
			return truncateTo switch
			{
				DateTimePart.Year => new(Year, 1, 1),
				DateTimePart.Month => new(TotalDays - Day + 1),
				DateTimePart.Day => this,
				DateTimePart.Hour => this,
				DateTimePart.Minute => this,
				DateTimePart.Second => this,
				DateTimePart.Millisecond => this,
				_ => throw new ArgumentOutOfRangeException(nameof(truncateTo), "Parameter was not a valid value for DateTimePart"),
			};
		}
		/// <summary>
		/// Creates a new <see cref="UtcDateTime"/> from this date and the provided <paramref name="time"/>.
		/// </summary>
		/// <param name="time">The time portion.</param>
		public UtcDateTime AsUtcDateTime(TimeSpan time)
		{
			return new((DateUtil.MillisPerDay * TotalDays) + (time.Ticks / TimeSpan.TicksPerMillisecond));
		}
		/// <summary>
		/// Creates a new <see cref="UtcDateTime"/> from this date and the provided parts.
		/// </summary>
		/// <param name="hour">The hours.</param>
		/// <param name="minute">The minutes.</param>
		/// <param name="second">The seconds.</param>
		/// <param name="millis">The milliseconds.</param>
		public UtcDateTime AsUtcDateTime(int hour, int minute, int second, int millis)
		{
			return new((DateUtil.MillisPerDay * TotalDays) + (DateUtil.MillisPerHour * hour) + (DateUtil.MillisPerMinute * minute) + (DateUtil.MillisPerSecond * second) + millis);
		}
		/// <summary>
		/// Creates a new <see cref="DateTime"/> from this date and the provided <paramref name="time"/>.
		/// </summary>
		/// <param name="time">The time portion.</param>
		/// <param name="kind">The kind.</param>
		public DateTime AsDateTime(TimeSpan time, DateTimeKind kind)
		{
			return new((TimeSpan.TicksPerDay * TotalDays) + time.Ticks, kind);
		}
		/// <summary>
		/// Creates a new <see cref="UtcDateTime"/> from this date and the provided parts.
		/// </summary>
		/// <param name="hour">The hours.</param>
		/// <param name="minute">The minutes.</param>
		/// <param name="second">The seconds.</param>
		/// <param name="millis">The milliseconds.</param>
		/// <param name="kind">The kind.</param>
		public DateTime AsDateTime(int hour, int minute, int second, int millis, DateTimeKind kind)
		{
			return new((TimeSpan.TicksPerDay * TotalDays) + (TimeSpan.TicksPerHour * hour) + (TimeSpan.TicksPerMinute * minute) + (TimeSpan.TicksPerSecond * second) + (millis * TimeSpan.TicksPerMillisecond), kind);
		}
		/// <summary>
		/// Returns true of <paramref name="obj"/> is a <see cref="Date"/> and they refer to the same point in time.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>True if equal, false if not.</returns>
		public override bool Equals([AllowNull] object obj)
		{
			return obj is Date time && Equals(time);
		}
		/// <summary>
		/// Calls <see cref="GetHashCode"/> on <see cref="TotalDays"/>.
		/// </summary>
		/// <returns>A hashcode.</returns>
		public override int GetHashCode()
		{
			return TotalDays.GetHashCode();
		}
		/// <summary>
		/// Returns true if this instance and <paramref name="other"/> refer to the same point in time.
		/// </summary>
		public bool Equals(Date other)
		{
			return TotalDays == other.TotalDays;
		}
		/// <summary>
		/// If this instance is later than <paramref name="other"/>, returns 1.
		/// If this instance is earlier than <paramref name="other"/>, returns -1.
		/// If they are the same point in time, returns 0.
		/// </summary>
		public int CompareTo(Date other)
		{
			return TotalDays > other.TotalDays ? 1 : TotalDays < other.TotalDays ? -1 : 0;
		}
		/// <summary>
		/// Returns true if <paramref name="left"/> and <paramref name="right"/> refer to the same point in time, false otherwise.
		/// </summary>
		public static bool operator ==(Date left, Date right) => left.TotalDays == right.TotalDays;
		/// <summary>
		/// Returns true if <paramref name="left"/> and <paramref name="right"/> refer to different point in time, false otherwise.
		/// </summary>
		public static bool operator !=(Date left, Date right) => left.TotalDays != right.TotalDays;
		/// <summary>
		/// Returns true if <paramref name="left"/> is earlier than <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator <(Date left, Date right) => left.CompareTo(right) < 0;
		/// <summary>
		/// Returns true if <paramref name="left"/> is earlier than or the same point in time as <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator <=(Date left, Date right) => left.CompareTo(right) <= 0;
		/// <summary>
		/// Returns true if <paramref name="left"/> is later than <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator >(Date left, Date right) => left.CompareTo(right) > 0;
		/// <summary>
		/// Returns true if <paramref name="left"/> is later than or the same point in time as <paramref name="right"/>, false otherwise.
		/// </summary>
		public static bool operator >=(Date left, Date right) => left.CompareTo(right) >= 0;
	}
}
