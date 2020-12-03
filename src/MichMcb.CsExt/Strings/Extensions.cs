namespace MichMcb.CsExt.Strings
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// Provides extension methods for strings and ReadOnlySpan&lt;char&gt;s
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// A hash set containing the results of <see cref="Path.GetInvalidFileNameChars"/>.
		/// </summary>
		public static readonly HashSet<char> invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());
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
		public static bool StartsWith(this string s, char c)
		{
			return s.Length >= 1 && s[0] == c;
		}
		public static bool EndsWith(this string s, char c)
		{
			return s.Length >= 1 && s[s.Length - 1] == c;
		}
#endif
#if !NETSTANDARD2_0
		/// <summary>
		/// Returns a collection of <see cref="Range"/>, which can be used to take slices of <paramref name="str"/>, giving the same effect as splitting a string.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="separator">The char to use to split the string.</param>
		/// <param name="options"></param>
		/// <returns>A collection of <see cref="Range"/> which can be used to take slices of <paramref name="str"/>.</returns>
		public static ICollection<Range> Split(in this ReadOnlySpan<char> str, char separator, StringSplitOptions options = StringSplitOptions.None)
		{
			List<Range> ranges = new List<Range>();
			int from;
			int to = -1;
			while (true)
			{
				from = to + 1;
				to = str[from..].IndexOf(separator);
				if (to != -1)
				{
					if (options == StringSplitOptions.None || to - from >= 0)
					{
						ranges.Add(from..to);
					}
				}
				else
				{
					// Last one
					if (options == StringSplitOptions.None || from == str.Length - 1)
					{
						ranges.Add(from..);
					}
					break;
				}
			}
			return ranges;
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
			List<Range> lineBreaks = new List<Range>();
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
