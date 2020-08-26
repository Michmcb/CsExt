#if !NETSTANDARD2_0
using System;
using System.Globalization;

namespace MichMcb.CsExt.Strings
{
	public static class Parse
	{
		public static Opt<int> Int(in ReadOnlySpan<char> str)
		{
			return int.TryParse(str, out int val) ? new Opt<int>(val, true) : new Opt<int>(val, false);
		}
		public static Opt<int> Int(in ReadOnlySpan<char> str, NumberStyles style, IFormatProvider provider)
		{
			return int.TryParse(str, style, provider, out int val) ? new Opt<int>(val, true) : new Opt<int>(val, false);
		}
		public static Opt<DateTime> DateTime(in ReadOnlySpan<char> str)
		{
			return System.DateTime.TryParse(str, out DateTime val) ? new Opt<DateTime>(val, true) : new Opt<DateTime>(val, false);
		}
		public static Opt<DateTime> DateTime(in ReadOnlySpan<char> str, IFormatProvider provider, DateTimeStyles styles)
		{
			return System.DateTime.TryParse(str, provider, styles, out DateTime val) ? new Opt<DateTime>(val, true) : new Opt<DateTime>(val, false);
		}
		/// <summary>
		/// Parses an ISO-8601 string as a UtcDateTime. Only accurate to the millisecond; further accuracy is truncated.
		/// All parsed ISO-8601 strings are interpreted as UTC.
		/// Any leading or trailing whitespace is ignored.
		/// Valid ISO-8601 strings are, for example...
		/// <code>Date Only: 2010-07-15 or 20100715</code>
		/// <code>Year and Month: 2010-07 but NOT 201007 (dates may be ambiguous with yyMMdd)</code>
		/// <code>With Timezone: 2010-07-15T07:21:39.123+10:00 or 20100715T072139.123+10:00</code>
		/// <code>UTC: 2010-07-15T07:11:39Z or 20100715T071139.123Z</code>
		/// <code>Ordinal Date: 2010-197 or 2010197</code>
		/// </summary>
		/// <param name="assumeMissingTimeZoneAs">If the string is missing a timezone designator, then it uses this. If null, local time is used if a timezone designator is missing.</param>
		/// <returns>A UtcDateTime if parsing was successful, or an error message otherwise.</returns>
		public static Maybe<UtcDateTime, string> Iso8601StringAsUtcDateTime(in ReadOnlySpan<char> str, TimeSpan? assumeMissingTimeZoneAs = null)
		{
			ReadOnlySpan<char> ts = str.Trim();
			Maybe<LexedIso8601, string> rLex = LexIso8601(ts);
			if (!rLex.Success(out LexedIso8601 luthor, out string errMsg))
			{
				return errMsg;
			}
			int year = int.Parse(str[luthor.Year]);
			int month = 0, day = 1, hour = 0, minute = 0, second = 0, millis = 0;
			if ((luthor.PartsFound & Iso8601Parts.Month) == Iso8601Parts.Month)
			{
				month = int.Parse(str[luthor.Month]);
			}
			if ((luthor.PartsFound & Iso8601Parts.Day) == Iso8601Parts.Day)
			{
				day = int.Parse(str[luthor.Day]);
			}
			if ((luthor.PartsFound & Iso8601Parts.Hour) == Iso8601Parts.Hour)
			{
				hour = int.Parse(str[luthor.Hour]);
			}
			if ((luthor.PartsFound & Iso8601Parts.Minute) == Iso8601Parts.Minute)
			{
				minute = int.Parse(str[luthor.Minute]);
			}
			if ((luthor.PartsFound & Iso8601Parts.Second) == Iso8601Parts.Second)
			{
				second = int.Parse(str[luthor.Second]);
			}
			if ((luthor.PartsFound & Iso8601Parts.Millis) == Iso8601Parts.Millis)
			{
				// Only parse the first 3 characters of milliseconds, since that's the highest degree of accuracy we allow for
				(int offset, int length) = luthor.Millis.GetOffsetAndLength(str.Length);
				millis = int.Parse(str.Slice(offset, length > 3 ? 3 : length));
			}
			int tzHours = 0;
			int tzMinutes = 0;
			switch (luthor.TimezoneChar)
			{
				case 'Z':
					break;
				case '-':
				case '+':
					{
						if ((luthor.PartsFound & Iso8601Parts.Tz_Hour) == Iso8601Parts.Tz_Hour)
						{
							tzHours = int.Parse(str[luthor.TimezoneHours]);
						}
						if ((luthor.PartsFound & Iso8601Parts.Tz_Minute) == Iso8601Parts.Tz_Minute)
						{
							tzMinutes = int.Parse(str[luthor.TimezoneMinutes]);
						}
						// The offsets mean that this time has already had the offset added. Therefore if it's +10:00, we need to subtract 10 so the result is in UTC, or add 10 if it's -10:00
						if (luthor.TimezoneChar == '+')
						{
							tzHours = -tzHours;
							tzMinutes = -tzMinutes;
						}
					}
					break;
				case '\0':
					// No timezone means as should assume local time, or whatever they tell us
					TimeSpan tzSpan = assumeMissingTimeZoneAs ?? TimeZoneInfo.Local.BaseUtcOffset;
					tzHours = -tzSpan.Hours;
					tzMinutes = -tzSpan.Minutes;
					break;
			}
			ArgumentOutOfRangeException? ex;
			if ((luthor.PartsFound & Iso8601Parts.Mask_Date) == Iso8601Parts.YearDay)
			{
				ex = Dates.MillisFromParts_OrdinalDays(year, day, hour, minute, second, millis, tzHours, tzMinutes, out long ms);
				if (ex == null)
				{
					return new UtcDateTime(ms);
				}
			}
			else
			{
				ex = Dates.MillisFromParts(year, month, day, hour, minute, second, millis, tzHours, tzMinutes, out long ms);
				if (ex == null)
				{
					return new UtcDateTime(ms);
				}
			}
			return ex.Message;
		}
		public static Maybe<DateTime, string> Iso8601StringAsDateTime(in ReadOnlySpan<char> str, TimeSpan? assumeMissingTimeZoneAs = null)
		{
			// TODO don't be so hacky; perhaps move the parsing into the Lexed struct, so we can just call its methods, or change the parsing of the lexed ranges to produce integers instead keeping in mind we may have microseconds and 100-nanosecond ticks of accuracy.
			Maybe<UtcDateTime, string> r = Iso8601StringAsUtcDateTime(str, assumeMissingTimeZoneAs);
			if (r.Success(out UtcDateTime val, out string? err))
			{
				return (DateTime)val;
			}
			else
			{
				return err;
			}
		}
		/// <summary>
		/// Returns an instance which denotes what ranges of <paramref name="s"/> correspond to what parts in ISO-8601 valid strings.
		/// Makes sure that it's well formed.
		/// </summary>
		/// <param name="s">The string to parse</param>
		/// <returns>An Iso8601Lexer on success, or an error message on failure.</returns>
		internal static Maybe<LexedIso8601, string> LexIso8601(in ReadOnlySpan<char> s)
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
				lxYear = 0..4;
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