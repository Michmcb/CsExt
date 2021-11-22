namespace MichMcb.CsExt.Test.Dates.UtcDateTime
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class Casting
	{
		[Fact]
		public static void FromDotNetDateTimes()
		{
			UtcDateTime converted = (UtcDateTime)new DateTime(2001, 1, 1, 15, 10, 10, DateTimeKind.Utc);
			Assert.Equal(new UtcDateTime(2001, 1, 1, 15, 10, 10), converted);

			converted = (UtcDateTime)new DateTime(2001, 1, 1, 15, 10, 10, DateTimeKind.Local);
			Assert.Equal(new UtcDateTime(2001, 1, 1, 5, 10, 10), converted);

			converted = (UtcDateTime)new DateTimeOffset(2001, 1, 1, 15, 10, 10, TimeSpan.Zero);
			Assert.Equal(new UtcDateTime(2001, 1, 1, 15, 10, 10), converted);

			converted = (UtcDateTime)new DateTimeOffset(2001, 1, 1, 15, 10, 10, new TimeSpan(5, 0, 0));
			Assert.Equal(new UtcDateTime(2001, 1, 1, 10, 10, 10), converted);
		}
		[Fact]
		public static void ToDotNetDateTimes()
		{
			DateTime converted1 = new UtcDateTime(2001, 2, 3, 4, 5, 6, 700);
			Assert.Equal(new DateTime(2001, 2, 3, 4, 5, 6, 700, DateTimeKind.Utc), converted1);

			DateTimeOffset converted2 = new UtcDateTime(2001, 2, 3, 4, 5, 6, 700);
			Assert.Equal(new DateTimeOffset(2001, 2, 3, 4, 5, 6, 700, TimeSpan.Zero), converted2);
		}
	}
}
