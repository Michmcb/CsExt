namespace MichMcb.CsExt
{
	using System;
	using System.Collections.Generic;
	/// <summary>
	/// A class which disposes multiple things when it itself is disposed of, and then clears its collection of disposables.
	/// The order in which the objects are disposed is the reverse of the order in which they are added. That is,
	/// it's essentially a Stack of IDisposables.
	/// You can dispose of this object multiple times.
	/// </summary>
	public sealed class MultiDisposer : IDisposable
	{
		private readonly Stack<IDisposable> disposables;
		public MultiDisposer()
		{
			disposables = new Stack<IDisposable>();
		}
		public MultiDisposer(IEnumerable<IDisposable> disposables)
		{
			this.disposables = new Stack<IDisposable>(disposables);
		}
		public MultiDisposer(params IDisposable[] disposables)
		{
			this.disposables = new Stack<IDisposable>(disposables);
		}
		public MultiDisposer Add(IDisposable disposable)
		{
			disposables.Push(disposable);
			return this;
		}
		public MultiDisposer AddRange(params IDisposable[] disposables)
		{
			foreach (IDisposable d in disposables)
			{
				this.disposables.Push(d);
			}
			return this;
		}
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
		public void Dispose()
		{
			while (disposables.Count > 0)
			{
				disposables.Pop().Dispose();
			}
		}
	}
}
