namespace MichMcb.CsExt.Test.Strings.Extensions
{
	using MichMcb.CsExt.Strings;
	using System;
	using Xunit;
	public sealed class Split
	{
		private static readonly ReadOnlyMemory<StringSplitOptions> SplitOptions = new StringSplitOptions[]
		{
			StringSplitOptions.None,
			StringSplitOptions.RemoveEmptyEntries,
			StringSplitOptions.TrimEntries,
			StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
		};
		private static void AssertSplitSpanMatchesDotNet(string str, char sep)
		{
			foreach (StringSplitOptions options in SplitOptions.Span)
			{
				string[] expected = str.Split(sep, options);
				string[] actual = new string[expected.Length];
				int i = 0;

				SpanSplit split = new(str, sep, options);
				while (split.Next().HasVal(out Range slice))
				{
					actual[i++] = new string(str[slice]);
				}

				Assert.Equal(expected.Length, i);
				for (i = 0; i < expected.Length; i++)
				{
					Assert.Equal(expected[i], actual[i]);
				}
			}
		}
		[Fact]
		public static void SplitEmptyStringNoSeparators() { AssertSplitSpanMatchesDotNet("", '.'); }
		[Fact]
		public static void SplitWhitespaceNoSeparators() { AssertSplitSpanMatchesDotNet("   ", '.'); }
		[Fact]
		public static void OnlyOneSeparator() { AssertSplitSpanMatchesDotNet(".", '.'); }
		[Fact]
		public static void OnlyThreeSeparators() { AssertSplitSpanMatchesDotNet("...", '.'); }
		[Fact]
		public static void SplitOnlyWhitespaceAndSeparator() { AssertSplitSpanMatchesDotNet("  .  ", '.'); }
		[Fact]
		public static void SplitOnlyWhitespaceAndSeparatorAtEnd() { AssertSplitSpanMatchesDotNet("    .", '.'); }
		[Fact]
		public static void SplitOnlyWhitespaceAndSeparatorAtStart() { AssertSplitSpanMatchesDotNet(".    ", '.'); }
		[Fact]
		public static void SplitOnlyWhitespaceAndSeparators() { AssertSplitSpanMatchesDotNet("  .\t.\r.\n.\v.", '.'); }
		[Fact]
		public static void SplitNoMatches() { AssertSplitSpanMatchesDotNet("string", '.'); }
		[Fact]
		public static void SplitOneSeparators() { AssertSplitSpanMatchesDotNet("one.two", '.'); }
		[Fact]
		public static void SplitTwoSeparators() { AssertSplitSpanMatchesDotNet("a.b.c", '.'); }
		[Fact]
		public static void SeparatorOnlyAtBeginning() { AssertSplitSpanMatchesDotNet(".b", '.'); }
		[Fact]
		public static void SeparatorAtBeginning() { AssertSplitSpanMatchesDotNet(".b.c", '.'); }
		[Fact]
		public static void SeparatorOnlyAtEnd() { AssertSplitSpanMatchesDotNet("a.", '.'); }
		[Fact]
		public static void SeparatorAtEnd() { AssertSplitSpanMatchesDotNet("a.b.", '.'); }
	}
}
