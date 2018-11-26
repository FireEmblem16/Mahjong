namespace Engine.Cards.CardTypes
{
	/// <summary>
	/// Represents the suit of a card.
	/// </summary>
	public interface CardSuit
	{
		/// <summary>
		/// Creates a deep copy of this suit.
		/// </summary>
		/// <returns>Returns a copy of this suit.</returns>
		CardSuit Clone();

		/// <summary>
		/// Returns true if this suit is the same suit as the provided suit.
		/// </summary>
		/// <param name="s">The suit to compare with.</param>
		/// <returns>Returns true if the two suits are the same and false otherwise.</returns>
		bool Equals(CardSuit s);

		/// <summary>
		/// Returns a string that represents this suit.
		/// The suit is pluralized.
		/// </summary>
		/// <returns>Returns a string the represents this suit.</returns>
		string ToString();

		/// <summary>
		/// A way to identify a suit's actually suits.
		/// That is, a card suit can have more than one basic suit.
		/// </summary>
		SuitIdentifier ID
		{get;}

		/// <summary>
		/// The color of the suit.
		/// </summary>
		SuitColor Color
		{get;}
	}

	/// <summary>
	/// A set of flags for suits.
	/// </summary>
	[System.Flags]
	public enum SuitIdentifier
	{
		// Playing card suits
		SUITLESS = 0x00,
		SPADE = 0x01,
		HEART = 0x02,
		CLUB = 0x04,
		DIMOND = 0x08,
		SUIT_5 = 0x10,
		SUIT_6 = 0x20,
		SUIT_7 = 0x40,
		SUIT_8 = 0x80,

		// Mahjong suits
		BAMBOO = 0x01
	}

	/// <summary>
	/// Colors associated with a suit. Hearts are red, Spades are black, etc...
	/// </summary>
	[System.Flags]
	public enum SuitColor
	{
		// Playing card colors
		COLORLESS = 0x0,
		RED = 0x1,
		BLACK = 0x2,
		SUIT_3 = 0x4,
		SUIT_4 = 0x8,

		// Mahjong tile types
		SIMPLE = 0x7,
		HONORS = 0x42
	}
}
