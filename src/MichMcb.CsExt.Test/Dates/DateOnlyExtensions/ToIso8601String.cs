namespace MichMcb.CsExt.Test.Dates.DateOnlyExtensions
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class ToIso8601String
	{
		[Fact]
		public static void Extended()
		{
			Assert.Equal("2010-04-01", new DateOnly(2010, 4, 1).ToIso8601String(extended: true));
		}
		[Fact]
		public static void Basic()
		{
			Assert.Equal("20100401", new DateOnly(2010, 4, 1).ToIso8601String(extended: false));
		}
	}
}
