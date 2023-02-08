namespace MichMcb.CsExt.Test
{
	using Xunit;

	public static class ParseTest
	{
		[Fact]
		public static void LatinIntWorks()
		{
			Assert.Equal(1234567890, Parse.LatinInt("1234567890").ValueOrException());
			Assert.Equal(987654321, Parse.LatinInt("0987654321").ValueOrException());
			Assert.Equal(2147483647, Parse.LatinInt("2147483647").ValueOrException());
		}
		[Fact]
		public static void LatinIntFails()
		{
			Assert.Equal("String is longer than 10, which is too large for an Int32 to hold: 12345678901234567890", Parse.LatinInt("12345678901234567890").ErrorOr(null));
			Assert.Equal("Found a non-latin digit in the string: 123abc", Parse.LatinInt("123abc").ErrorOr(null));
			Assert.Equal("Value overflowed. String: 9999999999", Parse.LatinInt("9999999999").ErrorOr(null));
			Assert.Equal("Value overflowed. String: 2147483648", Parse.LatinInt("2147483648").ErrorOr(null));
		}
	}
}
