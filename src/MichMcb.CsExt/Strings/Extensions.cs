using System;
using System.Collections.Generic;
using System.IO;

namespace MichMcb.CsExt.Strings
{
	/// <summary>
	/// Provides extension methods for strings and ReadOnlySpan&lt;char&gt;s
	/// </summary>
	public static class Extensions
	{
		private static readonly HashSet<char> invalidChars = new HashSet<char>(Path.GetInvalidFileNameChars());
		/// <summary>
		/// Strips any invalid filename characters from <paramref name="s"/>. If none were found, returns the original string unchanged.
		/// </summary>
		/// <param name="s"></param>
		public static string StripInvalidFilenameChars(this string s)
		{
			Span<char> newStr = stackalloc char[s.Length];
			int i = 0;
			foreach (char c in s)
			{
				if (!invalidChars.Contains(c))
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
		public static string EnsureStartsWith(this string s, char start)
		{
			if (!s.StartsWith(start))
			{
				return start.ToString() + s;
			}
			return s;
		}
		public static string EnsureStartsWith(this string s, string start)
		{
			if (!s.StartsWith(start))
			{
				return start + s;
			}
			return s;
		}
		public static string EnsureEndsWith(this string s, char end)
		{
			if (!s.EndsWith(end))
			{
				return s + end.ToString();
			}
			return s;
		}
		public static string EnsureEndsWith(this string s, string end)
		{
			if (!s.EndsWith(end))
			{
				return s + end;
			}
			return s;
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
#endif
	}
}
