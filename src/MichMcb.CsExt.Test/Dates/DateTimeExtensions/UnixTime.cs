namespace MichMcb.CsExt.Test.Dates.DateTimeExtensions
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class UnixTime
	{
		private static readonly TimeSpan localTz = TimeZoneInfo.Local.BaseUtcOffset;
		[Fact]
		public static void ToUnixTimeSeconds()
		{
			Assert.Equal(0, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUnixTimeSeconds());
			Assert.Equal(0, (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local) + localTz).ToUnixTimeSeconds());
			Assert.Equal(0, (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified) + localTz).ToUnixTimeSeconds());
		}
		[Fact]
		public static void ToUnixTimeMilliseconds()
		{
			Assert.Equal(0, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUnixTimeMilliseconds());
			Assert.Equal(0, (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local) + localTz).ToUnixTimeMilliseconds());
			Assert.Equal(0, (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified) + localTz).ToUnixTimeMilliseconds());
		}
	}
}
