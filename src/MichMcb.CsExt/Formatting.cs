namespace MichMcb.CsExt
{
	using System;
	using System.Diagnostics;
	using System.Runtime.CompilerServices;
	/// <summary>
	/// Fast methods for formatting small numbers. 
	/// </summary>
	public static class Formatting
	{
		/// <summary>
		/// Writes a two-digit value to <paramref name="dest"/>. Parameter <paramref name="val"/> must be within range 0 to 99.
		/// This is a private method in corefx, taken from here: <see href="https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Globalization/DateTimeFormat.cs"/> 
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Write2Digits(uint val, Span<char> dest, int offset = 0)
		{
			Debug.Assert(val <= 99);

			uint temp = '0' + val;
			val /= 10;
			dest[offset + 1] = (char)(temp - (val * 10));
			dest[offset] = (char)('0' + val);
		}
		/// <summary>
		/// Writes a four-digit value to <paramref name="dest"/>. Parameter <paramref name="val"/> must be within range 0 to 999.
		/// Based on this: <see href="https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Globalization/DateTimeFormat.cs"/> 
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Write3Digits(uint val, Span<char> dest, int offset = 0)
		{
			Debug.Assert(val <= 999);
			uint temp = '0' + val;
			val /= 10;
			dest[offset + 2] = (char)(temp - (val * 10));

			temp = '0' + val;
			val /= 10;
			dest[offset + 1] = (char)(temp - (val * 10));

			dest[offset] = (char)('0' + val);
		}
		/// <summary>
		/// Writes a four-digit value to <paramref name="dest"/>. Parameter <paramref name="val"/> must be within range 0 to 9999.
		/// This is a private method in corefx, taken from here: <see href="https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Globalization/DateTimeFormat.cs"/> 
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Write4Digits(uint val, Span<char> dest, int offset = 0)
		{
			Debug.Assert(val <= 9999);

			uint temp = '0' + val;
			val /= 10;
			dest[offset + 3] = (char)(temp - (val * 10));

			temp = '0' + val;
			val /= 10;
			dest[offset + 2] = (char)(temp - (val * 10));

			temp = '0' + val;
			val /= 10;
			dest[offset + 1] = (char)(temp - (val * 10));

			dest[offset] = (char)('0' + val);
		}
	}
}