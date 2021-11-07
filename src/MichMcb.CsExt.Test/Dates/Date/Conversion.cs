namespace MichMcb.CsExt.Test.Dates.Date
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class Conversion
	{
		[Fact]
		public static void AsUtcDateTime()
		{
			Date d = new(2020, 5, 10);
			Assert.Equal(new DateTime(2020, 5, 10, 10, 15, 20, DateTimeKind.Utc), d.AsDateTime(new TimeSpan(10, 15, 20), DateTimeKind.Utc));
			Assert.Equal(new DateTime(2020, 5, 10, 10, 15, 20, 500, DateTimeKind.Local), d.AsDateTime(new TimeSpan(0, 10, 15, 20, 500), DateTimeKind.Local));
			Assert.Equal(new DateTime(d.TotalDays * TimeSpan.TicksPerDay + 500000, DateTimeKind.Utc), d.AsDateTime(new TimeSpan(500000), DateTimeKind.Unspecified));

			Assert.Equal(new DateTime(2020, 5, 10, 10, 15, 20, DateTimeKind.Utc), d.AsDateTime(10, 15, 20, 0, DateTimeKind.Utc));
			Assert.Equal(new DateTime(2020, 5, 10, 10, 15, 20, 500, DateTimeKind.Local), d.AsDateTime(10, 15, 20, 500, DateTimeKind.Local));

			Assert.Equal(new UtcDateTime(2020, 5, 10, 10, 15, 20), d.AsUtcDateTime(new TimeSpan(10, 15, 20)));
			Assert.Equal(new UtcDateTime(2020, 5, 10, 10, 15, 20, 500), d.AsUtcDateTime(new TimeSpan(0, 10, 15, 20, 500)));
			Assert.Equal(new UtcDateTime(d.TotalDays * DateUtil.MillisPerDay + 500000), d.AsUtcDateTime(TimeSpan.FromMilliseconds(500000)));

			Assert.Equal(new UtcDateTime(2020, 5, 10, 10, 15, 20), d.AsUtcDateTime(10, 15, 20, 0));
			Assert.Equal(new UtcDateTime(2020, 5, 10, 10, 15, 20, 500), d.AsUtcDateTime(10, 15, 20, 500));
		}
	}
}
