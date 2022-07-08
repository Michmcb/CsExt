namespace MichMcb.CsExt.Test.Dates.Iso8601Format
{
	using Xunit;
	using MichMcb.CsExt.Dates;

	public static class PredefinedFormats
	{
		[Fact]
		public static void AreCorrect()
		{
			Assert.Equal(24, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_UtcTz).ValueOrException().LengthRequired);
			Assert.Equal(29, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_FullTz).ValueOrException().LengthRequired);
			Assert.Equal(23, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_LocalTz).ValueOrException().LengthRequired);
			Assert.Equal(20, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_NoFractional_UtcTz).ValueOrException().LengthRequired);
			Assert.Equal(25, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_NoFractional_FullTz).ValueOrException().LengthRequired);
			Assert.Equal(19, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_NoFractional_LocalTz).ValueOrException().LengthRequired);
			Assert.Equal(20, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_UtcTz).ValueOrException().LengthRequired);
			Assert.Equal(24, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_FullTz).ValueOrException().LengthRequired);
			Assert.Equal(19, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_LocalTz).ValueOrException().LengthRequired);
			Assert.Equal(16, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_NoFractional_UtcTz).ValueOrException().LengthRequired);
			Assert.Equal(20, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_NoFractional_FullTz).ValueOrException().LengthRequired);
			Assert.Equal(15, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_NoFractional_LocalTz).ValueOrException().LengthRequired);
			Assert.Equal(10, Iso8601Format.TryCreate(Iso8601Parts.Format_DateOnly).ValueOrException().LengthRequired);
			Assert.Equal(8, Iso8601Format.TryCreate(Iso8601Parts.Format_DateOnlyWithoutSeparators).ValueOrException().LengthRequired);
			Assert.Equal(8, Iso8601Format.TryCreate(Iso8601Parts.Format_DateOrdinal).ValueOrException().LengthRequired);
		}
	}
}
