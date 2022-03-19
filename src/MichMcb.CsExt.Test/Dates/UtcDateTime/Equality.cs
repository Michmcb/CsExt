namespace MichMcb.CsExt.Test.Dates.UtcDateTime
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

#pragma warning disable CS1718 // Comparison made to same variable
	public static class Equality
	{
		[Fact]
		public static void HashCodeCalculatedOnMilliseconds()
		{
			UtcDateTime udt = new(2021, 01, 25, 10, 15, 30, 937);
			Assert.Equal(udt.TotalMilliseconds.GetHashCode(), udt.GetHashCode());
		}
		[Fact]
		public static void EqualsMethod()
		{
			UtcDateTime udt1 = new(2021, 01, 25, 10, 15, 30, 937);
			UtcDateTime udt2 = new(2021, 01, 25, 10, 15, 30, 937);
			UtcDateTime udt3 = new(1980, 02, 20, 01, 02, 03, 123);

			Assert.True(udt1.Equals(udt2));
			Assert.True(udt2.Equals(udt1));
			Assert.True(udt1.Equals((object?)udt2));
			Assert.True(udt2.Equals((object?)udt1));

			Assert.False(udt1.Equals(udt3));
			Assert.False(udt3.Equals(udt1));
			Assert.False(udt1.Equals((object?)udt3));
			Assert.False(udt3.Equals((object?)udt1));

			Assert.False(udt1.Equals(50));
			Assert.False(udt1.Equals(null));
		}
		[Fact]
		public static void EqualsDifference()
		{
			UtcDateTime udt1 = new(2021, 01, 25, 10, 15, 30, 500);
			UtcDateTime udt2 = new(2021, 01, 25, 10, 15, 30, 250);

			Assert.False(udt1.Equals(udt2, 0));
			Assert.False(udt1.Equals(udt2, 249));
			Assert.True(udt1.Equals(udt2, 250));
			Assert.True(udt1.Equals(udt2, 251));
		}
		[Fact]
		public static void CompareTo()
		{
			UtcDateTime udt1 = new(2021, 01, 25, 10, 15, 30);
			UtcDateTime udt2 = new(2021, 01, 25, 10, 15, 31);
			UtcDateTime udt3 = new(2021, 01, 25, 10, 15, 32);

			Assert.Equal(0, udt1.CompareTo(udt1));
			Assert.Equal(-1, udt1.CompareTo(udt2));
			Assert.Equal(-1, udt1.CompareTo(udt3));

			Assert.Equal(1, udt2.CompareTo(udt1));
			Assert.Equal(0, udt2.CompareTo(udt2));
			Assert.Equal(-1, udt2.CompareTo(udt3));

			Assert.Equal(1, udt3.CompareTo(udt1));
			Assert.Equal(1, udt3.CompareTo(udt2));
			Assert.Equal(0, udt3.CompareTo(udt3));
		}
		[Fact]
		public static void Operators()
		{
			UtcDateTime udt1 = new(2021, 01, 25, 10, 15, 30);
			UtcDateTime udt1c = new(2021, 01, 25, 10, 15, 30);
			UtcDateTime udt2 = new(2021, 01, 25, 10, 15, 31);

			Assert.True(udt1 == udt1);
			Assert.True(udt1 == udt1c);
			Assert.True(udt1c == udt1);
			Assert.False(udt1 == udt2);
			Assert.False(udt2 == udt1);

			Assert.False(udt1 != udt1);
			Assert.False(udt1 != udt1c);
			Assert.False(udt1c != udt1);
			Assert.True(udt1 != udt2);
			Assert.True(udt2 != udt1);

			Assert.False(udt1 < udt1);
			Assert.False(udt1 < udt1c);
			Assert.False(udt1c < udt1);
			Assert.True(udt1 < udt2);
			Assert.False(udt2 < udt1);

			Assert.True(udt1 <= udt1);
			Assert.True(udt1 <= udt1c);
			Assert.True(udt1c <= udt1);
			Assert.True(udt1 <= udt2);
			Assert.False(udt2 <= udt1);

			Assert.False(udt1 > udt1);
			Assert.False(udt1 > udt1c);
			Assert.False(udt1c > udt1);
			Assert.False(udt1 > udt2);
			Assert.True(udt2 > udt1);

			Assert.True(udt1 >= udt1);
			Assert.True(udt1 >= udt1c);
			Assert.True(udt1c >= udt1);
			Assert.False(udt1 >= udt2);
			Assert.True(udt2 >= udt1);

			TimeSpan h1m2s3 = new(1, 2, 3);

			Assert.Equal(new UtcDateTime(2021, 01, 25, 11, 17, 33), udt1 + h1m2s3);
			Assert.Equal(new UtcDateTime(2021, 01, 25, 9, 13, 27), udt1 - h1m2s3);
			Assert.Equal(new TimeSpan(29, 0, 0, 0), new UtcDateTime(2021, 1, 30) - new UtcDateTime(2021, 1, 1));
		}
	}
#pragma warning restore CS1718 // Comparison made to same variable
}
