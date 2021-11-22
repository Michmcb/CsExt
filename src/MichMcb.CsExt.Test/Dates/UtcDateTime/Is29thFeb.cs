namespace MichMcb.CsExt.Test.Dates.UtcDateTime
{
	using MichMcb.CsExt.Dates;
	using Xunit;

	public static class Is29thFeb
	{
		[Fact]
		public static void Test()
		{
			Assert.False(new UtcDateTime(2020, 2, 28).Is29thFeb);
			Assert.True(new UtcDateTime(2020, 2, 29).Is29thFeb);
			Assert.False(new UtcDateTime(2020, 3, 1).Is29thFeb);

			Assert.False(new UtcDateTime(2021, 2, 28).Is29thFeb);
			Assert.False(new UtcDateTime(2021, 3, 1).Is29thFeb);
		}
	}
}
