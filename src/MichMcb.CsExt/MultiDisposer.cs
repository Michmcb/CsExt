namespace MichMcb.CsExt
{
	using System;
	using System.Collections.Generic;
	/// <summary>
	/// A class which disposes multiple things when it itself is disposed of, and then clears its collection of disposables.
	/// The order in which the objects are disposed is the reverse of the order in which they are added.
	/// That is, this is essentially a Stack of <see cref="IDisposable"/>.
	/// You can dispose of this object multiple times.
	/// </summary>
	public sealed class MultiDisposer : IDisposable
	{
		private readonly Stack<IDisposable> disposables;
		public MultiDisposer()
		{
			disposables = new Stack<IDisposable>();
		}
		/// <summary>
		/// Creates a new instance which disposes of all <paramref name="disposables"/> in reverse order.
		/// </summary>
		/// <param name="disposables">The objects to dispose of once this instance is disposed of.</param>
		public MultiDisposer(IEnumerable<IDisposable> disposables)
		{
			this.disposables = new Stack<IDisposable>(disposables);
		}
		/// <summary>
		/// Creates a new instance which disposes of all <paramref name="disposables"/> in reverse order.
		/// </summary>
		/// <param name="disposables">The objects to dispose of once this instance is disposed of.</param>
		public MultiDisposer(params IDisposable[] disposables)
		{
			this.disposables = new Stack<IDisposable>(disposables);
		}
		/// <summary>
		/// Adds <paramref name="disposable"/> to the top of the stack of disposables. It will be the first one disposed of.
		/// </summary>
		/// <param name="disposable">The object to dispose of once this instance is disposed of.</param>
		/// <returns>This instance.</returns>
		public MultiDisposer Add(IDisposable disposable)
		{
			disposables.Push(disposable);
			return this;
		}
		/// <summary>
		/// Adds <paramref name="disposables"/> to the top of the stack of disposables, in order.
		/// </summary>
		/// <param name="disposables">The objects to dispose of once this instance is disposed of.</param>
		/// <returns>This instance.</returns>
		public MultiDisposer AddRange(params IDisposable[] disposables)
		{
			foreach (IDisposable d in disposables)
			{
				this.disposables.Push(d);
			}
			return this;
		}
		/// <summary>
		/// Adds <paramref name="disposables"/> to the top of the stack of disposables, in order.
		/// </summary>
		/// <param name="disposables">The objects to dispose of once this instance is disposed of.</param>
		/// <returns>This instance.</returns>
		public MultiDisposer AddRange(IEnumerable<IDisposable> disposables)
		{
			foreach (IDisposable d in disposables)
			{
				this.disposables.Push(d);
			}
			return this;
		}
		/// <summary>
		/// The objects which will be disposed of when this object is disposed
		/// </summary>
		public IReadOnlyCollection<IDisposable> Disposables => disposables;
		/// <summary>
		/// Disposes of all <see cref="Disposables"/>, and empties the stack.
		/// Not thread safe.
		/// </summary>
		public void Dispose()
		{
			while (disposables.Count > 0)
			{
				disposables.Pop().Dispose();
			}
		}
	}
}
