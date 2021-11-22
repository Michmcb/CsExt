namespace MichMcb.CsExt.Dates
{
	using System;
	/// <summary>
	/// A lexer which picks out the ranges in an ISO-8601 string. This can then turned into a UTC Date Time.
	/// The ranges are guaranteed to be able to be parsed as integers.
	/// </summary>
	public sealed class Iso8601
	{
		internal Iso8601(int year, int monthOrWeek, int day, int hour, int minute, int second, int millis, int? timezoneOffset, Iso8601Parts partsFound)
		{
			Year = year;
			MonthOrWeek = monthOrWeek;
			Day = day;
			Hour = hour;
			Minute = minute;
			Second = second;
			Millis = millis;
			TimezoneMinutesOffset = timezoneOffset;
			PartsFound = partsFound;
			// 214 748 364 7
		}
		/// <summary>
		/// The year component.
		/// </summary>
		public int Year { get; }
		/// <summary>
		/// Either the month or the week component.
		/// If <see cref="PartsFound"/> has the flag <see cref="Iso8601Parts.Month"/> set, this is the month.
		/// If <see cref="PartsFound"/> has the flag <see cref="Iso8601Parts.Week"/> set, this is the week.
		/// If neither flags are set, this was not found. This can be the case if the ISO-8601 string only had Year and Day.
		/// </summary>
		public int MonthOrWeek { get; }
		/// <summary>
		/// The day component.
		/// If <see cref="PartsFound"/> has flags <see cref="Iso8601Parts.Month"/> and <see cref="Iso8601Parts.Day"/> set, this is the day of the month.
		/// If <see cref="PartsFound"/> has flags <see cref="Iso8601Parts.Week"/> and <see cref="Iso8601Parts.Day"/> set, this is the day of the week.
		/// If <see cref="PartsFound"/> only has the flag <see cref="Iso8601Parts.Day"/> set, then this is the day of the year.
		/// </summary>
		public int Day { get; }
		/// <summary>
		/// The hour component.
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
		/// Can be positive or negative, or zero for UTC, or null if there was no timezone designator on the input string (which means use the local timezone).
		/// </summary>
		public int? TimezoneMinutesOffset { get; }
		/// <summary>
		/// Each individual part that was found when parsing an ISO-8601 string.
		/// </summary>
		public Iso8601Parts PartsFound { get; }
		/// <summary>
		/// Parses <paramref name="s"/> as an ISO-8601 string, returning the components of the string.
		/// Makes sure that it's well formed.
		/// </summary>
		/// <param name="s">The string to parse</param>
		/// <returns>A <see cref="Iso8601"/> on success, or an error message on failure.</returns>
		public static Maybe<Iso8601, string> Parse(in ReadOnlySpan<char> s)
		{
#pragma warning disable IDE0057 // Use range operator
			if (s.Length == 0)
			{
				return "String was empty";
			}	
			// We always require a Year so it's never set to default
			int year;
			int monthOrWeek;
			int day;
			int hour = 0;
			int minute = 0;
			int second = 0;
			int millis = 0;
			int? tz = null;
			#region DatePart
			int start;
			int end;
			Iso8601Parts parts;
			if (s[0] == 'T')
			{
				// First char is a T, so there's no date to parse, just time
				// That means the next thing will be an hour
				parts = Iso8601Parts.None;
				start = 1;
				end = 3;
				year = monthOrWeek = day = 0;
				goto timeParse;
			}
			if (s.Length >= 4)
			{
				if (!CsExt.Parse.LatinInt(s.Slice(0, 4)).Success(out year, out string err))
				{
					return Compat.StringConcat("Failed to parse year because ".AsSpan(), err.AsSpan(), " String: ".AsSpan(), s);
				}
			}
			else
			{
				return Compat.StringConcat("Failed to parse year because it was not 4 digits long. String: ".AsSpan(), s);
			}
			// If that is the end of the string, that's no good, we need more than just the year
			if (s.Length == 4)
			{
				return Compat.StringConcat("String only consists of a year: ".AsSpan(), s);
			}
			bool sep = s[4] == '-';
			// If we ended up on a separator, next is 5. Else it's 4.
			start = sep ? 5 : 4;
			end = start + 2;
			parts = Iso8601Parts.Year;

			if (s.Length < end)
			{
				return Compat.StringConcat("Failed to parse Months/Weeks/Ordinal Days because it was not at least 2 digits long. String: ".AsSpan(), s);
			}
			// If the char is W, then it's weeks
			if (s[start] == 'W' || s[start] == 'w')
			{
				// Www-D or WwwD or Www
				// Start is W, so we need to shift start and end forwards by 1 to capture the digits
				if (s.Length >= end + 1)
				{
					// Omitting the D part is the same as it being the 1st day of the week.
					day = 1;
					++start;
					if (!CsExt.Parse.LatinInt(s.Slice(start, ++end - start)).Success(out monthOrWeek, out string err))
					{
						return Compat.StringConcat("Failed to parse week because ".AsSpan(), err.AsSpan(), " String: ".AsSpan(), s);
					}
					parts |= Iso8601Parts.Week;
					// If we're not at the end of string, then check what the next char is.
					if (s.Length != end && s[end] != 'T' && s[end] != 't')
					{
						if (s[end] == '-')
						{
							if (!sep) return Compat.StringConcat("Separator between Week/Day was missing, but separator between Year/Week was present. String: ".AsSpan(), s);
							++end;
						}
						else
						{
							if (sep) return Compat.StringConcat("Separator between Week/Day was present, but separator between Year/Week was missing. String: ".AsSpan(), s);
						}
						if (s.Length >= end)
						{
							if (!CsExt.Parse.LatinInt(s.Slice(end, 1)).Success(out day, out err))
							{
								return Compat.StringConcat("Failed to parse weekday because ".AsSpan(), err.AsSpan(), " String: ".AsSpan(), s);
							}
							// Shift end forwards by 1. The next char should now be a T or a t (or end of string)
							++end;
							parts |= Iso8601Parts.Day;
						}
					}
				}
				else
				{
					// Not enough digits!
					return Compat.StringConcat("Weeks part was not at least 2 digits long. String: ".AsSpan(), s);
				}
				// We need to get 2 digits. Then, the next is either a separator, or a digit, or T.
			}
			// If one past end would be the end of the string, then it might be ordinal days (or it's a separator)
			// Or if the length is longer and the char after the last char is T or t, then it should also be ordinal days
			else if ((s.Length == end + 1)
				|| (s.Length > end + 1 && (s[end + 1] == 'T' || s[end + 1] == 't')))
			{
				// To capture all 3 digits, shift end forwards by 1
				if (!CsExt.Parse.LatinInt(s.Slice(start, ++end - start)).Success(out day, out string err))
				{
					return Compat.StringConcat("Failed to parse ordinal days because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
				}
				// Ordinal days so set MonthOrWeek to 0
				monthOrWeek = 0;
				parts = Iso8601Parts.YearDay | (sep ? Iso8601Parts.Separator_Date : 0);
			}
			// Otherwise it's just the month as per normal
			else
			{
				if (!CsExt.Parse.LatinInt(s.Slice(start, end - start)).Success(out monthOrWeek, out string err))
				{
					return Compat.StringConcat("Failed to parse month because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
				}
				parts = Iso8601Parts.YearMonth;
				// When we're here, we've parsed a Year and a Month.
				// So try to parse a day now
				// Check to see if we're not at the end of the date part (indicated by end of string or T)
				// We've finished the date part if this isn't true
				if (s.Length != end && s[end] != 'T' && s[end] != 't')
				{
					// If we ended up on a separator, advance by 1
					// Also make sure that the presence/absence of a separator is the same as year/month
					if (s[end] == '-')
					{
						if (!sep) return Compat.StringConcat("Separator between Year/Month was missing, but separator between Month/Day was present. String: ".AsSpan(), s);
						++end;
					}
					else
					{
						if (sep) return Compat.StringConcat("Separator between Year/Month was present, but separator between Month/Day was missing. String: ".AsSpan(), s);
					}
					// Parse the days
					start = end;
					end += 2;
					if (s.Length >= end)
					{
						if (!CsExt.Parse.LatinInt(s.Slice(start, end - start)).Success(out day, out err))
						{
							return Compat.StringConcat("Failed to parse day because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
						}
						parts |= Iso8601Parts.Day;
					}
					else
					{
						return Compat.StringConcat("Days part was not at least 2 digits long. String: ".AsSpan(), s);
					}
				}
				else
				{
					// If there wasn't a day, then assume it was the 1st day of the month
					day = 1;
				}
			}
			// If we ONLY parsed the Year and Month, that's only valid if we got a separator. i.e. yyyy-MM is valid but yyyyMM is not.
			if ((parts & Iso8601Parts.Mask_Date) == Iso8601Parts.YearMonth && !sep)
			{
				return Compat.StringConcat("Parsed only a year and month without a separator, which ISO-8601 disallows because it can be confused with yyMMdd. Only yyyy-MM is valid, not yyyyMM. String: ".AsSpan(), s);
			}
			// If separators, then set the flag for having them
			if (sep)
			{
				parts |= Iso8601Parts.Separator_Date;
			}
			// If that is the end of the string, that's fine
			if (s.Length == end)
			{
				goto success;
			}
			#endregion

			#region TimePart
			// TIME
			// If we're not at the end of the string yet, we are parsing the time, so we need a T
			// And yes we're not allowed the timezone, unless we also have a time!
			if (s[end] != 'T' && s[end] != 't')
			{
				return Compat.StringConcat("Date and Time separator T was expected at index ".AsSpan(), end.ToString().AsSpan(), ". String: ".AsSpan(), s);
			}
			// Hours
			start = ++end;
			end += 2;
		timeParse:
			sep = false;

			if (s.Length >= end)
			{
				if (!CsExt.Parse.LatinInt(s.Slice(start, end - start)).Success(out hour, out string err))
				{
					return Compat.StringConcat("Failed to parse hour because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
				}
				parts |= Iso8601Parts.Hour;
			}
			else
			{
				return Compat.StringConcat("Hours part was not 2 digits long. String: ".AsSpan(), s);
			}
			if (s.Length == end)
			{
				goto success;
			}

			switch (s[end])
			{
				case ':':
					sep = true;
					++end;
					break;
				case 'Z':
				case '+':
				case '-':
					goto tzParse;
				default:
					break;
			}
			// Minutes
			start = end;
			end += 2;
			if (s.Length >= end)
			{
				if (!CsExt.Parse.LatinInt(s.Slice(start, end - start)).Success(out minute, out string err))
				{
					return Compat.StringConcat("Failed to parse minute because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
				}
				parts |= Iso8601Parts.Minute;
			}
			else
			{
				return Compat.StringConcat("Minutes part was not 2 digits long. String: ".AsSpan(), s);

			}
			if (s.Length == end)
			{
				// Hour/Minute, and only 1 separator
				parts |= sep ? Iso8601Parts.Separator_Time : 0;
				goto success;
			}
			// If we ended up on a separator, advance by 1
			switch (s[end])
			{
				case ':':
					if (!sep) return "Separator between Hour/Minute was missing, but separator between Minute/Second was present";
					++end;
					break;
				case 'Z':
				case '+':
				case '-':
					goto tzParse;
				default:
					if (sep) return "Separator between Hour/Minute was present, but separator between Minute/Second was missing";
					break;
			}
			// Seconds
			start = end;
			end += 2;
			if (s.Length >= end)
			{
				if (!CsExt.Parse.LatinInt(s.Slice(start, end - start)).Success(out second, out string err))
				{
					return Compat.StringConcat("Failed to parse second because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
				}
				parts |= Iso8601Parts.Second;
			}
			else
			{
				return Compat.StringConcat("Seconds part was not 2 digits long. String: ".AsSpan(), s);

			}
			parts |= sep ? Iso8601Parts.Separator_Time : 0;
			if (s.Length == end)
			{
				goto success;
			}
			// If we end up on the milliseconds separator, advance by 1 and parse the milliseconds, could be any number of digits
			// Unlike the hour:minute:second separators, this one's required if you use milliseconds
			if (s[end] == '.' || s[end] == ',')
			{
				start = ++end;
				while (end < s.Length && s[end] >= '0' && s[end] <= '9')
				{
					++end;
				}
				// If the string was 0 chars long, that's not good
				if (start == end)
				{
					return Compat.StringConcat("Milliseconds separator was found but no milliseconds were found. String: ".AsSpan(), s);
				}
				// We know this range is all digits, so that's all good

				(int offset, int length) = OffLen.StartEnd(start, end);
				if (CsExt.Parse.LatinInt(s.Slice(offset, length > 3 ? 3 : length)).Success(out millis, out string err))
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
					return Compat.StringConcat("Failed to parse millisecond because ".AsSpan(), err.AsSpan(), ". String: ".AsSpan(), s);
				}
				parts |= Iso8601Parts.Millis;
			}
			// No timezone, exit
			if (s.Length == end)
			{
				parts |= sep ? Iso8601Parts.Separator_Time : 0;
				goto success;
			}
		#endregion

		#region TimezonePart
		tzParse:
			// Now, we should be on either a Z/z, +, or -
			switch (s[end])
			{
				case 'z':
				case 'Z':
					parts |= Iso8601Parts.Tz_Utc;
					tz = 0;
					break;
				case '-':
				case '+':
					// If it's negative we need to negate the timezone offset
					bool negate = s[end] == '-';
					// offset by 1 so we're on the digit
					{
						// Hours
						start = ++end;
						end += 2;
						if (s.Length >= end)
						{
							if (CsExt.Parse.LatinInt(s.Slice(start, end - start)).Success(out int tzh, out string err))
							{
								tz = negate ? -tzh * 60 : tzh * 60;
							}
							else
							{
								return err;
							}
							parts |= Iso8601Parts.Tz_Hour;
						}
						else
						{
							return Compat.StringConcat("Timezone hours part was not 2 digits long. String: ".AsSpan(), s);
						}
						// This is OK; just hours for a Timezone is acceptable
						if (s.Length == end)
						{
							goto success;
						}
						// If we ended up on a separator, advance by 1
						if (sep = s[end] == ':')
						{
							++end;
						}
						// Minutes
						start = end;
						end += 2;
						if (s.Length >= end)
						{
							if (CsExt.Parse.LatinInt(s.Slice(start, end - start)).Success(out int tzm, out string err))
							{
								tz += negate ? -tzm : tzm;
							}
							else
							{
								return err;
							}
							parts |= Iso8601Parts.Tz_HourMinute | (sep ? Iso8601Parts.Separator_Tz : 0);
						}
						else
						{
							return Compat.StringConcat("Timezone minutes part was not 2 digits long. String: ".AsSpan(), s);
						}
					}
					break;
				default:
					return Compat.StringConcat("Timezone designator was not valid. String: ".AsSpan(), s);
			}
		#endregion

		success:
			return new Iso8601
				(
					year: year,
					monthOrWeek: monthOrWeek,
					day: day,
					hour: hour,
					minute: minute,
					second: second,
					millis: millis,
					timezoneOffset: tz,
					partsFound: parts
				);
#pragma warning restore IDE0057 // Use range operator
		}
	}
}