#if NETSTANDARD2_0
namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
	internal sealed class MaybeNullWhenAttribute : Attribute
	{
		public MaybeNullWhenAttribute(bool returnValue)
		{
			ReturnValue = returnValue;
		}
		public bool ReturnValue { get; }
	}
}
#endif