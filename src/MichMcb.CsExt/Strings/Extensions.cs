namespace MichMcb.CsExt.Strings
{
	using System;
	using System.Buffers;
	using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// Provides extension methods for <see cref="string"/> and <see cref="ReadOnlySpan{T}"/>s of <see cref="char"/>.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// A hash set containing the results of <see cref="Path.GetInvalidFileNameChars"/>.
		/// </summary>
		public static readonly HashSet<char> invalidFileNameChars = new(Path.GetInvalidFileNameChars());
		/// <summary>
		/// Strips any invalid filename characters from <paramref name="s"/>.
		/// </summary>
		/// <param name="s">The string to remove invalid filename chars from.</param>
		/// <returns>A new string with invalid filename chars removed.</returns>
		public static string StripInvalidFileNameChars(this string s)
		{
			if (s.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
			{
				return s;
			}
			Span<char> newStr = stackalloc char[s.Length];
			int i = 0;
			foreach (char c in s)
			{
				if (!invalidFileNameChars.Contains(c))
				{
					newStr[i++] = c;
				}
			}
#if NETSTANDARD2_0
			return new string(newStr.Slice(0, i).ToArray());
#else
			return new string(newStr.Slice(0, i));
#endif
		}
		/// <summary>
		/// Prepends <paramref name="s"/> with <paramref name="start"/>, if it doesn't start with <paramref name="start"/> already.
		/// </summary>
		/// <param name="s">The string.</param>
		/// <param name="start">The char to make sure <paramref name="s"/> starts with.</param>
		/// <returns><paramref name="s"/> with <paramref name="start"/> guaranteed to be prepended.</returns>
		public static string EnsureStartsWith(this string s, char start)
		{
			return !s.StartsWith(start) ? start + s : s;
		}
		/// <summary>
		/// Prepends <paramref name="s"/> with <paramref name="start"/>, if it doesn't start with <paramref name="start"/> already.
		/// </summary>
		/// <param name="s">The string.</param>
		/// <param name="start">The string to make sure <paramref name="s"/> starts with.</param>
		/// <returns><paramref name="s"/> with <paramref name="start"/> guaranteed to be prepended.</returns>
		public static string EnsureStartsWith(this string s, string start)
		{
			return !s.StartsWith(start) ? start + s : s;
		}
		/// <summary>
		/// Appends <paramref name="s"/> with <paramref name="end"/>, if it doesn't end with <paramref name="end"/> already.
		/// </summary>
		/// <param name="s">The string.</param>
		/// <param name="end">The char to make sure <paramref name="s"/> ends with.</param>
		/// <returns><paramref name="s"/> with <paramref name="end"/> guaranteed to be appended.</returns>
		public static string EnsureEndsWith(this string s, char end)
		{
			return !s.EndsWith(end) ? s + end : s;
		}
		/// <summary>
		/// Appends <paramref name="s"/> with <paramref name="end"/>, if it doesn't end with <paramref name="end"/> already.
		/// </summary>
		/// <param name="s">The string.</param>
		/// <param name="end">The string to make sure <paramref name="s"/> ends with.</param>
		/// <returns><paramref name="s"/> with <paramref name="end"/> guaranteed to be appended.</returns>
		public static string EnsureEndsWith(this string s, string end)
		{
			return !s.EndsWith(end) ? s + end : s;
		}
		/// <summary>
		/// Truncates <paramref name="s"/> to <paramref name="length"/>.
		/// </summary>
		/// <param name="s">The string.</param>
		/// <param name="length">The max length</param>
		/// <returns>A substring of <paramref name="s"/> if it is longer than <paramref name="length"/>, otherwise returns <paramref name="s"/>.</returns>
		public static string Truncate(this string s, int length)
		{
			return s.Length > length ? s.Substring(0, length) : s;
		}
		/// <summary>
		/// Truncates <paramref name="s"/> to <paramref name="length"/>.
		/// </summary>
		/// <param name="s">The string.</param>
		/// <param name="length">The max length</param>
		/// <returns>A slice of <paramref name="s"/> if it is longer than <paramref name="length"/>, otherwise returns <paramref name="s"/>.</returns>
		public static Span<char> Truncate(this Span<char> s, int length)
		{
			return s.Length > length ? s.Slice(0, length) : s;
		}
		/// <summary>
		/// Truncates <paramref name="s"/> to <paramref name="length"/>.
		/// </summary>
		/// <param name="s">The string.</param>
		/// <param name="length">The max length</param>
		/// <returns>A slice of <paramref name="s"/> if it is longer than <paramref name="length"/>, otherwise returns <paramref name="s"/>.</returns>
		public static ReadOnlySpan<char> Truncate(this ReadOnlySpan<char> s, int length)
		{
			return s.Length > length ? s.Slice(0, length) : s;
		}
#if NETSTANDARD2_0
		/// <summary>
		/// Checks if the string starts with <paramref name="c"/>.
		/// </summary>
		/// <param name="s">The string.</param>
		/// <param name="c">The character.</param>
		/// <returns>True if string starts with <paramref name="c"/>, false otherwise.</returns>
		public static bool StartsWith(this string s, char c)
		{
			return s.Length >= 1 && s[0] == c;
		}
		/// <summary>
		/// Checks if the string ends with <paramref name="c"/>.
		/// </summary>
		/// <param name="s">The string.</param>
		/// <param name="c">The character.</param>
		/// <returns>True if string ends with <paramref name="c"/>, false otherwise.</returns>
		public static bool EndsWith(this string s, char c)
		{
			return s.Length >= 1 && s[s.Length - 1] == c;
		}
#endif
#if !NETSTANDARD2_0
		/// <summary>
		/// Splits the <paramref name="str"/> into ranges, and invokes <paramref name="callback"/> once for every range that is found, passing the slice of <paramref name="str"/>
		/// as well as the range that was used to create that slice of the string.
		/// </summary>
		/// <param name="str">The string to split.</param>
		/// <param name="separator">The character on which to split.</param>
		/// <param name="state">The state to pass in.</param>
		/// <param name="options">he options.</param>
		/// <param name="callback">The callback invoked once for every slice of the string.</param>
		public static void Split<T>(in this ReadOnlySpan<char> str, char separator, T state, StringSplitOptions options, ReadOnlySpanAction<char, T> callback)
		{
			bool removeEmpty = (options & StringSplitOptions.RemoveEmptyEntries) == StringSplitOptions.RemoveEmptyEntries;
			bool trim = (options & StringSplitOptions.TrimEntries) == StringSplitOptions.TrimEntries;
			int lastSep = -1;
			int from;
			int to;
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] == separator)
				{
					// When we find a separator, we want to take a slice of the string: (lastSep, i]
					// So shift lastSep forwards by 1. Since it's half-open, the difference must be larger than 0.
					from = lastSep + 1;
					to = i;
					if (trim)
					{
						// We have to trim off the whitespace, meaning keep pushing from forwards and to backwards until it isn't whitespace
						while (from < to && char.IsWhiteSpace(str[from])) { from++; }
						while (from < to && char.IsWhiteSpace(str[to])) { to--; }
					}
					if (to - from > 0 || !removeEmpty)
					{
						// Non-empty
						callback(str[from..to], state);
					}
					lastSep = i;
				}
			}

			// We're at the end of the string, so if we need
			from = lastSep + 1;
			to = str.Length;
			if (trim)
			{
				// Trim off whitespace once more
				while (from < to && char.IsWhiteSpace(str[from])) { from++; }
			}
			if (to - from > 0 || !removeEmpty)
			{
				callback(str[from..to], state);
			}
		}
		/// <summary>
		/// Returns substrings of <paramref name="str"/>, where each substring is no longer than <paramref name="maxLineLength"/>.
		/// This will try to avoid breaking in the middle of a word, preferring to break in whitespace instead.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="maxLineLength">The maximum length of the lines</param>
		/// <returns>A collection of substrings.</returns>
		public static ICollection<string> Lines(this string str, int maxLineLength)
		{
			ICollection<Range> ranges = Lines(str.AsSpan(), maxLineLength);
			string[] lines = new string[ranges.Count];
			int i = 0;
			foreach (Range r in ranges)
			{
				lines[i++] = str[r];
			}
			return lines;
		}
		/// <summary>
		/// Returns ranges of <paramref name="str"/>, where each slice is no longer than <paramref name="maxLineLength"/>.
		/// This will try to avoid breaking in the middle of a word, preferring to break in whitespace instead.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="maxLineLength">The maximum length of the lines</param>
		/// <returns>A collection of <see cref="Range"/> which can be used to take slices of <paramref name="str"/>.</returns>
		public static ICollection<Range> Lines(in this ReadOnlySpan<char> str, int maxLineLength)
		{
			List<Range> lineBreaks = new();
			int currentIndex;
			int prevIndex = 0;
			while (str.Length >= prevIndex)
			{
				currentIndex = prevIndex + maxLineLength;
				if (currentIndex < str.Length)
				{
					char c = str[currentIndex];
					if (char.IsWhiteSpace(c))
					{
						// We can break at this char
						lineBreaks.Add(prevIndex..currentIndex);
						prevIndex = currentIndex + 1;
					}
					else
					{
						// If not, then we need to search backwards for a whitespace character.
						// If we don't find any, then just split up this word.
						currentIndex = LastIndexOfWhitespace(str, currentIndex, prevIndex);
						if (currentIndex != -1)
						{
							lineBreaks.Add(prevIndex..currentIndex);
							prevIndex = currentIndex + 1;
						}
						else
						{
							lineBreaks.Add(prevIndex..currentIndex);
							prevIndex += maxLineLength;
						}
					}
				}
				else
				{
					lineBreaks.Add(prevIndex..str.Length);
					prevIndex = currentIndex;
				}
			}
			return lineBreaks;

			static int LastIndexOfWhitespace(in ReadOnlySpan<char> str, int start, int finish)
			{
				for (int i = start; i >= finish; i--)
				{
					if (char.IsWhiteSpace(str[i]))
					{
						return i;
					}
				}
				return -1;
			}
		}
#endif
	}
}
