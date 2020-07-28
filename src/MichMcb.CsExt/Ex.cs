using System;

namespace MichMcb.CsExt
{
	public readonly struct Ex<E> where E : Exception
	{
		public Ex(bool ok, E ex)
		{
			Ok = ok;
			Exception = ex;
		}
		public bool Ok { get; }
		public E Exception { get; }
		public static bool operator true(Ex<E> o) => o.Ok;
		public static bool operator false(Ex<E> o) => !o.Ok;
		public static bool operator &(Ex<E> lhs, Ex<E> rhs) => lhs.Ok && rhs.Ok;
		public static bool operator |(Ex<E> lhs, Ex<E> rhs) => lhs.Ok || rhs.Ok;
		public static implicit operator bool(Ex<E> opt) => opt.Ok;
		public static bool operator ==(Ex<E> lhs, Ex<E> rhs)
		{
			throw new InvalidOperationException();
		}
		public static bool operator !=(Ex<E> lhs, Ex<E> rhs)
		{
			throw new InvalidOperationException();
		}
		public override bool Equals(object obj)
		{
			throw new InvalidOperationException();
		}
		public override int GetHashCode()
		{
			throw new InvalidOperationException();
		}
	}
}
