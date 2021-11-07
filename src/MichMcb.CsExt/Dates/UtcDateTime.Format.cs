namespace MichMcb.CsExt.Dates
{
	using System;

	public readonly partial struct UtcDateTime : IEquatable<UtcDateTime>, IComparable<UtcDateTime>
	{
		/// <summary>
		/// Parses an ISO-8601 string as a UtcDateTime. Only accurate to the millisecond (3 places); further accuracy is truncated.
		/// A timezone designator is required.
		/// All parsed ISO-8601 strings are adjusted to UTC.
		/// Any leading or trailing whitespace is ignored.
		/// </summary>
		/// <param name="str">The string to parse</param>
		/// <returns>A UtcDateTime if parsing was successful, or an error message otherwise.</returns>
		public static Maybe<UtcDateTime, string> TryParseIso8601String(in ReadOnlySpan<char> str)
		{
			return TryParseIso8601String(str, default, false);
		}
		/// <summary>
		/// Parses an ISO-8601 string as a UtcDateTime. Only accurate to the millisecond (3 places); further accuracy is truncated.
		/// A timezone designator is not required.
		/// All parsed ISO-8601 strings are adjusted to UTC.
		/// Any leading or trailing whitespace is ignored.
		/// </summary>
		/// <param name="str">The string to parse</param>
		/// <param name="timezoneWhenMissing">If the string is missing a timezone designator then this is the timezone assumed. Use <see cref="TimeZoneInfo.Local"/> if you want to interpret this as the local timezone.</param>
		/// <returns>A UtcDateTime if parsing was successful, or an error message otherwise.</returns>
		public static Maybe<UtcDateTime, string> TryParseIso8601String(in ReadOnlySpan<char> str, TimeSpan timezoneWhenMissing)
		{
			return TryParseIso8601String(str, timezoneWhenMissing, true);
		}
		/// <summary>
		/// Parses an ISO-8601 string as a UtcDateTime. Only accurate to the millisecond (3 places); further accuracy is truncated.
		/// A timezone designator is only required if <paramref name="allowMissingTimezone"/> is false.
		/// All parsed ISO-8601 strings are adjusted to UTC.
		/// Any leading or trailing whitespace is ignored.
		/// </summary>
		/// <param name="str">The string to parse</param>
		/// <param name="timezoneWhenMissing">If the string is missing a timezone designator then this is the timezone assumed. Use <see cref="TimeZoneInfo.Local"/> if you want to interpret this as the local timezone.</param>
		/// <param name="allowMissingTimezone">If true, a timezone designator is allowed to be missing, and will be assumed to be <paramref name="timezoneWhenMissing"/>. Otherwise, a timezone designator is required.</param>
		/// <returns>A UtcDateTime if parsing was successful, or an error message otherwise.</returns>
		public static Maybe<UtcDateTime, string> TryParseIso8601String(in ReadOnlySpan<char> str, TimeSpan timezoneWhenMissing, bool allowMissingTimezone)
		{
			ReadOnlySpan<char> ts = str.Trim();
			if (!LexedIso8601.LexIso8601(ts).Success(out LexedIso8601 luthor, out string errMsg))
			{
				return errMsg;
			}
			luthor.Parse(ts, out int year, out int month, out int day, out int hour, out int minute, out int second, out int millis, out int tzHours, out int tzMinutes);
			if (luthor.TimezoneChar == '\0')
			{
				if (!allowMissingTimezone)
				{
#if !NETSTANDARD2_0
					return string.Concat("This ISO-8601 time was missing a timezone designator: ", str);
#else
				return Shim.StringConcat("This ISO-8601 time was missing a timezone designator: ".AsSpan(), str);
#endif
				}
				else
				{
					TimeSpan tzSpan = timezoneWhenMissing;
					tzHours = tzSpan.Hours;
					tzMinutes = tzSpan.Minutes;
				}
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
		/// Note that if you omit the Time, this may cause data loss; when read again, time is assumed to be 00:00 of whatever timezone the string is interpreted as.
		/// </summary>
		/// <param name="timezone">If writing a non-UTC timezone designator or unqualified, writes the time with this offset. If using UTC timezone designator this is ignored.</param>
		/// <param name="format">How to format the string.</param>
		/// <returns>An ISO-8601 representing this UtcDateTime, or an error message.</returns>
		public Maybe<string, string> TryToIso8601String(TimeSpan timezone, Iso8601Parts format)
		{
			return DateUtil.TryGetLengthRequired(format).Success(out int len, out string? errMsg)
#if !NETSTANDARD2_0
				? Maybe<string, string>.Value(string.Create(len, this, (dest, inst) => inst.FormatUnchecked(dest, timezone, format)))
#else
				? Maybe<string, string>.Value(Shim.StringCreate(len, this, (dest, inst) => inst.FormatUnchecked(dest, timezone, format)))
#endif
				: Maybe<string, string>.Error(errMsg);
		}
		/// <summary>
		/// Calls <see cref="TryToIso8601String(TimeSpan, Iso8601Parts)"/>.
		/// </summary>
		/// <param name="timezone">If writing a non-UTC timezone designator or unqualified, writes the time with this offset. If using UTC timezone designator this is ignored.</param>
		/// <param name="format">How to format the string. By default, this is ISO-8601 extended, with UTC timezone designator</param>
		/// <returns>An ISO-8601 representing this UtcDateTime.</returns>
		/// <exception cref="ArgumentException">When <paramref name="format"/> is not valid.</exception>
		public string ToIso8601String(TimeSpan timezone, Iso8601Parts format)
		{
			return TryToIso8601String(timezone, format).Success(out string iso, out string errMsg)
				? iso
				: throw new ArgumentException(errMsg, nameof(format));
		}
		/// <summary>
		/// Formats this istnance as an ISO-8601 string using Extended Format with UTC as Timezone Designator (<see cref="Iso8601Parts.Format_ExtendedFormat_UtcTz"/>).
		/// </summary>
		/// <param name="millis">Whether or not to include milliseconds (to 3 decimal places).</param>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public string ToIso8601StringExtendedUtc(bool millis = true)
		{
#if !NETSTANDARD2_0
			return string.Create(millis ? DateUtil.Length_Format_ExtendedFormat_UtcTz : DateUtil.Length_Format_ExtendedFormat_NoMillis_UtcTz, this, (dest, inst) => inst.FormatExtendedFormatUtc(dest, millis));
#else
			return Shim.StringCreate(millis ? DateUtil.Length_Format_ExtendedFormat_UtcTz : DateUtil.Length_Format_ExtendedFormat_NoMillis_UtcTz, this, (dest, inst) => inst.FormatExtendedFormatUtc(dest, millis));
#endif
		}
		/// <summary>
		/// Formats this istnance as an ISO-8601 string using Basic Format with UTC as Timezone Designator (<see cref="Iso8601Parts.Format_BasicFormat_UtcTz"/>).
		/// </summary>
		/// <param name="millis">Whether or not to include milliseconds (to 3 decimal places).</param>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public string ToIso8601StringBasicUtc(bool millis = true)
		{
#if !NETSTANDARD2_0
			return string.Create(millis ? DateUtil.Length_Format_BasicFormat_UtcTz : DateUtil.Length_Format_BasicFormat_NoMillis_UtcTz, this, (dest, inst) => inst.FormatBasicFormatUtc(dest, millis));
#else
			return Shim.StringCreate(millis ? DateUtil.Length_Format_BasicFormat_UtcTz : DateUtil.Length_Format_BasicFormat_NoMillis_UtcTz, this, (dest, inst) => inst.FormatBasicFormatUtc(dest, millis));
#endif
		}
		/// <summary>
		/// Formats this istnance as an ISO-8601 string using Extended Format with <paramref name="timezone"/> as Timezone Designator (<see cref="Iso8601Parts.Format_ExtendedFormat_FullTz"/>).
		/// Or if <paramref name="extended"/> is false, uses Basic Format (<see cref="Iso8601Parts.Format_BasicFormat_FullTz"/>).
		/// </summary>
		/// <param name="extended">How to format the string. By default, this is ISO-8601 extended, with UTC timezone designator</param>
		/// <param name="millis">Whether or not to include milliseconds (to 3 decimal places).</param>
		/// <param name="timezone">The timezone to use. If null, uses offset of <see cref="TimeZoneInfo.Local"/></param>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public string ToIso8601StringWithTimeZone(bool extended = true, bool millis = true, TimeSpan? timezone = null)
		{
			Iso8601Parts fmt = (extended ? Iso8601Parts.Format_ExtendedFormat_NoMillis_FullTz : Iso8601Parts.Format_BasicFormat_NoMillis_FullTz) | (millis ? Iso8601Parts.Millis : 0);
#if !NETSTANDARD2_0
			return string.Create(DateUtil.TryGetLengthRequired(fmt).ValueOr(0), this, (dest, inst) => inst.FormatUnchecked(dest, timezone, fmt));
#else
			return Shim.StringCreate(DateUtil.TryGetLengthRequired(fmt).ValueOr(0), this, (dest, inst) => inst.FormatUnchecked(dest, timezone, fmt));
#endif
		}
		/// <summary>
		/// Formats this UtcDateTime to <paramref name="destination"/> as an ISO-8601 string, according to the rules specified by <paramref name="format"/>.
		/// The provided <paramref name="timezone"/> specifies the timezone designator to use and then writes the string according to the <paramref name="format"/>.
		/// Note that if <paramref name="timezone"/> is provided and <paramref name="format"/> specifies a UTC Timezone designator or no Timezone designator (Local) this doesn't have any effect; use Tz_Hour or Tz_HourMinute.
		/// </summary>
		/// <param name="destination">The destination to write to. If this doesn't have the length required to hold the resultant string, returns an error.</param>
		/// <param name="format">How to format the string. By default, this is ISO-8601 extended (Everything, with separators, and UTC timezone)</param>
		/// <param name="timezone">If writing a non-UTC timezone designator or unqualified, writes the time with this offset. If null, uses UTC offset of <see cref="TimeZoneInfo.Local"/>. If using UTC timezone designator this no used (and <see cref="TimeZoneInfo.Local"/> is not accessed).</param>
		/// <returns>The numbers of chars written, or an error message.</returns>
		public Maybe<int, string> TryFormat(Span<char> destination, TimeSpan? timezone = null, Iso8601Parts format = Iso8601Parts.Format_ExtendedFormat_UtcTz)
		{
			if (!DateUtil.TryGetLengthRequired(format).Success(out int len, out string? errMsg))
			{
				return errMsg;
			}
			if (destination.Length < len)
			{
				return string.Concat("Destination span is too small. Required length is ", len, " but the destination span length is only ", destination.Length);
			}
			return FormatUnchecked(destination, timezone, format);
		}
		private int FormatUnchecked(Span<char> destination, TimeSpan? timezone = null, Iso8601Parts format = Iso8601Parts.Format_ExtendedFormat_UtcTz)
		{
			switch (format)
			{
				case Iso8601Parts.Format_ExtendedFormat_UtcTz: return FormatExtendedFormatUtc(destination, true);
				case Iso8601Parts.Format_ExtendedFormat_NoMillis_UtcTz: return FormatExtendedFormatUtc(destination, false);
				case Iso8601Parts.Format_BasicFormat_UtcTz: return FormatBasicFormatUtc(destination, true);
				case Iso8601Parts.Format_BasicFormat_NoMillis_UtcTz: return FormatBasicFormatUtc(destination, false);
				default:
					break;
			}

			bool seps = (format & Iso8601Parts.Separator_Date) == Iso8601Parts.Separator_Date;

			Iso8601Parts ftz = format & Iso8601Parts.Mask_Tz;
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
					destination[i++] = '-';
				}
			}
			else
			{
				// If no year, write --. Doesn't matter if we specified separators or not, that's what we write.
				// This is primarily used for VCF format for birthdays, when the year is unknown (--MM-dd or --MMdd)
				destination[i++] = '-';
				destination[i++] = '-';
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
						destination[i++] = '-';
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
						destination[i++] = ':';
					}
					Formatting.Write2Digits((uint)minute, destination, i);
					i += 2;
				}
				if ((format & Iso8601Parts.Second) == Iso8601Parts.Second)
				{
					if (seps)
					{
						destination[i++] = ':';
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
		/// An optimized method for writing using the format <see cref="Iso8601Parts.Format_BasicFormat_UtcTz"/>, with or without millseconds.
		/// </summary>
		/// <param name="destination">The destination to write to. Must be 20 long if <paramref name="millis"/> is true or 16 long if <paramref name="millis"/> is false.</param>
		/// <param name="millis">Whether or not to include milliseconds (to 3 decimal places).</param>
		/// <returns>The number of chars written (20 if <paramref name="millis"/> is true or 16 if <paramref name="millis"/> is false), or 0 if <paramref name="destination"/> is too small.</returns>
		public int FormatBasicFormatUtc(Span<char> destination, bool millis)
		{
			int len = millis ? DateUtil.Length_Format_BasicFormat_UtcTz : DateUtil.Length_Format_BasicFormat_NoMillis_UtcTz;
			if (destination.Length < len)
			{
				return 0;
			}
			{ _ = destination[len - 1]; }
			// yyyyMMddTHHmmss.sssZ
			// 01234567890123456789

			DateUtil.CalcDateTimeParts(TotalMilliseconds, out int year, out int month, out int day, out int hour, out int minute, out int second, out int ms);
			Formatting.Write4Digits((uint)year, destination, 0);
			Formatting.Write2Digits((uint)month, destination, 4);
			Formatting.Write2Digits((uint)day, destination, 6);
			destination[8] = 'T';
			Formatting.Write2Digits((uint)hour, destination, 9);
			Formatting.Write2Digits((uint)minute, destination, 11);
			Formatting.Write2Digits((uint)second, destination, 13);
			if (millis)
			{
				destination[15] = '.';
				Formatting.Write3Digits((uint)ms, destination, 16);
				destination[19] = 'Z';
			}
			else
			{
				destination[15] = 'Z';
			}
			return len;
		}
		/// <summary>
		/// An optimized method for writing using the format <see cref="Iso8601Parts.Format_ExtendedFormat_UtcTz"/>, with or without millseconds.
		/// This is the method used by <see cref="ToString"/>.
		/// </summary>
		/// <param name="destination">The destination to write to. Must be 24 long if <paramref name="millis"/> is true or 20 long if <paramref name="millis"/> is false.</param>
		/// <param name="millis">Whether or not to include milliseconds (to 3 decimal places).</param>
		/// <returns>The number of chars written (24 if <paramref name="millis"/> is true or 20 if <paramref name="millis"/> is false), or 0 if <paramref name="destination"/> is too small.</returns>
		public int FormatExtendedFormatUtc(Span<char> destination, bool millis)
		{
			int len = millis ? DateUtil.Length_Format_ExtendedFormat_UtcTz : DateUtil.Length_Format_ExtendedFormat_NoMillis_UtcTz;
			if (destination.Length < len)
			{
				return 0;
			}
			{ _ = destination[len - 1]; }
			// yyyy-MM-ddTHH:mm:ss.sssZ
			// 012345678901234567890123

			DateUtil.CalcDateTimeParts(TotalMilliseconds, out int year, out int month, out int day, out int hour, out int minute, out int second, out int ms);
			Formatting.Write4Digits((uint)year, destination, 0);
			destination[4] = '-';
			Formatting.Write2Digits((uint)month, destination, 5);
			destination[7] = '-';
			Formatting.Write2Digits((uint)day, destination, 8);
			destination[10] = 'T';
			Formatting.Write2Digits((uint)hour, destination, 11);
			destination[13] = ':';
			Formatting.Write2Digits((uint)minute, destination, 14);
			destination[16] = ':';
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