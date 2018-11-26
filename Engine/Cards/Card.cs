using Engine.Cards.CardTypes;

namespace Engine.Cards
{
	/// <summary>
	/// Represents a card.
	/// </summary>
	public interface Card
	{
		/// <summary>
		/// Creates a deep copy of this card.
		/// </summary>
		/// <returns>Returns a copy of this card.</returns>
		Card Clone();

		/// <summary>
		/// Returns true if this card is the same card as the provided object.
		/// </summary>
		/// <param name="obj">The object to compare with.</param>
		/// <returns>Returns true if the card and the object are the same and false otherwise.</returns>
		bool Equals(object obj);

		/// <summary>
		/// Returns true if this card is the same card as the provided card.
		/// </summary>
		/// <param name="c">The card to compare with.</param>
		/// <returns>Returns true if the two cards are the same and false otherwise.</returns>
		bool Equals(Card c);

		/// <summary>
		/// Returns a string that represents this card.
		/// </summary>
		/// <returns>Returns a string the represents this card.</returns>
		string ToString();

		/// <summary>
		/// The suit of the card.
		/// </summary>
		CardSuit Suit
		{get;}

		/// <summary>
		/// The value of the card.
		/// </summary>
		CardValue Value
		{get;}
	}
}
