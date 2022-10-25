#if !NETSTANDARD2_0
namespace MichMcb.CsExt.Strings
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Splits a span into ranges. Works like <see cref="string.Split(char, StringSplitOptions)"/>, only it uses a <see cref="ReadOnlySpan{T}"/> of <see cref="char"/>.
	/// </summary>
	public ref struct SpanSplit
	{
		private int lastSep = -1;
		private int curSep;
		/// <summary>
		/// Creates a new instance, which will split <paramref name="str"/> on <paramref name="separator"/>.
		/// </summary>
		/// <param name="str">The string to split.</param>
		/// <param name="separator">The character that delimits slices.</param>
		/// <param name="options">Whether to trim substrings and remove empty entries.</param>
		public SpanSplit(ReadOnlySpan<char> str, char separator, StringSplitOptions options)
		{
			Str = str;
			Separator = separator;
			Options = options;
			lastSep = -1;
			curSep = 0;
		}
		/// <summary>
		/// The string being split.
		/// </summary>
		public ReadOnlySpan<char> Str { get; }
		/// <summary>
		/// The separator on which to split.
		/// </summary>
		public char Separator { get; }
		/// <summary>
		/// The options to use when splitting.
		/// </summary>
		public StringSplitOptions Options { get; }
		/// <summary>
		/// Gets all the ranges and returns them as a list.
		/// </summary>
		/// <returns></returns>
		public List<Range> All()
		{
			List<Range> result = new();
			while (Next().HasVal(out Range r))
			{
				result.Add(r);
			}
			return result;
		}
		/// <summary>
		/// Gets the next range which can be used to slice the original string.
		/// </summary>
		/// <returns>A range, or no value if there are no more slices.</returns>
		public Opt<Range> Next()
		{
			int from;
			int to;
			for (; curSep < Str.Length; curSep++)
			{
				if (Str[curSep] == Separator)
				{
					// When we find a separator, we want to take a slice of the string: (lastSep, i]
					// So shift lastSep forwards by 1. Since it's half-open, the difference must be larger than 0.
					from = lastSep + 1;
					lastSep = curSep;
					to = curSep;
					if ((Options & StringSplitOptions.TrimEntries) == StringSplitOptions.TrimEntries)
					{
						// We have to trim off the whitespace, meaning keep pushing from forwards and to backwards until it isn't whitespace
						while (from < to && char.IsWhiteSpace(Str[from])) { from++; }
						while (from < to && char.IsWhiteSpace(Str[to])) { to--; }
					}
					if (to - from > 0 || ((Options & StringSplitOptions.RemoveEmptyEntries) != StringSplitOptions.RemoveEmptyEntries))
					{
						// Non-empty
						curSep++;
						return new Opt<Range>(from..to);
					}
				}
			}

			if (curSep == Str.Length)
			{
				// We're at the end of the string, get the last slice from the separator to the end of the string, if we need to.
				curSep++;
				// Bump the value one more time so we won't visit this block a second time
				from = lastSep + 1;
				to = Str.Length;
				if ((Options & StringSplitOptions.TrimEntries) == StringSplitOptions.TrimEntries)
				{
					// Trim off whitespace once more
					while (from < to && char.IsWhiteSpace(Str[from])) { from++; }
				}
				if (to - from > 0 || ((Options & StringSplitOptions.RemoveEmptyEntries) != StringSplitOptions.RemoveEmptyEntries))
				{
					return new Opt<Range>(from..to);
				}
			}
			return default;
		}
	}
}
#endif