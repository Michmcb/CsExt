namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using MichMcb.CsExt.Dates;
	using System;
	using System.Collections.Generic;

	[SimpleJob(RuntimeMoniker.Net60)]
	[RPlotExporter]
	public class EnumUtilIsDefined
	{
		private static readonly EnumUtil<DateTimePart> util = EnumUtil<DateTimePart>.Compile(string.Empty, StringComparer.OrdinalIgnoreCase).ValueOr(null);
		private static readonly HashSet<DateTimePart> hashset= new((DateTimePart[])Enum.GetValues(typeof(DateTimePart)));
		[Benchmark]
		public void Compiled()
		{
			util.IsDefinedFunc(DateTimePart.Millisecond);
			util.IsDefinedFunc(DateTimePart.Second);
			util.IsDefinedFunc(DateTimePart.Minute);
			util.IsDefinedFunc(DateTimePart.Hour);
			util.IsDefinedFunc(DateTimePart.Day);
			util.IsDefinedFunc(DateTimePart.Month);
			util.IsDefinedFunc(DateTimePart.Year);
		}
		[Benchmark]
		public void HashSet()
		{
			hashset.Contains(DateTimePart.Millisecond);
			hashset.Contains(DateTimePart.Second);
			hashset.Contains(DateTimePart.Minute);
			hashset.Contains(DateTimePart.Hour);
			hashset.Contains(DateTimePart.Day);
			hashset.Contains(DateTimePart.Month);
			hashset.Contains(DateTimePart.Year);
		}
		[Benchmark]
		public void IsDefined()
		{
			Enum.IsDefined(DateTimePart.Millisecond);
			Enum.IsDefined(DateTimePart.Second);
			Enum.IsDefined(DateTimePart.Minute);
			Enum.IsDefined(DateTimePart.Hour);
			Enum.IsDefined(DateTimePart.Day);
			Enum.IsDefined(DateTimePart.Month);
			Enum.IsDefined(DateTimePart.Year);
		}
	}
}
