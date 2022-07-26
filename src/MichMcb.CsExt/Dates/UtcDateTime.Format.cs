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
		public static Maybe<UtcDateTime, string> TryParseRfc3339String(ReadOnlySpan<char> str, bool allowSpaceInsteadOfT = false)
		{
			return Rfc3339.Parse(str, allowSpaceInsteadOfT).Success(out Rfc3339? rfc, out string? err)
				? TicksFromYearMonthDay(rfc.Year, rfc.Month, rfc.Day).Success(out long timeTicks, out err)
					&& TicksFromHourMinuteSecondMillisTimezoneOffset(rfc.Hour, rfc.Minute, rfc.Second, rfc.Millis, rfc.Timezone).Success(out long dateTicks, out err)
					? new UtcDateTime(timeTicks + dateTicks)
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
		public static Maybe<UtcDateTime, string> TryParseIso8601String(ReadOnlySpan<char> str)
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
		public static Maybe<UtcDateTime, string> TryParseIso8601String(ReadOnlySpan<char> str, Tz timezoneWhenMissing)
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
		public static Maybe<UtcDateTime, string> TryParseIso8601String(ReadOnlySpan<char> str, Tz timezoneWhenMissing, bool allowMissingTimezone)
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

			Tz tz;
			if (iso.Timezone == null)
			{
				if (!allowMissingTimezone)
				{
					return Compat.StringConcat("This ISO-8601 time was missing a timezone designator: ".AsSpan(), str);
				}
				else
				{
					tz = timezoneWhenMissing;
				}
			}
			else
			{
				tz = iso.Timezone.Value;
			}

			string err;
			if ((iso.PartsFound & Iso8601Parts.Mask_Date) == Iso8601Parts.YearDay)
			{
				return TicksFromYearOrdinalDays(iso.Year, iso.Day).Success(out long tTicks, out err) && TicksFromHourMinuteSecondMillisTimezoneOffset(iso.Hour, iso.Minute, iso.Second, iso.Millis, tz).Success(out long dTicks, out err)
					? new UtcDateTime(tTicks + dTicks)
					: err;
			}
			else if ((iso.PartsFound & Iso8601Parts.Week) == Iso8601Parts.Week)
			{
				return TicksFromYearWeekDay(iso.Year, iso.MonthOrWeek, (IsoDayOfWeek)iso.Day).Success(out long tTicks, out err) && TicksFromHourMinuteSecondMillisTimezoneOffset(iso.Hour, iso.Minute, iso.Second, iso.Millis, tz).Success(out long dTicks, out err)
					? new UtcDateTime(tTicks + dTicks)
					: err;
			}
			else
			{
				return TicksFromYearMonthDay(iso.Year, iso.MonthOrWeek, iso.Day).Success(out long tTicks, out err) && TicksFromHourMinuteSecondMillisTimezoneOffset(iso.Hour, iso.Minute, iso.Second, iso.Millis, tz).Success(out long dTicks, out err)
					? new UtcDateTime(tTicks + dTicks)
					: err;
			}
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string using Extended Format with UTC as the Timezone Designator (<see cref="Iso8601Format.ExtendedFormat_UtcTz"/>) with 3 decimal places.
		/// e.g. 2010-12-30T13:30:20.123Z
		/// </summary>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public override string ToString()
		{
			return Iso8601Format.ExtendedFormat_UtcTz.CreateString(Ticks);
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string. Always uses UTC as the timezone designator.
		/// </summary>
		/// <param name="extended">If true uses extended format, if false uses basic format.</param>
		/// <param name="decimalPlaces">The number of decimal places to include.</param>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public string ToIso8601StringUtc(bool extended = true, int decimalPlaces = 3)
		{
			return Iso8601Format.GetFormat(TimeZoneType.Utc, extended, decimalPlaces).CreateString(Ticks);
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string. Always uses a full timezone designator.
		/// </summary>
		/// <param name="extended">If true uses extended format, if false uses basic format.</param>
		/// <param name="decimalPlaces">The number of decimal places to include.</param>
		/// <param name="timezone">The timezone to use.  Null will use <see cref="TimeZoneInfo.BaseUtcOffset"/> of <see cref="TimeZoneInfo.Local"/>.</param>
		/// <returns>An ISO-8601 representing this UtcDateTime</returns>
		public string ToIso8601StringTz(bool extended = true, int decimalPlaces = 3, Tz? timezone = null)
		{
			return Iso8601Format.GetFormat(TimeZoneType.Full, extended, decimalPlaces).CreateString(Ticks, timezone);
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string according to the rules provided.
		/// </summary>
		/// <param name="tz">What kind of timezone to use.</param>
		/// <param name="extended">If true, dashes and colons are used to separate date/time/timezone. If false, no separators are used.</param>
		/// <param name="decimalPlaces">How many decimal places to write, or none. If less than 0, throws <see cref="ArgumentOutOfRangeException"/>.</param>
		/// <param name="timezone">The timezone to use.  Null will use <see cref="TimeZoneInfo.BaseUtcOffset"/> of <see cref="TimeZoneInfo.Local"/>.</param>
		/// <returns>An <see cref="Iso8601Format"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="decimalPlaces"/> is less than 0.</exception>
		public string ToIso8601String(TimeZoneType tz, bool extended = true, int decimalPlaces = 3, Tz? timezone = null)
		{
			return Iso8601Format.GetFormat(tz, extended, decimalPlaces).CreateString(Ticks, timezone);
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string according to the rules specified by <paramref name="format"/>.
		/// Note that if you omit the Time, this may cause data loss; when read again, time is assumed to be 00:00 of whatever timezone the string is interpreted as.
		/// </summary>
		/// <param name="format">How to format the string.</param>
		/// <param name="timezone">For non-UTC timezone designators or a local designator, writes the time with this offset. Null means the <see cref="TimeZoneInfo.BaseUtcOffset"/> of <see cref="TimeZoneInfo.Local"/>. If using UTC timezone designator this is ignored.</param>
		public string ToIso8601StringAsFormat(Iso8601Format format, Tz? timezone)
		{
			return format.CreateString(Ticks, timezone);
		}
		/// <summary>
		/// Formats this UtcDateTime to <paramref name="destination"/> as an ISO-8601 string, according to the rules specified by <paramref name="format"/>.
		/// The provided <paramref name="timezone"/> specifies the timezone designator to use and then writes the string according to the <paramref name="format"/>.
		/// Note that if <paramref name="timezone"/> is provided and <paramref name="format"/> specifies a UTC Timezone designator or no Timezone designator (Local) this doesn't have any effect; use Tz_Hour or Tz_HourMinute.
		/// </summary>
		/// <param name="destination">The destination to write to. If this doesn't have the length required to hold the resultant string, returns an error.</param>
		/// <param name="timezone">For non-UTC timezone designators or a local designator, writes the time with this offset. Null means the <see cref="TimeZoneInfo.BaseUtcOffset"/> of <see cref="TimeZoneInfo.Local"/>. If using UTC timezone designator this is ignored.</param>
		/// <param name="format">How to format the string.</param>
		/// <returns>The numbers of chars written, or an error message.</returns>
		public int Format(Span<char> destination, Tz? timezone, Iso8601Format format = default)
		{
			return format.WriteString(destination, Ticks, timezone);
		}
		/// <summary>
		/// An optimized method for writing using the format <see cref="Iso8601Parts.Format_BasicFormat_UtcTz"/>, with or without millseconds.
		/// </summary>
		/// <param name="destination">The destination to write to. Must be 16 long if <paramref name="decimalPlaces"/> is 0, or 17 + <paramref name="decimalPlaces"/> otherwise.</param>
		/// <param name="decimalPlaces">The number of decimal places to include.</param>
		/// <returns>The number of chars written, or if <paramref name="destination"/> is too small, the space required as a negative number.</returns>
		public int FormatBasicFormatUtc(Span<char> destination, int decimalPlaces = 3)
		{
			return Iso8601Format.FormatBasicFormatUtc(destination, Ticks, decimalPlaces);
		}
		/// <summary>
		/// An optimized method for writing using the format <see cref="Iso8601Parts.Format_ExtendedFormat_UtcTz"/>, with or without millseconds.
		/// </summary>
		/// <param name="destination">The destination to write to. Must be 16 long if <paramref name="decimalPlaces"/> is 0, or 17 + <paramref name="decimalPlaces"/> otherwise.</param>
		/// <param name="decimalPlaces">The number of decimal places to include.</param>
		/// <returns>The number of chars written, or if <paramref name="destination"/> is too small, the space required as a negative number.</returns>
		public int FormatExtendedFormatUtc(Span<char> destination, int decimalPlaces = 3)
		{
			return Iso8601Format.FormatExtendedFormatUtc(destination, Ticks, decimalPlaces);
		}
	}
}