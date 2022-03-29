namespace MichMcb.CsExt.Test
{
	using Xunit;

	public static class OptTest
	{
		[Fact]
		public static void Ctor_HasVal()
		{
			Opt<int> opt = new(10);
			Assert.True(opt.HasVal(out int value));
			Assert.Equal(10, value);
		}
		[Fact]
		public static void ValueOrDefault()
		{
			Opt<string?> opt = default;
			Assert.Equal(string.Empty, opt.ValueOrDefault(string.Empty));

			opt = "hello";
			Assert.Equal("hello", opt.ValueOrDefault(string.Empty));
		}
		[Fact]
		public static void ValueOrException()
		{
			Opt<string?> opt = default;
			Assert.Throws<NoValueException>(() => opt.ValueOrException());

			opt = "hello";
			Assert.Equal("hello", opt.ValueOrException());
		}
		[Fact]
		public static void ImplicitCast()
		{
			Opt<string?> opt = "hello";
			Assert.True(opt.HasVal(out string? value));
			Assert.Equal("hello", value);
		}
	}
}
