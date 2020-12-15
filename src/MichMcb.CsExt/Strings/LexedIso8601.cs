#if !NETSTANDARD2_0
namespace MichMcb.CsExt.Strings
{
	using System;
	/// <summary>
	/// A lexer which picks out the ranges in an ISO-8601 string. This can then turned into a UTC Date Time.
	/// The ranges are guaranteed to be able to be parsed as integers.
	/// </summary>
	public readonly struct LexedIso8601
	{
		public readonly Range Year;
		public readonly Range Month;
		public readonly Range Day;
		public readonly Range Hour;
		public readonly Range Minute;
		public readonly Range Second;
		public readonly Range Millis;
		/// <summary>
		/// The timezone character. It can be Z, +, -, or \0 (if no timezone char was found)
		/// </summary>
		public readonly char TimezoneChar;
		public readonly Range TimezoneHours;
		public readonly Range TimezoneMinutes;
		/// <summary>
		/// The parts found in the lexed string. Note that if a portion is too small to have separators, it's detected as not having them.
		/// </summary>
		public readonly Iso8601Parts PartsFound;
		internal LexedIso8601(Range year, Range month, Range day, Range hour, Range minute, Range second, Range millis, char timezoneChar, Range timezoneHours, Range timezoneMinutes, Iso8601Parts partsFound)
		{
			if (timezoneChar != '\0' && timezoneChar != 'Z' && timezoneChar != '+' && timezoneChar != '-')
			{
				throw new ArgumentOutOfRangeException(nameof(timezoneChar), "Timezone Character must be either Z, +, -, or \\0");
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
			Range lxYear;
			Range lxDay = default;
			Range lxMonth = default;
			Range lxHour = default;
			Range lxMinute = default;
			Range lxSecond = default;
			Range lxMillis = default;
			Range lxTzHours = default;
			Range lxTzMinutes = default;
			char tzChar = default;
			#region DatePart
			if (s.Length >= 4)
			{
				lxYear = new Range(0, 4);
				if (!IsRangeAllLatinDigits(s[lxYear]))
				{
					return "Year is not all latin digits: " + new string(s[lxYear]);
				}
			}
			else
			{
				return "Years part was not 4 characters long, it was: " + s.Length.ToString();
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
					lxDay = start..++end;
					if (!IsRangeAllLatinDigits(s[lxDay]))
					{
						return "Ordinal Day is not all latin digits: " + new string(s[lxDay]);
					}
					// Since there's only 1 separator in this case, be sneaky and set them to be the same so our check for consistent separators doesn't fail
					sep2 = sep1;
					parts = Iso8601Parts.YearDay | (sep1 ? Iso8601Parts.Separator_Date : 0);
				}
				else
				{
					lxMonth = start..end;
					if (!IsRangeAllLatinDigits(s[lxMonth]))
					{
						if (s[lxMonth][0] == 'W')
						{
							return "ISO-8601 weeks are not supported: " + new string(s[lxMonth]);
						}
						else
						{
							return "Month is not all latin digits: " + new string(s[lxMonth]);
						}
					}
					parts = Iso8601Parts.YearMonth;
				}
			}
			else
			{
				return "Months part was not 2 characters long, it was: " + (end - s.Length).ToString();
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
							lxDay = start..end;
							if (!IsRangeAllLatinDigits(s[lxDay]))
							{
								return "Day is not all latin digits: " + new string(s[lxDay]);
							}
							parts |= Iso8601Parts.Day;
						}
						else
						{
							return "Days part was not 2 characters long, it was: " + (end - s.Length).ToString();
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
				return "Date and Time separator T was expected at" + end.ToString();
			}
			sep1 = sep2 = false;
			// Hours
			start = ++end;
			end += 2;

			if (s.Length >= end)
			{
				lxHour = start..end;
				if (!IsRangeAllLatinDigits(s[lxHour]))
				{
					return "Hour is not all latin digits: " + new string(s[lxHour]);
				}
				parts |= Iso8601Parts.Hour;
			}
			else
			{
				return "Hours part was not 2 characters long, it was: " + (end - s.Length).ToString();
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
				lxMinute = start..end;
				if (!IsRangeAllLatinDigits(s[lxMinute]))
				{
					return "Minute is not all latin digits: " + new string(s[lxMinute]);
				}
				parts |= Iso8601Parts.Minute;
			}
			else
			{
				return "Minutes part was not 2 characters long, it was: " + (end - s.Length).ToString();

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
				lxSecond = start..end;
				if (!IsRangeAllLatinDigits(s[lxSecond]))
				{
					return "Second is not all latin digits: " + new string(s[lxSecond]);
				}
				parts |= Iso8601Parts.Second;
			}
			else
			{
				return "Seconds part was not 2 characters long, it was: " + (end - s.Length).ToString();

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
				lxMillis = start..end;
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
					return "Timezone designator was not valid, it was: " + new string(s[end..]);
			}
			if (parseTimezone)
			{
				// Hours
				start = end;
				end += 2;
				if (s.Length >= end)
				{
					lxTzHours = start..end;
					parts |= Iso8601Parts.Tz_Hour;
				}
				else
				{
					return "Timezone hours part was not 2 characters long, it was: " + (end - s.Length).ToString();
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
					lxTzMinutes = start..end;
					parts |= Iso8601Parts.Tz_HourMinute | (sep1 ? Iso8601Parts.Separator_Tz : 0);
				}
				else
				{
					return "Timezone minutes part was not 2 characters long, it was: " + (end - s.Length).ToString();
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
#endif