using System;
using System.Collections.Generic;

namespace MichMcb.CsExt
{
	/// <summary>
	/// A class which disposes multiple things when it itself is disposed of
	/// </summary>
	public class MultiDisposer : IDisposable
	{
		private readonly List<IDisposable> disposables;
		public MultiDisposer() 
		{
			disposables = new List<IDisposable>();
		}
		public MultiDisposer(IEnumerable<IDisposable> disposables)
		{
			this.disposables = new List<IDisposable>(disposables);
		}
		public MultiDisposer(params IDisposable[] disposables)
		{
			this.disposables = new List<IDisposable>(disposables);
		}
		public MultiDisposer Add(IDisposable disposable)
		{
			disposables.Add(disposable);
			return this;
		}
		public MultiDisposer AddRange(params IDisposable[] disposables)
		{
			this.disposables.AddRange(disposables);
			return this;
		}
		public MultiDisposer AddRange(IEnumerable<IDisposable> disposables)
		{
			this.disposables.AddRange(disposables);
			return this;
		}
		/// <summary>
		/// The objects which will be disposed of when this object is disposed
		/// </summary>
		public ICollection<IDisposable> Disposables => disposables;
		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					foreach (IDisposable d in Disposables)
					{
						d.Dispose();
					}
				}

				disposedValue = true;
			}
		}
		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
