namespace MichMcb.CsExt.Test.Dates.Iso8601Format
{
	using Xunit;
	using MichMcb.CsExt.Dates;

	public static class PredefinedFormats
	{
		[Fact]
		public static void AreCorrect()
		{
			Assert.Equal(24, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_UtcTz, 3).ValueOrException().LengthRequired);
			Assert.Equal(29, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_FullTz, 3).ValueOrException().LengthRequired);
			Assert.Equal(23, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_LocalTz, 3).ValueOrException().LengthRequired);
			Assert.Equal(20, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_NoFractional_UtcTz, 0).ValueOrException().LengthRequired);
			Assert.Equal(25, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_NoFractional_FullTz, 0).ValueOrException().LengthRequired);
			Assert.Equal(19, Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_NoFractional_LocalTz, 0).ValueOrException().LengthRequired);
			Assert.Equal(20, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_UtcTz, 3).ValueOrException().LengthRequired);
			Assert.Equal(24, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_FullTz, 3).ValueOrException().LengthRequired);
			Assert.Equal(19, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_LocalTz, 3).ValueOrException().LengthRequired);
			Assert.Equal(16, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_NoFractional_UtcTz, 0).ValueOrException().LengthRequired);
			Assert.Equal(20, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_NoFractional_FullTz, 0).ValueOrException().LengthRequired);
			Assert.Equal(15, Iso8601Format.TryCreate(Iso8601Parts.Format_BasicFormat_NoFractional_LocalTz, 0).ValueOrException().LengthRequired);
			Assert.Equal(10, Iso8601Format.TryCreate(Iso8601Parts.Format_DateOnly, 0).ValueOrException().LengthRequired);
			Assert.Equal(8, Iso8601Format.TryCreate(Iso8601Parts.Format_DateOnlyWithoutSeparators, 0).ValueOrException().LengthRequired);
			Assert.Equal(8, Iso8601Format.TryCreate(Iso8601Parts.Format_DateOrdinal, 0).ValueOrException().LengthRequired);
		}
	}
}
