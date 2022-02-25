namespace MichMcb.CsExt.Events
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	/// <summary>
	/// Invokes the registered event handlers in parallel, running all at once and then waiting for all of them to complete.
	/// </summary>
	/// <typeparam name="TArgs">The type of the event args.</typeparam>
	public sealed class ParallelAsyncEvent<TArgs> : IAsyncEvent<TArgs>
	{
		private readonly List<Func<object?, TArgs, Task>> handlers;
		/// <summary>
		/// Creates a new event with no handlers.
		/// </summary>
		public ParallelAsyncEvent()
		{
			handlers = new();
		}
		/// <summary>
		/// All registered handlers.
		/// </summary>
		public IReadOnlyCollection<Func<object?, TArgs, Task>> Handlers => handlers;
		/// <summary>
		/// Adds a handler.
		/// </summary>
		/// <param name="handler">The handler to add.</param>
		public void AddHandler(Func<object?, TArgs, Task> handler)
		{
			handlers.Add(handler);
		}
		/// <summary>
		/// Removes a handler. Returns true if the handler was found and removed, false otherwise.
		/// </summary>
		/// <param name="handler">The handler to remove.</param>
		public bool RemoveHandler(Func<object?, TArgs, Task> handler)
		{
			return handlers.Remove(handler);
		}
		/// <summary>
		/// Removes all registered handlers.
		/// </summary>
		public void Clear()
		{
			handlers.Clear();
		}
		/// <summary>
		/// Invokes all handlers. Handlers are executed in parallel; all handlers are executed at once, then this waits for all of them to complete.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The event data.</param>
		/// <returns>A task which completes once all handlers have completed.</returns>
		public async Task Invoke(object? sender, TArgs args)
		{
			Task[] t = new Task[handlers.Count];
			int i = 0;
			foreach (Func<object?, TArgs, Task> handler in handlers)
			{
				t[i++] = handler(sender, args);
			}
			await Task.WhenAll(t);
		}
	}
}
