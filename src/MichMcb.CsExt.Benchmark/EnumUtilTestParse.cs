namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using MichMcb.CsExt.Dates;
	using System;
	using System.Collections.Generic;

	[SimpleJob(RuntimeMoniker.Net60)]
	[RPlotExporter]
	public class EnumUtilTestParse
	{
		private static readonly EnumUtil<DateTimePart> util = EnumUtil<DateTimePart>.Compile(string.Empty, StringComparer.OrdinalIgnoreCase).ValueOr(null);
		private static readonly Dictionary<string, DateTimePart> vals = new(StringComparer.OrdinalIgnoreCase);
		private static readonly StringComparer sc = StringComparer.OrdinalIgnoreCase;
		private static readonly string Val = "Millisecond";
		[Benchmark]
		public DateTimePart Switch()
		{
			switch (Val)
			{
				case "Millisecond": return DateTimePart.Millisecond;
				case "Second": return DateTimePart.Second;
				case "Minute": return DateTimePart.Minute;
				case "Hour": return DateTimePart.Hour;
				case "Day": return DateTimePart.Day;
				case "Month": return DateTimePart.Month;
				case "Year": return DateTimePart.Year;
				default: return 0;
			}
		}
		[Benchmark]
		public DateTimePart Compiled()
		{
			return util.TryParse(Val).ValueOr(DateTimePart.Year);
		}
		[Benchmark]
		public DateTimePart EnumTryParse()
		{
			Enum.TryParse(Val, out DateTimePart p);
			return p;
		}
	}
}
