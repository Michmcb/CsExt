namespace MichMcb.CsExt.Test.Dates.UtcDateTime
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class TimeOfDay
	{
		[Fact]
		public static void Test()
		{
			Assert.Equal(new TimeSpan(10, 12, 15), new UtcDateTime(2021, 1, 5, 10, 12, 15).TimeOfDay);
		}
	}
}
