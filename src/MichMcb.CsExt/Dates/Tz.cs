namespace MichMcb.CsExt.Dates
{
	using System;

	/// <summary>
	/// Represents a TimeZone. Similar to <see cref="TimeSpan"/>, but this only allows a range of +14:00 to -12:00.
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
		public const long MinTicks = -12 * TimeSpan.TicksPerHour;
		/// <summary>
		/// +14:00
		/// </summary>
		public static readonly Tz MaxValue = new(MaxTicks);
		/// <summary>
		/// -12:00
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
		/// Attempts to create a <see cref="Tz"/> from <paramref name="timespan"/>.
		/// If <paramref name="clamp"/> is true, then this will always succeed, and the value will never be larger than <see cref="MaxValue"/> or smaller than <see cref="MinValue"/>.
		/// </summary>
		/// <param name="timespan">The <see cref="TimeSpan"/>.</param>
		/// <param name="clamp">If false, returns <see cref="Nil"/> on failure. Otherwise, this method never fails.</param>
		/// <returns>A <see cref="Tz"/>, or nothing on failure.</returns>
		public static Maybe<Tz, Nil> TryFromTimeSpan(TimeSpan timespan, bool clamp = false)
		{
			return clamp
				? timespan.Ticks > MaxTicks
					? MaxValue
					: timespan.Ticks < MinTicks
						? MinValue
						: new Tz(timespan.Ticks)
				: timespan.Ticks <= MaxTicks && timespan.Ticks >= MinTicks
					? new Tz(timespan.Ticks)
					: Nil.Inst;
		}
		/// <summary>
		/// Attempts to create a new instance with the hour and minute provided.
		/// For negative timezones, <paramref name="hour"/> must be negative. Negative values for <paramref name="minute"/> are converted to positive values.
		/// </summary>
		/// <param name="hour">The hour. Negative for negative timezones.</param>
		/// <param name="minute">The minute. Doesn't matter if it's negative or positive.</param>
		/// <returns>A <see cref="Tz"/> if <paramref name="hour"/> and <paramref name="minute"/> are within the acceptable range, or an error message otherwise.</returns>
		public static Maybe<Tz, string> TryCreate(int hour, int minute)
		{
			if (minute > 59 || minute < -59)
			{
				return "Timezone minute was out of range. It must be between 0 to 59, inclusive. Its value is: " + minute;
			}
			if (hour < -12 || hour > 14)
			{
				return "Timezone hour was out of range. It must be between -12 to 14, inclusive. Its value is: " + hour;
			}
			long th = TimeSpan.TicksPerHour * hour;
			long tm = minute < 0 ? TimeSpan.TicksPerMinute * -minute : TimeSpan.TicksPerMinute * minute;
			long t = th < 0
				? th - tm
				: th + tm;
			return t < MinTicks || t > MaxTicks
				? string.Concat("Timezone was out of range. Hours: ", hour, " Minutes: ", minute)
				: new Tz(t);
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
#if NET5_0_OR_GREATER
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
#if NET5_0_OR_GREATER
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
