namespace MichMcb.CsExt.Test.Dates.Iso8601Format
{
	using Xunit;
	using MichMcb.CsExt.Dates;

	public static class PredefinedFormats
	{
		[Fact]
		public static void AreCorrect()
		{
			Assert.Equal(24, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_UtcTz).ValueOrException().Length);
			Assert.Equal(29, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_FullTz).ValueOrException().Length);
			Assert.Equal(23, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_LocalTz).ValueOrException().Length);
			Assert.Equal(20, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_NoMillis_UtcTz).ValueOrException().Length);
			Assert.Equal(25, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_NoMillis_FullTz).ValueOrException().Length);
			Assert.Equal(19, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_NoMillis_LocalTz).ValueOrException().Length);
			Assert.Equal(20, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_UtcTz).ValueOrException().Length);
			Assert.Equal(24, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_FullTz).ValueOrException().Length);
			Assert.Equal(19, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_LocalTz).ValueOrException().Length);
			Assert.Equal(16, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_NoMillis_UtcTz).ValueOrException().Length);
			Assert.Equal(20, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_NoMillis_FullTz).ValueOrException().Length);
			Assert.Equal(15, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_NoMillis_LocalTz).ValueOrException().Length);
			Assert.Equal(10, Iso8601Format.TryCreate(Iso8601Parts.Format_DateOnly).ValueOrException().Length);
			Assert.Equal(8, Iso8601Format.TryCreate(Iso8601Parts.Format_DateOnlyWithoutSeparators).ValueOrException().Length);
			Assert.Equal(8, Iso8601Format.TryCreate(Iso8601Parts.Format_DateOrdinal).ValueOrException().Length);
		}
	}
}
