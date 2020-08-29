using System;
using System.Diagnostics.CodeAnalysis;

namespace MichMcb.CsExt
{
	public readonly struct Opt<TVal>
	{
		internal Opt([DisallowNull] TVal value, bool ok)
		{
			if (ok && value == null)
			{
				throw new ArgumentNullException(nameof(value), "Can't make an Opt from a null value when ok is true");
			}
			Value = value;
			Ok = ok;
		}
		public TVal Value { get; }
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
		public TVal ValueOr([AllowNull] TVal ifNone) => Ok ? Value : ifNone;
		/// <summary>
		/// Returns HasValue, sets <paramref name="val"/> to Value, and <paramref name="errMsg"/> to ErrMsg
		/// </summary>
		/// <param name="val"></param>
		public bool HasValue([NotNullWhen(true)] out TVal val)
		{
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
			val = Value;
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
				hasVal(Value);
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
				hasVal(Value);
			}
			else
			{
				noVal();
			}
		}
		public override string ToString()
		{
			return Value?.ToString() ?? "";
		}
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			return Value.Equals(obj);
		}
		public static bool operator ==(Opt<TVal> left, Opt<TVal> right)
		{
			if (left.Value == null)
			{
				if (right.Value == null)
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
			if (right.Value == null)
			{
				// Left not null, Right null
				return false;
			}
			return left.Value.Equals(right.Value);
		}
		public static bool operator !=(Opt<TVal> left, Opt<TVal> right)
		{
			if (left.Value == null)
			{
				if (right.Value == null)
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
			if (right.Value == null)
			{
				// Left not null, Right null
				return true;
			}
			return !left.Value.Equals(right.Value);
		}
	}
}