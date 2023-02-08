namespace MichMcb.CsExt.Test
{
	using System.IO;
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
#pragma warning disable CS0618 // Type or member is obsolete
			Opt<string?> opt = default;
			Assert.Throws<NoValueException>(() => opt.ValueOrException());
			Assert.Equal("Message", Assert.Throws<NoValueException>(() => opt.ValueOrException("Message")).Message);
			FileNotFoundException ex = Assert.Throws<FileNotFoundException>(() => opt.ValueOrException(() => new FileNotFoundException("Hello")));
			Assert.Equal("Hello", ex.Message);

			opt = "hello";
			Assert.Equal("hello", opt.ValueOrException());
			Assert.Equal("hello", opt.ValueOrException("Message"));
			Assert.Equal("hello", opt.ValueOrException(() => new FileNotFoundException("Hello")));
#pragma warning restore CS0618 // Type or member is obsolete
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
