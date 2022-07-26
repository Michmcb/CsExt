namespace MichMcb.CsExt.Test.Dates.Iso8601Format
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class CreateWriteString
	{
		[Fact]
		public static void BadLength()
		{
			Span<char> str = stackalloc char[5];
			Assert.Equal(-Iso8601Format.BasicFormat_NoFractional_UtcTz.LengthRequired, Iso8601Format.BasicFormat_NoFractional_UtcTz.WriteString(str, 1, null));
		}
		[Fact]
		public static void ClampedTicks()
		{
			Test("9999-12-31T23:59:59.999+10:00", Iso8601Format.ExtendedFormat_FullTz, long.MaxValue, Tz.TryCreate(10, 0).ValueOrException());
			Test("0001-01-01T00:00:00.000-10:00", Iso8601Format.ExtendedFormat_FullTz, long.MinValue, Tz.TryCreate(-10, 0).ValueOrException());
		}
		[Fact]
		public static void DateTimeOffsetStr()
		{
			Iso8601Format fmt = Iso8601Format.ExtendedFormat_NoFractional_FullTz;
			Assert.Equal("2010-04-19T02:32:11+00:00", fmt.CreateString(new DateTimeOffset(2010, 4, 19, 2, 32, 11, TimeSpan.Zero)));
			Assert.Equal("2010-04-19T12:32:11+10:00", fmt.CreateString(new DateTimeOffset(2010, 4, 19, 12, 32, 11, new TimeSpan(10, 0, 0))));
		}

		private static void Test(string expected, Iso8601Format fmt, long ticks, Tz tz)
		{
			Span<char> str = stackalloc char[fmt.LengthRequired];
			Assert.Equal(fmt.LengthRequired, fmt.WriteString(str, ticks, tz));
			Assert.Equal(expected, new string(str));
		}
	}
}
