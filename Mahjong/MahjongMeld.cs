using System;
using System.Collections.Generic;
using Engine.Cards;
using Engine.Cards.CardTypes;
using Engine.Cards.CardTypes.Suits.Mahjong;
using Engine.Cards.CardTypes.Values;
using Engine.Cards.Hands;

namespace Mahjong
{
	/// <summary>
	/// Represents a meld.
	/// </summary>
	public class MahjongMeld : StandardHand
	{
		/// <summary>
		/// Creates a new meld from the given tiles.
		/// </summary>
		/// <param name="meld">The tiles to meld.</param>
		/// <param name="concealed">If true, the meld will be treated as concealed.</param>
		public MahjongMeld(IEnumerable<Card> meld, bool concealed = false) : base(meld)
		{
			Chow = false;
			Pung = false;
			Kong = false;
			Eye = false;

			ThirteenOrphans = false;
			NineGates = false;
			SevenPair = false;

			Concealed = concealed;
			ExposedFromExposed = false;

			// There are two special cases we need to handle
			// There's thirteen orphans and nine gates (the latter is a special case because it must be all concealed and has meaning beyond the melds it makes)
			if(CardsInHand == 14)
			{
				// Thirteen orphans is easier to check, so do that first
				Card[] orphans = {new StandardCard(new SimpleSuit(SuitIdentifier.BAMBOO),new ValueN(1,"One")),new StandardCard(new SimpleSuit(SuitIdentifier.BAMBOO),new ValueN(9,"Nine")),
								  new StandardCard(new SimpleSuit(SuitIdentifier.CHARACTER),new ValueN(1,"One")),new StandardCard(new SimpleSuit(SuitIdentifier.CHARACTER),new ValueN(9,"Nine")),
								  new StandardCard(new SimpleSuit(SuitIdentifier.DOT),new ValueN(1,"One")),new StandardCard(new SimpleSuit(SuitIdentifier.DOT),new ValueN(9,"Nine")),
								  new StandardCard(new HonourSuit(SuitIdentifier.GREEN_DRAGON),new ValueN(0,null),"Green Dragon"),new StandardCard(new HonourSuit(SuitIdentifier.RED_DRAGON),new ValueN(0,null),"Red Dragon"),new StandardCard(new HonourSuit(SuitIdentifier.WHITE_DRAGON),new ValueN(0,null),"White Dragon"),
								  new StandardCard(new HonourSuit(SuitIdentifier.EAST_WIND),new ValueN(0,null),"East Wind"),new StandardCard(new HonourSuit(SuitIdentifier.SOUTH_WIND),new ValueN(0,null),"South Wind"),new StandardCard(new HonourSuit(SuitIdentifier.WEST_WIND),new ValueN(0,null),"West Wind"),new StandardCard(new HonourSuit(SuitIdentifier.NORTH_WIND),new ValueN(0,null),"North Wind")};

				for(int i = 0;i < 13;i++)
					if(!Cards.Contains(orphans[i]))
						break;
					else if(i == 12)
					{
						ThirteenOrphans = true;
						return;
					}

				// We don't have thirteen orphans, so check for seven pairs
				Hand temp = new StandardHand(Cards);

				// Check for pairs by sheer brute force
				for(int i = 0;i < 7;i++)
				{
					Card c = temp.PlayCard(0);

					if(!temp.Cards.Contains(c))
						break;

					temp.PlayCard(c);

					if(i == 6)
					{
						SevenPair = true;
						return;
					}
				}

				// We don't have thirteen orphans or seven pairs, so check for nine gates
				CardSuit suit = Cards[0].Suit;

				// For optimisation, we check this here rather than letting it be implicitly check later by asking for values
				if(suit.Color != SuitColor.SIMPLE)
					return;

				// Each card must be the same suit
				for(int i = 1;i < 14;i++)
					if(!suit.Equals(Cards[i].Suit))
						return;

				// Now we need the tiles 1112345678999 and one extra
				temp = new StandardHand(Cards);
				string[] value_names = {"One","Two","Three","Four","Five","Six","Seven","Eight","Nine"};
				
				// Check if all the cards are there by brute force
				for(int i = 1;i < 10;i++)
					for(int j = (i == 1 || i == 9 ? 0 : 2);j < 3;j++) // Ones and nines need to be played thrice
					{
						Card c = new StandardCard(suit,new ValueN(i,value_names[i - 1]));

						if(!temp.Cards.Contains(c))
							return;

						temp.PlayCard(c);
					}

				// The last card doesn't matter as we know it's the right suit, so we're done
				NineGates = true;
				return;
			}

			if(CardsInHand < 2 || CardsInHand > 4)
				return;

			if(CardsInHand == 2)
			{
				if(Cards[0].Equals(Cards[1]))
					Eye = true;

				return;
			}

			// If we have four tiles, our only option is a kong
			if(CardsInHand == 4)
			{
				// A kong must have four equal tiles
				for(int i = 0;i < 3;i++)
					if(!Cards[i].Equals(Cards[i+1]))
						return;

				Kong = true;
				return;
			}

			// If we have three tiles and the first two are equal, we either have a pung or nothing
			if(Cards[0].Equals(Cards[1]))
				if(!Cards[1].Equals(Cards[2]))
					return;
				else
				{
					Pung = true;
					return;
				}

			// We now have a chow or nothing, so make sure the tiles are of the same suit and are sequential
			if(!Cards[0].Suit.Equals(Cards[1].Suit) || !Cards[1].Suit.Equals(Cards[2].Suit))
				return;

			SortByValue();

			if(!ApproxEqual(Cards[0].Value.MaxValue + 1.0,Cards[1].Value.MaxValue) || !ApproxEqual(Cards[1].Value.MaxValue + 1.0,Cards[2].Value.MaxValue))
				return;

			Chow = true;
			return;
		}

		protected bool ApproxEqual(double d1, double d2, double epsilon = 0.0001)
		{return Math.Abs(d1 - d2) < epsilon;}

		/// <summary>
		/// Converts a pung to a kong if able.
		/// </summary>
		/// <param name="c">The tile to add to the pung.</param>
		/// <returns>Returns true if the tile could be added and false otherwise.</returns>
		public bool ConvertToKong(Card c)
		{
			if(!Pung)
				return false;

			if(!Cards[0].Equals(c))
				return false;

			// Add the card to the meld
			DrawCard(c);

			// Update our flags
			Pung = false;
			Kong = true;

			if(Exposed)
				ExposedFromExposed = true;

			return true;
		}

		/// <summary>
		/// Creates a deep copy of this meld.
		/// </summary>
		/// <returns>Returns a copy of this meld.</returns>
		public MahjongMeld Clone()
		{
			MahjongMeld ret = new MahjongMeld(Cards);

			// We have to do this manually, because the above constructor could potentially clobber data
			ret.Chow = Chow;
			ret.Pung = Pung;
			ret.Kong = Kong;
			ret.Eye = Eye;

			ret.ThirteenOrphans = ThirteenOrphans;
			ret.NineGates = NineGates;
			ret.SevenPair = SevenPair;

			ret.Concealed = Concealed;
			ret.ExposedFromExposed = ExposedFromExposed;

			return ret;
		}

		/// <summary>
		/// Returns the meld tiles in a list.
		/// </summary>
		public override string ToString()
		{
			if(!Valid)
				return "[]";

			string ret = "[" + Cards[0];

			for(int i = 1;i < CardsInHand;i++)
				ret += ", " + Cards[i];
			
			return ret + "]";
		}

		/// <summary>
		/// True if this is a valid meld and false otherwise.
		/// </summary>
		public bool Valid
		{
			get
			{return Chow || Pung || Kong || Eye || ThirteenOrphans || NineGates || SevenPair;}
		}
		
		/// <summary>
		/// True if this meld is a chow.
		/// </summary>
		public bool Chow
		{get; protected set;}

		/// <summary>
		/// True if this meld is a pung.
		/// </summary>
		public bool Pung
		{get; protected set;}

		/// <summary>
		/// True if this meld is a kong
		/// </summary>
		public bool Kong
		{get; protected set;}

		/// <summary>
		/// True if this meld is an eye, which technically can't be melded, but it's a useful thing to allow here for code elsewhere.
		/// </summary>
		public bool Eye
		{get; protected set;}

		/// <summary>
		/// True if this is a meld of thirteen orphans (plus one extra tile)
		/// </summary>
		public bool ThirteenOrphans
		{get; protected set;}

		/// <summary>
		/// True if this is a meld of nine gates (plus its fourteenth tile to win).
		/// Must be completely concealed.
		/// </summary>
		public bool NineGates
		{get; protected set;}

		/// <summary>
		/// True if this is a meld of seven pairs.
		/// </summary>
		public bool SevenPair
		{get; protected set;}

		/// <summary>
		/// True if this is a concealed meld, that is it was not made from a discarded tile.
		/// </summary>
		public bool Concealed
		{get; protected set;}

		/// <summary>
		/// True if this is an exposed meld.
		/// </summary>
		public bool Exposed
		{
			get
			{return !Concealed;}
		}

		/// <summary>
		/// True if this is a kong formed by adding a drawn tile to an exposed pung.
		/// </summary>
		public bool ExposedFromExposed
		{get; protected set;}
	}
}
