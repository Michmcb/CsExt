namespace MichMcb.CsExt
{
	using System;
	/// <summary>
	/// Methods for parsing.
	/// </summary>
	public static class Parse
	{
		/// <summary>
		/// Optimized method for parsing short numbers that should be entirely digits 0-9.
		/// This mostly exists because .NET Standard 2.0 can't parse a <see cref="ReadOnlySpan{T}"/>. So it's both an easy fix and an optimization.
		/// This is about 4.5x faster than .NET's int.Parse (but of course way less flexible)
		/// </summary>
		/// <param name="s">The string to parse.</param>
		/// <returns>The parsed integer, or an error message if a non-latin digit was found.</returns>
		public static Maybe<int, string> LatinInt(ReadOnlySpan<char> s)
		{
			int result = 0;
			int mult = 1;
			for (int i = s.Length - 1; i >= 0; i--)
			{
				char c = s[i];
				if (c < '0' || c > '9')
				{
					return "Found a non-latin digit in the string: " +
#if NETSTANDARD2_0
						s.ToString();
#else
						new string(s);
#endif
				}
				result += (c - '0') * mult;
				mult *= 10;
			}
			return result;
		}
	}
}
