namespace MichMcb.CsExt.Collections
{
	using System.Collections;
	using System.Collections.Generic;
#if NET5_0_OR_GREATER
	/// <summary>
	/// Wraps a <see cref="ISet{T}"/>, to make it a <see cref="IReadOnlySet{T}"/>.
	/// </summary>
	public sealed class ReadOnlySet<T> : IReadOnlySet<T>
	{
		private readonly ISet<T> set;
		/// <summary>
		/// Wraps <see cref="set"/>.
		/// </summary>
		/// <param name="set">The set to wrap.</param>
		public ReadOnlySet(ISet<T> set)
		{
			this.set = set;
		}
		/// <inheritdoc/>
		public int Count => set.Count;
		/// <inheritdoc/>
		public bool Contains(T item)
		{
			return set.Contains(item);
		}
		/// <inheritdoc/>
		public IEnumerator<T> GetEnumerator()
		{
			return set.GetEnumerator();
		}
		/// <inheritdoc/>
		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			return set.IsProperSubsetOf(other);
		}
		/// <inheritdoc/>
		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			return set.IsProperSupersetOf(other);
		}
		/// <inheritdoc/>
		public bool IsSubsetOf(IEnumerable<T> other)
		{
			return set.IsSubsetOf(other);
		}
		/// <inheritdoc/>
		public bool IsSupersetOf(IEnumerable<T> other)
		{
			return set.IsSupersetOf(other);
		}
		/// <inheritdoc/>
		public bool Overlaps(IEnumerable<T> other)
		{
			return set.Overlaps(other);
		}
		/// <inheritdoc/>
		public bool SetEquals(IEnumerable<T> other)
		{
			return set.SetEquals(other);
		}
		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return set.GetEnumerator();
		}
	}
#endif
}
