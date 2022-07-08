namespace MichMcb.CsExt.Dates
{
	using System;

	/// <summary>
	/// A lexer/parser which picks out the ranges in an RFC3339 string. RFC3339 is a stricter form of ISO-8601; specifically, it is identical to the
	/// ISO-8601 extended format, UTC, with or without milliseconds.
	/// </summary>
	public sealed class Rfc3339
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public Rfc3339(int year, int month, int day, int hour, int minute, int second, int millis, int timezoneMinutesOffset)
		{
			Year = year;
			Month = month;
			Day = day;
			Hour = hour;
			Minute = minute;
			Second = second;
			Millis = millis;
			TimezoneMinutesOffset = timezoneMinutesOffset;
		}
		/// <summary>
		/// The year component.
		/// </summary>
		public int Year { get; }
		/// <summary>
		/// The month component.
		/// </summary>
		public int Month { get; }
		/// <summary>
		/// The day component.
		/// </summary>
		public int Day { get; }
		/// <summary>
		/// The minute component.
		/// </summary>
		public int Hour { get; }
		/// <summary>
		/// The minute component.
		/// </summary>
		public int Minute { get; }
		/// <summary>
		/// The second component.
		/// </summary>
		public int Second { get; }
		/// <summary>
		/// The millisecond component.
		/// </summary>
		public int Millis { get; }
		/// <summary>
		/// The timezone offset, in minutes.
		/// </summary>
		public int TimezoneMinutesOffset { get; }
		/// <summary>
		/// Parses <paramref name="s"/> as an RFC3339 string, returning the components of the string.
		/// Makes sure that it's well formed.
		/// </summary>
		/// <param name="s">The string to parse.</param>
		/// <param name="allowSpaceInsteadOfT">If true, an empty space is allowed instead of T or t to separate date/time. Otherwise, only T or t is allowed.</param>
		/// <returns>A <see cref="Rfc3339"/> on success, or an error message on failure.</returns>
		public static Maybe<Rfc3339, string> Parse(ReadOnlySpan<char> s, bool allowSpaceInsteadOfT = false)
		{
			// TODO add a write method for Rfc3339, maybe. It can just call through to a Iso8601Format instance.
#pragma warning disable IDE0057 // Use range operator
			if (s.Length < 20)
			{
				return Compat.StringConcat("String is too short to be a valid RFC3339 string: ".AsSpan(), s);
			}
			if (s[4] != '-')
			{
				return Compat.StringConcat("Separator at index 4 must be dash (-): ".AsSpan(), s);
			}
			if (s[7] != '-')
			{
				return Compat.StringConcat("Separator at index 7 must be dash (-): ".AsSpan(), s);
			}
			if (s[13] != ':')
			{
				return Compat.StringConcat("Separator at index 13 must be colon (:): ".AsSpan(), s);
			}
			if (s[16] != ':')
			{
				return Compat.StringConcat("Separator at index 16 must be colon (:): ".AsSpan(), s);
			}
			if (s[10] != 'T' && s[10] != 't' && (!allowSpaceInsteadOfT || s[10] != ' '))
			{
				return allowSpaceInsteadOfT
					? Compat.StringConcat("Separator at index 10 must be T, t, or space: ".AsSpan(), s)
					: Compat.StringConcat("Separator at index 10 must be T or t: ".AsSpan(), s);
			}
			// yyyy-MM-ddTHH:mm:ssZ
			// yyyy-MM-ddTHH:mm:ss+00:00
			// yyyy-MM-ddTHH:mm:ss.nnn+00:00
			// 01234567890123456789012345678
			if (CsExt.Parse.LatinInt(s.Slice(0, 4)).Failure(out int year, out string err))
			{
				return Compat.StringConcat("Failed to parse year because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
			}
			if (CsExt.Parse.LatinInt(s.Slice(5, 2)).Failure(out int month, out err))
			{
				return Compat.StringConcat("Failed to parse month because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
			}
			if (CsExt.Parse.LatinInt(s.Slice(8, 2)).Failure(out int day, out err))
			{
				return Compat.StringConcat("Failed to parse day because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
			}
			if (CsExt.Parse.LatinInt(s.Slice(11, 2)).Failure(out int hour, out err))
			{
				return Compat.StringConcat("Failed to parse hour because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
			}
			if (CsExt.Parse.LatinInt(s.Slice(14, 2)).Failure(out int minute, out err))
			{
				return Compat.StringConcat("Failed to parse minute because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
			}
			if (CsExt.Parse.LatinInt(s.Slice(17, 2)).Failure(out int second, out err))
			{
				return Compat.StringConcat("Failed to parse second because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
			}
			int next = 19;
			char c = s[19];
			int millis;
			// Now, we parse the milliseconds, if we've hit a dot.
			if (c == '.')
			{
				// Offset by 1 so we're not on the dot anymore
				next++;
				// Keep going until we hit a non-digit
				while (next < s.Length && s[next] >= '0' && s[next] <= '9')
				{
					++next;
				}
				// If the variable has not changed at all, that means there was a dot but no milliseconds following it.
				if (20 == next)
				{
					return Compat.StringConcat("Milliseconds separator was found but no milliseconds were found. String: ".AsSpan(), s);
				}

				// Then parse it
				(int offset, int length) = OffLen.StartEnd(20, next);
				if (CsExt.Parse.LatinInt(s.Slice(offset, length > 3 ? 3 : length)).Success(out millis, out err))
				{
					switch (length)
					{
						case 1:
							millis *= 100;
							break;
						case 2:
							millis *= 10;
							break;
						default:
							break;
					}
				}
				else
				{
					// I don't think this will ever happen, because we stop once we hit a non-digit, it's practically guaranteed that we're going to parse a string entirely comprised of digits
					return Compat.StringConcat("Failed to parse millisecond because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
				}
			}
			else
			{
				millis = 0;
			}

			// At this point we can't be sure the string is actually long enough, so, we have to check the length
			if (s.Length == next)
			{
				return Compat.StringConcat("Found end of string when trying to parse timezone: ".AsSpan(), s);
			}
			char tzc = s[next];
			int timezoneMinutes;
			switch (tzc)
			{
				case 'Z':
					if (s.Length != next + 1)
					{
						return Compat.StringConcat("String has Z as a timezone, but has extra chars following the end of the string: ".AsSpan(), s);
					}
					timezoneMinutes = 0;
					break;
				case '+':
				case '-':
					// After the +/-, we need 5 more chars: HH:mm
					if (s.Length != next + 6)
					{
						return Compat.StringConcat("String has +/- as a timezone, but does not have exactly 5 chars (HH:mm) following the +/-: ".AsSpan(), s);
					}
					if (s[next + 3] != ':')
					{
						return Compat.StringConcat("Separator for timezone must be colon (:): ".AsSpan(), s);
					}
					if (CsExt.Parse.LatinInt(s.Slice(next + 1, 2)).Failure(out int tzh, out err))
					{
						return Compat.StringConcat("Failed to parse timezone hours because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
					}
					if (CsExt.Parse.LatinInt(s.Slice(next + 4, 2)).Failure(out int tzm, out err))
					{
						return Compat.StringConcat("Failed to parse timezone minutes because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
					}
					timezoneMinutes = tzc == '+'
						? (tzh * 60) + tzm
						: (-tzh * 60) - tzm;
					break;
				default:
					return Compat.StringConcat("Timezone designator is not Z, +, or -. String: ".AsSpan(), s);
			}

			return new Rfc3339(year, month, day, hour, minute, second, millis, timezoneMinutes);
#pragma warning restore IDE0057 // Use range operator
		}
	}
}