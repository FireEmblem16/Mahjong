namespace Engine.Cards.CardTypes.Suits.Mahjong
{
	/// <summary>
	/// The basic definition of a honours tile.
	/// </summary>
	public class HonourSuit : BasicSuit
	{
		/// <summary>
		/// Creates a new honours suit.
		/// <param name="type">The type of simple suit to make. Only valid if the type is simple. Defaults to east wind otherwise.</param>
		/// </summary>
		public HonourSuit(SuitIdentifier type) : base()
		{
			if(((int)type & (int)SuitColor.HONOURS) > 0)
				ID = type;
			else
				ID = SuitIdentifier.EAST_WIND;

			return;
		}
		
		/// <summary>
		/// Creates a deep copy of this suit.
		/// </summary>
		/// <returns>Returns a copy of this suit.</returns>
		public override CardSuit Clone()
		{return new HonourSuit(ID);}

		/// <summary>
		/// Returns a string that represents this suit.
		/// The suit is pluralized.
		/// </summary>
		/// <returns>Returns a string the represents this suit.</returns>
		public override string ToString()
		{
			switch(ID)
			{
			case SuitIdentifier.GREEN_DRAGON:
				return "Green Dragons";
			case SuitIdentifier.RED_DRAGON:
				return "Red Dragons";
			case SuitIdentifier.WHITE_DRAGON:
				return "White Dragons";
			case SuitIdentifier.EAST_WIND:
				return "East Winds";
			case SuitIdentifier.SOUTH_WIND:
				return "South Winds";
			case SuitIdentifier.WEST_WIND:
				return "West Winds";
			case SuitIdentifier.NORTH_WIND:
				return "North Winds";
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
			{return SuitColor.HONOURS;}
		}
	}
}
