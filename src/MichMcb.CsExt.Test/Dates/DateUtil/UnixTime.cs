﻿namespace MichMcb.CsExt.Test.Dates.DateUtil
{
	using System;
	using Xunit;
	using MichMcb.CsExt.Dates;
	public static class UnixTime
	{
		[Fact]
		public static void Seconds()
		{
			// The unix timestamp came from wikipedia
			DateTime unix = DateUtil.DateTimeFromUnixTimeSeconds(1647645905L);
			Assert.Equal(new DateTime(2022, 3, 18, 23, 25, 05, DateTimeKind.Utc), unix);

			unix = DateUtil.DateTimeFromUnixTimeSeconds(0);
			Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), unix);

			unix = DateUtil.DateTimeFromUnixTimeSeconds(DotNetTime.MinSecondsAsUnixTime);
			Assert.Equal(new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc), unix);

			unix = DateUtil.DateTimeFromUnixTimeSeconds(DotNetTime.MaxSecondsAsUnixTime);
			Assert.Equal(new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc), unix);
		}
	}
}
