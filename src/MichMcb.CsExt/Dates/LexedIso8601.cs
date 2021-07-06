namespace MichMcb.CsExt.Dates
{
	using System;
#if NETSTANDARD2_0
	using static MichMcb.CsExt.Shim;
#endif
	/// <summary>
	/// A lexer which picks out the ranges in an ISO-8601 string. This can then turned into a UTC Date Time.
	/// The ranges are guaranteed to be able to be parsed as integers.
	/// </summary>
	public readonly struct LexedIso8601
	{
		/// <summary>
		/// The range of the string which represents the years.
		/// </summary>
		public readonly OffLen Year;
		/// <summary>										
		/// The range of the string which represents the months.
		/// </summary>
		public readonly OffLen Month;
		/// <summary>										
		/// The range of the string which represents the days.
		/// </summary>
		public readonly OffLen Day;
		/// <summary>										
		/// The range of the string which represents the hours.
		/// </summary>
		public readonly OffLen Hour;
		/// <summary>										
		/// The range of the string which represents the minutes.
		/// </summary>
		public readonly OffLen Minute;
		/// <summary>										
		/// The range of the string which represents the seconds.
		/// </summary>
		public readonly OffLen Second;
		/// <summary>
		/// The range of the string which represents the milliseconds.
		/// </summary>
		public readonly OffLen Millis;
		/// <summary>
		/// The timezone character, always uppercase. It can be Z, +, -, or \0 (if no timezone char was found)
		/// </summary>
		public readonly char TimezoneChar;
		/// <summary>
		/// The range of the string which represents the hours portion of the timezone.
		/// </summary>
		public readonly OffLen TimezoneHours;
		/// The range of the string which represents the minutes portion of the timezone.
		public readonly OffLen TimezoneMinutes;
		/// <summary>
		/// The parts found in the lexed string.
		/// </summary>
		public readonly Iso8601Parts PartsFound;
		internal LexedIso8601(OffLen year, OffLen month, OffLen day, OffLen hour, OffLen minute, OffLen second, OffLen millis, char timezoneChar, OffLen timezoneHours, OffLen timezoneMinutes, Iso8601Parts partsFound)
		{
			if (timezoneChar != '\0' && timezoneChar != 'Z' && timezoneChar != '+' && timezoneChar != '-')
			{
				throw new ArgumentOutOfRangeException(nameof(timezoneChar), "Timezone Character must be either Z, +, -, or \\0 (zero)");
			}
			Year = year;
			Month = month;
			Day = day;
			Hour = hour;
			Minute = minute;
			Second = second;
			Millis = millis;
			TimezoneChar = timezoneChar;
			TimezoneHours = timezoneHours;
			TimezoneMinutes = timezoneMinutes;
			PartsFound = partsFound;
		}
		/// <summary>
		/// Returns an instance which denotes what ranges of <paramref name="s"/> correspond to what parts in ISO-8601 valid strings.
		/// Makes sure that it's well formed.
		/// </summary>
		/// <param name="s">The string to parse</param>
		/// <returns>A <see cref="LexedIso8601"/> on success, or an error message on failure.</returns>
		public static Maybe<LexedIso8601, string> LexIso8601(in ReadOnlySpan<char> s)
		{
			// We always require a Year, so start it as that
			OffLen lxYear;
			OffLen lxDay = default;
			OffLen lxMonth = default;
			OffLen lxHour = default;
			OffLen lxMinute = default;
			OffLen lxSecond = default;
			OffLen lxMillis = default;
			OffLen lxTzHours = default;
			OffLen lxTzMinutes = default;
			char tzChar = default;
			#region DatePart
			if (s.Length >= 4)
			{
				lxYear = new(0, 4);
				if (!IsRangeAllLatinDigits(lxYear.Slice(s)))
				{
#if !NETSTANDARD2_0
					return string.Concat("Year is not all latin digits: ", lxYear.Slice(s));
#else
					return StringConcat("Year is not all latin digits: ".AsSpan(), lxYear.Slice(s));
#endif
				}
			}
			else
			{
				return string.Concat("Years part was not 4 digits long, it was: ", s.Length);
			}
			// If that is the end of the string, that's no good, we need more than just the year
			if (s.Length == 4)
			{
				return "String only consists of a Year part";
			}
			bool sep = s[4] == '-';
			// If we ended up on a separator, next is 5. Else it's 4.
			int start = sep ? 5 : 4;
			int end = start + 2;
			Iso8601Parts parts = Iso8601Parts.Year;

			if (s.Length < end)
			{
				return string.Concat("Months/Weeks/Ordinal Days part was not at least 2 digits long, it was: ", s.Length - start);
			}
			// If the char is W, then it's weeks
			if (s[start] == 'W' || s[start] == 'w')
			{
				// Www-D or WwwD or Www
				// TODO support weeks
				return "ISO-8601 weeks are not supported";
				// Start is W, so we need to shift start and end forwards by 1 to capture the digits
				/*if (s.Length >= end + 1)
				{
					lxMonth = OffLen.StartEnd(++start, ++end);
					if (!IsRangeAllLatinDigits(lxMonth.Slice(s)))
					{
						// Week is not all latin digits
					}
					parts |= Iso8601Parts.Week;
					// If we're not at the end of string, then check what the next char is.
					if (s.Length != end && s[end] != 'T' && s[end] != 't')
					{
						if (s[end] == '-')
						{
							if (!sep) return "Separator between Week/Day was missing, but separator between Year/Week was present";
							++end;
						}
						else
						{
							if (sep) return "Separator between Week/Day was present, but separator between Year/Week was missing";
						}
						if (s.Length >= end)
						{
							// The weekday isn't a latin digit
							lxDay = OffLen.StartEnd(end, end + 1);
							if (s[end] < '0' || s[end] > '9')
							{
#if !NETSTANDARD2_0
								return string.Concat("Week Day is not all latin digits: ", lxDay.Slice(s));
#else
								return StringConcat("Week Day is not all latin digits: ".AsSpan(), lxDay.Slice(s));
#endif
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
					return string.Concat("Weeks part was not at least 2 digits long, it was: ", s.Length - start);
				}*/
				// We need to get 2 digits. Then, the next is either a separator, or a digit, or T.
			}
			// If one past end would be the end of the string, then it might be ordinal days (or it's a separator)
			// Or if the length is longer and the char after the last char is T or t, then it should also be ordinal days
			else if ((s.Length == end + 1)
				|| (s.Length > end + 1 && (s[end + 1] == 'T' || s[end + 1] == 't')))
			{
				// To capture all 3 digits, shift end forwards by 1
				lxDay = OffLen.StartEnd(start, ++end);
				if (!IsRangeAllLatinDigits(lxDay.Slice(s)))
				{
#if !NETSTANDARD2_0
					return string.Concat("Ordinal Day is not all latin digits: ", lxDay.Slice(s));
#else
					return StringConcat("Ordinal Day is not all latin digits: ".AsSpan(), lxDay.Slice(s));
#endif
				}
				parts = Iso8601Parts.YearDay | (sep ? Iso8601Parts.Separator_Date : 0);
			}
			// Otherwise it's just the month as per normal
			else
			{
				lxMonth = OffLen.StartEnd(start, end);
				if (!IsRangeAllLatinDigits(lxMonth.Slice(s)))
				{
#if !NETSTANDARD2_0
					return string.Concat("Month is not all latin digits: ", lxMonth.Slice(s));
#else
					return StringConcat("Month is not all latin digits: ".AsSpan(), lxMonth.Slice(s));
#endif
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
						if (!sep) return "Separator between Year/Month was missing, but separator between Month/Day was present";
						++end;
					}
					else
					{
						if (sep) return "Separator between Year/Month was present, but separator between Month/Day was missing";
					}
					// Parse the days
					start = end;
					end += 2;
					if (s.Length >= end)
					{
						lxDay = OffLen.StartEnd(start, end);
						if (!IsRangeAllLatinDigits(lxDay.Slice(s)))
						{
#if !NETSTANDARD2_0
							return string.Concat("Day is not all latin digits: ", lxDay.Slice(s));
#else
							return StringConcat("Day is not all latin digits: ".AsSpan(), lxDay.Slice(s));
#endif
						}
						parts |= Iso8601Parts.Day;
					}
					else
					{
						return string.Concat("Days part was not at least 2 digits long, it was: ", s.Length - start);
					}
				}
			}
			// If we ONLY parsed the Year and Month, that's only valid if we got a separator. i.e. yyyy-MM is valid but yyyyMM is not.
			if ((parts & Iso8601Parts.Mask_Date) == Iso8601Parts.YearMonth && !sep)
			{
				return "Parsed only a year and month without a separator, which is disallowed because it can be confused with yyMMdd. Only yyyy-MM is valid, not yyyyMM";
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
				return string.Concat("Date and Time separator T was expected at index ", end);
			}
			sep = false;
			// Hours
			start = ++end;
			end += 2;

			if (s.Length >= end)
			{
				lxHour = OffLen.StartEnd(start, end);
				if (!IsRangeAllLatinDigits(lxHour.Slice(s)))
				{
#if !NETSTANDARD2_0
					return string.Concat("Hour is not all latin digits: ", lxHour.Slice(s));
#else
					return StringConcat("Hour is not all latin digits: ".AsSpan(), lxHour.Slice(s));
#endif
				}
				parts |= Iso8601Parts.Hour;
			}
			else
			{
				return string.Concat("Hours part was not 2 digits long, it was: ", s.Length - start);
			}
			if (s.Length == end)
			{
				goto success;
			}
			// If we ended up on a separator, advance by 1
			//if (sep = s[end] == ':')
			//{
			//	++end;
			//}
			//sep = s[end];
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
				lxMinute = OffLen.StartEnd(start, end);
				if (!IsRangeAllLatinDigits(lxMinute.Slice(s)))
				{
#if !NETSTANDARD2_0
					return string.Concat("Minute is not all latin digits: ", lxMinute.Slice(s));
#else
					return StringConcat("Minute is not all latin digits: ".AsSpan(), lxMinute.Slice(s));
#endif
				}
				parts |= Iso8601Parts.Minute;
			}
			else
			{
				return string.Concat("Minutes part was not 2 digits long, it was: ", s.Length - start);

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
				lxSecond = OffLen.StartEnd(start, end);
				if (!IsRangeAllLatinDigits(lxSecond.Slice(s)))
				{
#if !NETSTANDARD2_0
					return string.Concat("Second is not all latin digits: ", lxSecond.Slice(s));
#else
					return StringConcat("Second is not all latin digits: ".AsSpan(), lxSecond.Slice(s));
#endif
				}
				parts |= Iso8601Parts.Second;
			}
			else
			{
				return string.Concat("Seconds part was not 2 digits long, it was: ", s.Length - start);

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
					return "Milliseconds separator was found but no milliseconds were found";
				}
				// We know this range is all digits, so that's all good
				lxMillis = OffLen.StartEnd(start, end);
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
			bool parseTimezone = false;
			switch (s[end])
			{
				case 'z':
				case 'Z':
					tzChar = 'Z';
					parts |= Iso8601Parts.Tz_Utc;
					break;
				case '-':
				case '+':
					// offset by 1 so we're on the digit
					tzChar = s[end++];
					parseTimezone = true;
					break;
				default:
#if !NETSTANDARD2_0
					return string.Concat("Timezone designator was not valid, it was: ", s[end..]);
#else
					return StringConcat("Timezone designator was not valid, it was: ".AsSpan(), s.Slice(end));
#endif
			}
			if (parseTimezone)
			{
				// Hours
				start = end;
				end += 2;
				if (s.Length >= end)
				{
					lxTzHours = OffLen.StartEnd(start, end);
					parts |= Iso8601Parts.Tz_Hour;
				}
				else
				{
					return string.Concat("Timezone hours part was not 2 digits long, it was: ", s.Length - start);
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
					lxTzMinutes = OffLen.StartEnd(start, end);
					parts |= Iso8601Parts.Tz_HourMinute | (sep ? Iso8601Parts.Separator_Tz : 0);
				}
				else
				{
					return string.Concat("Timezone minutes part was not 2 digits long, it was: ", s.Length - start);
				}
			}
		#endregion

		success:
			return new LexedIso8601(lxYear, lxMonth, lxDay, lxHour, lxMinute, lxSecond, lxMillis, tzChar, lxTzHours, lxTzMinutes, parts);
		}
		/// <summary>
		/// True if all chars of <paramref name="s"/> are 0-9
		/// </summary>
		public static bool IsRangeAllLatinDigits(in ReadOnlySpan<char> s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] < '0' || s[i] > '9')
				{
					return false;
				}
			}
			return true;
		}
	}
}