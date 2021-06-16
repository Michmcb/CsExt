namespace MichMcb.CsExt.Test.EnumUtil
{
	using System;
	using System.Collections.Generic;
	using Xunit;

	public static class CompileLambda
	{
		[Fact]
		public static void Compile()
		{
			EnumUtil<DateTimeKind> util = EnumUtil<DateTimeKind>.Compile("Unknown", StringComparer.Ordinal).ValueOrException();
			Assert.Equal(nameof(DateTimeKind.Unspecified), util.ToString(DateTimeKind.Unspecified));
			Assert.Equal(nameof(DateTimeKind.Local), util.ToString(DateTimeKind.Local));
			Assert.Equal(nameof(DateTimeKind.Utc), util.ToString(DateTimeKind.Utc));
			Assert.Equal("Unknown", util.ToString((DateTimeKind)999));

			Assert.True(util.IsDefined(DateTimeKind.Unspecified));
			Assert.True(util.IsDefined(DateTimeKind.Local));
			Assert.True(util.IsDefined(DateTimeKind.Utc));

			Assert.Equal(DateTimeKind.Unspecified, util.TryParse(nameof(DateTimeKind.Unspecified)).ValueOrException());
			Assert.Equal(DateTimeKind.Local, util.TryParse(nameof(DateTimeKind.Local)).ValueOrException());
			Assert.Equal(DateTimeKind.Utc, util.TryParse(nameof(DateTimeKind.Utc)).ValueOrException());
		}
		[Fact]
		public static void CompileDict()
		{
			EnumUtil<DateTimeKind> util = EnumUtil<DateTimeKind>.Compile(string.Empty, StringComparer.Ordinal, new Dictionary<DateTimeKind, string>()
			{
				{ DateTimeKind.Unspecified, "Unknown" },
				{ DateTimeKind.Local, "Local Time" },
				{ DateTimeKind.Utc, "UTC Time" },
			}).ValueOrException();

			Assert.Equal("Unknown", util.ToString(DateTimeKind.Unspecified));
			Assert.Equal("Local Time", util.ToString(DateTimeKind.Local));
			Assert.Equal("UTC Time", util.ToString(DateTimeKind.Utc));

			Assert.Equal(DateTimeKind.Unspecified, util.TryParse("Unknown").ValueOrException());
			Assert.Equal(DateTimeKind.Local, util.TryParse("Local Time").ValueOrException());
			Assert.Equal(DateTimeKind.Utc, util.TryParse("UTC Time").ValueOrException());

			Assert.False(util.TryParse("gdsfg").Ok);
		}
		[Fact]
		public static void CompileFlags()
		{
			EnumUtil<ConsoleModifiers> util = EnumUtil<ConsoleModifiers>.Compile().ValueOrException();
			ConsoleModifiers ctrlAlt = ConsoleModifiers.Control | ConsoleModifiers.Alt;
			Assert.False(util.HasFlagFunc(ctrlAlt, ConsoleModifiers.Shift));
			Assert.True(util.HasFlagFunc(ctrlAlt, ConsoleModifiers.Control));
			Assert.True(util.HasFlagFunc(ctrlAlt, ConsoleModifiers.Alt));
		}
	}
}
