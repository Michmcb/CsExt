namespace MichMcb.CsExt.Dates
{
	using System;
	/// <summary>
	/// A validated format for ISO-8601 strings. To create one of these you need to use <see cref="TryCreate(Iso8601Parts, int)"/>,
	/// or use one of the static predefined ones.
	/// </summary>
	public readonly struct Iso8601Format
	{
		/// <summary>
		/// The base length without any timezone or milliseconds portion
		/// </summary>
		private const int ExtendedFormat_BaseLength = 19;
		/// <summary>
		/// The base length without any timezone or milliseconds portion
		/// </summary>
		private const int BasicFormat_BaseLength = 15;
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss.fffZ
		/// A good default format. This conforms to RFC3339
		/// This is known in ISO-8601 as "Extended Format".
		/// </summary>
		public static readonly Iso8601Format ExtendedFormat_UtcTz = new(Iso8601Parts.Format_ExtendedFormat_UtcTz, 24, 3);
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss.fff+00:00
		/// This conforms to RFC3339.
		/// </summary>
		public static readonly Iso8601Format ExtendedFormat_FullTz = new(Iso8601Parts.Format_ExtendedFormat_FullTz, 29, 3);
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss.sss
		/// </summary>
		public static readonly Iso8601Format ExtendedFormat_LocalTz = new(Iso8601Parts.Format_ExtendedFormat_LocalTz, 23, 3);
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ssZ
		/// </summary>
		public static readonly Iso8601Format ExtendedFormat_NoMillis_UtcTz = new(Iso8601Parts.Format_ExtendedFormat_NoFractional_UtcTz, 20, 0);
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss+00:00
		/// </summary>
		public static readonly Iso8601Format ExtendedFormat_NoMillis_FullTz = new(Iso8601Parts.Format_ExtendedFormat_NoFractional_FullTz, 25, 0);
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss
		/// </summary>
		public static readonly Iso8601Format ExtendedFormat_NoMillis_LocalTz = new(Iso8601Parts.Format_ExtendedFormat_NoFractional_LocalTz, 19, 0);
		/// <summary>
		/// yyyyMMddTHHmmss.sssZ
		/// Everything, except without separators
		/// This is known in ISO-8601 as "Basic Format"
		/// </summary>
		public static readonly Iso8601Format BasicFormat_UtcTz = new(Iso8601Parts.Format_BasicFormat_UtcTz, 20, 3);
		/// <summary>
		/// yyyyMMddTHHmmss.sss+0000
		/// </summary>
		public static readonly Iso8601Format BasicFormat_FullTz = new(Iso8601Parts.Format_BasicFormat_FullTz, 24, 3);
		/// <summary>
		/// yyyyMMddTHHmmss.sss
		/// </summary>
		public static readonly Iso8601Format BasicFormat_LocalTz = new(Iso8601Parts.Format_BasicFormat_LocalTz, 19, 3);
		/// <summary>
		/// yyyyMMddTHHmmssZ
		/// </summary>
		public static readonly Iso8601Format BasicFormat_NoMillis_UtcTz = new(Iso8601Parts.Format_BasicFormat_NoFractional_UtcTz, 16, 0);
		/// <summary>
		/// yyyyMMddTHHmmss+0000
		/// </summary>
		public static readonly Iso8601Format BasicFormat_NoMillis_FullTz = new(Iso8601Parts.Format_BasicFormat_NoFractional_FullTz, 20, 0);
		/// <summary>
		/// yyyyMMddTHHmmss
		/// </summary>
		public static readonly Iso8601Format BasicFormat_NoMillis_LocalTz = new(Iso8601Parts.Format_BasicFormat_NoFractional_LocalTz, 15, 0);
		/// <summary>
		/// yyyy-MM-dd
		/// </summary>
		public static readonly Iso8601Format DateOnly = new(Iso8601Parts.Format_DateOnly, 10, 0);
		/// <summary>
		/// yyyyMMdd
		/// </summary>
		public static readonly Iso8601Format DateOnlyWithoutSeparators = new(Iso8601Parts.Format_DateOnlyWithoutSeparators, 8, 0);
		/// <summary>
		/// yyyy-ddd
		/// </summary>
		public static readonly Iso8601Format DateOrdinal = new(Iso8601Parts.Format_DateOrdinal, 8, 0);
		private Iso8601Format(Iso8601Parts format, int length, int decimalPlaces)
		{
			Format = format;
			LengthRequired = length;
			DecimalPlaces = decimalPlaces;
		}
		/// <summary>
		/// The parts that make up this format.
		/// </summary>
		public Iso8601Parts Format { get; }
		/// <summary>
		/// The total length, in characters, that the format will produce.
		/// Any Spans passed in must be at least this size.
		/// </summary>
		public int LengthRequired { get; }
		/// <summary>
		/// The number of decimal places for the fractional part, not counting the dot.
		/// </summary>
		public int DecimalPlaces { get; }
		/// <summary>
		/// Returns an <see cref="Iso8601Format"/> from the options provided.
		/// </summary>
		/// <param name="tz">What kind of timezone to use.</param>
		/// <param name="extended">If true, dashes and colons are used to separate date/time/timezone. If false, no separators are used.</param>
		/// <param name="decimalPlaces">How many decimal places to write, or none. If less than 0, throws <see cref="ArgumentOutOfRangeException"/>.</param>
		/// <returns>An <see cref="Iso8601Format"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="decimalPlaces"/> is less than 0.</exception>
		public static Iso8601Format GetFormat(TimezoneType tz = TimezoneType.Utc, bool extended = true, int decimalPlaces = 0)
		{
			if (decimalPlaces < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places cannot be less than zero");
			}
			int lengthForMillis = decimalPlaces == 0 ? 0 : decimalPlaces + 1;
			switch (tz)
			{
				default:
				case TimezoneType.Utc:
					// +1 for the timezone, just Z
					return extended
						? new(Iso8601Parts.Format_ExtendedFormat_UtcTz, ExtendedFormat_BaseLength + lengthForMillis + 1, decimalPlaces)
						: new(Iso8601Parts.Format_BasicFormat_UtcTz, BasicFormat_BaseLength + lengthForMillis + 1, decimalPlaces);
				case TimezoneType.Full:
					// +6 for the timezone if it is extended, +5 otherwise
					// +00:00 vs +0000
					int tzLen = extended ? 6 : 5;
					return extended
						? new(Iso8601Parts.Format_ExtendedFormat_FullTz, ExtendedFormat_BaseLength + lengthForMillis + tzLen, decimalPlaces)
						: new(Iso8601Parts.Format_BasicFormat_FullTz, BasicFormat_BaseLength + lengthForMillis + tzLen, decimalPlaces);
				case TimezoneType.Local:
					// +0 for the timezone
					return extended
						? new(Iso8601Parts.Format_ExtendedFormat_LocalTz, ExtendedFormat_BaseLength + lengthForMillis, decimalPlaces)
						: new(Iso8601Parts.Format_BasicFormat_LocalTz, BasicFormat_BaseLength + lengthForMillis, decimalPlaces);
			}
		}
		/// <summary>
		/// Returns the length of the string that will be created if a <see cref="UtcDateTime"/> is formatted using <paramref name="format"/>.
		/// Or if <paramref name="format"/> is not a valid format, returns that error message.
		/// The default value for <paramref name="format"/> is the same as <see cref="Iso8601Parts.Format_ExtendedFormat_UtcTz"/>.
		/// Note there are also constant lengths exposed on this class for predefined formats, saving a call to this function.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="decimalPlaces">The number of decimal places for the fractional part.</param>
		/// <returns>Length on success, or an error message on failure.</returns>
		public static Maybe<Iso8601Format, string> TryCreate(Iso8601Parts format, int decimalPlaces = 3)
		{
			// Commonly used formats, known to be valid
			// But they only use 3 decimal places
			if (decimalPlaces == 3)
			{
				switch (format)
				{
					case Iso8601Parts.None:
					case Iso8601Parts.Format_ExtendedFormat_UtcTz: return ExtendedFormat_UtcTz;
					case Iso8601Parts.Format_ExtendedFormat_FullTz: return ExtendedFormat_FullTz;
					case Iso8601Parts.Format_ExtendedFormat_LocalTz: return ExtendedFormat_LocalTz;
					case Iso8601Parts.Format_ExtendedFormat_NoFractional_UtcTz: return ExtendedFormat_NoMillis_UtcTz;
					case Iso8601Parts.Format_ExtendedFormat_NoFractional_FullTz: return ExtendedFormat_NoMillis_FullTz;
					case Iso8601Parts.Format_ExtendedFormat_NoFractional_LocalTz: return ExtendedFormat_NoMillis_LocalTz;
					case Iso8601Parts.Format_BasicFormat_UtcTz: return BasicFormat_UtcTz;
					case Iso8601Parts.Format_BasicFormat_FullTz: return BasicFormat_FullTz;
					case Iso8601Parts.Format_BasicFormat_LocalTz: return BasicFormat_LocalTz;
					case Iso8601Parts.Format_BasicFormat_NoFractional_UtcTz: return BasicFormat_NoMillis_UtcTz;
					case Iso8601Parts.Format_BasicFormat_NoFractional_FullTz: return BasicFormat_NoMillis_FullTz;
					case Iso8601Parts.Format_BasicFormat_NoFractional_LocalTz: return BasicFormat_NoMillis_LocalTz;
					case Iso8601Parts.Format_DateOnly: return DateOnly;
					case Iso8601Parts.Format_DateOnlyWithoutSeparators: return DateOnlyWithoutSeparators;
					case Iso8601Parts.Format_DateOrdinal: return DateOrdinal;
					default:
						break;
				}
			}

			if (format == Iso8601Parts.None)
			{
				return "Nothing was provided for the format";
			}
			int length;
			Iso8601Parts d = format & Iso8601Parts.Mask_Date;
			Iso8601Parts t = format & Iso8601Parts.Mask_Time;
			Iso8601Parts tz = format & Iso8601Parts.Mask_Tz;
			if (d == 0 && t == 0)
			{
				return "At least a date and time are required";
			}
			if (tz != 0 && t == 0)
			{
				return "If a timezone is specified, a time is required";
			}
			bool dateSep = (format & Iso8601Parts.Separator_Date) == Iso8601Parts.Separator_Date;
			switch (d)
			{
				case Iso8601Parts.None:
					length = 0;
					break;
				case Iso8601Parts.YearDay: // yyyy-ddd
					length = dateSep ? 8 : 7;
					break;
				case Iso8601Parts.YearMonthDay: // yyyy-MM-dd
					length = dateSep ? 10 : 8;
					break;
				case Iso8601Parts.YearWeek: // yyyy-Www
					length = dateSep ? 8 : 7;
					break;
				case Iso8601Parts.YearWeekDay: // yyyy-Www-d
					length = dateSep ? 10 : 8;
					break;
				case Iso8601Parts.YearMonth: // yyyy-MM
					if (!dateSep)
					{
						return "Writing year and month only without separators is not valid; it can be confused with yyMMdd";
					}
					length = 7;
					break;
				case Iso8601Parts.Year:
					return "The provided format for the date portion needs to specify more than just a year";
				default:
					return "The provided format for the date portion is not valid";
			}
			bool timeSep = (format & Iso8601Parts.Separator_Time) == Iso8601Parts.Separator_Time;
			switch (t)
			{
				case Iso8601Parts.None:
					break;
				case Iso8601Parts.Hour: // HH
					length += 2;
					break;
				case Iso8601Parts.HourMinute: // HH:mm or HHMM
					length += timeSep ? 5 : 4;
					break;
				case Iso8601Parts.HourMinuteSecond: // HH:mm:ss or HHmmss
					length += timeSep ? 8 : 6;
					break;
				case Iso8601Parts.HourMinuteSecondFractional: // HH:mm:ss.fff or HHmmss.fff
					if (decimalPlaces < 1)
					{
						return "Decimal places must be at least 1 if fractions of a second are being written";
					}
					length += (timeSep ? 9 : 7) + decimalPlaces;
					break;
				default:
					return "The provided format for the time portion is not valid";
			}
			switch (tz)
			{
				case Iso8601Parts.None:
					break;
				case Iso8601Parts.Tz_Utc:
					length += 1;
					break;
				case Iso8601Parts.Tz_Hour:
					length += 3;
					break;
				case Iso8601Parts.Tz_HourMinute:
					length += (format & Iso8601Parts.Separator_Tz) == Iso8601Parts.Separator_Tz ? 6 : 5;
					break;
				case Iso8601Parts.Tz_Minute:
					return "Timezone designator can't be just minutes; it needs hours and minutes";
				default:
					return "The provided format for the timezone designator is not valid";
			}
			// If we have a time, add 1 length for the T
			if (t != 0)
			{
				length++;
			}

			return new Iso8601Format(format, length, decimalPlaces);
		}
		/// <summary>
		/// Creates a new string containing date and time represented by <paramref name="ticks"/>.
		/// </summary>
		/// <param name="ticks">The ticks.</param>
		/// <param name="timezone">For non-UTC timezone designators or a local designator, writes the time with this offset. Null means the <see cref="TimeZoneInfo.BaseUtcOffset"/> of <see cref="TimeZoneInfo.Local"/>. If using UTC timezone designator this is ignored.</param>
		public string CreateString(long ticks, TimeSpan? timezone = null)
		{
#if !NETSTANDARD2_0
			return string.Create(LengthRequired, this, (dest, inst) => inst.WriteString(dest, ticks, timezone));
#else
			return Compat.StringCreate(LengthRequired, this, (dest, inst) => inst.WriteString(dest, ticks, timezone));
#endif
		}
		/// <summary>
		/// Writes to <paramref name="destination"/> a string containing the date and time represented by <paramref name="ticks"/>.
		/// If successful, returns the number of chars written.
		/// If <paramref name="destination"/> is too small, returns 0.
		/// </summary>
		/// <param name="destination">The destination to write to.</param>
		/// <param name="ticks">The ticks.</param>
		/// <param name="timezone">For non-UTC timezone designators or a local designator, writes the time with this offset. Null means the <see cref="TimeZoneInfo.BaseUtcOffset"/> of <see cref="TimeZoneInfo.Local"/>. If using UTC timezone designator this is ignored.</param>
		/// <returns>The number of chars written, or 0 if <paramref name="destination"/> is too small.</returns>
		public int WriteString(Span<char> destination, long ticks, TimeSpan? timezone = null)
		{
			if (destination.Length < LengthRequired)
			{
				return 0;
			}
			switch (Format)
			{
				// This fall-through is intentional; when format is default, then Iso8601Parts will be None, and that corresponds to extended format, UTC.
				case Iso8601Parts.None:
				case Iso8601Parts.Format_ExtendedFormat_UtcTz: return FormatExtendedFormatUtc(destination, ticks, DecimalPlaces);
				case Iso8601Parts.Format_ExtendedFormat_NoFractional_UtcTz: return FormatExtendedFormatUtc(destination, ticks, DecimalPlaces);
				case Iso8601Parts.Format_BasicFormat_UtcTz: return FormatBasicFormatUtc(destination, ticks, DecimalPlaces);
				case Iso8601Parts.Format_BasicFormat_NoFractional_UtcTz: return FormatBasicFormatUtc(destination, ticks, DecimalPlaces);
				default:
					break;
			}

			bool seps = (Format & Iso8601Parts.Separator_Date) == Iso8601Parts.Separator_Date;

			Iso8601Parts ftz = Format & Iso8601Parts.Mask_Tz;
			// When we aren't writing the time, and timezone designator is absent, we still offset to the local timezone,
			// which can cause the date to change. The thing is, when we write just the Date that technically causes data loss.

			// If we're writing a UTC Timezone designator, timezone is meaningless and our offset is 0
			// If we're writing unqualified or hour/min, we need to take timezone into account. So we'll either write the value of timezone later, or just assume that it was local
			TimeSpan tz;
			long tzOffsetTicks;
			if (ftz == Iso8601Parts.Tz_Utc)
			{
				tzOffsetTicks = 0;
				tz = TimeSpan.Zero;
			}
			else
			{
				tz = timezone ?? TimeZoneInfo.Local.BaseUtcOffset;
				tzOffsetTicks = tz.Ticks;
			}

			// We clamp the milliseconds so they can't be larger than MaxMillis or lower than zero.
			long ticksWithOffset = ticks + tzOffsetTicks;
			ticks = ticksWithOffset > UtcDateTime.MaxTicks
				? UtcDateTime.MaxTicks
				: ticksWithOffset < 0
					? 0
					: ticksWithOffset;
			UtcDateTime.DateTimePartsNoMillisFromTicks(ticks, out int year, out int month, out int day, out int hour, out int minute, out int second, out int frac);

			int i = 0;

			// Weeks have to be handled differently, because 2019-12-30 for example is actually written 2020-W01-1.
			if ((Format & Iso8601Parts.YearWeek) == Iso8601Parts.YearWeek)
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
				if ((Format & Iso8601Parts.Day) == Iso8601Parts.Day)
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
				if ((Format & Iso8601Parts.Year) == Iso8601Parts.Year)
				{
					Formatting.Write4Digits((uint)year, destination, 0);
					i += 4;
					if (seps)
					{
						destination[i++] = '-';
					}
				}
				if ((Format & Iso8601Parts.Month) == 0 && ((Format & Iso8601Parts.Day) == Iso8601Parts.Day))
				{
					// Month and no Day is the ordinal format; we need to turn months into days and add that together with day to get the number to write
					int ordinalDay = (DateTime.IsLeapYear(year) ? UtcDateTime.TotalDaysFromStartLeapYearToMonth : UtcDateTime.TotalDaysFromStartYearToMonth)[month - 1] + day;
					Formatting.Write3Digits((uint)ordinalDay, destination, i);
					i += 3;
				}
				else
				{
					if ((Format & Iso8601Parts.Month) == Iso8601Parts.Month)
					{
						Formatting.Write2Digits((uint)month, destination, i);
						i += 2;
					}
					if ((Format & Iso8601Parts.Day) == Iso8601Parts.Day)
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

			if ((Format & Iso8601Parts.Mask_Time) != 0)
			{
				seps = (Format & Iso8601Parts.Separator_Time) == Iso8601Parts.Separator_Time;
				// Always have to write T, even if no date was specified
				destination[i++] = 'T';
				if ((Format & Iso8601Parts.Hour) == Iso8601Parts.Hour)
				{
					Formatting.Write2Digits((uint)hour, destination, i);
					i += 2;
				}
				if ((Format & Iso8601Parts.Minute) == Iso8601Parts.Minute)
				{
					if (seps)
					{
						destination[i++] = ':';
					}
					Formatting.Write2Digits((uint)minute, destination, i);
					i += 2;
				}
				if ((Format & Iso8601Parts.Second) == Iso8601Parts.Second)
				{
					if (seps)
					{
						destination[i++] = ':';
					}
					Formatting.Write2Digits((uint)second, destination, i);
					i += 2;
				}
				if ((Format & Iso8601Parts.Fractional) == Iso8601Parts.Fractional)
				{
					i += WriteDecimalPlaces(destination, (uint)frac, DecimalPlaces, i);
				}
			}

			switch (ftz)
			{
				case Iso8601Parts.Tz_Utc:
					destination[i++] = 'Z';
					break;
				case Iso8601Parts.Tz_Hour:
					destination[i++] = tzOffsetTicks >= 0 ? '+' : '-';
					int tzi = Math.Abs(tz.Hours);
					Formatting.Write2Digits((uint)tzi, destination, i);
					i += 2;
					break;
				case Iso8601Parts.Tz_HourMinute:
					destination[i++] = tzOffsetTicks >= 0 ? '+' : '-';
					tzi = Math.Abs(tz.Hours);
					Formatting.Write2Digits((uint)tzi, destination, i);
					i += 2;
					if ((Format & Iso8601Parts.Separator_Tz) == Iso8601Parts.Separator_Tz)
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
		/// <param name="destination">The destination to write to. Must be 16 long if <paramref name="decimalPlaces"/> is 0, or 17 + <paramref name="decimalPlaces"/> otherwise.</param>
		/// <param name="ticks">The ticks.</param>
		/// <param name="decimalPlaces">The number of decimal places to include.</param>
		/// <returns>The number of chars written, or if <paramref name="destination"/> is too small, the space required as a negative number.</returns>
		public static int FormatBasicFormatUtc(Span<char> destination, long ticks, int decimalPlaces)
		{
			if (decimalPlaces < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places cannot be less than zero");
			}
			// +1 for the Z, and then the decimal places
			int len = BasicFormat_BaseLength + 1 + (decimalPlaces == 0 ? 0 : decimalPlaces + 1);
			if (destination.Length < len)
			{
				return -len;
			}
			{ _ = destination[len - 1]; }
			// yyyyMMddTHHmmss.sssZ
			// 01234567890123456789

			UtcDateTime.DateTimePartsNoMillisFromTicks(ticks, out int year, out int month, out int day, out int hour, out int minute, out int second, out int frac);
			Formatting.Write4Digits((uint)year, destination, 0);
			Formatting.Write2Digits((uint)month, destination, 4);
			Formatting.Write2Digits((uint)day, destination, 6);
			destination[8] = 'T';
			Formatting.Write2Digits((uint)hour, destination, 9);
			Formatting.Write2Digits((uint)minute, destination, 11);
			Formatting.Write2Digits((uint)second, destination, 13);
			int i = 15 + WriteDecimalPlaces(destination, (uint)frac, decimalPlaces, 15);
			destination[i] = 'Z';
			return len;
		}
		/// <summary>
		/// An optimized method for writing using the format <see cref="Iso8601Parts.Format_ExtendedFormat_UtcTz"/>, with or without millseconds.
		/// </summary>
		/// <param name="destination">The destination to write to. Must be 16 long if <paramref name="decimalPlaces"/> is 0, or 17 + <paramref name="decimalPlaces"/> otherwise.</param>
		/// <param name="ticks">The ticks.</param>
		/// <param name="decimalPlaces">The number of decimal places to include.</param>
		/// <returns>The number of chars written, or if <paramref name="destination"/> is too small, the space required as a negative number.</returns>
		public static int FormatExtendedFormatUtc(Span<char> destination, long ticks, int decimalPlaces)
		{
			if (decimalPlaces < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places cannot be less than zero");
			}
			// +1 for the Z, and then the decimal places
			int len = ExtendedFormat_BaseLength + 1 + (decimalPlaces <= 0 ? 0 : decimalPlaces + 1);
			if (destination.Length < len)
			{
				return -len;
			}
			{ _ = destination[len - 1]; }
			// yyyy-MM-ddTHH:mm:ss.sssZ
			// 012345678901234567890123
			// yyyy-MM-ddTHH:mm:ssZ

			UtcDateTime.DateTimePartsNoMillisFromTicks(ticks, out int year, out int month, out int day, out int hour, out int minute, out int second, out int frac);
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
			int i = 19 + WriteDecimalPlaces(destination, (uint)frac, decimalPlaces, 19);
			destination[i] = 'Z';
			return len;
		}
		/// <summary>
		/// Writes the decimal point and any further digits
		/// </summary>
		private static int WriteDecimalPlaces(Span<char> destination, uint ticks, int decimalPlaces, int offset)
		{
			uint fractionalUnits;
			switch (decimalPlaces)
			{
				default:
					// Anything larger and we just pad with zeroes
					fractionalUnits = ticks;
					destination[offset++] = '.';
					Formatting.Write7Digits(fractionalUnits, destination, offset);
					offset += 7;
					for (int f = 7; f < decimalPlaces; f++)
					{
						destination[offset++] = '0';
					}
					break;
				case 7:
					fractionalUnits = ticks;
					destination[offset++] = '.';
					Formatting.Write7Digits(fractionalUnits, destination, offset);
					break;
				case 6:
					fractionalUnits = ticks / 10;
					destination[offset++] = '.';
					Formatting.Write6Digits(fractionalUnits, destination, offset);
					break;
				case 5:
					fractionalUnits = ticks / 100;
					destination[offset++] = '.';
					Formatting.Write5Digits(fractionalUnits, destination, offset);
					break;
				case 4:
					fractionalUnits = ticks / 1_000;
					destination[offset++] = '.';
					Formatting.Write4Digits(fractionalUnits, destination, offset);
					break;
				case 3:
					fractionalUnits = ticks / 10_000;
					destination[offset++] = '.';
					Formatting.Write3Digits(fractionalUnits, destination, offset);
					break;
				case 2:
					fractionalUnits = ticks / 100_000;
					destination[offset++] = '.';
					Formatting.Write2Digits(fractionalUnits, destination, offset);
					break;
				case 1:
					fractionalUnits = ticks / 1_000_000;
					destination[offset++] = '.';
					destination[offset] = (char)('0' + fractionalUnits);
					break;
				case 0:
					return 0;
			}
			return decimalPlaces + 1;
		}
	}
}
