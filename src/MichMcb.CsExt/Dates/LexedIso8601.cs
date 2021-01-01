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
		/// The portion of the
		/// </summary>
		public readonly OffLen Year;
		public readonly OffLen Month;
		public readonly OffLen Day;
		public readonly OffLen Hour;
		public readonly OffLen Minute;
		public readonly OffLen Second;
		public readonly OffLen Millis;
		/// <summary>
		/// The timezone character. It can be Z, +, -, or \0 (if no timezone char was found)
		/// </summary>
		public readonly char TimezoneChar;
		public readonly OffLen TimezoneHours;
		public readonly OffLen TimezoneMinutes;
		/// <summary>
		/// The parts found in the lexed string. Note that if a portion is too small to have separators, it's detected as not having them.
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
			bool sep1 = s[4] == '-';
			bool sep2 = false;
			// If we ended up on a separator, next is 5. Else it's 4.
			int start = sep1 ? 5 : 4;
			int end = start + 2;
			Iso8601Parts parts = Iso8601Parts.Year;

			if (s.Length >= end)
			{
				// This may be ordinal days, if...
				// The next char is also a number (3 numbers in a row), and then either that's the end of the string, or it's immediately followed by a T
				if (s.Length >= end + 1 && s[end] >= '0' && s[end] <= '9' && (s.Length == end + 1 || s.Length >= end + 2 && s[end + 1] == 'T'))
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
					// Since there's only 1 separator in this case, be sneaky and set them to be the same so our check for consistent separators doesn't fail
					sep2 = sep1;
					parts = Iso8601Parts.YearDay | (sep1 ? Iso8601Parts.Separator_Date : 0);
				}
				else
				{
					lxMonth = OffLen.StartEnd(start, end);
					if (!IsRangeAllLatinDigits(lxMonth.Slice(s)))
					{
						return lxMonth.Slice(s)[0] == 'W'
#if !NETSTANDARD2_0
							? string.Concat("ISO-8601 weeks are not supported: ", lxMonth.Slice(s))
							: string.Concat("Month is not all latin digits: ", lxMonth.Slice(s));
#else
							? StringConcat("ISO-8601 weeks are not supported: ".AsSpan(), lxMonth.Slice(s))
							: StringConcat("Month is not all latin digits: ".AsSpan(), lxMonth.Slice(s));
#endif
					}
					parts = Iso8601Parts.YearMonth;
				}
			}
			else
			{
				return string.Concat("Months part was not 2 digits long, it was: ", end - s.Length);
			}
			// There's a few different possibilities here. If we parsed Year/Month, it may the end of the string, or T, or days.
			// Or if we parsed Year/Day, only the end of string or T is valid.
			if ((parts & Iso8601Parts.YearMonth) == Iso8601Parts.YearMonth)
			{
				// When we're here, we've parsed a YearMonth. Check to see if we have more to go
				if (s.Length != end)
				{
					// If the next char is T, then there's no date, 
					if (s[end] != 'T')
					{
						// If we ended up on a separator, advance by 1
						if (sep2 = s[end] == '-')
						{
							++end;
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
							return string.Concat("Days part was not 2 digits long, it was: ", end - s.Length);
						}
					}
					else
					{
						// Only 1 separator in this case (Year/Month)
						sep2 = sep1;
					}
				}
				else
				{
					// Only 1 separator in this case (Year/Month)
					sep2 = sep1;
				}
			}
			if (sep1 != sep2)
			{
				return "Inconsistent separators in the Date portion of the string";
			}
			// If we only parsed the Year and Month, that's only valid if we got a separator. i.e. yyyy-MM is valid but yyyyMM is not.
			if ((parts & Iso8601Parts.Mask_Date) == Iso8601Parts.YearMonth && !sep1)
			{
				return "Parsed year/month without a separator";
			}
			// If separators, then set the flag for having them
			if (sep1)
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
			if (s[end] != 'T')
			{
				return string.Concat("Date and Time separator T was expected at", end);
			}
			sep1 = sep2 = false;
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
				return string.Concat("Hours part was not 2 digits long, it was: ", end - s.Length);
			}
			if (s.Length == end)
			{
				goto success;
			}
			// If we ended up on a separator, advance by 1
			if (sep1 = s[end] == ':')
			{
				++end;
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
				return string.Concat("Minutes part was not 2 digits long, it was: ", end - s.Length);

			}
			if (s.Length == end)
			{
				// Hour/Minute, and only 1 separator
				parts |= sep1 ? Iso8601Parts.Separator_Time : 0;
				goto success;
			}
			// If we ended up on a separator, advance by 1
			if (sep2 = s[end] == ':')
			{
				++end;
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
				return string.Concat("Seconds part was not 2 digits long, it was: ", end - s.Length);

			}
			// Possibly inconsistent separators at this point, so check that
			if (sep1 != sep2)
			{
				return "Inconsistent separators in the Time portion of the string";
			}
			parts |= sep1 ? Iso8601Parts.Separator_Time : 0;
			if (s.Length == end)
			{
				goto success;
			}
			// If we end up on the milliseconds separator, advance by 1 and parse the milliseconds, could be any number of digits
			// Unlike the hour:minute:second separators, this one's required if you use milliseconds
			if (s[end] == '.')
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
				parts |= sep1 ? Iso8601Parts.Separator_Time : 0;
				goto success;
			}
#endregion

#region TimezonePart
			// Now, we should be on either a Z, +, or -
			bool parseTimezone = false;
			switch (s[end])
			{
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
					return string.Concat("Timezone hours part was not 2 digits long, it was: ", end - s.Length);
				}
				// This is OK; just hours for a Timezone is acceptable
				if (s.Length == end)
				{
					goto success;
				}
				// If we ended up on a separator, advance by 1
				if (sep1 = s[end] == ':')
				{
					++end;
				}
				// Minutes
				start = end;
				end += 2;
				if (s.Length >= end)
				{
					lxTzMinutes = OffLen.StartEnd(start, end);
					parts |= Iso8601Parts.Tz_HourMinute | (sep1 ? Iso8601Parts.Separator_Tz : 0);
				}
				else
				{
					return string.Concat("Timezone minutes part was not 2 digits long, it was: ", end - s.Length);
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