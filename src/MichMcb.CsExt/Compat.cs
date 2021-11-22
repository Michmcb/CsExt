namespace MichMcb.CsExt
{
	using System;
	using System.Buffers;

	/// <summary>
	/// Compatibility methods, useful for .netstandard2.0
	/// </summary>
	public static class Compat
	{
		/// <summary>
		/// Shim for string.Create. Creates an array of chars, invokes <paramref name="action"/>, and then creates a new string from the array.
		/// </summary>
		public static string StringCreate<TState>(int length, TState state, SpanAction<char, TState> action)
		{
			char[] str = new char[length];
			action(str, state);
			return new string(str);
		}
#if NETSTANDARD2_0
		/// <summary>
		/// Shim for string.Concat to be able to concatenate some <see cref="ReadOnlySpan{T}"/>
		/// </summary>
		public static string StringConcat(in ReadOnlySpan<char> span0, in ReadOnlySpan<char> span1)
		{
			char[] chars = new char[span0.Length + span1.Length];
			Span<char> str = chars;
			span0.CopyTo(str);
			span1.CopyTo(str.Slice(span0.Length));
			return new string(chars);
		}
		/// <summary>
		/// Shim for string.Concat to be able to concatenate some <see cref="ReadOnlySpan{T}"/>
		/// </summary>
		public static string StringConcat(in ReadOnlySpan<char> span0, in ReadOnlySpan<char> span1, in ReadOnlySpan<char> span2)
		{
			char[] chars = new char[span0.Length + span1.Length + span2.Length];
			Span<char> str = chars;
			span0.CopyTo(str);
			span1.CopyTo(str.Slice(span0.Length));
			span2.CopyTo(str.Slice(span0.Length + span1.Length));
			return new string(chars);
		}
		/// <summary>
		/// Shim for string.Concat to be able to concatenate some <see cref="ReadOnlySpan{T}"/>
		/// </summary>
		public static string StringConcat(in ReadOnlySpan<char> span0, in ReadOnlySpan<char> span1, in ReadOnlySpan<char> span2, in ReadOnlySpan<char> span3)
		{
			char[] chars = new char[span0.Length + span1.Length + span2.Length + span3.Length];
			Span<char> str = chars;
			span0.CopyTo(str);
			span1.CopyTo(str.Slice(span0.Length));
			span2.CopyTo(str.Slice(span0.Length + span1.Length));
			span3.CopyTo(str.Slice(span0.Length + span1.Length + span2.Length));
			return new string(chars);
		}
#else
		/// <summary>
		/// Calls <see cref="string.Concat(ReadOnlySpan{char}, ReadOnlySpan{char})"/>.
		/// </summary>
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public static string StringConcat(in ReadOnlySpan<char> span0, in ReadOnlySpan<char> span1)
		{
			return string.Concat(span0, span1);
		}
		/// <summary>
		/// Calls <see cref="string.Concat(ReadOnlySpan{char}, ReadOnlySpan{char}, ReadOnlySpan{char})"/>.
		/// </summary>
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public static string StringConcat(in ReadOnlySpan<char> span0, in ReadOnlySpan<char> span1, in ReadOnlySpan<char> span2)
		{
			return string.Concat(span0, span1, span2);
		}
		/// <summary>
		/// Calls <see cref="string.Concat(ReadOnlySpan{char}, ReadOnlySpan{char}, ReadOnlySpan{char}, ReadOnlySpan{char})"/>.
		/// </summary>
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public static string StringConcat(in ReadOnlySpan<char> span0, in ReadOnlySpan<char> span1, in ReadOnlySpan<char> span2, in ReadOnlySpan<char> span3)
		{
			return string.Concat(span0, span1, span2, span3);
		}
#endif
	}
}