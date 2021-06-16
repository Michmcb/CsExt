namespace MichMcb.CsExt
{
	using System;
	/// <summary>
	/// Shorthand static class for calling through to <see cref="EnumUtil{T}.Inst"/>.
	/// Make sure you have compiled specific instances of <see cref="EnumUtil{T}"/> first! If you have not, you'll get a <see cref="NullReferenceException"/>.
	/// </summary>
	public static class EnumUtil
	{
		/// <summary>
		/// Calls <see cref="EnumUtil{T}.ToStringFunc"/>.
		/// Turns <paramref name="value"/> into a string.
		/// </summary>
		/// <param name="value">The value to turn into a string.</param>
		/// <returns>A string representation of <paramref name="value"/>.</returns>
		public static string ToString<T>(T value) where T : struct
		{
			return EnumUtil<T>.Inst.ToStringFunc(value);
		}
		/// <summary>
		/// Calls <see cref="EnumUtil{T}.IsDefinedFunc"/>.
		/// Determines if <paramref name="value"/> is a valid value of <typeparamref name="T"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>true if defined, false otherwise.</returns>
		public static bool IsDefined<T>(T value) where T : struct
		{
			return EnumUtil<T>.Inst.IsDefinedFunc(value);
		}
		/// <summary>
		/// Calls <see cref="EnumUtil{T}.TryParse(string)"/>.
		/// Uses <see cref="EnumUtil{T}.NameToValue"/> to look up <paramref name="str"/>.
		/// </summary>
		/// <param name="str">The raw string.</param>
		/// <returns>The parsed value, or an error message.</returns>
		public static Maybe<T, string> TryParse<T>(string str) where T : struct
		{
			return EnumUtil<T>.Inst.TryParse(str);
		}
	}
}