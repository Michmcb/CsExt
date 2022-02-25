namespace MichMcb.CsExt.Events
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	/// <summary>
	/// An event handler which can be invoked asynchronously, and returns a <see cref="Task"/>, instead of void.
	/// </summary>
	/// <typeparam name="TArgs">The type of the event args.</typeparam>
	public interface IAsyncEvent<TArgs>
	{
		/// <summary>
		/// All registered handlers.
		/// </summary>
		IReadOnlyCollection<Func<object?, TArgs, Task>> Handlers { get; }
		/// <summary>
		/// Adds a handler.
		/// </summary>
		/// <param name="handler">The handler to add.</param>
		void AddHandler(Func<object?, TArgs, Task> handler);
		/// <summary>
		/// Removes a handler. Returns true if the handler was found and removed, false otherwise.
		/// </summary>
		/// <param name="handler">The handler to remove.</param>
		bool RemoveHandler(Func<object?, TArgs, Task> handler);
		/// <summary>
		/// Removes all registered handlers.
		/// </summary>
		void Clear();
		/// <summary>
		/// Invokes all handlers.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The event data.</param>
		/// <returns>A task which completes once all handlers have completed.</returns>
		Task Invoke(object? sender, TArgs args);
	}
}
