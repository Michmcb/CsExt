namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using MichMcb.CsExt.Dates;
	using System;

	[SimpleJob(RuntimeMoniker.Net60)]
	[RPlotExporter]
	public class EnumUtilTestToString
	{
		private static readonly EnumUtil<DateTimePart> util = EnumUtil<DateTimePart>.Compile(string.Empty, StringComparer.OrdinalIgnoreCase).ValueOr(null);
		[Benchmark]
		public void EnumToString()
		{
			DateTimePart.Millisecond.ToString();
			DateTimePart.Second.ToString();
			DateTimePart.Minute.ToString();
			DateTimePart.Hour.ToString();
			DateTimePart.Day.ToString();
			DateTimePart.Month.ToString();
			DateTimePart.Year.ToString();
		}
		[Benchmark]
		public void EnumUtil()
		{
			util.ToStringFunc(DateTimePart.Millisecond);
			util.ToStringFunc(DateTimePart.Second);
			util.ToStringFunc(DateTimePart.Minute);
			util.ToStringFunc(DateTimePart.Hour);
			util.ToStringFunc(DateTimePart.Day);
			util.ToStringFunc(DateTimePart.Month);
			util.ToStringFunc(DateTimePart.Year);
		}
	}
}
