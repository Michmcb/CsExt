namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using MichMcb.CsExt.Dates;
	using System;

	[SimpleJob(RuntimeMoniker.NetCoreApp50)]
	[RPlotExporter]
	public class FormatUtcDateTime
	{
		public static readonly UtcDateTime utcNow = UtcDateTime.Now;
		public static readonly DateTime now = DateTime.UtcNow;
		public static readonly Memory<char> mem = new char[DateUtil.LengthRequired(Iso8601Parts.Format_ExtendedFormat_UtcTz)];
		[Benchmark]
		public void UtcDateTimeToString()
		{
			utcNow.ToString();
		}
		//[Benchmark]
		//public void DateTimeToStringExplicit()
		//{
		//	now.ToString("yyyy-MM-ddTHH:mm:ss.fffK");
		//}
		[Benchmark]
		public void DateTimeToStringO()
		{
			now.ToString("O");
		}
		[Benchmark]
		public void ExplicitFormat()
		{
			utcNow.ToIso8601StringUtc(Iso8601Parts.Format_ExtendedFormat_UtcTz);
		}
		[Benchmark]
		public void TryFormat()
		{
			utcNow.TryFormat(mem.Span, TimeSpan.Zero, format: Iso8601Parts.Format_ExtendedFormat_UtcTz);
		}
	}
}
