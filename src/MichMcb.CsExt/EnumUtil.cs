namespace MichMcb.CsExt
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Shorthand static class for calling through to <see cref="EnumUtil{T}.Inst"/>.
	/// Make sure you have compiled specific instances of <see cref="EnumUtil{T}"/> first! If you have not, you'll get a <see cref="NullReferenceException"/>.
	/// </summary>
	public static class EnumUtil
	{
		/// <summary>
		/// Calls <see cref="EnumUtil{T}.NameValues"/>.
		/// Enumerates through all of the string representations and values of <typeparamref name="T"/>.
		/// </summary>
		/// <returns>The values and string representations of <typeparamref name="T"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<NameValue<T>> NameValues<T>() where T : struct
		{
			return EnumUtil<T>.Inst.NameValues();
		}
		/// <summary>
		/// Calls <see cref="EnumUtil{T}.GetNameValues"/>.
		/// Creates a new <see cref="IReadOnlyList{NameValue}"/> with all names/values.
		/// </summary>
		/// <returns>The values and string representations of <typeparamref name="T"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<NameValue<T>> GetNameValues<T>() where T : struct
		{
			return EnumUtil<T>.Inst.GetNameValues();
		}
		/// <summary>
		/// Calls <see cref="EnumUtil{T}.Values"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<T> Values<T>() where T : struct
		{
			return EnumUtil<T>.Inst.Values;
		}
		/// <summary>
		/// Calls <see cref="EnumUtil{T}.Names"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<string> Names<T>() where T : struct
		{
			return EnumUtil<T>.Inst.Names;
		}
		/// <summary>
		/// Calls <see cref="EnumUtil{T}.ToStringFunc"/>.
		/// Turns <paramref name="value"/> into a string.
		/// </summary>
		/// <param name="value">The value to turn into a string.</param>
		/// <returns>A string representation of <paramref name="value"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDefined<T>(T value) where T : struct
		{
			return EnumUtil<T>.Inst.IsDefinedFunc(value);
		}
		/// <summary>
		/// Calls <see cref="EnumUtil{T}.HasFlagFunc"/>.
		/// Determines if <paramref name="value"/> has the provided <paramref name="flag"/> set.
		/// </summary>
		/// <param name="value">The value to check for a flag.</param>
		/// <param name="flag">The flag value.</param>
		/// <returns>true if the flag is set, false otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasFlag<T>(T value, T flag) where T : struct
		{
			return EnumUtil<T>.Inst.HasFlagFunc(value, flag);
		}
		/// <summary>
		/// Calls <see cref="EnumUtil{T}.TryParse(string)"/>.
		/// Uses <see cref="EnumUtil{T}.NameToValue"/> to look up <paramref name="str"/>.
		/// </summary>
		/// <param name="str">The raw string.</param>
		/// <returns>The parsed value, or an error message.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Maybe<T, string> TryParse<T>(string str) where T : struct
		{
			return EnumUtil<T>.Inst.TryParse(str);
		}
	}
}