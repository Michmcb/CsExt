namespace MichMcb.CsExt.Test.Dates.Tz
{
	using MichMcb.CsExt.Dates;
	using Xunit;

	public static class ToString
	{
		[Fact]
		public static void Works()
		{
			Assert.Equal("+10:30", Tz.TryCreate(10, 30).ValueOrException().ToString());
			Assert.Equal("+10:30", Tz.TryCreate(10, -30).ValueOrException().ToString());
			Assert.Equal("-10:30", Tz.TryCreate(-10, 30).ValueOrException().ToString());
			Assert.Equal("-10:30", Tz.TryCreate(-10, -30).ValueOrException().ToString());
		}
	}
}
