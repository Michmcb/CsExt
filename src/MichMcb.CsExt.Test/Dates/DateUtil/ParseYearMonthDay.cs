namespace MichMcb.CsExt.Test.Dates.DateUtil
{
	using System;
	using Xunit;
	using MichMcb.CsExt.Dates;

	public static class ParseYearMonthDay
	{
		[Fact]
		public static void Good()
		{
			Assert.Equal(new DateOnly(2010, 12, 1), DateUtil.ParseYearMonthDay("2010-12-01").ValueOrException());
			Assert.Equal(new DateOnly(2010, 12, 1), DateUtil.ParseYearMonthDay("20101201").ValueOrException());
		}
		[Fact]
		public static void Bad()
		{
			Assert.Equal("String was not either 10 or 8 chars long: 101201", DateUtil.ParseYearMonthDay("101201").ErrorOr(null));

			Assert.Equal("Found a non-latin digit in the string: 2x10", DateUtil.ParseYearMonthDay("2x10-12-01").ErrorOr(null));
			Assert.Equal("Found a non-latin digit in the string: x2", DateUtil.ParseYearMonthDay("2010-x2-01").ErrorOr(null));
			Assert.Equal("Found a non-latin digit in the string: 0x", DateUtil.ParseYearMonthDay("2010-12-0x").ErrorOr(null));

			Assert.Equal("Found a non-latin digit in the string: 2x10", DateUtil.ParseYearMonthDay("2x101201").ErrorOr(null));
			Assert.Equal("Found a non-latin digit in the string: x2", DateUtil.ParseYearMonthDay("2010x201").ErrorOr(null));
			Assert.Equal("Found a non-latin digit in the string: 0x", DateUtil.ParseYearMonthDay("2010120x").ErrorOr(null));

			Assert.Equal("Month must be at least 1 and at most 12", DateUtil.ParseYearMonthDay("2010-99-01").ErrorOr(null));
			Assert.Equal("Day must be at least 1 and, for the provided month (12), at most 31", DateUtil.ParseYearMonthDay("2010-12-99").ErrorOr(null));

			Assert.Equal("Month must be at least 1 and at most 12", DateUtil.ParseYearMonthDay("20109901").ErrorOr(null));
			Assert.Equal("Day must be at least 1 and, for the provided month (12), at most 31", DateUtil.ParseYearMonthDay("20101299").ErrorOr(null));
		}
	}
}
