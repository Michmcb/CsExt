namespace MichMcb.CsExt
{
	/// <summary>
	/// A combination of an error code and a state object.
	/// </summary>
	/// <typeparam name="T">The type of the state object.</typeparam>
	public readonly struct ErrState<T> where T : class
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <param name="message">The error message.</param>
		public ErrState(T? state, string? message)
		{
			State = state;
			Message = message;
		}
		/// <summary>
		/// The associated state.
		/// </summary>
		public T? State { get; }
		/// <summary>
		/// The error message.
		/// </summary>
		public string? Message { get; }
		/// <summary>
		/// Returns <see cref="Message"/>.
		/// </summary>
		/// <returns><see cref="Message"/>.</returns>
		public override string? ToString()
		{
			return Message;
		}
	}
	/// <summary>
	/// Helper class to make <see cref="ErrState{T}"/> instances.
	/// </summary>
	public static class ErrState
	{
		/// <summary>
		/// Creates a new <see cref="ErrState{T}"/>
		/// </summary>
		public static ErrState<T> New<T>(T? state, string? message) where T : class
		{
			return new ErrState<T>(state, message);
		}
	}
}
