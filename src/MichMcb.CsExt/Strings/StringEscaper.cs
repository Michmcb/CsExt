namespace MichMcb.CsExt.Strings
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	/// <summary>
	/// A class which can escape characters. For example, it can turn a \ into a \\, or a &lt; into a &amp;lt;
	/// </summary>
	public sealed class StringEscaper
	{
		/// <summary>
		/// Creates a new instance, with the provided escape sequences.
		/// The key should be the character that must be escaped, and the value is the sequence
		/// of characters which can escape that string.
		/// </summary>
		/// <param name="escapeSequences">Escape sequences for certain characters.</param>
		public StringEscaper(IReadOnlyDictionary<char, string> escapeSequences)
		{
			CharEscapeSequences = escapeSequences;
		}
		/// <summary>
		/// The escape sequences used.
		/// </summary>
		public IReadOnlyDictionary<char, string> CharEscapeSequences { get; }
		/// <summary>
		/// Writes <paramref name="str"/> to <paramref name="target"/>. If any characters are found that need to be escaped, then the escape
		/// sequence is written instead of the character.
		/// </summary>
		/// <param name="str">The string to write to <paramref name="target"/>, escaped.</param>
		/// <param name="target">The Stream to which the escaped string is written.</param>
		/// <returns>The actual number of characters written.</returns>
		public int WriteEscapedString(in ReadOnlySpan<char> str, TextWriter target)
		{
			int w = 0;
			foreach (char c in str)
			{
				if (!CharEscapeSequences.TryGetValue(c, out string seq))
				{
					++w;
					target.Write(c);
				}
				else
				{
					w += seq.Length;
					target.Write(seq);
				}
			}
			return w;
		}
		/// <summary>
		/// Writes <paramref name="str"/> to <paramref name="target"/>. If any characters are found that need to be escaped, then the escape
		/// sequence is written instead of the character.
		/// </summary>
		/// <param name="str">The string to write to <paramref name="target"/>, escaped.</param>
		/// <param name="target">The StringBuilder to which the escaped string is written.</param>
		/// <returns>The actual number of characters written.</returns>
		public int WriteEscapedString(in ReadOnlySpan<char> str, StringBuilder target)
		{
			int w = 0;
			foreach (char c in str)
			{
				if (!CharEscapeSequences.TryGetValue(c, out string seq))
				{
					++w;
					target.Append(c);
				}
				else
				{
					w += seq.Length;
					target.Append(seq);
				}
			}
			return w;
		}
		/// <summary>
		/// Writes <paramref name="str"/> to <paramref name="target"/>. If any characters are found that need to be escaped, then the escape
		/// sequence is written instead of the character.
		/// </summary>
		/// <param name="str">The string to write to <paramref name="target"/>, escaped.</param>
		/// <param name="target">The Span to which the escaped string is written.</param>
		/// <returns>The actual number of characters written.</returns>
		public int WriteEscapedString(in ReadOnlySpan<char> str, Span<char> target)
		{
			int w = 0;
			foreach (char c in str)
			{
				if (!CharEscapeSequences.TryGetValue(c, out string seq))
				{
					target[w++] = c;
				}
				else
				{
					foreach (char seqc in seq)
					{
						target[w++] = seqc;
					}
				}
			}
			return w;
		}
		/// <summary>
		/// Creates a new instance which can escape JSON Property names and strings.
		/// Won't use any unicode escapes (i.e. \u1C2A)
		/// </summary>
		public static StringEscaper Json()
		{
			return new StringEscaper(new Dictionary<char, string>() 
			{
				{ '"', "\\\"" },
				{ '\\', @"\\" },
				{ '/', @"\/" },
				{ '\b', @"\b" }, // Backspace
				{ '\f', @"\f" }, // Form Feed
				{ '\n', @"\n" }, // Newline
				{ '\r', @"\r" }, // Carriage Return
				{ '\t', @"\t" }, // Horizontal Tab
			});
		}
		/// <summary>
		/// Creates a new instance which can escape XML and HTML text, but NOT attribute values (That is, it won't escape " or ').
		/// If you want to escape attributes, use <see cref="XmlBodyAndAttribute"/>.
		/// </summary>
		public static StringEscaper XmlBody()
		{
			return new StringEscaper(new Dictionary<char, string>()
			{
				{ '&', "&amp;" },
				{ '<', "&lt;" },
				{ '>', "&gt;" },
			});
		}
		/// <summary>
		/// Creates a new instance which can escape XML and HTML text and attribute values.
		/// </summary>
		public static StringEscaper XmlBodyAndAttribute()
		{
			return new StringEscaper(new Dictionary<char, string>()
			{
				{ '&', "&amp;" },
				{ '<', "&lt;" },
				{ '>', "&gt;" },
				{ '"', "&quot;" },
				{ '\'', "&#39;" },
			});
		}
	}
}
