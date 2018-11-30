using System;
using Engine.Cards;
using Engine.Cards.DeckTypes;

namespace Mahjong
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Deck d = new MahjongDeck();
			Console.Out.WriteLine(d);

			return;
		}
	}
}
