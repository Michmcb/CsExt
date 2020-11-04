namespace MichMcb.CsExt
{
	using System;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// Returns a value along with a success/failure boolean.
	/// </summary>
	/// <typeparam name="TVal">The value to return.</typeparam>
	public readonly struct Opt<TVal>
	{
		internal Opt([DisallowNull] TVal val, bool ok)
		{
			if (ok && val == null)
			{
				throw new ArgumentNullException(nameof(val), "Can't make an Opt from a null value when ok is true");
			}
			Val = val;
			Ok = ok;
		}
		/// <summary>
		/// The value
		/// </summary>
		public TVal Val { get; }
		/// <summary>
		/// Success or failure
		/// </summary>
		public bool Ok { get; }
		/// <summary>
		/// Same as <see cref="Ok"/>
		/// </summary>
		/// <param name="o"></param>
		/// <returns>Value of <see cref="Ok"/></returns>
		public static bool operator true(Opt<TVal> o) => o.Ok;
		/// <summary>
		/// Opposite of <see cref="Ok"/>
		/// </summary>
		/// <param name="o"></param>
		/// <returns>Opposite of <see cref="Ok"/></returns>
		public static bool operator false(Opt<TVal> o) => !o.Ok;
		/// <summary>
		/// Works on the values of <see cref="Ok"/>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static bool operator &(Opt<TVal> lhs, Opt<TVal> rhs) => lhs.Ok && rhs.Ok;
		/// <summary>
		/// Works on the values of <see cref="Ok"/>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static bool operator |(Opt<TVal> lhs, Opt<TVal> rhs) => lhs.Ok || rhs.Ok;
		/// <summary>
		/// Returns Val if Ok is true. Otherwise, returns <paramref name="ifNone"/>.
		/// </summary>
		[return: NotNullIfNotNull("ifNone")]
		public TVal ValueOr([AllowNull] TVal ifNone) => Ok ? Val : ifNone;
		/// <summary>
		/// Returns <see cref="Ok"/>, and sets <paramref name="val"/> to <see cref="Val"/>.
		/// </summary>
		/// <param name="val"></param>
		public bool HasValue([NotNullWhen(true)] out TVal val)
		{
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
			val = Val;
			return Ok;
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
		}
		/// <summary>
		/// Executes <paramref name="hasVal"/> is HasVal is true.
		/// </summary>
		public void Do(Action<TVal> hasVal)
		{
			if (Ok)
			{
				hasVal(Val);
			}
		}
		/// <summary>
		/// Executes <paramref name="hasVal"/> is HasVal is true.
		/// Otherwise, executes <paramref name="noVal"/>.
		/// </summary>
		public void Do(Action<TVal> hasVal, Action noVal)
		{
			if (Ok)
			{
				hasVal(Val);
			}
			else
			{
				noVal();
			}
		}
		/// <summary>
		/// Equivalent to new Opt(<paramref name="value"/>, true);
		/// </summary>
		public static implicit operator Opt<TVal>([DisallowNull] TVal value)
		{
			return new Opt<TVal>(value, true);
		}
		/// <summary>
		/// Calls ToString() on <see cref="Val"/>.
		/// If Val is null, returns <see cref="string.Empty"/>.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Val?.ToString() ?? string.Empty;
		}
	}
}