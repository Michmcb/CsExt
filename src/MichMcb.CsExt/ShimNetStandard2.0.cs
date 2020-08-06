#if NETSTANDARD2_0
#pragma warning disable IDE0060 // Remove unused parameter
namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
	public sealed class AllowNullAttribute : Attribute	{	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
	public sealed class DisallowNullAttribute : Attribute {	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
	public sealed class MaybeNull : Attribute { }
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	public sealed class NotNullWhenAttribute : Attribute {
		public NotNullWhenAttribute(bool returnValue) { }
	}
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
	public sealed class NotNullIfNotNullAttribute : Attribute
	{
		public NotNullIfNotNullAttribute(string parameterName) { }
	}
}
#pragma warning restore IDE0060 // Remove unused parameter

namespace System
{
	public static class Shims
	{
		
	}
}
#endif