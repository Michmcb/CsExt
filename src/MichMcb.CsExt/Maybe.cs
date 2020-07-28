using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MichMcb.CsExt
{
	public readonly struct Maybe<TVal, TErr> : IEquatable<Maybe<TVal, TErr>>
	{
		public Maybe([DisallowNull] TVal value, [DisallowNull]TErr error, bool ok)
		{
			if (value == null && error == null)
			{
				throw new ArgumentNullException(nameof(value), "Can't make a Maybe from two null values");
			}
			Value = value;
			Error = error;
			Ok = ok;
		}
		public TVal Value { get; }
		public TErr Error { get; }
		public bool Ok { get; }
		public static bool operator true(Maybe<TVal, TErr> o) => o.Ok;
		public static bool operator false(Maybe<TVal, TErr> o) => !o.Ok;
		public static bool operator &(Maybe<TVal, TErr> lhs, Maybe<TVal, TErr> rhs) => lhs.Ok && rhs.Ok;
		public static bool operator |(Maybe<TVal, TErr> lhs, Maybe<TVal, TErr> rhs) => lhs.Ok || rhs.Ok;
		public static implicit operator bool(Maybe<TVal, TErr> opt) => opt.Ok;
		public static bool operator ==(Maybe<TVal, TErr> left, Maybe<TVal, TErr> right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(Maybe<TVal, TErr> left, Maybe<TVal, TErr> right)
		{
			return !(left == right);
		}
		public TVal ValueOr(TVal ifNone) => Ok ? Value : ifNone;
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
		public bool HasValue([NotNullWhen(true)] out TVal val)
		{
			val = Value;
			return Ok;
		}
		public bool HasError([NotNullWhen(true)] out TErr error)
		{
			error = Error;
			return !Ok;
		}
		public bool Get([NotNullWhen(true)] out TVal val, [NotNullWhen(false)] out TErr error)
		{
			val = Value;
			error = Error;
			return Ok;
		}
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
		public override string ToString()
		{
			return Value?.ToString() ?? "";
		}
		public override int GetHashCode()
		{
			return HashCode.Combine(Value);
		}
		public override bool Equals(object? obj)
		{
			return obj is Maybe<TVal, TErr> maybe && Equals(maybe);
		}
		public bool Equals(Maybe<TVal, TErr> other)
		{
			return EqualityComparer<TVal>.Default.Equals(Value, other.Value);
		}
	}
}
