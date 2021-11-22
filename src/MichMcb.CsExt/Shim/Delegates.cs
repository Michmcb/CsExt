#if NETSTANDARD2_0
namespace System.Buffers
{
	/// <summary>
	/// Shim for SpanAction which is not present in .netstandard 2.0
	/// </summary>
	public delegate void SpanAction<T, in TArg>(Span<T> span, TArg arg);
}
#endif