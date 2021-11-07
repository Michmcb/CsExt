namespace MichMcb.CsExt.Test.Dates.Date
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;
	public static class Formatting
	{
		[Fact]
		public static void DefaultStringFormat()
		{
			Assert.Equal("2020-10-20", new Date(2020, 10, 20).ToString());
		}
		[Fact]
		public static void BasicFormat()
		{
			Span<char> str = stackalloc char[8];
			Assert.Equal(8, new Date(2020, 10, 20).FormatBasicFormat(str));
			Assert.Equal("20201020", new string(str));
			Assert.Equal("20201020", new Date(2020, 10, 20).ToIso8601StringBasic());
		}
		[Fact]
		public static void ExtendedFormat()
		{
			Span<char> str = stackalloc char[10];
			Assert.Equal(10, new Date(2020, 10, 20).FormatExtendedFormat(str));
			Assert.Equal("2020-10-20", new string(str));
			Assert.Equal("2020-10-20", new Date(2020, 10, 20).ToIso8601StringExtended());
		}
	}
}
