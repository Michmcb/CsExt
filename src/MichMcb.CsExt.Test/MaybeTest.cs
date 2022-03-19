namespace MichMcb.CsExt.Test
{
	using System;
	using Xunit;

	public static class MaybeTest
	{
		[Fact]
		public static void Ok()
		{
			Assert.True(new Maybe<int, bool>(1).Ok);
			Assert.False(new Maybe<int, bool>(true).Ok);
			Assert.True(Maybe<int, int>.Value(3).Ok);
			Assert.False(Maybe<int, int>.Error(5).Ok);
		}
		[Fact]
		public static void ValueOr()
		{
			Assert.Equal(1, new Maybe<int, string>(1).ValueOr(2));
			Assert.Equal(1, new Maybe<int, string>(1).ValueOrException());
			Assert.Equal(2, new Maybe<int, string>("foo").ValueOr(2));
			Assert.Throws<NoValueException>(() => new Maybe<int, string>("foo").ValueOrException());
		}
		[Fact]
		public static void ErrorOr()
		{
			Assert.Equal("bar", new Maybe<int, string>(1).ErrorOr("bar"));
			Assert.Equal("foo", new Maybe<int, string>("foo").ErrorOr("bar"));
		}
		[Fact]
		public static void Success()
		{
			Maybe<int, string> m1 = 1;
			Assert.True(m1.Success(out int val1));
			Assert.Equal(1, val1);

			Maybe<int, string> m2 = 2;
			Assert.True(m2.Success(out int val2, out string? error2));
			Assert.Equal(2, val2);
			Assert.Null(error2);

			Maybe<int, string> m3 = "foo";
			Assert.False(m3.Success(out int val3));
			Assert.Equal(0, val3);

			Maybe<int, string> m4 = "bar";
			Assert.False(m4.Success(out int val4, out string? error4));
			Assert.Equal(0, val4);
			Assert.Equal("bar", error4);
		}
		[Fact]
		public static void Failure()
		{
			Maybe<int, string> m1 = 1;
			Assert.False(m1.Failure(out string error1));
			Assert.Null(error1);

			Maybe<int, string> m2 = 2;
			Assert.False(m2.Failure(out int val2, out string? error2));
			Assert.Equal(2, val2);
			Assert.Null(error2);

			Maybe<int, string> m3 = "foo";
			Assert.True(m3.Failure(out string error3));
			Assert.Equal("foo", error3);

			Maybe<int, string> m4 = "bar";
			Assert.True(m4.Failure(out int val4, out string? error4));
			Assert.Equal(0, val4);
			Assert.Equal("bar", error4);
		}
		[Fact]
		public static void ToStringTest()
		{
			Assert.Equal("1", new Maybe<int, string>(1).ToString());
			Assert.Equal("foo", new Maybe<int, string>("foo").ToString());
		}
		[Fact]
		public static void ComparisonThrows()
		{
			Maybe<int, string> m1 = 1;
			Maybe<int, string> m2 = "bad";
			Assert.Throws<InvalidOperationException>(() => m1 == m2);
			Assert.Throws<InvalidOperationException>(() => m1 != m2);
			Assert.Throws<InvalidOperationException>(() => m1.Equals(m2));
			Assert.Throws<InvalidOperationException>(() => m1.GetHashCode());
		}
		[Fact]
		public static void ValueOrException()
		{
			Maybe<int, string> m1 = 1;
			Maybe<int, string> m2 = "bad";
			Assert.Equal(1, m1.ValueOrException());
			Assert.Throws<NoValueException>(() => m2.ValueOrException());
		}
	}
}
