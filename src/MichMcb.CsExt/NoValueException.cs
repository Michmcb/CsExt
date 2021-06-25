namespace MichMcb.CsExt
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// An exception thrown when there is no valid value.
	/// Thrown from <see cref="Maybe{TVal, TErr}.ValueOrException"/> is called when there is no value.
	/// </summary>
	public class NoValueException : Exception
	{
		/// <summary>
		/// Creates a new instance
		/// </summary>
		public NoValueException() { }
		/// <summary>
		/// Creates a new instance with a message
		/// </summary>
		public NoValueException(string? message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the System.Exception class with a specified error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		public NoValueException(string? message, Exception? innerException) : base(message, innerException) { }
		/// <summary>
		/// Initializes a new instance of the System.Exception class with serialized data.
		/// </summary>
		protected NoValueException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
