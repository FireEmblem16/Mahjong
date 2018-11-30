namespace Engine.Cards.CardTypes.Suits.Mahjong
{
	/// <summary>
	/// The basic definition of a tile.
	/// </summary>
	public abstract class BasicSuit : CardSuit
	{
		/// <summary>
		/// Creates a new simple suit.
		/// </summary>
		public BasicSuit()
		{return;}

		/// <summary>
		/// Creates a deep copy of this suit.
		/// </summary>
		/// <returns>Returns a copy of this suit.</returns>
		public abstract CardSuit Clone();

		/// <summary>
		/// Returns true if this suit is the same suit as the provided suit.
		/// </summary>
		/// <param name="s">The suit to compare with.</param>
		/// <returns>Returns true if the two suits are the same and false otherwise.</returns>
		public bool Equals(CardSuit s)
		{return ToString() == s.ToString();}

		/// <summary>
		/// Returns a string that represents this suit.
		/// The suit is pluralized.
		/// </summary>
		/// <returns>Returns a string the represents this suit.</returns>
		public override abstract string ToString();

		/// <summary>
		/// A way to identify a suit's actually suits.
		/// That is, a card suit can have more than one basic suit.
		/// </summary>
		public abstract SuitIdentifier ID
		{get; protected set;}

		/// <summary>
		/// The color of the suit.
		/// </summary>
		public abstract SuitColor Color
		{get;}
	}
}
