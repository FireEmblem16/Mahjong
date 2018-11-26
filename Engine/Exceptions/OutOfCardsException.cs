using System;
using System.Runtime.Serialization;

namespace Engine.Exceptions
{
	/// <summary>
	/// An exception representing that a deck, hand or other stack of cards are out when an attempt was made to draw, play from, etc the cards.
	/// </summary>
	public class OutOfCardsException : Exception
	{
		/// <summary>
		/// Creates a new OutOfCards exception.
		/// </summary>
		public OutOfCardsException() : base()
		{return;}

		/// <summary>
		/// Creates a new OutOfCards exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		public OutOfCardsException(string message) : base(message)
		{return;}

		/// <summary>
		/// Creates a new OutOfCards exception.
		/// </summary>
		/// <param name="info">Holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">Contains the contextual information about the source or the destination.</param>
		public OutOfCardsException(SerializationInfo info, StreamingContext context) : base(info,context)
		{return;}

		/// <summary>
		/// Creates a new OutOfCards exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
		public OutOfCardsException(string message, Exception innerException) : base(message,innerException)
		{return;}
	}
}
