﻿#if NETSTANDARD2_0
namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
	internal sealed class AllowNullAttribute : Attribute	{	}
}
#endif