using System;
using Engine.Cards;
using Engine.Cards.DeckTypes;
using Engine.Cards.Hands;

namespace Mahjong
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Deck d = new MahjongDeck();
			Hand h = new StandardHand();

			h.DrawCard(d.Deck[0]);
			h.DrawCard(d.Deck[3]);
			h.DrawCard(d.Deck[6]);

			MahjongMeld m = new MahjongMeld(h.Cards);

			return;
		}
	}
}
