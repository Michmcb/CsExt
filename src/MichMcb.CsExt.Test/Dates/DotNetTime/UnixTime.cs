namespace MichMcb.CsExt.Test.Dates.DotNetTime
{
	using MichMcb.CsExt.Dates;
	using Xunit;

	public static class UnixTime
	{
		[Fact]
		public static void Seconds()
		{
			UtcDateTime udt = new(1970, 1, 1, 0, 0, 0);
			Assert.Equal(0, DotNetTime.TicksToUnixTimeSeconds(udt.Ticks));
			Assert.Equal(udt, new UtcDateTime(DotNetTime.UnixTimeSecondsToTicks(0)));
		}
	}
}
