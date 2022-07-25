namespace MichMcb.CsExt.Test.Dates.Iso8601Format
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class WriteString
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
			Iso8601Format fmt = Iso8601Format.ExtendedFormat_FullTz;
			Span<char> str = stackalloc char[fmt.LengthRequired];
			Assert.Equal(fmt.LengthRequired, fmt.WriteString(str, long.MaxValue, Tz.TryCreate(10, 0).ValueOrException()));
			Assert.Equal("9999-12-31T23:59:59.999+10:00", new string(str));

			Assert.Equal(fmt.LengthRequired, fmt.WriteString(str, long.MinValue, Tz.TryCreate(-10, 0).ValueOrException()));
			Assert.Equal("0001-01-01T00:00:00.000-10:00", new string(str));
		}
	}
}
