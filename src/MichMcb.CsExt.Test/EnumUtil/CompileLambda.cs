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
			Assert.Equal(nameof(DateTimeKind.Utc), util.ToString(DateTimeKind.Utc));
			Assert.Equal(nameof(DateTimeKind.Local), util.ToString(DateTimeKind.Local));
			Assert.Equal("Unknown", util.ToString((DateTimeKind)999));

			Assert.True(util.IsDefined(DateTimeKind.Unspecified));
			Assert.True(util.IsDefined(DateTimeKind.Local));
			Assert.True(util.IsDefined(DateTimeKind.Utc));

			Assert.Equal(DateTimeKind.Unspecified, util.TryParse(nameof(DateTimeKind.Unspecified)).ValueOrException());
			Assert.Equal(DateTimeKind.Utc, util.TryParse(nameof(DateTimeKind.Utc)).ValueOrException());
			Assert.Equal(DateTimeKind.Local, util.TryParse(nameof(DateTimeKind.Local)).ValueOrException());

			IReadOnlyList<NameValue<DateTimeKind>> nvs = util.GetNameValues();
			Assert.Equal(3, nvs.Count);
			NameValue<DateTimeKind> nv = nvs[0];
			Assert.Equal(DateTimeKind.Unspecified, nv.Value);
			Assert.Equal(nameof(DateTimeKind.Unspecified), nv.Name);
			nv = nvs[1];
			Assert.Equal(DateTimeKind.Utc, nv.Value);
			Assert.Equal(nameof(DateTimeKind.Utc), nv.Name);
			nv = nvs[2];
			Assert.Equal(DateTimeKind.Local, nv.Value);
			Assert.Equal(nameof(DateTimeKind.Local), nv.Name);

			Assert.Collection(util.Values,
				x => Assert.Equal(DateTimeKind.Unspecified, x),
				x => Assert.Equal(DateTimeKind.Utc, x),
				x => Assert.Equal(DateTimeKind.Local, x));

			Assert.Collection(util.Names,
				x => Assert.Equal(nameof(DateTimeKind.Unspecified), x),
				x => Assert.Equal(nameof(DateTimeKind.Utc), x),
				x => Assert.Equal(nameof(DateTimeKind.Local), x));
		}
		[Fact]
		public static void CompileDict()
		{
			EnumUtil<DateTimeKind> util = EnumUtil<DateTimeKind>.Compile(string.Empty, StringComparer.Ordinal, new Dictionary<DateTimeKind, string>()
			{
				{ DateTimeKind.Unspecified, "Unknown" },
				{ DateTimeKind.Utc, "UTC Time" },
				{ DateTimeKind.Local, "Local Time" },
			}).ValueOrException();

			Assert.Equal("Unknown", util.ToString(DateTimeKind.Unspecified));
			Assert.Equal("UTC Time", util.ToString(DateTimeKind.Utc));
			Assert.Equal("Local Time", util.ToString(DateTimeKind.Local));

			Assert.Equal(DateTimeKind.Unspecified, util.TryParse("Unknown").ValueOrException());
			Assert.Equal(DateTimeKind.Utc, util.TryParse("UTC Time").ValueOrException());
			Assert.Equal(DateTimeKind.Local, util.TryParse("Local Time").ValueOrException());

			Assert.False(util.TryParse("gdsfg").Ok);

			IReadOnlyList<NameValue<DateTimeKind>> nvs = util.GetNameValues();
			Assert.Equal(3, nvs.Count);
			NameValue<DateTimeKind> nv = nvs[0];
			Assert.Equal(DateTimeKind.Unspecified, nv.Value);
			Assert.Equal("Unknown", nv.Name);
			nv = nvs[1];
			Assert.Equal(DateTimeKind.Utc, nv.Value);
			Assert.Equal("UTC Time", nv.Name);
			nv = nvs[2];
			Assert.Equal(DateTimeKind.Local, nv.Value);
			Assert.Equal("Local Time", nv.Name);

			Assert.Collection(util.Values,
				x => Assert.Equal(DateTimeKind.Unspecified, x),
				x => Assert.Equal(DateTimeKind.Utc, x),
				x => Assert.Equal(DateTimeKind.Local, x));

			Assert.Collection(util.Names,
				x => Assert.Equal("Unknown", x),
				x => Assert.Equal("UTC Time", x),
				x => Assert.Equal("Local Time", x));
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
