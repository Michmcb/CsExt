namespace MichMcb.CsExt.Dates
{
	using System;

	/// <summary>
	/// Represents a TimeZone. Similar to <see cref="TimeSpan"/>, but this only allows a range of +14:00 to -14:00.
	/// </summary>
	public readonly struct Tz : IEquatable<Tz>
	{
		/// <summary>
		/// The maximum number of ticks this can hold, corresponding to <see cref="MaxValue"/>.
		/// </summary>
		public const long MaxTicks = 14 * TimeSpan.TicksPerHour;
		/// <summary>
		/// The minimum number of ticks this can hold, corresponding to <see cref="MinValue"/>.
		/// </summary>
		public const long MinTicks = -14 * TimeSpan.TicksPerHour;
		/// <summary>
		/// +14:00
		/// </summary>
		public static readonly Tz MaxValue = new(MaxTicks);
		/// <summary>
		/// -14:00
		/// </summary>
		public static readonly Tz MinValue = new(MinTicks);
		/// <summary>
		/// +00:00. The default value.
		/// </summary>
		public static readonly Tz Utc = new(0);
		/// <summary>
		/// Creates a new instance with <paramref name="ticks"/>, which must be within <see cref="MinTicks"/> and <see cref="MaxTicks"/>.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public Tz(long ticks)
		{
			if (ticks < MinTicks || ticks > MaxTicks)
			{
				throw new ArgumentOutOfRangeException(nameof(ticks), "Ticks was out of range. Its value is: " + ticks);
			}
			Ticks = ticks;
		}
		/// <summary>
		/// Equivalent to <see cref="TryCreate(int, int)"/>, except throws <see cref="ArgumentOutOfRangeException"/> on failure.
		/// </summary>
		/// <param name="hours">The hours.</param>
		/// <param name="minutes">The minutes.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public Tz(int hours, int minutes)
		{
			Ticks = TryCreate(hours, minutes).Success(out Tz tz, out ErrState<string> err)
				? tz.Ticks
				: throw new ArgumentOutOfRangeException(err.State, err.Message);
		}
		/// <summary>
		/// The timezone offset, in ticks.
		/// </summary>
		public long Ticks { get; }
		/// <summary>
		/// The hours.
		/// </summary>
		public int Hours => (int)(Ticks / TimeSpan.TicksPerHour);
		/// <summary>
		/// The minutes.
		/// </summary>
		public int Minutes => (int)(Ticks / TimeSpan.TicksPerMinute) % 60;
		/// <summary>
		/// Gets the hour and minutes parts, which are always positive.
		/// The parameter <paramref name="positive"/> indicates whether this is a positive or a negative offset.
		/// </summary>
		/// <param name="hours">The hours.</param>
		/// <param name="minutes">The minutes.</param>
		/// <param name="positive">If <see cref="Ticks"/> was positive, true. Otherwise, false.</param>
		public void GetAbsoluteParts(out int hours, out int minutes, out bool positive)
		{
			long t;
			if (Ticks < 0)
			{
				positive = false;
				t = -Ticks;
			}
			else
			{
				positive = true;
				t = Ticks;
			}
			hours = (int)(t / TimeSpan.TicksPerHour);
			minutes = (int)(t / TimeSpan.TicksPerMinute) % 60;
		}
		/// <summary>
		/// Returns a <see cref="TimeSpan"/> with <see cref="Ticks"/>.
		/// </summary>
		/// <returns>A <see cref="TimeSpan"/> that represents a span of time identical to this <see cref="Tz"/>.</returns>
		public TimeSpan AsTimeSpan()
		{
			return new TimeSpan(Ticks);
		}
		/// <summary>
		/// Attempts to create a <see cref="Tz"/> from <paramref name="timespan"/>.
		/// If <paramref name="timespan"/> is too small, returns <see cref="MinTicks"/>.
		/// If <paramref name="timespan"/> is too large, returns <see cref="MaxTicks"/>.
		/// </summary>
		/// <param name="timespan">The <see cref="TimeSpan"/>.</param>
		/// <returns>A <see cref="Tz"/> if successful.</returns>
		public static Maybe<Tz, long> TryFromTimeSpan(TimeSpan timespan)
		{
			return timespan.Ticks > MaxTicks
				? MaxTicks
				: timespan.Ticks < MinTicks
					? MinTicks
					: new Tz(timespan.Ticks);
		}
		/// <summary>
		/// Creates a <see cref="Tz"/> from <paramref name="timespan"/>.
		/// If <paramref name="timespan"/> is too small or too large, the return value is clamped.
		/// </summary>
		/// <param name="timespan">The <see cref="TimeSpan"/>.</param>
		/// <returns>A <see cref="Tz"/>.</returns>
		public static Tz FromTimeSpanClamped(TimeSpan timespan)
		{
			return timespan.Ticks > MaxTicks
				? MaxValue
				: timespan.Ticks < MinTicks
					? MinValue
					: new Tz(timespan.Ticks);
		}
		/// <summary>
		/// Attempts to create a new instance with the hour and minute provided.
		/// For negative timezones, <paramref name="hours"/> must be negative. Negative values for <paramref name="minutes"/> are converted to positive values.
		/// </summary>
		/// <param name="hours">The hour. Negative for negative timezones.</param>
		/// <param name="minutes">The minute. Doesn't matter if it's negative or positive.</param>
		/// <returns>A <see cref="Tz"/> if <paramref name="hours"/> and <paramref name="minutes"/> are within the acceptable range. On failure, State is the parameter name which is out of range.</returns>
		public static Maybe<Tz, ErrState<string>> TryCreate(int hours, int minutes)
		{
			if (hours < -14 || hours > 14)
			{
				return ErrState.New(nameof(hours), string.Concat("Timezone hours out of range. Hours: ", hours, " Minutes: " + minutes));
			}
			if (minutes != 0 && (hours == 14 || hours == -14))
			{
				return ErrState.New(nameof(minutes), string.Concat("Timezone hours and minutes out of range. Hours: ", hours, " Minutes: " + minutes));
			}
			if (minutes > 59 || minutes < -59)
			{
				return ErrState.New(nameof(minutes), string.Concat("Timezone minutes out of range. Hours: ", hours, " Minutes: " + minutes));
			}
			long th = TimeSpan.TicksPerHour * hours;
			long tm = minutes < 0 ? TimeSpan.TicksPerMinute * -minutes : TimeSpan.TicksPerMinute * minutes;
			long t = th < 0
				? th - tm
				: th + tm;
			// Should never be out of range
			return new Tz(t);
		}
		/// <inheritdoc/>
		public override bool Equals(object? obj)
		{
			return obj is Tz zone && Equals(zone);
		}
		/// <inheritdoc/>
		public bool Equals(Tz other)
		{
			return Ticks == other.Ticks;
		}
		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return Ticks.GetHashCode();
		}
		/// <summary>
		/// Returns a string like "+HH:mm".
		/// </summary>
		public override string ToString()
		{
#if NET6_0_OR_GREATER
			return string.Create(6, this, (dest, obj) => obj.Write(dest, true));
#else
			return Compat.StringCreate(6, this, (dest, obj) => obj.Write(dest, true));
#endif
		}
		/// <summary>
		/// Returns a string like "+HH:mm" or "+HHmm"
		/// </summary>
		public string ToString(bool colon)
		{
#if NET6_0_OR_GREATER
			return string.Create(colon ? 6 : 5, this, (dest, obj) => obj.Write(dest, colon));
#else
			return Compat.StringCreate(colon ? 6 : 5, this, (dest, obj) => obj.Write(dest, colon));
#endif
		}
		/// <summary>
		/// Writes this <see cref="Tz"/> to <paramref name="dest"/>.
		/// </summary>
		/// <param name="dest">The <see cref="Span{T}"/> to write to.</param>
		/// <param name="colon">If true, includes a :. If false, does not.</param>
		/// <returns>The number of characters written, or the number of characters required as a negative number if <paramref name="dest"/> is too short.</returns>
		public int Write(Span<char> dest, bool colon)
		{
			if (colon)
			{
				if (dest.Length < 6)
				{
					return -6;
				}
				// +00:00
				// 012345
				GetAbsoluteParts(out int h, out int m, out bool positive);
				dest[0] = positive ? '+' : '-';
				dest[3] = ':';
				Formatting.Write2Digits((uint)h, dest, offset: 1);
				Formatting.Write2Digits((uint)m, dest, offset: 4);
				return 6;
			}
			else
			{
				if (dest.Length < 5)
				{
					return -5;
				}
				// +0000
				// 01234
				GetAbsoluteParts(out int h, out int m, out bool positive);
				dest[0] = positive ? '+' : '-';
				Formatting.Write2Digits((uint)h, dest, offset: 1);
				Formatting.Write2Digits((uint)m, dest, offset: 3);
				return 5;
			}
		}
		/// <inheritdoc/>
		public static bool operator ==(Tz left, Tz right) => left.Equals(right);
		/// <inheritdoc/>
		public static bool operator !=(Tz left, Tz right) => !(left == right);
	}
}
