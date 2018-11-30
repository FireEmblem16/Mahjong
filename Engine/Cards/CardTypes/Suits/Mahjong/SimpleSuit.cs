namespace Engine.Cards.CardTypes.Suits.Mahjong
{
	/// <summary>
	/// The basic definition of a simple tile.
	/// </summary>
	public class SimpleSuit : BasicSuit
	{
		/// <summary>
		/// Creates a new simple suit.
		/// <param name="type">The type of simple suit to make. Only valid if the type is simple. Defaults to bamboo otherwise.</param>
		/// </summary>
		public SimpleSuit(SuitIdentifier type) : base()
		{
			if(((int)type & (int)SuitColor.SIMPLE) > 0)
				ID = type;
			else
				ID = SuitIdentifier.BAMBOO;

			return;
		}
		
		/// <summary>
		/// Creates a deep copy of this suit.
		/// </summary>
		/// <returns>Returns a copy of this suit.</returns>
		public override CardSuit Clone()
		{return new SimpleSuit(ID);}

		/// <summary>
		/// Returns a string that represents this suit.
		/// The suit is pluralized.
		/// </summary>
		/// <returns>Returns a string the represents this suit.</returns>
		public override string ToString()
		{
			switch(ID)
			{
			case SuitIdentifier.BAMBOO:
				return "Bamboo";
			case SuitIdentifier.DOT:
				return "Dots";
			case SuitIdentifier.CHARACTER:
				return "Characters";
			}
			
			return "";
		}

		/// <summary>
		/// A way to identify a suit's actually suits.
		/// That is, a card suit can have more than one basic suit.
		/// </summary>
		public override SuitIdentifier ID
		{get; protected set;}

		/// <summary>
		/// The color of the suit.
		/// </summary>
		public override SuitColor Color
		{
			get
			{return SuitColor.SIMPLE;}
		}
	}
}
