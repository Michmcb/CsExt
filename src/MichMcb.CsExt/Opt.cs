namespace MichMcb.CsExt
{
	using System;
	using System.Diagnostics.CodeAnalysis;

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
		public TVal Val { get; }
		public bool Ok { get; }
		public static bool operator true(Opt<TVal> o) => o.Ok;
		public static bool operator false(Opt<TVal> o) => !o.Ok;
		public static bool operator &(Opt<TVal> lhs, Opt<TVal> rhs) => lhs.Ok && rhs.Ok;
		public static bool operator |(Opt<TVal> lhs, Opt<TVal> rhs) => lhs.Ok || rhs.Ok;
		public static implicit operator bool(Opt<TVal> opt) => opt.Ok;
		/// <summary>
		/// Returns Val if Ok is true. Otherwise, returns <paramref name="ifNone"/>.
		/// </summary>
		[return: NotNullIfNotNull("ifNone")]
		public TVal ValueOr([AllowNull] TVal ifNone) => Ok ? Val : ifNone;
		/// <summary>
		/// Returns HasValue, sets <paramref name="val"/> to Val, and <paramref name="errMsg"/> to ErrMsg
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
		public override string ToString()
		{
			return Val?.ToString() ?? "";
		}
		public override int GetHashCode()
		{
			return Val.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			return Val.Equals(obj);
		}
		public static bool operator ==(Opt<TVal> left, Opt<TVal> right)
		{
			if (left.Val == null)
			{
				if (right.Val == null)
				{
					// Both null
					return true;
				}
				else
				{
					// Left null, Right not null
					return false;
				}
			}
			if (right.Val == null)
			{
				// Left not null, Right null
				return false;
			}
			return left.Val.Equals(right.Val);
		}
		public static bool operator !=(Opt<TVal> left, Opt<TVal> right)
		{
			if (left.Val == null)
			{
				if (right.Val == null)
				{
					// Both null
					return false;
				}
				else
				{
					// Left null, Right not null
					return true;
				}
			}
			if (right.Val == null)
			{
				// Left not null, Right null
				return true;
			}
			return !left.Val.Equals(right.Val);
		}
	}
}