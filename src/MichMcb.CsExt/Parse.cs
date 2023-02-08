namespace MichMcb.CsExt
{
	using System;

	/// <summary>
	/// Methods for parsing.
	/// </summary>
	public static class Parse
	{
		/// <summary>
		/// Method for parsing short numbers that should be entirely digits 0-9.
		/// This mostly exists because .NET Standard 2.0 can't parse a <see cref="ReadOnlySpan{T}"/>. So it's an easy fix and a very slight optimization.
		/// </summary>
		/// <param name="s">The string to parse.</param>
		/// <returns>The parsed integer, or an error message if a non-latin digit was found or the value overflowed.</returns>
		public static Maybe<int, string> LatinInt(ReadOnlySpan<char> s)
		{
			if (s.Length > 10)
			{
				return Compat.StringConcat("String is longer than 10, which is too large for an Int32 to hold: ".AsSpan(), s);
			}
			int result = 0;
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if (c < '0' || c > '9')
				{
					return Compat.StringConcat("Found a non-latin digit in the string: ".AsSpan(), s);
				}
				if ((uint)result > (int.MaxValue / 10))
				{
					return Compat.StringConcat("Value overflowed. String: ".AsSpan(), s);
				}
				result *= 10;
				result += c - '0';
				if (result < 0)
				{
					return Compat.StringConcat("Value overflowed. String: ".AsSpan(), s);
				}
			}
			return result;
		}
	}
}
