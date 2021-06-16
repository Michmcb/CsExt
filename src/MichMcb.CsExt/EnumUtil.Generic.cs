namespace MichMcb.CsExt
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;

	/// <summary>
	/// A class which provides helpful utilities for an enum of type <typeparamref name="T"/>, such as parsing and quick ToString/HasFlag/IsDefined calls.
	/// It can compile an expression tree for specific enum types.
	/// </summary>
	/// <typeparam name="T">The type of the enum</typeparam>
	public sealed class EnumUtil<T> where T : struct
	{
		private readonly string TypeName;
		/// <summary>
		/// The compiled instance for <typeparamref name="T"/>.
		/// This is never set by the class. You must set it yourself if you want to use it.
		/// </summary>
		public static EnumUtil<T> Inst { get; set; } = null!;
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="toString">The function to return a string representation of <typeparamref name="T"/>.</param>
		/// <param name="isDefined">The function to tell if a value of <typeparamref name="T"/> is a defined enum type.</param>
		/// <param name="hasFlag">The function used to tell if a value of <typeparamref name="T"/> has a certain flag defined.</param>
		/// <param name="nameToValue">A dictionary of strings to values, used for parsing.</param>
		public EnumUtil(Func<T, string> toString, Func<T, bool> isDefined, Func<T, T, bool> hasFlag, IReadOnlyDictionary<string, T> nameToValue)
		{
			TypeName = typeof(T).Name;
			ToStringFunc = toString;
			IsDefinedFunc = isDefined;
			HasFlagFunc = hasFlag;
			NameToValue = nameToValue;
		}
		/// <summary>
		/// The underlying function to use to create string representations of <typeparamref name="T"/>.
		/// </summary>
		public Func<T, string> ToStringFunc { get; }
		/// <summary>
		/// The underlying function to use to check if a value is a valid representation of <typeparamref name="T"/>.
		/// </summary>
		public Func<T, bool> IsDefinedFunc { get; }
		/// <summary>
		/// The underlying function to use to check if a value of <typeparamref name="T"/> has a certain flag set.
		/// </summary>
		public Func<T, T, bool> HasFlagFunc { get; }
		/// <summary>
		/// A dictionary of strings to values, used when parsing string representations of <typeparamref name="T"/>.
		/// </summary>
		public IReadOnlyDictionary<string, T> NameToValue { get; }
		/// <summary>
		/// Calls <see cref="ToStringFunc"/>.
		/// Turns <paramref name="value"/> into a string.
		/// </summary>
		/// <param name="value">The value to turn into a string.</param>
		/// <returns>A string representation of <paramref name="value"/>.</returns>
		public string ToString(T value)
		{
			return ToStringFunc(value);
		}
		/// <summary>
		/// Calls <see cref="IsDefinedFunc"/>.
		/// Determines if <paramref name="value"/> is a valid value of <typeparamref name="T"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>true if defined, false otherwise.</returns>
		public bool IsDefined(T value)
		{
			return IsDefinedFunc(value);
		}
		/// <summary>
		/// Calls <see cref="HasFlagFunc"/>.
		/// Determines if <paramref name="value"/> has the specified <paramref name="flag"/> set.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <param name="flag">The flag.</param>
		/// <returns>true if <paramref name="flag"/> is set in <paramref name="value"/>, false otherwise.</returns>
		public bool HasFlag(T value, T flag)
		{
			return HasFlagFunc(value, flag);
		}
		/// <summary>
		/// Uses <see cref="NameToValue"/> to look up <paramref name="str"/>.
		/// </summary>
		/// <param name="str">The raw string.</param>
		/// <returns>The parsed value, or an error message.</returns>
		public Maybe<T, string> TryParse(string str)
		{
			return NameToValue.TryGetValue(str, out T val) ? val : string.Concat(str, " cannot be parsed as an enum of type ", TypeName);
		}
		/// <summary>
		/// Compiles an instance of <see cref="EnumUtil{T}"/>. Uses <see cref="EnumStringAttribute"/> if it exists to determine
		/// the strings to use when parsing and turning <typeparamref name="T"/> into a string. Otherwise, uses the member name, or <see cref="EnumStringAttribute"/> if one is defined for that member.
		/// String comparisons use <see cref="StringComparer.Ordinal"/>, and the default value for ToString is <see cref="string.Empty"/>.
		/// HasFlag does this: (value &amp; flag) == flag
		/// </summary>
		/// <returns>An instance of <see cref="EnumUtil{T}"/> with compiled functions, or an error message.</returns>
		public static Maybe<EnumUtil<T>, string> Compile()
		{
			return Compile(string.Empty, StringComparer.Ordinal);
		}
		/// <summary>
		/// Compiles an instance of <see cref="EnumUtil{T}"/>. Uses <see cref="EnumStringAttribute"/> if it exists to determine
		/// the strings to use when parsing and turning <typeparamref name="T"/> into a string. Otherwise, uses the member name, or <see cref="EnumStringAttribute"/> if one is defined for that member.
		/// String comparisons use <see cref="StringComparer.Ordinal"/>.
		/// HasFlag does this: (value &amp; flag) == flag
		/// </summary>
		/// <param name="defaultStringValue">The string value to return when turning <typeparamref name="T"/> into a string, if the value is not defined.</param>
		/// <returns>An instance of <see cref="EnumUtil{T}"/> with compiled functions, or an error message.</returns>
		public static Maybe<EnumUtil<T>, string> Compile(string defaultStringValue)
		{
			return Compile(defaultStringValue, StringComparer.Ordinal);
		}
		/// <summary>
		/// Compiles an instance of <see cref="EnumUtil{T}"/>. Uses <see cref="EnumStringAttribute"/> if it exists to determine
		/// the strings to use when parsing and turning <typeparamref name="T"/> into a string. Otherwise, uses the member name, or <see cref="EnumStringAttribute"/> if one is defined for that member.
		/// HasFlag does this: (value &amp; flag) == flag
		/// </summary>
		/// <param name="defaultStringValue">The string value to return when turning <typeparamref name="T"/> into a string, if the value is not defined.</param>
		/// <param name="comparer">The comparer to use for parsing.</param>
		/// <returns>An instance of <see cref="EnumUtil{T}"/> with compiled functions, or an error message.</returns>
		public static Maybe<EnumUtil<T>, string> Compile(string defaultStringValue, StringComparer comparer)
		{
			Dictionary<string, T> dict = new(comparer);
			FieldInfo[] infos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			for (int i = 0; i < infos.Length; i++)
			{
				FieldInfo fi = infos[i];
				EnumStringAttribute? customName = fi.GetCustomAttribute<EnumStringAttribute>(inherit: false);
				string name = customName == null ? fi.Name : customName.Name;
				T val = (T)fi.GetRawConstantValue()!;
				if (dict.ContainsKey(name))
				{
					return string.Concat("The string representation for enum value ", val, " appears multiple times: ", name);
				}
				dict[name] = val;
			}
			if (dict.Count != infos.Length)
			{
				return "Some enum values have the same string representation with regards to the provided StringComparer. Check to see if there are any duplicate EnumStringAttributes applied, or if some enum fields only differ on case.";
			}

			Func<T, string> toString = CompileToString(dict, defaultStringValue);
			Func<T, bool> isDefined = CompileIsDefined(dict.Values);
			Func<T, T, bool> hasFlag = CompileHasFlag(Enum.GetUnderlyingType(typeof(T)));
			return new EnumUtil<T>(toString, isDefined, hasFlag, dict);
		}
		/// <summary>
		/// Compiles an instance of <see cref="EnumUtil{T}"/>. Uses the provided <paramref name="stringRepresentations"/>
		/// to determine how to parse and create string representations of <typeparamref name="T"/>.
		/// HasFlag does this: (value &amp; flag) == flag
		/// </summary>
		/// <param name="defaultStringValue">The string value to return when turning <typeparamref name="T"/> into a string, if the value is not defined.</param>
		/// <param name="comparer">The comparer to use for parsing.</param>
		/// <param name="stringRepresentations">String representations of each <typeparamref name="T"/>.</param>
		/// <returns>An instance of <see cref="EnumUtil{T}"/> with compiled functions, or an error message.</returns>
		public static Maybe<EnumUtil<T>, string> Compile(string defaultStringValue, StringComparer comparer, IReadOnlyDictionary<T, string> stringRepresentations)
		{
			FieldInfo[] infos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			Dictionary<string, T> dict = new(comparer);
			for (int i = 0; i < infos.Length; i++)
			{
				FieldInfo fi = infos[i];
				T val = (T)fi.GetRawConstantValue()!;
				if (!stringRepresentations.TryGetValue(val, out string? name))
				{
					return "The dictionary passed is missing a key for this enum value: " + val.ToString();
				}
				if (dict.ContainsKey(name))
				{
					return string.Concat("The string representation for enum value ", val, " appears multiple times: ", name);
				}
				dict[name] = val;
			}
			if (dict.Count != infos.Length)
			{
				return "Some enum values have the same string representation with regards to the provided StringComparer. Check to see if there are any duplicate EnumStringAttributes applied, or if some enum fields only differ on case.";
			}

			Func<T, T, bool> hasFlag = CompileHasFlag(Enum.GetUnderlyingType(typeof(T)));
			Func<T, string> toString = CompileToString(dict, defaultStringValue);
			Func<T, bool> isDefined = CompileIsDefined(dict.Values);
			return new EnumUtil<T>(toString, isDefined, hasFlag, dict);
		}
		private static Func<T, T, bool> CompileHasFlag(Type underlyingType)
		{
			LabelTarget returnTarget = Expression.Label(typeof(bool));
			ParameterExpression p = Expression.Parameter(typeof(T), "val");
			ParameterExpression flag = Expression.Parameter(typeof(T), "flag");

			BlockExpression b = Expression.Block(typeof(bool), Expression.Label(returnTarget, Expression.Equal(Expression.Convert(flag, underlyingType), Expression.And(Expression.Convert(p, underlyingType), Expression.Convert(flag, underlyingType)))));
			Expression<Func<T, T, bool>> lambda = Expression.Lambda<Func<T, T, bool>>(b, p, flag);
			return lambda.Compile();
		}
		/// <summary>
		/// This is about 3 times as fast as looking the value up in a <see cref="HashSet{T}"/>, much faster than using <see cref="Enum.IsDefined(Type, object)"/>.
		/// </summary>
		/// <param name="vals">The valid values of the enum.</param>
		/// <returns>A compiled lambda which checks if the enum value is defined.</returns>
		private static Func<T, bool> CompileIsDefined(ICollection<T> vals)
		{
			LabelTarget returnTarget = Expression.Label(typeof(bool));

			SwitchCase[] cases = new SwitchCase[vals.Count];
			int i = 0;
			foreach (T val in vals)
			{
				cases[i++] = Expression.SwitchCase(Expression.Return(returnTarget, Expression.Constant(true)), Expression.Constant(val));
			}
			// The last expression of a block expression is what returns the value, so we set the last expression to the returnlabel with the default value.
			ParameterExpression p = Expression.Parameter(typeof(T), "val");
			BlockExpression b = Expression.Block(typeof(bool), Expression.Switch(p, cases), Expression.Label(returnTarget, Expression.Constant(false)));
			Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(b, p);
			return lambda.Compile();
		}
		private static Func<T, string> CompileToString(ICollection<KeyValuePair<string, T>> vals, string defaultValue)
		{
			LabelTarget returnTarget = Expression.Label(typeof(string));

			SwitchCase[] cases = new SwitchCase[vals.Count];
			int i = 0;
			foreach (KeyValuePair<string, T> kvp in vals)
			{
				cases[i++] = Expression.SwitchCase(Expression.Return(returnTarget, Expression.Constant(kvp.Key)), Expression.Constant(kvp.Value));
			}
			// The last expression of a block expression is what returns the value, so we set the last expression to the returnlabel with the default value.
			ParameterExpression p = Expression.Parameter(typeof(T), "val");
			BlockExpression b = Expression.Block(typeof(string), Expression.Switch(p, cases), Expression.Label(returnTarget, Expression.Constant(defaultValue)));
			Expression<Func<T, string>> lambda = Expression.Lambda<Func<T, string>>(b, p);
			return lambda.Compile();
		}
	}
}