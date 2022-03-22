﻿namespace MichMcb.CsExt.Test.Dates.Rfc3339
{
	using Xunit;
	using MichMcb.CsExt.Dates;
	public static class Parse
	{
		private static void Check(Rfc3339 r, int year, int month, int day, int hour, int minute, int second, int millis, int timezoneMinutesOffset)
		{
			Assert.Equal(year, r.Year);
			Assert.Equal(month, r.Month);
			Assert.Equal(day, r.Day);
			Assert.Equal(hour, r.Hour);
			Assert.Equal(minute, r.Minute);
			Assert.Equal(second, r.Second);
			Assert.Equal(millis, r.Millis);
			Assert.Equal(timezoneMinutesOffset, r.TimezoneMinutesOffset);
		}
		[Fact]
		public static void Ok()
		{
			Rfc3339 r = Rfc3339.Parse("1234-05-15T10:20:30Z").ValueOrException();
			Check(r, 1234, 5, 15, 10, 20, 30, 0, 0);

			r = Rfc3339.Parse("1234-05-15T10:20:30.1Z").ValueOrException();
			Check(r, 1234, 5, 15, 10, 20, 30, 100, 0);

			r = Rfc3339.Parse("1234-05-15T10:20:30.12Z").ValueOrException();
			Check(r, 1234, 5, 15, 10, 20, 30, 120, 0);

			r = Rfc3339.Parse("1234-05-15T10:20:30.123Z").ValueOrException();
			Check(r, 1234, 5, 15, 10, 20, 30, 123, 0);

			r = Rfc3339.Parse("1234-05-15T10:20:30+10:00").ValueOrException();
			Check(r, 1234, 5, 15, 10, 20, 30, 0, 600);

			r = Rfc3339.Parse("1234-05-15T10:20:30.3+10:00").ValueOrException();
			Check(r, 1234, 5, 15, 10, 20, 30, 300, 600);

			r = Rfc3339.Parse("1234-05-15T10:20:30.32+10:00").ValueOrException();
			Check(r, 1234, 5, 15, 10, 20, 30, 320, 600);

			r = Rfc3339.Parse("1234-05-15T10:20:30.321+10:00").ValueOrException();
			Check(r, 1234, 5, 15, 10, 20, 30, 321, 600);

			r = Rfc3339.Parse("1234-05-15T10:20:30-10:00").ValueOrException();
			Check(r, 1234, 5, 15, 10, 20, 30, 0, -600);

			r = Rfc3339.Parse("1234-05-15T10:20:30.4-10:00").ValueOrException();
			Check(r, 1234, 5, 15, 10, 20, 30, 400, -600);

			r = Rfc3339.Parse("1234-05-15t10:20:30.45-10:00").ValueOrException();
			Check(r, 1234, 5, 15, 10, 20, 30, 450, -600);

			r = Rfc3339.Parse("1234-05-15t10:20:30.456-10:00").ValueOrException();
			Check(r, 1234, 5, 15, 10, 20, 30, 456, -600);

			r = Rfc3339.Parse("1234-05-15 10:20:30.456-10:00", allowSpaceInsteadOfT: true).ValueOrException();
			Check(r, 1234, 5, 15, 10, 20, 30, 456, -600);
		}
		[Fact]
		public static void Fail()
		{
			Assert.Equal("String is too short to be a valid RFC3339 string: abc", Rfc3339.Parse("abc").ErrorOr(null));
			Assert.Equal("Separator at index 4 must be dash (-): 1234x05-15T10:20:30Z", Rfc3339.Parse("1234x05-15T10:20:30Z").ErrorOr(null));
			Assert.Equal("Separator at index 7 must be dash (-): 1234-05x15T10:20:30Z", Rfc3339.Parse("1234-05x15T10:20:30Z").ErrorOr(null));
			Assert.Equal("Separator at index 10 must be T or t: 1234-05-15x10:20:30Z", Rfc3339.Parse("1234-05-15x10:20:30Z").ErrorOr(null));
			Assert.Equal("Separator at index 10 must be T, t, or space: 1234-05-15x10:20:30Z", Rfc3339.Parse("1234-05-15x10:20:30Z", true).ErrorOr(null));
			Assert.Equal("Separator at index 13 must be colon (:): 1234-05-15T10x20:30Z", Rfc3339.Parse("1234-05-15T10x20:30Z").ErrorOr(null));
			Assert.Equal("Separator at index 16 must be colon (:): 1234-05-15T10:20x30Z", Rfc3339.Parse("1234-05-15T10:20x30Z").ErrorOr(null));

			Assert.Equal("Failed to parse year because Found a non-latin digit in the string: 1x34. String: 1x34-05-15T10:20:30Z", Rfc3339.Parse("1x34-05-15T10:20:30Z").ErrorOr(null));
			Assert.Equal("Failed to parse month because Found a non-latin digit in the string: x5. String: 1234-x5-15T10:20:30Z", Rfc3339.Parse("1234-x5-15T10:20:30Z").ErrorOr(null));
			Assert.Equal("Failed to parse day because Found a non-latin digit in the string: x5. String: 1234-05-x5T10:20:30Z", Rfc3339.Parse("1234-05-x5T10:20:30Z").ErrorOr(null));
			Assert.Equal("Failed to parse hour because Found a non-latin digit in the string: 1x. String: 1234-05-15T1x:20:30Z", Rfc3339.Parse("1234-05-15T1x:20:30Z").ErrorOr(null));
			Assert.Equal("Failed to parse minute because Found a non-latin digit in the string: 2x. String: 1234-05-15T10:2x:30Z", Rfc3339.Parse("1234-05-15T10:2x:30Z").ErrorOr(null));
			Assert.Equal("Failed to parse second because Found a non-latin digit in the string: 3x. String: 1234-05-15T10:20:3xZ", Rfc3339.Parse("1234-05-15T10:20:3xZ").ErrorOr(null));

			Assert.Equal("Milliseconds separator was found but no milliseconds were found. String: 1234-05-15T10:20:30.Z", Rfc3339.Parse("1234-05-15T10:20:30.Z").ErrorOr(null));
			Assert.Equal("Timezone designator is not Z, +, or -. String: 1234-05-15T10:20:30F", Rfc3339.Parse("1234-05-15T10:20:30F").ErrorOr(null));

			Assert.Equal("Found end of string when trying to parse timezone: 1234-05-15T10:20:30.123", Rfc3339.Parse("1234-05-15T10:20:30.123").ErrorOr(null));

			Assert.Equal("String has Z as a timezone, but has extra chars following the end of the string: 1234-05-15T10:20:30.123Z1", Rfc3339.Parse("1234-05-15T10:20:30.123Z1").ErrorOr(null));
			Assert.Equal("String has +/- as a timezone, but does not have exactly 5 chars (HH:mm) following the +/-: 1234-05-15T10:20:30.123+1", Rfc3339.Parse("1234-05-15T10:20:30.123+1").ErrorOr(null));
			Assert.Equal("Separator for timezone must be colon (:): 1234-05-15T10:20:30.123+10x00", Rfc3339.Parse("1234-05-15T10:20:30.123+10x00").ErrorOr(null));

			Assert.Equal("Failed to parse timezone hours because Found a non-latin digit in the string: 1x. String: 1234-05-15T10:20:30.123+1x:00", Rfc3339.Parse("1234-05-15T10:20:30.123+1x:00").ErrorOr(null));
			Assert.Equal("Failed to parse timezone minutes because Found a non-latin digit in the string: x0. String: 1234-05-15T10:20:30.123+10:x0", Rfc3339.Parse("1234-05-15T10:20:30.123+10:x0").ErrorOr(null));

		}
	}
}