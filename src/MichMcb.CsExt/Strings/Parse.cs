using System;
using System.Globalization;

namespace MichMcb.CsExt.Strings
{
	public static class Parse
	{
		public static Opt<int> Int(in ReadOnlySpan<char> str)
		{
			return int.TryParse(str, out int val) ? new Opt<int>(val, true) : new Opt<int>(val, false);
		}
		public static Opt<int> Int(in ReadOnlySpan<char> str, NumberStyles style, IFormatProvider provider)
		{
			return int.TryParse(str, style, provider, out int val) ? new Opt<int>(val, true) : new Opt<int>(val, false);
		}
		public static Opt<DateTime> DateTime(in ReadOnlySpan<char> str)
		{
			return System.DateTime.TryParse(str, out DateTime val) ? new Opt<DateTime>(val, true) : new Opt<DateTime>(val, false);
		}
		public static Opt<DateTime> DateTime(in ReadOnlySpan<char> str, IFormatProvider provider, DateTimeStyles styles)
		{
			return System.DateTime.TryParse(str, provider, styles, out DateTime val) ? new Opt<DateTime>(val, true) : new Opt<DateTime>(val, false);
		}
	}
}
