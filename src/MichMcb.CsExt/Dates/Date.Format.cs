namespace MichMcb.CsExt.Dates
{
	using System;

	public readonly partial struct Date
	{
		/// <summary>
		/// Parses an ISO-8601 string as a Date.
		/// Any leading or trailing whitespace is ignored.
		/// </summary>
		/// <param name="str">The string to parse</param>
		/// <param name="disallowTime">If true, rejects any strings which have time components. Otherwise, just ignores the time component. Date is not adjusted in any way by the time component.</param>
		/// <returns>A Date if parsing was successful, or an error message otherwise.</returns>
		public static Maybe<Date, string> TryParseIso8601String(in ReadOnlySpan<char> str, bool disallowTime = true)
		{
			ReadOnlySpan<char> ts = str.Trim();
			if (!LexedIso8601.LexIso8601(ts).Success(out LexedIso8601 luthor, out string errMsg))
			{
				return errMsg;
			}
			// If we are only allowing the date, then we need to check to make sure the only parts we found were the date and separators
			if (disallowTime && (((Iso8601Parts.Mask_Date | Iso8601Parts.Mask_Separators) & luthor.PartsFound) != luthor.PartsFound))
			{
#if !NETSTANDARD2_0
				return string.Concat("Was expecting only the date component of an ISO-8601, but it has time: ", str);
#else
				return Shim.StringConcat("Was expecting only the date component of an ISO-8601, but it has time: ".AsSpan(), str);
#endif
			}

			luthor.Parse(ts, out int year, out int month, out int day, out _, out _, out _, out _, out _, out _);

			ArgumentOutOfRangeException? ex;
			if ((luthor.PartsFound & Iso8601Parts.Mask_Date) == Iso8601Parts.YearDay)
			{
				ex = DateUtil.TotalDaysFromParts_OrdinalDays(year, day, out int days);
				if (ex == null)
				{
					return new Date(days);
				}
			}
			else
			{
				ex = DateUtil.TotalDaysFromParts(year, month, day, out int days);
				if (ex == null)
				{
					return new Date(days);
				}
			}
			return ex.Message;
		}
		/// <summary>
		/// Formats this instance as an ISO-8601 string using Extended Format with UTC as the Timezone Designator (<see cref="Iso8601Parts.Format_DateOnly"/>).
		/// e.g. 2010-12-30
		/// </summary>
		/// <returns>An ISO-8601 representing this Date</returns>
		public override string ToString()
		{
#if !NETSTANDARD2_0
			return string.Create(DateUtil.Length_Format_DateOnly, this, (dest, inst) => inst.FormatExtendedFormat(dest));
#else
			return Shim.StringCreate(DateUtil.Length_Format_DateOnly, this, (dest, inst) => inst.FormatExtendedFormat(dest));
#endif
		}
		/// <summary>
		/// Formats this istnance as an ISO-8601 string using Extended Format.
		/// </summary>
		/// <returns>An ISO-8601 representing this Date</returns>
		public string ToIso8601StringExtended()
		{
#if !NETSTANDARD2_0
			return string.Create(DateUtil.Length_Format_DateOnly, this, (dest, inst) => inst.FormatExtendedFormat(dest));
#else
			return Shim.StringCreate(DateUtil.Length_Format_DateOnly, this, (dest, inst) => inst.FormatExtendedFormat(dest));
#endif
		}
		/// <summary>
		/// Formats this istnance as an ISO-8601 string using Basic Format.
		/// </summary>
		/// <returns>An ISO-8601 representing this Date</returns>
		public string ToIso8601StringBasic()
		{
#if !NETSTANDARD2_0
			return string.Create(DateUtil.Length_Format_DateOnlyWithoutSeparators, this, (dest, inst) => inst.FormatBasicFormat(dest));
#else
			return Shim.StringCreate(DateUtil.Length_Format_DateOnlyWithoutSeparators, this, (dest, inst) => inst.FormatBasicFormat(dest));
#endif
		}
		/// <summary>
		/// Formats this Date to <paramref name="destination"/> as an ISO-8601 string, according to the rules specified by <paramref name="format"/>.
		/// </summary>
		/// <param name="destination">The destination to write to. If this doesn't have the length required to hold the resultant string, returns an error.</param>
		/// <param name="format">How to format the string. By default, this is ISO-8601 extended, date only. You shouldn't specify anything except for date/separators.</param>
		/// <returns>The numbers of chars written, or an error message.</returns>
		public Maybe<int, string> TryFormat(Span<char> destination, Iso8601Parts format = Iso8601Parts.Format_DateOnly)
		{
			if (!DateUtil.TryGetLengthRequired(format).Success(out int len, out string? errMsg))
			{
				return errMsg;
			}
			if (destination.Length < len)
			{
				return string.Concat("Destination span is too small. Required length is ", len, " but the destination span length is only ", destination.Length);
			}
			return FormatUnchecked(destination, format);
		}
		private int FormatUnchecked(Span<char> destination, Iso8601Parts format = Iso8601Parts.Format_ExtendedFormat_UtcTz)
		{
			switch (format)
			{
				case Iso8601Parts.Format_DateOnly: return FormatExtendedFormat(destination);
				case Iso8601Parts.Format_DateOnlyWithoutSeparators: return FormatBasicFormat(destination);
				default:
					break;
			}

			bool seps = (format & Iso8601Parts.Separator_Date) == Iso8601Parts.Separator_Date;
			DateUtil.CalcDateParts(TotalDays, out int year, out int month, out int day);

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

			return i;
		}
		/// <summary>
		/// An optimized method for writing using the format <see cref="Iso8601Parts.Format_DateOnly"/>.
		/// </summary>
		/// <param name="destination">The destination to write to. Must be 10 long.</param>
		/// <returns>The number of chars written (always 10), or 0 if <paramref name="destination"/> is too small.</returns>
		public int FormatExtendedFormat(Span<char> destination)
		{
			int len = DateUtil.Length_Format_DateOnly;
			if (destination.Length < len)
			{
				return 0;
			}
			{ _ = destination[len - 1]; }
			// yyyy-MM-dd
			// 0123456789

			DateUtil.CalcDateParts(TotalDays, out int year, out int month, out int day);
			Formatting.Write4Digits((uint)year, destination, 0);
			destination[4] = '-';
			Formatting.Write2Digits((uint)month, destination, 5);
			destination[7] = '-';
			Formatting.Write2Digits((uint)day, destination, 8);
			return len;
		}
		/// <summary>
		/// An optimized method for writing using the format <see cref="Iso8601Parts.Format_DateOnlyWithoutSeparators"/>.
		/// </summary>
		/// <param name="destination">The destination to write to. Must be 8 long.</param>
		/// <returns>The number of chars written (always 8), or 0 if <paramref name="destination"/> is too small.</returns>
		public int FormatBasicFormat(Span<char> destination)
		{
			int len = DateUtil.Length_Format_DateOnlyWithoutSeparators;
			if (destination.Length < len)
			{
				return 0;
			}
			{ _ = destination[len - 1]; }
			// yyyyMMdd
			// 01234567

			DateUtil.CalcDateParts(TotalDays, out int year, out int month, out int day);
			Formatting.Write4Digits((uint)year, destination, 0);
			Formatting.Write2Digits((uint)month, destination, 4);
			Formatting.Write2Digits((uint)day, destination, 6);
			return len;
		}
	}
}