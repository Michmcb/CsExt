namespace MichMcb.CsExt.Dates.Test.Dates
{
	using MichMcb.CsExt.Dates;
	using Xunit;

	public static class LengthRequired
	{
		[Fact]
		public static void CorrectLengths()
		{
			(Iso8601Parts format, int lengthExpected)[] formats = new[]
			{
				// Built-in ones
				(Iso8601Parts.Format_ExtendedFormat_UtcTz, "yyyy-MM-ddTHH:mm:ss.sssZ".Length),
				(Iso8601Parts.Format_ExtendedFormat_FullTz, "yyyy-MM-ddTHH:mm:ss.sss+00:00".Length),
				(Iso8601Parts.Format_ExtendedFormat_LocalTz, "yyyy-MM-ddTHH:mm:ss.sss".Length),
				(Iso8601Parts.Format_ExtendedFormat_NoMillis_UtcTz, "yyyy-MM-ddTHH:mm:ssZ".Length),
				(Iso8601Parts.Format_ExtendedFormat_NoMillis_FullTz, "yyyy-MM-ddTHH:mm:ss+00:00".Length),
				(Iso8601Parts.Format_ExtendedFormat_NoMillis_LocalTz, "yyyy-MM-ddTHH:mm:ss".Length),
				(Iso8601Parts.Format_BasicFormat_UtcTz, "yyyyMMddTHHmmss.sssZ".Length),
				(Iso8601Parts.Format_BasicFormat_FullTz, "yyyyMMddTHHmmss.sss+0000".Length),
				(Iso8601Parts.Format_BasicFormat_LocalTz, "yyyyMMddTHHmmss.sss".Length),
				(Iso8601Parts.Format_BasicFormat_NoMillis_UtcTz, "yyyyMMddTHHmmssZ".Length),
				(Iso8601Parts.Format_BasicFormat_NoMillis_FullTz, "yyyyMMddTHHmmss+0000".Length),
				(Iso8601Parts.Format_BasicFormat_NoMillis_LocalTz, "yyyyMMddTHHmmss".Length),
				(Iso8601Parts.Format_DateOnly, "yyyy-MM-dd".Length),
				(Iso8601Parts.Format_DateOnlyWithoutSeparators, "yyyyMMdd".Length),
				(Iso8601Parts.Format_DateOrdinal, "yyyy-ddd".Length),
				(Iso8601Parts.Format_VcfUnknownYear, "--MM-dd".Length),

				// Custom ones
				(Iso8601Parts.Separator_Date | Iso8601Parts.YearMonth,			"yyyy-MM".Length),

				(Iso8601Parts.YearDay,														"yyyyddd".Length),
				(Iso8601Parts.YearDay | Iso8601Parts.Hour,                     "yyyydddTHH".Length),
				(Iso8601Parts.YearDay | Iso8601Parts.HourMinute,               "yyyydddTHHmm".Length),
				(Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecond,         "yyyydddTHHmmss".Length),
				(Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecondMillis,   "yyyydddTHHmmss.sss".Length),
				(Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecondMillis,   "yyyydddTHHmmss.sss".Length),

				(Iso8601Parts.Separator_Date | Iso8601Parts.YearDay,															"yyyy-ddd".Length),
				(Iso8601Parts.Separator_Date | Iso8601Parts.YearDay | Iso8601Parts.Hour,								"yyyy-dddTHH".Length),
				(Iso8601Parts.Separator_Date | Iso8601Parts.YearDay | Iso8601Parts.HourMinute,						"yyyy-dddTHHmm".Length),
				(Iso8601Parts.Separator_Date | Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecond,				"yyyy-dddTHHmmss".Length),
				(Iso8601Parts.Separator_Date | Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecondMillis,		"yyyy-dddTHHmmss.sss".Length),

				(Iso8601Parts.Separator_All | Iso8601Parts.YearDay,															"yyyy-ddd".Length),
				(Iso8601Parts.Separator_All | Iso8601Parts.YearDay | Iso8601Parts.Hour,									"yyyy-dddTHH".Length),
				(Iso8601Parts.Separator_All | Iso8601Parts.YearDay | Iso8601Parts.HourMinute,							"yyyy-dddTHH:mm".Length),
				(Iso8601Parts.Separator_All | Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecond,					"yyyy-dddTHH:mm:ss".Length),
				(Iso8601Parts.Separator_All | Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecondMillis,			"yyyy-dddTHH:mm:ss.sss".Length),

				(Iso8601Parts.YearMonthDay,																							"yyyyMMdd".Length),
				(Iso8601Parts.YearMonthDay | Iso8601Parts.Hour,																	"yyyyMMddTHH".Length),
				(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute,															"yyyyMMddTHHmm".Length),
				(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond,													"yyyyMMddTHHmmss".Length),
				(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondMillis,											"yyyyMMddTHHmmss.sss".Length),

				(Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay,													"yyyy-MM-dd".Length),
				(Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay | Iso8601Parts.Hour,							"yyyy-MM-ddTHH".Length),
				(Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute,					"yyyy-MM-ddTHHmm".Length),
				(Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond,			"yyyy-MM-ddTHHmmss".Length),
				(Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondMillis,	"yyyy-MM-ddTHHmmss.sss".Length),

				(Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay,														"yyyy-MM-dd".Length),
				(Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay | Iso8601Parts.Hour,							"yyyy-MM-ddTHH".Length),
				(Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute,					"yyyy-MM-ddTHH:mm".Length),
				(Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond,			"yyyy-MM-ddTHH:mm:ss".Length),
				(Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondMillis,	"yyyy-MM-ddTHH:mm:ss.sss".Length),
			};
			foreach ((Iso8601Parts format, int lengthExpected) in formats)
			{
				Assert.Equal(lengthExpected, DateUtil.LengthRequired(format));
			}
		}
		[Fact]
		public static void InvalidStuff()
		{

		}
	}
}
