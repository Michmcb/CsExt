namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using MichMcb.CsExt.Dates;
	using System;

	[SimpleJob(RuntimeMoniker.Net60)]
	[RPlotExporter]
	public class FormatUtcDateTime
	{
		public static readonly UtcDateTime utcNow = UtcDateTime.Now;
		public static readonly DateTime now = DateTime.UtcNow;
		public static readonly Memory<char> mem = new char[Iso8601Format.ExtendedFormat_UtcTz.LengthRequired];
		[Benchmark]
		public void UtcDateTimeToString()
		{
			utcNow.ToString();
		}
		[Benchmark]
		public void DateTimeToStringO()
		{
			now.ToString("O");
		}
		[Benchmark]
		public void ExplicitFormat()
		{
			utcNow.FormatExtendedFormatUtc(mem.Span, decimalPlaces: 3);
		}
		[Benchmark]
		public void TryFormat()
		{
			utcNow.Format(mem.Span, Tz.Utc, Iso8601Format.ExtendedFormat_UtcTz);
		}
	}
}
