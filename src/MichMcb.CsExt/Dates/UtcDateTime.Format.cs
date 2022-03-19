namespace MichMcb.CsExt.Dates
{
	using System;

	public readonly partial struct UtcDateTime
	{
		/// <summary>
		/// Parses an RFC3339 string as a <see cref="UtcDateTime"/>. Only accurate to the millisecond (3 places); further accuracy is truncated.
		/// </summary>
		/// <param name="str">The string to parse.</param>
		/// <param name="allowSpaceInsteadOfT">If true, an empty space is allowed instead of T or t to separate date/time. Otherwise, only T or t is allowed.</param>
		/// <returns>A UtcDateTime if parsing was successful, or an error message otherwise.</returns>
		public static Maybe<UtcDateTime, string> TryParseRfc3339String(in ReadOnlySpan<char> str, bool allowSpaceInsteadOfT = false)
		{
			return Rfc3339.Parse(str, allowSpaceInsteadOfT).Success(out Rfc3339? rfc, out string? err)
				? MillisFromYearMonthDay(rfc.Year, rfc.Month, rfc.Day).Success(out long timeMs, out err)
					&& MillisFromHourMinuteSecondMillisTimezoneOffset(rfc.Hour, rfc.Minute, rfc.Second, rfc.Millis, rfc.TimezoneMinutesOffset).Success(out long dateMs, out err)
					? new UtcDateTime(timeMs + dateMs)
					: err
				: err;
		}
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
			if (!Iso8601.Parse(ts).Success(out Iso8601? iso, out string? errMsg))
			{
				return errMsg;
			}

			if ((iso.PartsFound & Iso8601Parts.Mask_Date) == 0)
			{
				return "When parsing a UtcDateTime, a date part is required. If you only want to parse a time, use the Iso8601 class directly.";
			}

			int tz;
			if (iso.TimezoneMinutesOffset == null)
			{
				if (!allowMissingTimezone)
				{
					return Compat.StringConcat("This ISO-8601 time was missing a timezone designator: ".AsSpan(), str);
				}
				else
				{
					TimeSpan tzSpan = timezoneWhenMissing;
					tz = tzSpan.Hours * 60 + tzSpan.Minutes;
				}
			}
			else
			{
				tz = iso.TimezoneMinutesOffset.Value;
			}

			string err;
			if ((iso.PartsFound & Iso8601Parts.Mask_Date) == Iso8601Parts.YearDay)
			{
				return MillisFromYearOrdinalDays(iso.Year, iso.Day).Success(out long timeMs, out err) && MillisFromHourMinuteSecondMillisTimezoneOffset(iso.Hour, iso.Minute, iso.Second, iso.Millis, tz).Success(out long dateMs, out err)
					? new UtcDateTime(timeMs + dateMs)
					: err;
			}
			else if ((iso.PartsFound & Iso8601Parts.Week) == Iso8601Parts.Week)
			{
				return MillisFromYearWeekDay(iso.Year, iso.MonthOrWeek, (IsoDayOfWeek)iso.Day).Success(out long timeMs, out err) && MillisFromHourMinuteSecondMillisTimezoneOffset(iso.Hour, iso.Minute, iso.Second, iso.Millis, tz).Success(out long dateMs, out err)
					? new UtcDateTime(timeMs + dateMs)
					: err;
			}
			else
			{
				return MillisFromYearMonthDay(iso.Year, iso.MonthOrWeek, iso.Day).Success(out long timeMs, out err) && MillisFromHourMinuteSecondMillisTimezoneOffset(iso.Hour, iso.Minute, iso.Second, iso.Millis, tz).Success(out long dateMs, out err)
					? new UtcDateTime(timeMs + dateMs)
					: err;
			}
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string using Extended Format with UTC as the Timezone Designator (<see cref="Iso8601Format.ExtendedFormat_UtcTz"/>).
		/// e.g. 2010-12-30T13:30:20.123Z
		/// </summary>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public override string ToString()
		{
#if !NETSTANDARD2_0
			return string.Create(Iso8601Format.ExtendedFormat_UtcTz.Length, this, (dest, inst) => inst.FormatExtendedFormatUtc(dest, millis: true));
#else
			return Compat.StringCreate(Iso8601Format.ExtendedFormat_UtcTz.Length, this, (dest, inst) => inst.FormatExtendedFormatUtc(dest, millis: true));
#endif
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string. Always uses UTC as the timezone designator.
		/// </summary>
		/// <param name="extended">If true uses extended format, if false uses basic format.</param>
		/// <param name="millis">Whether or not to include milliseconds.</param>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public string ToIso8601StringUtc(bool extended = true, bool millis = true)
		{
#if !NETSTANDARD2_0
			return extended
				? string.Create(millis ? Iso8601Format.ExtendedFormat_UtcTz.Length : Iso8601Format.ExtendedFormat_NoMillis_UtcTz.Length, this, (dest, inst) => inst.FormatExtendedFormatUtc(dest, millis))
				: string.Create(millis ? Iso8601Format.BasicFormat_UtcTz.Length : Iso8601Format.BasicFormat_NoMillis_UtcTz.Length, this, (dest, inst) => inst.FormatBasicFormatUtc(dest, millis));
#else
			return extended
				? Compat.StringCreate(millis ? Iso8601Format.ExtendedFormat_UtcTz.Length : Iso8601Format.ExtendedFormat_NoMillis_UtcTz.Length, this, (dest, inst) => inst.FormatExtendedFormatUtc(dest, millis))
				: Compat.StringCreate(millis ? Iso8601Format.BasicFormat_UtcTz.Length : Iso8601Format.BasicFormat_NoMillis_UtcTz.Length, this, (dest, inst) => inst.FormatBasicFormatUtc(dest, millis));
#endif
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string. Always uses a full timezone designator.
		/// </summary>
		/// <param name="extended">If true uses extended format, if false uses basic format.</param>
		/// <param name="millis">Whether or not to include milliseconds.</param>
		/// <param name="timezone">The timezone to use.  Null will use <see cref="TimeZoneInfo.BaseUtcOffset"/> of <see cref="TimeZoneInfo.Local"/>.</param>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public string ToIso8601StringTz(bool extended = true, bool millis = true, TimeSpan? timezone = null)
		{
			Iso8601Format fmt = extended
				? millis ? Iso8601Format.ExtendedFormat_FullTz : Iso8601Format.ExtendedFormat_NoMillis_FullTz
				: millis ? Iso8601Format.BasicFormat_FullTz : Iso8601Format.BasicFormat_NoMillis_FullTz;
#if !NETSTANDARD2_0
			return string.Create(fmt.Length, this, (dest, inst) => inst.Format(dest, timezone, fmt));
#else
			return Compat.StringCreate(fmt.Length, this, (dest, inst) => inst.Format(dest, timezone, fmt));
#endif
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string according to the rules specified by <paramref name="format"/>.
		/// Note that if you omit the Time, this may cause data loss; when read again, time is assumed to be 00:00 of whatever timezone the string is interpreted as.
		/// </summary>
		/// <param name="format">How to format the string.</param>
		/// <param name="timezone">For non-UTC timezone designators or a local designator, writes the time with this offset. Null means the <see cref="TimeZoneInfo.BaseUtcOffset"/> of <see cref="TimeZoneInfo.Local"/>. If using UTC timezone designator this is ignored.</param>
		/// <returns>An ISO-8601 representing this UtcDateTime, or an error message.</returns>
		public Maybe<string, string> TryToIso8601String(Iso8601Parts format, TimeSpan? timezone)
		{
			return Iso8601Format.TryCreate(format).Success(out Iso8601Format fmt, out string? errMsg)
				? Maybe<string, string>.Value(ToIso8601String(fmt, timezone))
				: Maybe<string, string>.Error(errMsg);
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string according to the rules specified by <paramref name="format"/>.
		/// Note that if you omit the Time, this may cause data loss; when read again, time is assumed to be 00:00 of whatever timezone the string is interpreted as.
		/// </summary>
		/// <param name="format">How to format the string.</param>
		/// <param name="timezone">For non-UTC timezone designators or a local designator, writes the time with this offset. Null means the <see cref="TimeZoneInfo.BaseUtcOffset"/> of <see cref="TimeZoneInfo.Local"/>. If using UTC timezone designator this is ignored.</param>
		public string ToIso8601String(Iso8601Format format, TimeSpan? timezone)
		{
#if !NETSTANDARD2_0
			return string.Create(format.Length, this, (dest, inst) => inst.Format(dest, timezone, format));
#else
			return Compat.StringCreate(format.Length, this, (dest, inst) => inst.Format(dest, timezone, format));
#endif
		}
		/// <summary>
		/// Formats this UtcDateTime to <paramref name="destination"/> as an ISO-8601 string, according to the rules specified by <paramref name="format"/>.
		/// The provided <paramref name="timezone"/> specifies the timezone designator to use and then writes the string according to the <paramref name="format"/>.
		/// Note that if <paramref name="timezone"/> is provided and <paramref name="format"/> specifies a UTC Timezone designator or no Timezone designator (Local) this doesn't have any effect; use Tz_Hour or Tz_HourMinute.
		/// </summary>
		/// <param name="destination">The destination to write to. If this doesn't have the length required to hold the resultant string, returns an error.</param>
		/// <param name="timezone">For non-UTC timezone designators or a local designator, writes the time with this offset. Null means the <see cref="TimeZoneInfo.BaseUtcOffset"/> of <see cref="TimeZoneInfo.Local"/>. If using UTC timezone designator this is ignored.</param>
		/// <param name="format">How to format the string. By default, this is <see cref="Iso8601Parts.Format_ExtendedFormat_UtcTz"/>.</param>
		/// <returns>The numbers of chars written, or an error message.</returns>
		public Maybe<int, string> TryFormat(Span<char> destination, TimeSpan? timezone, Iso8601Parts format = Iso8601Parts.Format_ExtendedFormat_UtcTz)
		{
			if (!Iso8601Format.TryCreate(format).Success(out Iso8601Format fmt, out string? errMsg))
			{
				return errMsg;
			}
			int w = Format(destination, timezone, fmt);
			return w == 0
				? string.Concat("Destination span is too small. Required length is ", fmt.Length, " but the destination span length is only ", destination.Length)
				: w;
		}
		/// <summary>
		/// Formats this UtcDateTime to <paramref name="destination"/> as an ISO-8601 string, according to the rules specified by <paramref name="format"/>.
		/// The provided <paramref name="timezone"/> specifies the timezone designator to use and then writes the string according to the <paramref name="format"/>.
		/// Note that if <paramref name="timezone"/> is provided and <paramref name="format"/> specifies a UTC Timezone designator or no Timezone designator (Local) this doesn't have any effect; use Tz_Hour or Tz_HourMinute.
		/// </summary>
		/// <param name="destination">The destination to write to. If this doesn't have the length required to hold the resultant string, returns an error.</param>
		/// <param name="timezone">For non-UTC timezone designators or a local designator, writes the time with this offset. Null means the <see cref="TimeZoneInfo.BaseUtcOffset"/> of <see cref="TimeZoneInfo.Local"/>. If using UTC timezone designator this is ignored.</param>
		/// <param name="format">How to format the string. If default, this is <see cref="Iso8601Format.ExtendedFormat_UtcTz"/>.</param>
		/// <returns>The numbers of chars written, or an error message.</returns>
		public int Format(Span<char> destination, TimeSpan? timezone, Iso8601Format format = default)
		{
			if (destination.Length < format.Length)
			{
				return 0;
			}
			Iso8601Parts fmt = format.Format;
			switch (fmt)
			{
				// This is intentional; when format is default, then Iso8601Parts will be None, and that corresponds to extended format, UTC.
				case Iso8601Parts.None:
				case Iso8601Parts.Format_ExtendedFormat_UtcTz: return FormatExtendedFormatUtc(destination, true);
				case Iso8601Parts.Format_ExtendedFormat_NoMillis_UtcTz: return FormatExtendedFormatUtc(destination, false);
				case Iso8601Parts.Format_BasicFormat_UtcTz: return FormatBasicFormatUtc(destination, true);
				case Iso8601Parts.Format_BasicFormat_NoMillis_UtcTz: return FormatBasicFormatUtc(destination, false);
				default:
					break;
			}

			bool seps = (fmt & Iso8601Parts.Separator_Date) == Iso8601Parts.Separator_Date;

			Iso8601Parts ftz = fmt & Iso8601Parts.Mask_Tz;
			// When we aren't writing the time, and timezone designator is absent, we still offset to the local timezone,
			// which can cause the date to change. The thing is, when we write just the Date that technically causes data loss.

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

			// We clamp the milliseconds so they can't be larger than MaxMillis or lower than zero.
			long msWithOffset = TotalMilliseconds + tzOffsetMs;
			DateTimePartsFromTotalMilliseconds(msWithOffset > MaxMillis ? MaxMillis : msWithOffset < 0 ? 0 : msWithOffset, out int year, out int month, out int day, out int hour, out int minute, out int second, out int ms);

			int i = 0;

			// Weeks have to be handled differently, because 2019-12-30 for example is actually written 2020-W01-1.
			if ((fmt & Iso8601Parts.YearWeek) == Iso8601Parts.YearWeek)
			{
				// We know this will never fail because we got this out of the method above
				IsoYearWeek isoYearWeek = IsoYearWeek.Create(year, month, day).ValueOrException();
				Formatting.Write4Digits((uint)isoYearWeek.Year, destination, 0);
				i += 4;
				if (seps)
				{
					destination[i++] = '-';
				}

				destination[i++] = 'W';
				Formatting.Write2Digits((uint)isoYearWeek.Week, destination, i);
				i += 2;
				if ((fmt & Iso8601Parts.Day) == Iso8601Parts.Day)
				{
					if (seps)
					{
						destination[i++] = '-';
					}
					destination[i++] = (char)('0' + (int)isoYearWeek.WeekDay);
				}
			}
			else
			{
				if ((fmt & Iso8601Parts.Year) == Iso8601Parts.Year)
				{
					Formatting.Write4Digits((uint)year, destination, 0);
					i += 4;
					if (seps)
					{
						destination[i++] = '-';
					}
				}
				if ((fmt & Iso8601Parts.Month) == 0 && ((fmt & Iso8601Parts.Day) == Iso8601Parts.Day))
				{
					// Month and no Day is the ordinal format; we need to turn months into days and add that together with day to get the number to write
					int ordinalDay = (DateTime.IsLeapYear(year) ? TotalDaysFromStartLeapYearToMonth : TotalDaysFromStartYearToMonth)[month - 1] + day;
					Formatting.Write3Digits((uint)ordinalDay, destination, i);
					i += 3;
				}
				else
				{
					if ((fmt & Iso8601Parts.Month) == Iso8601Parts.Month)
					{
						Formatting.Write2Digits((uint)month, destination, i);
						i += 2;
					}
					if ((fmt & Iso8601Parts.Day) == Iso8601Parts.Day)
					{
						if (seps)
						{
							destination[i++] = '-';
						}
						Formatting.Write2Digits((uint)day, destination, i);
						i += 2;
					}
				}
			}

			if ((fmt & Iso8601Parts.Mask_Time) != 0)
			{
				seps = (fmt & Iso8601Parts.Separator_Time) == Iso8601Parts.Separator_Time;
				// Always have to write T, even if no date was specified
				destination[i++] = 'T';
				if ((fmt & Iso8601Parts.Hour) == Iso8601Parts.Hour)
				{
					Formatting.Write2Digits((uint)hour, destination, i);
					i += 2;
				}
				if ((fmt & Iso8601Parts.Minute) == Iso8601Parts.Minute)
				{
					if (seps)
					{
						destination[i++] = ':';
					}
					Formatting.Write2Digits((uint)minute, destination, i);
					i += 2;
				}
				if ((fmt & Iso8601Parts.Second) == Iso8601Parts.Second)
				{
					if (seps)
					{
						destination[i++] = ':';
					}
					Formatting.Write2Digits((uint)second, destination, i);
					i += 2;
				}
				if ((fmt & Iso8601Parts.Millis) == Iso8601Parts.Millis)
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
					if ((fmt & Iso8601Parts.Separator_Tz) == Iso8601Parts.Separator_Tz)
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
			int len = millis ? Iso8601Format.BasicFormat_UtcTz.Length : Iso8601Format.BasicFormat_NoMillis_UtcTz.Length;
			if (destination.Length < len)
			{
				return 0;
			}
			{ _ = destination[len - 1]; }
			// yyyyMMddTHHmmss.sssZ
			// 01234567890123456789

			Deconstruct(out int year, out int month, out int day, out int hour, out int minute, out int second, out int ms);
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
			int len = millis ? Iso8601Format.ExtendedFormat_UtcTz.Length : Iso8601Format.ExtendedFormat_NoMillis_UtcTz.Length;
			if (destination.Length < len)
			{
				return 0;
			}
			{ _ = destination[len - 1]; }
			// yyyy-MM-ddTHH:mm:ss.sssZ
			// 012345678901234567890123

			Deconstruct(out int year, out int month, out int day, out int hour, out int minute, out int second, out int ms);
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