namespace MichMcb.CsExt.Dates
{
	/// <summary>
	/// A validated format for ISO-8601 strings. To create one of these you need to use <see cref="TryCreate(Iso8601Parts)"/>,
	/// or use one of the predefined ones such as 
	/// </summary>
	public readonly struct Iso8601Format
	{
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss.sssZ
		/// A good default format. This conforms to RFC3339
		/// This is known in ISO-8601 as "Extended Format".
		/// </summary>
		public static readonly Iso8601Format ExtendedFormat_UtcTz = new(Iso8601Parts.Format_ExtendedFormat_UtcTz, 24);
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss.sss+00:00
		/// This conforms to RFC3339.
		/// </summary>
		public static readonly Iso8601Format ExtendedFormat_FullTz = new(Iso8601Parts.Format_ExtendedFormat_FullTz, 29);
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss.sss
		/// </summary>
		public static readonly Iso8601Format ExtendedFormat_LocalTz = new(Iso8601Parts.Format_ExtendedFormat_LocalTz, 23);
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ssZ
		/// </summary>
		public static readonly Iso8601Format ExtendedFormat_NoMillis_UtcTz = new(Iso8601Parts.Format_ExtendedFormat_NoMillis_UtcTz, 20);
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss+00:00
		/// </summary>
		public static readonly Iso8601Format ExtendedFormat_NoMillis_FullTz = new(Iso8601Parts.Format_ExtendedFormat_NoMillis_FullTz, 25);
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss
		/// </summary>
		public static readonly Iso8601Format ExtendedFormat_NoMillis_LocalTz = new(Iso8601Parts.Format_ExtendedFormat_NoMillis_LocalTz, 19);
		/// <summary>
		/// yyyyMMddTHHmmss.sssZ
		/// Everything, except without separators
		/// This is known in ISO-8601 as "Basic Format"
		/// </summary>
		public static readonly Iso8601Format BasicFormat_UtcTz = new(Iso8601Parts.Format_BasicFormat_UtcTz, 20);
		/// <summary>
		/// yyyyMMddTHHmmss.sss+0000
		/// </summary>
		public static readonly Iso8601Format BasicFormat_FullTz = new(Iso8601Parts.Format_BasicFormat_FullTz, 24);
		/// <summary>
		/// yyyyMMddTHHmmss.sss
		/// </summary>
		public static readonly Iso8601Format BasicFormat_LocalTz = new(Iso8601Parts.Format_BasicFormat_LocalTz, 19);
		/// <summary>
		/// yyyyMMddTHHmmssZ
		/// </summary>
		public static readonly Iso8601Format BasicFormat_NoMillis_UtcTz = new(Iso8601Parts.Format_BasicFormat_NoMillis_UtcTz, 16);
		/// <summary>
		/// yyyyMMddTHHmmss+0000
		/// </summary>
		public static readonly Iso8601Format BasicFormat_NoMillis_FullTz = new(Iso8601Parts.Format_BasicFormat_NoMillis_FullTz, 20);
		/// <summary>
		/// yyyyMMddTHHmmss
		/// </summary>
		public static readonly Iso8601Format BasicFormat_NoMillis_LocalTz = new(Iso8601Parts.Format_BasicFormat_NoMillis_LocalTz, 15);
		/// <summary>
		/// yyyy-MM-dd
		/// </summary>
		public static readonly Iso8601Format DateOnly = new(Iso8601Parts.Format_DateOnly, 10);
		/// <summary>
		/// yyyyMMdd
		/// </summary>
		public static readonly Iso8601Format DateOnlyWithoutSeparators = new(Iso8601Parts.Format_DateOnlyWithoutSeparators, 8);
		/// <summary>
		/// yyyy-ddd
		/// </summary>
		public static readonly Iso8601Format DateOrdinal = new(Iso8601Parts.Format_DateOrdinal, 8);
		private Iso8601Format(Iso8601Parts format, int length)
		{
			Format = format;
			Length = length;
		}
		/// <summary>
		/// The parts that make up this format.
		/// </summary>
		public Iso8601Parts Format { get; }
		/// <summary>
		/// The total length, in characters, that the format can produce.
		/// </summary>
		public int Length { get; }
		/// <summary>
		/// Returns the length of the string that will be created if a <see cref="UtcDateTime"/> is formatted using <paramref name="format"/>.
		/// Or if <paramref name="format"/> is not a valid format, returns that error message.
		/// Note there are also constant lengths exposed on this class for predefined formats, saving a call to this function.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <returns>Length on success, or an error message on failure.</returns>
		public static Maybe<Iso8601Format, string> TryCreate(Iso8601Parts format)
		{
			// Commonly used formats, known to be valid
			switch (format)
			{
				case Iso8601Parts.Format_ExtendedFormat_UtcTz: return ExtendedFormat_UtcTz;
				case Iso8601Parts.Format_ExtendedFormat_FullTz: return ExtendedFormat_FullTz;
				case Iso8601Parts.Format_ExtendedFormat_LocalTz: return ExtendedFormat_LocalTz;
				case Iso8601Parts.Format_ExtendedFormat_NoMillis_UtcTz: return ExtendedFormat_NoMillis_UtcTz;
				case Iso8601Parts.Format_ExtendedFormat_NoMillis_FullTz: return ExtendedFormat_NoMillis_FullTz;
				case Iso8601Parts.Format_ExtendedFormat_NoMillis_LocalTz: return ExtendedFormat_NoMillis_LocalTz;
				case Iso8601Parts.Format_BasicFormat_UtcTz: return BasicFormat_UtcTz;
				case Iso8601Parts.Format_BasicFormat_FullTz: return BasicFormat_FullTz;
				case Iso8601Parts.Format_BasicFormat_LocalTz: return BasicFormat_LocalTz;
				case Iso8601Parts.Format_BasicFormat_NoMillis_UtcTz: return BasicFormat_NoMillis_UtcTz;
				case Iso8601Parts.Format_BasicFormat_NoMillis_FullTz: return BasicFormat_NoMillis_FullTz;
				case Iso8601Parts.Format_BasicFormat_NoMillis_LocalTz: return BasicFormat_NoMillis_LocalTz;
				case Iso8601Parts.Format_DateOnly: return DateOnly;
				case Iso8601Parts.Format_DateOnlyWithoutSeparators: return DateOnlyWithoutSeparators;
				case Iso8601Parts.Format_DateOrdinal: return DateOrdinal;
				default:
					break;
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
				case Iso8601Parts.HourMinute: // HH:mm
					length += timeSep ? 5 : 4;
					break;
				case Iso8601Parts.HourMinuteSecond: // HH:mm:ss
					length += timeSep ? 8 : 6;
					break;
				case Iso8601Parts.HourMinuteSecondMillis: // HH:mm:ss.fff
					length += timeSep ? 12 : 10;
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

			return new Iso8601Format(format, length);
		}
	}
}
