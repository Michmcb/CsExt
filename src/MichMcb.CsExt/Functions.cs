using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MichMcb.CsExt
{
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
			return new Opt<V>(val!, false);
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
		#region Ex
		/// <summary>
		/// Executes <paramref name="action"/> and catches an exception of type <typeparamref name="Ex1"/>.
		/// </summary>
		/// <param name="action">The action to attempt</param>
		public static Ex<Ex1> Try<Ex1>(Action action) where Ex1 : Exception
		{
			try
			{
				action();
				return new Ex<Ex1>(true, default!);
			}
			catch (Ex1 e) { return new Ex<Ex1>(false, e); }
		}
		/// <summary>
		/// Executes <paramref name="action"/> and catches an exception of type <typeparamref name="Ex1"/>.
		/// </summary>
		/// <param name="action">The action to attempt</param>
		public static async Task<Ex<Ex1>> TryAsync<Ex1>(Func<Task> action) where Ex1 : Exception
		{
			try
			{
				await action();
				return new Ex<Ex1>(true, default!);
			}
			catch (Ex1 e) { return new Ex<Ex1>(false, e); }
		}
		/// <summary>
		/// Executes <paramref name="action"/> and catches an exception of type <typeparamref name="Ex1"/>, invoking <paramref name="handler1"/>.
		/// Returns true if no exception was thrown, false if an exception was thrown.
		/// </summary>
		/// <param name="action">The action to attempt</param>
		/// <param name="handler1">The handler for exceptions of type <typeparamref name="Ex1"/></param>
		public static bool TryCatch<Ex1>(Action action, Action<Ex1> handler1) where Ex1 : Exception
		{
			try
			{
				action();
				return true;
			}
			catch (Ex1 e)
			{
				handler1(e);
				return false;
			}
		}
		/// <summary>
		/// Executes <paramref name="action"/>, invoking <paramref name="log"/> in the when clause of the catch handler.
		/// Useful to preserve the stack trace for logging purposes.
		/// <paramref name="log"/> should return false to allow the exception to be raised. If it returns true, it will swallow the exception.
		/// </summary>
		/// <param name="action">The action to attempt</param>
		/// <param name="log">The function to log the exception</param>
		public static void TryLog<Ex1>(Action action, Func<Ex1, bool> log) where Ex1 : Exception
		{
			try
			{
				action();
			}
			catch (Ex1 e) when (log(e)) { }
		}

		/// <summary>
		/// Executes <paramref name="action"/> and catches an exception of any of the types provided.
		/// The exception returned is typed as Exception. Mainly useful if you only care an exception occurred, but you don't care what actually happened.
		/// </summary>
		/// <param name="action">The action to attempt</param>
		public static Ex<Exception> Try<Ex1, Ex2>(Action action) where Ex1 : Exception where Ex2 : Exception
		{
			try
			{
				action();
				return new Ex<Exception>(true, NoException.Inst);
			}
			catch (Ex1 e) { return new Ex<Exception>(false, e); }
			catch (Ex2 e) { return new Ex<Exception>(false, e); }
		}
		/// <summary>
		/// Executes <paramref name="action"/> and catches an exception of any of the types provided. Handles any caught exception with <paramref name="handler"/>.
		/// Returns true if no exception was thrown, false if an exception was thrown.
		/// </summary>
		/// <param name="action">The action to attempt</param>
		/// <param name="handler">The handler for all exceptions</param>
		public static bool TryCatchAll<Ex1, Ex2>(Action action, Action<Exception> handler) where Ex1 : Exception where Ex2 : Exception
		{
			try
			{
				action();
				return true;
			}
			catch (Ex1 e)
			{
				handler(e);
				return false;
			}
			catch (Ex2 e)
			{
				handler(e);
				return false;
			}
		}
		#endregion
	}
}
