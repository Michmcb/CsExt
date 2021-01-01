#if NETSTANDARD2_0
#pragma warning disable IDE0060 // Remove unused parameter
namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
	internal sealed class AllowNullAttribute : Attribute	{	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
	internal sealed class DisallowNullAttribute : Attribute {	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
	internal sealed class MaybeNullAttribute : Attribute { }
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	internal sealed class NotNullWhenAttribute : Attribute {
		internal NotNullWhenAttribute(bool returnValue) { }
	}
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
	internal sealed class NotNullIfNotNullAttribute : Attribute
	{
		internal NotNullIfNotNullAttribute(string parameterName) { }
	}
}
namespace MichMcb.CsExt
{
	using System;
	public delegate void SpanAction<T, in TArg>(Span<T> span, TArg arg);
	public static class Shim
	{
		/// <summary>
		/// Shim for string.Create
		/// </summary>
		public static string StringCreate<TState>(int length, TState state, SpanAction<char, TState> action)
		{
			char[] str = new char[length];
			action(str, state);
			return new string(str);
		}
		/// <summary>
		/// Shim for string.Concat to be able to concatenate some <see cref="ReadOnlySpan{T}"/>
		/// </summary>
		public static string StringConcat(in ReadOnlySpan<char> span1, in ReadOnlySpan<char> span2)
		{
			char[] str = new char[span1.Length + span2.Length];
			span1.CopyTo(str);
			span2.CopyTo(((Span<char>)str).Slice(span1.Length));
			return new string(str);
		}
	}
}
#pragma warning restore IDE0060 // Remove unused parameter
#endif