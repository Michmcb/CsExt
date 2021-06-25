namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using MichMcb.CsExt.Dates;
	using MichMcb.CsExt.Rng;
	using System;

	[SimpleJob(RuntimeMoniker.Net50)]
	[RPlotExporter]
	public class FormatUtcDateTime
	{
		public static readonly UtcDateTime utcNow = UtcDateTime.Now;
		public static readonly DateTime now = DateTime.UtcNow;
		public static readonly Memory<char> mem = new char[DateUtil.TryGetLengthRequired(Iso8601Parts.Format_ExtendedFormat_UtcTz).ValueOrException()];
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
			utcNow.FormatExtendedFormatUtc(mem.Span, millis: true);
		}
		[Benchmark]
		public void TryFormat()
		{
			utcNow.TryFormat(mem.Span, TimeSpan.Zero, format: Iso8601Parts.Format_ExtendedFormat_UtcTz);
		}
	}
}
