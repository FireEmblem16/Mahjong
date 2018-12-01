using System;
using Engine.Cards;
using Engine.Cards.DeckTypes;

namespace Mahjong
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Deck d1 = new StandardDeck();
			Console.WriteLine(d1.Draw());
			Console.WriteLine(d1.Draw());
			Console.WriteLine(d1.Draw());

			Deck d2 = d1.Clone();

			Console.WriteLine(d1.Draw());
			Console.WriteLine(d1.Draw());

			Console.WriteLine(d2.Draw());
			Console.WriteLine(d2.Draw());

			return;
		}
	}
}
