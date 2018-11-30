using System.Collections.Generic;
using Engine.Cards.CardTypes;
using Engine.Cards.CardTypes.Suits.Mahjong;
using Engine.Cards.CardTypes.Values;

namespace Engine.Cards.DeckTypes
{
	/// <summary>
	/// A standard set of Mahjong tiles.
	/// </summary>
	public class MahjongDeck : StandardDeck
	{
		/// <summary>
		/// Creates a standard set of Mahjong tiles.
		/// </summary>
		public MahjongDeck() : base(false,false)
		{return;}

		/// <summary>
		/// Creates a standard tileset.
		/// </summary>
		/// <returns>Returns a list of cards that contains a standard tileset.</returns>
		protected override List<Card> CreateDeck()
		{
			List<Card> ret = new List<Card>(200); // There are 144 tiles, so make room for them and some extra so we don't waste time expanding the list

			// There are four of every tile, so let's just define the basic set and add it four times
			List<Card> subset = new List<Card>(50);
			string[] value_names = {"One","Two","Three","Four","Five","Six","Seven","Eight","Nine"};

			// Add the simples
			for(int i = 1;i < 10;i++)
			{
				subset.Add(new StandardCard(new SimpleSuit(SuitIdentifier.BAMBOO),new ValueN(i,value_names[i-1])));
				subset.Add(new StandardCard(new SimpleSuit(SuitIdentifier.CHARACTER),new ValueN(i,value_names[i-1])));
				subset.Add(new StandardCard(new SimpleSuit(SuitIdentifier.DOT),new ValueN(i,value_names[i-1])));
			}

			// Add the dragons
			subset.Add(new StandardCard(new HonourSuit(SuitIdentifier.GREEN_DRAGON),new ValueN(0,null),"Green Dragon"));
			subset.Add(new StandardCard(new HonourSuit(SuitIdentifier.RED_DRAGON),new ValueN(0,null),"Red Dragon"));
			subset.Add(new StandardCard(new HonourSuit(SuitIdentifier.WHITE_DRAGON),new ValueN(0,null),"White Dragon"));

			// Add the winds
			subset.Add(new StandardCard(new HonourSuit(SuitIdentifier.EAST_WIND),new ValueN(0,null),"East Wind"));
			subset.Add(new StandardCard(new HonourSuit(SuitIdentifier.SOUTH_WIND),new ValueN(0,null),"South Wind"));
			subset.Add(new StandardCard(new HonourSuit(SuitIdentifier.WEST_WIND),new ValueN(0,null),"West Wind"));
			subset.Add(new StandardCard(new HonourSuit(SuitIdentifier.NORTH_WIND),new ValueN(0,null),"North Wind"));

			// Loop!
			for(int i = 0;i < 4;i++)
				foreach(Card c in subset)
					ret.Add(c.Clone());

			// Add the singletone flowers and seasons
			ret.Add(new StandardCard(new BonusSuit(SuitIdentifier.FLOWER),new ValueN((int)SuitIdentifier.EAST_WIND,"(East)"),"Plum"));
			ret.Add(new StandardCard(new BonusSuit(SuitIdentifier.FLOWER),new ValueN((int)SuitIdentifier.SOUTH_WIND,"(South)"),"Orchid"));
			ret.Add(new StandardCard(new BonusSuit(SuitIdentifier.FLOWER),new ValueN((int)SuitIdentifier.WEST_WIND,"(West)"),"Chrysanthemum"));
			ret.Add(new StandardCard(new BonusSuit(SuitIdentifier.FLOWER),new ValueN((int)SuitIdentifier.NORTH_WIND,"(North)"),"Bamboo"));

			ret.Add(new StandardCard(new BonusSuit(SuitIdentifier.SEASON),new ValueN((int)SuitIdentifier.EAST_WIND,"(East)"),"Spring"));
			ret.Add(new StandardCard(new BonusSuit(SuitIdentifier.SEASON),new ValueN((int)SuitIdentifier.SOUTH_WIND,"(South)"),"Summer"));
			ret.Add(new StandardCard(new BonusSuit(SuitIdentifier.SEASON),new ValueN((int)SuitIdentifier.WEST_WIND,"(West)"),"Autumn"));
			ret.Add(new StandardCard(new BonusSuit(SuitIdentifier.SEASON),new ValueN((int)SuitIdentifier.NORTH_WIND,"(North)"),"Winter"));

			return ret;
		}

		/// <summary>
		/// The name of the deck.
		/// </summary>
		public override string DeckName
		{
			get
			{return "Mahjong Tileset";}
		}
	}
}
