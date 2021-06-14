namespace MichMcb.CsExt
{
	using System;

	/// <summary>
	/// You can apply this attribute to enum values to give them a specific string representation.
	/// Using <see cref="EnumUtil{T}"/> to convert to and from strings will use the string value defined on the enum.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class EnumStringAttribute : Attribute
	{
		public EnumStringAttribute(string name)
		{
			Name = name;
		}
		public string Name { get; }
	}
}