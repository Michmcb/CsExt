namespace MichMcb.CsExt
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	/// <summary>
	/// A class which has many static helper functions to create the Opt and Ex types.
	/// Intended that you have a "using static MichMcb.CsExt.Functions;" to be able to use these easily.
	/// </summary>
	public static class Functions
	{
#pragma warning disable IDE0060 // Remove unused parameter
		/// <summary>
		/// A method that does nothing.
		/// </summary>
		public static void NoAction() { }
		/// <summary>
		/// A method that does nothing.
		/// </summary>
		public static void NoAction<T>(T o) { }
		/// <summary>
		/// A method that does nothing.
		/// </summary>
		public static void NoAction<T>(params T[] o) { }
#pragma warning restore IDE0060 // Remove unused parameter
		public static Opt<V> None<V>([AllowNull] V val = default)
		{
			return new Opt<V>(val, false);
		}
		public static Opt<V> Some<V>([DisallowNull] V val)
		{
			return new Opt<V>(val, true);
		}
		#region Switch
		/// <summary>
		/// Switches on strings, using <paramref name="comparison"/> to compare against them.
		/// Equivalent to a loop that compares against each string, breaking on the first match.
		/// If no match is found, returns <paramref name="defaultValue"/>.
		/// </summary>
		/// <typeparam name="TResult">The type of the result</typeparam>
		/// <param name="switchOn">The string to switch on</param>
		/// <param name="comparison">The StringComparison to use</param>
		/// <param name="defaultValue">The value to return when no match is found</param>
		/// <param name="cases">Tuples representing the possible values and return values</param>
		public static TResult Switch<TResult>(string switchOn, StringComparison comparison, TResult defaultValue, params (string val, TResult result)[] cases)
		{
			foreach ((string val, TResult result) in cases)
			{
				if (switchOn.Equals(val, comparison))
				{
					return result;
				}
			}
			return defaultValue;
		}
		/// <summary>
		/// Switches on strings, using <paramref name="comparison"/> to compare against them.
		/// Equivalent to a loop that compares against each string, breaking on the first match.
		/// If no match is found, invokes <paramref name="defaultAction"/>.
		/// </summary>
		/// <param name="switchOn">The string to switch on</param>
		/// <param name="comparison">The StringComparison to use</param>
		/// <param name="defaultAction">The Action to invoke when no match is found</param>
		/// <param name="cases">Tuples representing the possible values and invocations</param>
		public static void Switch(string switchOn, StringComparison comparison, Action defaultAction, params (string val, Action action)[] cases)
		{
			foreach ((string val, Action action) in cases)
			{
				if (switchOn.Equals(val, comparison))
				{
					action();
					return;
				}
			}
			defaultAction();
		}
		/// <summary>
		/// Switches on strings, using <paramref name="comparison"/> to compare against them.
		/// Equivalent to a loop that compares against each string, breaking on the first match.
		/// If no match is found, invokes <paramref name="defaultFunc"/>.
		/// </summary>
		/// <param name="switchOn">The string to switch on</param>
		/// <param name="comparison">The StringComparison to use</param>
		/// <param name="defaultFunc">The Action to invoke when no match is found</param>
		/// <param name="cases">Tuples representing the possible values and invocations</param>
		public static TResult Switch<TResult>(string switchOn, StringComparison comparison, Func<TResult> defaultFunc, params (string val, Func<TResult> func)[] cases)
		{
			foreach ((string val, Func<TResult> func) in cases)
			{
				if (switchOn.Equals(val, comparison))
				{
					return func();
				}
			}
			return defaultFunc();
		}
		#endregion
	}
}
