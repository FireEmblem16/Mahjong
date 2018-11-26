using System;
using System.Runtime.Serialization;

namespace Engine.Exceptions
{
	/// <summary>
	/// Occurs when a card is attempted to be discarded in a deck that does not note that the card is missing.
	/// May also occur in other situations.
	/// </summary>
	public class CardNotInDeckException : Exception
	{
		/// <summary>
		/// Creates a new CardNotInDeck exception.
		/// </summary>
		public CardNotInDeckException() : base()
		{return;}

		/// <summary>
		/// Creates a new CardNotInDeck exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		public CardNotInDeckException(string message) : base(message)
		{return;}

		/// <summary>
		/// Creates a new CardNotInDeck exception.
		/// </summary>
		/// <param name="info">Holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">Contains the contextual information about the source or the destination.</param>
		public CardNotInDeckException(SerializationInfo info, StreamingContext context) : base(info,context)
		{return;}

		/// <summary>
		/// Creates a new CardNotInDeck exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
		public CardNotInDeckException(string message, Exception innerException) : base(message,innerException)
		{return;}
	}
}
