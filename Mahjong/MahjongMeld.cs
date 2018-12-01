using System;
using System.Collections.Generic;
using Engine.Cards;
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
			Concealed = concealed;

			Chow = false;
			Pung = false;
			Kong = false;
			ExposedFromExposed = false;

			if(CardsInHand < 3)
				return;

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
			ret.Concealed = Concealed;
			ret.ExposedFromExposed = ExposedFromExposed;

			return ret;
		}

		/// <summary>
		/// True if this is a valid meld and false otherwise.
		/// </summary>
		public bool Valid
		{
			get
			{return Chow || Pung || Kong;}
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
