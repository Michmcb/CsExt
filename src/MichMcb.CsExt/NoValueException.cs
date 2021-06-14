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
		public NoValueException() { }
		public NoValueException(string? message) : base(message) { }
		public NoValueException(string? message, Exception? innerException) : base(message, innerException) { }
		protected NoValueException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
