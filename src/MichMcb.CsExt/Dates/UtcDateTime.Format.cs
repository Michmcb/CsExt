namespace MichMcb.CsExt.Dates
{
	using System;
	using System.Globalization;
	using System.Runtime.CompilerServices;

	public readonly partial struct UtcDateTime : IEquatable<UtcDateTime>, IComparable<UtcDateTime>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IntParse(in ReadOnlySpan<char> str, NumberStyles numberStyles)
		{
#if NETSTANDARD2_0
			return int.Parse(str.ToString(), numberStyles);
#else
			return int.Parse(str, numberStyles);
#endif
		}
		/// <summary>
		/// Parses an ISO-8601 string as a UtcDateTime. Only accurate to the millisecond; further accuracy is truncated.
		/// All parsed ISO-8601 strings are adjusted to UTC.
		/// Any leading or trailing whitespace is ignored.
		/// Valid ISO-8601 strings are, for example...
		/// <code>Date Only: 2010-07-15 or 20100715</code>
		/// <code>Year and Month: 2010-07 but NOT 201007 (dates may be ambiguous with yyMMdd)</code>
		/// <code>With Timezone: 2010-07-15T07:21:39.123+10:00 or 20100715T072139.123+10:00</code>
		/// <code>UTC: 2010-07-15T07:11:39Z or 20100715T071139.123Z</code>
		/// <code>Ordinal Date: 2010-197 or 2010197</code>
		/// </summary>
		/// <param name="str">The string to parse</param>
		/// <param name="assumeMissingTimeZoneAs">If the string is missing a timezone designator, then it assumes this was the offset used. If null, uses offset of <see cref="TimeZoneInfo.Local"/>.</param>
		/// <returns>A UtcDateTime if parsing was successful, or an error message otherwise.</returns>
		public static Maybe<UtcDateTime, string> TryParseIso8601String(in ReadOnlySpan<char> str, TimeSpan? assumeMissingTimeZoneAs = null)
		{
			ReadOnlySpan<char> ts = str.Trim();
			if (!LexedIso8601.LexIso8601(ts).Success(out LexedIso8601 luthor, out string errMsg))
			{
				return errMsg;
			}
			int year = IntParse(luthor.Year.Slice(str), NumberStyles.None);
			int month = 0, day = 1, hour = 0, minute = 0, second = 0, millis = 0;
			if ((luthor.PartsFound & Iso8601Parts.Month) == Iso8601Parts.Month)
			{
				month = IntParse(luthor.Month.Slice(str), NumberStyles.None);
			}
			if ((luthor.PartsFound & Iso8601Parts.Day) == Iso8601Parts.Day)
			{
				day = IntParse(luthor.Day.Slice(str), NumberStyles.None);
			}
			if ((luthor.PartsFound & Iso8601Parts.Hour) == Iso8601Parts.Hour)
			{
				hour = IntParse(luthor.Hour.Slice(str), NumberStyles.None);
			}
			if ((luthor.PartsFound & Iso8601Parts.Minute) == Iso8601Parts.Minute)
			{
				minute = IntParse(luthor.Minute.Slice(str), NumberStyles.None);
			}
			if ((luthor.PartsFound & Iso8601Parts.Second) == Iso8601Parts.Second)
			{
				second = IntParse(luthor.Second.Slice(str), NumberStyles.None);
			}
			if ((luthor.PartsFound & Iso8601Parts.Millis) == Iso8601Parts.Millis)
			{
				// Only parse the first 3 characters of milliseconds, since that's the highest degree of accuracy we allow for
				(int offset, int length) = luthor.Millis;
				millis = IntParse(str.Slice(offset, length > 3 ? 3 : length), NumberStyles.None);
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
							tzHours = IntParse(luthor.TimezoneHours.Slice(str), NumberStyles.None);
						}
						if ((luthor.PartsFound & Iso8601Parts.Tz_Minute) == Iso8601Parts.Tz_Minute)
						{
							tzMinutes = IntParse(luthor.TimezoneMinutes.Slice(str), NumberStyles.None);
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
				ex = DateUtil.MillisFromParts_OrdinalDays(year, day, hour, minute, second, millis, tzHours, tzMinutes, out long ms);
				if (ex == null)
				{
					return new UtcDateTime(ms);
				}
			}
			else
			{
				ex = DateUtil.MillisFromParts(year, month, day, hour, minute, second, millis, tzHours, tzMinutes, out long ms);
				if (ex == null)
				{
					return new UtcDateTime(ms);
				}
			}
			return ex.Message;
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string using Extended Format with UTC as the Timezone Designator (<see cref="Iso8601Parts.Format_ExtendedFormat_UtcTz"/>).
		/// e.g. 2010-12-30T13:30:20.123Z
		/// </summary>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public override string ToString()
		{
#if !NETSTANDARD2_0
			return string.Create(DateUtil.Length_Format_ExtendedFormat_UtcTz, this, (dest, inst) => inst.FormatExtendedFormatUtc(dest, millis: true));
#else
			return Shim.StringCreate(DateUtil.Length_Format_ExtendedFormat_UtcTz, this, (dest, inst) => inst.FormatExtendedFormatUtc(dest, millis: true));
#endif
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string according to the rules specified by <paramref name="format"/>.
		/// Always written from the perspective of UTC. The default timezone designator used is UTC.
		/// Note that if you omit the Time, this may cause data loss; when read again, time is assumed to be 00:00 of whatever timezone the string is interpreted as.
		/// </summary>
		/// <param name="format">How to format the string. By default, this is ISO-8601 extended, with UTC timezone designator</param>
		/// <param name="dateSeparator">The separator placed between year/month/day</param>
		/// <param name="timeSeparator">The separator placed between hour/minute/second</param>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public string ToIso8601StringUtc(Iso8601Parts format = Iso8601Parts.Format_ExtendedFormat_UtcTz, char dateSeparator = '-', char timeSeparator = ':')
		{
			string? err = DateUtil.ValidateAsFormat(format);
			return err == null
#if !NETSTANDARD2_0
				? string.Create(DateUtil.LengthRequired(format), this, (dest, inst) => inst.TryFormat(dest, TimeSpan.Zero, format, dateSeparator, timeSeparator))
#else
				? Shim.StringCreate(DateUtil.LengthRequired(format), this, (dest, inst) => inst.TryFormat(dest, TimeSpan.Zero, format, dateSeparator, timeSeparator))
#endif
				: throw new ArgumentException(err, nameof(format));
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string according to the rules specified by <paramref name="format"/>.
		/// Always written from the perspective of <see cref="TimeZoneInfo.Local"/>. By default no timezone designator is used.
		/// This should NOT be used to communicate across timezones, as it is ambiguous!
		/// Note that if you omit the Time, this may cause data loss; when read again, time is assumed to be 00:00 of whatever timezone the string is interpreted as.
		/// </summary>
		/// <param name="format">How to format the string. By default, this is ISO-8601 extended, with UTC timezone designator</param>
		/// <param name="dateSeparator">The separator placed between year/month/day</param>
		/// <param name="timeSeparator">The separator placed between hour/minute/second</param>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public string ToIso8601StringLocal(Iso8601Parts format = Iso8601Parts.Format_ExtendedFormat_LocalTz, char dateSeparator = '-', char timeSeparator = ':')
		{
			string? err = DateUtil.ValidateAsFormat(format);
			return err == null
#if !NETSTANDARD2_0
				? string.Create(DateUtil.LengthRequired(format), this, (dest, inst) => inst.TryFormat(dest, TimeZoneInfo.Local.BaseUtcOffset, format, dateSeparator, timeSeparator))
#else
				? Shim.StringCreate(DateUtil.LengthRequired(format), this, (dest, inst) => inst.TryFormat(dest, TimeZoneInfo.Local.BaseUtcOffset, format, dateSeparator, timeSeparator))
#endif
				: throw new ArgumentException(err, nameof(format));
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string according to the rules specified by <paramref name="format"/>.
		/// Written from the perspective of <paramref name="timezone"/>. By default a full timezone designator is used.
		/// Note that if you omit the Time, this may cause data loss; when read again, time is assumed to be 00:00 of whatever timezone the string is interpreted as.
		/// </summary>
		/// <param name="timezone">If writing a non-UTC timezone designator or unqualified, writes the time with this offset. If using UTC timezone designator this is ignored.</param>
		/// <param name="format">How to format the string. By default, this is ISO-8601 extended, with full timzone designator</param>
		/// <param name="dateSeparator">The separator placed between year/month/day</param>
		/// <param name="timeSeparator">The separator placed between hour/minute/second</param>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public string ToIso8601StringWithTimeZone(TimeSpan timezone, Iso8601Parts format = Iso8601Parts.Format_ExtendedFormat_FullTz, char dateSeparator = '-', char timeSeparator = ':')
		{
			string? err = DateUtil.ValidateAsFormat(format);
			return err == null
#if !NETSTANDARD2_0
				? string.Create(DateUtil.LengthRequired(format), this, (dest, inst) => inst.TryFormat(dest, timezone, format, dateSeparator, timeSeparator))
#else
				? Shim.StringCreate(DateUtil.LengthRequired(format), this, (dest, inst) => inst.TryFormat(dest, timezone, format, dateSeparator, timeSeparator))
#endif
				: throw new ArgumentException(err, nameof(format));
		}
		/// <summary>
		/// Formats this UtcDateTime to <paramref name="destination"/> as an ISO-8601 string, according to the rules specified by <paramref name="format"/>.
		/// The provided <paramref name="timezone"/> specifies the timezone designator to use and then writes the string according to the <paramref name="format"/>.
		/// Note that if <paramref name="timezone"/> is provided and <paramref name="format"/> specifies a UTC Timezone designator or no Timezone designator (Local) this doesn't have any effect; use Tz_Hour or Tz_HourMinute.
		/// </summary>
		/// <param name="destination">The destination to write to. Assumes it has enough length to hold the string; the caller must verify.</param>
		/// <param name="format">How to format the string. By default, this is ISO-8601 extended (Everything, with separators, and UTC timezone)</param>
		/// <param name="timezone">If writing a non-UTC timezone designator or unqualified, writes the time with this offset. If null, uses UTC offset of <see cref="TimeZoneInfo.Local"/>. If using UTC timezone designator this no used (and <see cref="TimeZoneInfo.Local"/> is not accessed).</param>
		/// <param name="dateSeparator">The separator placed between year/month/day</param>
		/// <param name="timeSeparator">The separator placed between hour/minute/second</param>
		/// <returns>The numbers of chars written, or an error message.</returns>
		public Maybe<int, string> TryFormat(Span<char> destination, TimeSpan? timezone = null, Iso8601Parts format = Iso8601Parts.Format_ExtendedFormat_UtcTz, char dateSeparator = '-', char timeSeparator = ':')
		{
			string? err = DateUtil.ValidateAsFormat(format);
			if (err != null)
			{
				return err;
			}
			int len = DateUtil.LengthRequired(format);
			if (destination.Length < len)
			{
				return string.Concat("Destination span is too small. Required length is ", len, " but the destination span length is only ", destination.Length);
			}

			bool seps = (format & Iso8601Parts.Separator_Date) == Iso8601Parts.Separator_Date;

			Iso8601Parts ftz = format & Iso8601Parts.Mask_Tz;
			// TODO When omitting Time, treat this instance as midnight in whatever timezone they pass; or local or UTC, whatever.
			// When we aren't writing the time, and timezone designator is absent, we still offset to the local timezone,
			// which can cause the date to change. The thing is, when we write just the Date that technically causes data loss.
			// So when we write the date, I guess what we're saying is that we mean this date in our local timezone. In that case, we should not offset it at all

			// If we're writing a UTC Timezone designator, timezone is meaningless and our offset is 0
			// If we're writing unqualified or hour/min, we need to take timezone into account. So we'll either write the value of timezone later, or just assume that it was local
			TimeSpan tz;
			long tzOffsetMs;
			if (ftz == Iso8601Parts.Tz_Utc)
			{
				tzOffsetMs = 0;
				tz = TimeSpan.Zero;
			}
			else
			{
				tz = timezone ?? TimeZoneInfo.Local.BaseUtcOffset;
				tzOffsetMs = tz.Ticks / TimeSpan.TicksPerMillisecond;
			}
			DateUtil.CalcDateTimeParts(TotalMilliseconds + tzOffsetMs, out int year, out int month, out int day, out int hour, out int minute, out int second, out int ms);

			int i = 0;
			if ((format & Iso8601Parts.Year) == Iso8601Parts.Year)
			{
				Formatting.Write4Digits((uint)year, destination, 0);
				i += 4;
				if (seps)
				{
					destination[i++] = dateSeparator;
				}
			}
			else
			{
				// If no year, write --. Doesn't matter if we specified separators or not, that's what we write.
				// This is primarily used for VCF format for birthdays, when the year is unknown (--MM-dd or --MMdd)
				destination[i++] = dateSeparator;
				destination[i++] = dateSeparator;
			}
			if ((format & Iso8601Parts.Month) == 0 && ((format & Iso8601Parts.Day) == Iso8601Parts.Day))
			{
				// Month and no Day is the ordinal format; we need to turn months into days and add that together with day to get the number to write
				int[] totalDaysFromStartYearToMonth = DateTime.IsLeapYear(year) ? DateUtil.TotalDaysFromStartLeapYearToMonth : DateUtil.TotalDaysFromStartYearToMonth;
				Formatting.Write3Digits((uint)(totalDaysFromStartYearToMonth[month - 1] + day), destination, i);
				i += 3;
			}
			else
			{
				if ((format & Iso8601Parts.Month) == Iso8601Parts.Month)
				{
					Formatting.Write2Digits((uint)month, destination, i);
					i += 2;
				}
				if ((format & Iso8601Parts.Day) == Iso8601Parts.Day)
				{
					if (seps)
					{
						destination[i++] = dateSeparator;
					}
					Formatting.Write2Digits((uint)day, destination, i);
					i += 2;
				}
			}

			if ((format & Iso8601Parts.Mask_Time) != 0)
			{
				seps = (format & Iso8601Parts.Separator_Time) == Iso8601Parts.Separator_Time;
				destination[i++] = 'T';
				if ((format & Iso8601Parts.Hour) == Iso8601Parts.Hour)
				{
					Formatting.Write2Digits((uint)hour, destination, i);
					i += 2;
				}
				if ((format & Iso8601Parts.Minute) == Iso8601Parts.Minute)
				{
					if (seps)
					{
						destination[i++] = timeSeparator;
					}
					Formatting.Write2Digits((uint)minute, destination, i);
					i += 2;
				}
				if ((format & Iso8601Parts.Second) == Iso8601Parts.Second)
				{
					if (seps)
					{
						destination[i++] = timeSeparator;
					}
					Formatting.Write2Digits((uint)second, destination, i);
					i += 2;
				}
				if ((format & Iso8601Parts.Millis) == Iso8601Parts.Millis)
				{
					destination[i++] = '.';
					Formatting.Write3Digits((uint)ms, destination, i);
					i += 3;
				}
			}

			switch (ftz)
			{
				case Iso8601Parts.Tz_Utc:
					destination[i++] = 'Z';
					break;
				case Iso8601Parts.Tz_Hour:
					destination[i++] = tzOffsetMs >= 0 ? '+' : '-';
					int tzi = Math.Abs(tz.Hours);
					Formatting.Write2Digits((uint)tzi, destination, i);
					i += 2;
					break;
				case Iso8601Parts.Tz_HourMinute:
					destination[i++] = tzOffsetMs >= 0 ? '+' : '-';
					tzi = Math.Abs(tz.Hours);
					Formatting.Write2Digits((uint)tzi, destination, i);
					i += 2;
					if ((format & Iso8601Parts.Separator_Tz) == Iso8601Parts.Separator_Tz)
					{
						destination[i++] = ':';
					}
					tzi = Math.Abs(tz.Minutes);
					Formatting.Write2Digits((uint)tzi, destination, i);
					i += 2;
					break;
			}
			return i;
		}
		/// <summary>
		/// An optimized method for writing using the format <see cref="Iso8601Parts.Format_ExtendedFormat_UtcTz"/>, with or without millseconds.
		/// This is the method used by <see cref="ToString"/>.
		/// </summary>
		/// <param name="destination">The destination to write to.</param>
		/// <param name="millis">Whether or not to include milliseconds (to 3 decimal places).</param>
		/// <param name="dateSeparator">The separator placed between year/month/day</param>
		/// <param name="timeSeparator">The separator placed between hour/minute/second</param>
		/// <returns>The number of chars written (always 24 or 20), or 0 on failure.</returns>
		public int FormatExtendedFormatUtc(Span<char> destination, bool millis, char dateSeparator = '-', char timeSeparator = ':')
		{
			int len = millis ? DateUtil.Length_Format_ExtendedFormat_UtcTz : DateUtil.Length_Format_ExtendedFormat_NoMillis_UtcTz;
			if (destination.Length < len)
			{
				return 0;
			}
			{ _ = destination[len - 1]; }
			// yyyy-MM-ddTHH:mm:ss.sssZ
			// 012345678901234567890123

			// yyyy-MM-ddTHH:mm:ssZ
			// 01234567890123456789

			DateUtil.CalcDateTimeParts(TotalMilliseconds, out int year, out int month, out int day, out int hour, out int minute, out int second, out int ms);
			Formatting.Write4Digits((uint)year, destination, 0);
			destination[4] = dateSeparator;
			Formatting.Write2Digits((uint)month, destination, 5);
			destination[7] = dateSeparator;
			Formatting.Write2Digits((uint)day, destination, 8);
			destination[10] = 'T';
			Formatting.Write2Digits((uint)hour, destination, 11);
			destination[13] = timeSeparator;
			Formatting.Write2Digits((uint)minute, destination, 14);
			destination[16] = timeSeparator;
			Formatting.Write2Digits((uint)second, destination, 17);
			if (millis)
			{
				destination[19] = '.';
				Formatting.Write3Digits((uint)ms, destination, 20);
				destination[23] = 'Z';
			}
			else
			{
				destination[19] = 'Z';
			}
			return len;
		}
	}
}