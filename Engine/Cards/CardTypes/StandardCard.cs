#pragma warning disable 0659

namespace Engine.Cards.CardTypes
{
	/// <summary>
	/// A normal card with nothing particuarly special about it.
	/// </summary>
	public class StandardCard : Card
	{
		/// <summary>
		/// Creates a new standard card.
		/// Standard cards have no extra rules associated with them other than the rules of the game they are used in.
		/// </summary>
		/// <param name="suit">The suit of the card.</param>
		/// <param name="val">The value of the card.</param>
		/// <param name="name">If the card can not be refereded to by its value and suit then provide the name of the card here.</param>
		public StandardCard(CardSuit suit, CardValue val, string name = null)
		{
			Suit = suit.Clone();
			Value = val.Clone();

			Name = name;
			return;
		}

		/// <summary>
		/// Creates a deep copy of this card.
		/// </summary>
		/// <returns>Returns a copy of this card.</returns>
		public Card Clone()
		{return new StandardCard(Suit,Value,Name);}

		/// <summary>
		/// Returns true if this card is the same card as the provided object.
		/// </summary>
		/// <param name="obj">The object to compare with.</param>
		/// <returns>Returns true if the card and the object are the same and false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if(!(obj is Card))
				return false;

			return Equals((Card)obj);
		}

		/// <summary>
		/// Returns true if this card is the same card as the provided card.
		/// </summary>
		/// <param name="c">The card to compare with.</param>
		/// <returns>Returns true if the two cards are the same and false otherwise.</returns>
		public bool Equals(Card c)
		{return ToString() == c.ToString();} // The suit and value can be the same without it actually being the same card

		/// <summary>
		/// Returns a string that represents this card.
		/// </summary>
		/// <returns>Returns a string the represents this card.</returns>
		public override string ToString()
		{
			if(Name == null)
				return Value.ToString() + " of " + Suit.ToString();

			return Name;
		}

		/// <summary>
		/// The suit of the card.
		/// </summary>
		public CardSuit Suit
		{get; protected set;}

		/// <summary>
		/// The value of the card.
		/// </summary>
		public CardValue Value
		{get; protected set;}

		/// <summary>
		/// Contains a special name for this card to be used instead of the names of the suit and value.
		/// </summary>
		protected readonly string Name;
	}
}
