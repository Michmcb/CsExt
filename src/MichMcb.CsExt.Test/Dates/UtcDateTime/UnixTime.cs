namespace MichMcb.CsExt.Test.Dates.UtcDateTime
{
	using MichMcb.CsExt.Dates;
	using Xunit;
	public static class UnixTime
	{
		[Fact]
		public static void ToUnixTimeSeconds()
		{
			Assert.Equal(0, UtcDateTime.UnixEpoch.ToUnixTimeSeconds());
			Assert.Equal(UtcDateTime.MaxSecondsAsUnixTime, UtcDateTime.MaxValue.ToUnixTimeSeconds());
			Assert.Equal(UtcDateTime.MinSecondsAsUnixTime, UtcDateTime.MinValue.ToUnixTimeSeconds());
		}
		[Fact]
		public static void ToUnixTimeMilliseconds()
		{
			Assert.Equal(0, UtcDateTime.UnixEpoch.ToUnixTimeMilliseconds());
			Assert.Equal(UtcDateTime.MaxMillisAsUnixTime, UtcDateTime.MaxValue.ToUnixTimeMilliseconds());
			Assert.Equal(UtcDateTime.MinMillisAsUnixTime, UtcDateTime.MinValue.ToUnixTimeMilliseconds());
		}
		[Fact]
		public static void FromUnixTimeSeconds()
		{
			Assert.Equal(UtcDateTime.UnixEpoch, UtcDateTime.FromUnixTimeSeconds(0));
			Assert.Equal(UtcDateTime.MaxValue.Truncate(DateTimePart.Second), UtcDateTime.FromUnixTimeSeconds(UtcDateTime.MaxSecondsAsUnixTime));
			Assert.Equal(UtcDateTime.MinValue, UtcDateTime.FromUnixTimeSeconds(UtcDateTime.MinSecondsAsUnixTime));
		}
		[Fact]
		public static void FromUnixTimeMilliseconds()
		{
			Assert.Equal(UtcDateTime.UnixEpoch, UtcDateTime.FromUnixTimeMilliseconds(0));
			Assert.Equal(UtcDateTime.MaxValue, UtcDateTime.FromUnixTimeMilliseconds(UtcDateTime.MaxMillisAsUnixTime));
			Assert.Equal(UtcDateTime.MinValue, UtcDateTime.FromUnixTimeMilliseconds(UtcDateTime.MinMillisAsUnixTime));
		}
	}
}
