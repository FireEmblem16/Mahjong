using System.Collections.Generic;
using Engine.Cards;
using Engine.Cards.Hands;

namespace Mahjong
{
	/// <summary>
	/// Contains useful static functions for Mahjong
	/// </summary>
	public static class MahjongStaticFunctions
	{
		/// <summary>
		/// Counts the number of fan in the given hand.
		/// </summary>
		/// <param name="melds">The melds of the hand.</param>
		/// <returns>Returns the fan of the provided hand.</returns>
		public static int HandFan(List<MahjongMeld> melds)
		{


			return -1;
		}

		/// <summary>
		/// Determines if the given hand and melds form a mahjong with the extra tile provided.
		/// If no tile is provided (that is, it's null), then it's assumed that the hand contains all the necessary tiles.
		/// This algorithm does not clobber data.
		/// </summary>
		/// <param name="hand">The tiles in the hand.</param>
		/// <param name="melds">The melds already formed.</param>
		/// <param name="tile">The fourteenth tile (+- extras from kongs). If null, then the algorithm assumes that the hand and melds contain the full tileset necessary to go out.</param>
		/// <returns>Returns true if the given data represents a mahjong and false otherwise.</returns>
		public static bool HasMahjong(Hand hand, List<MahjongMeld> melds, Card tile = null)
		{
			// Copy all the data so we don't clobber it
			Hand h;
			List<MahjongMeld> m;

			CopyData(hand,melds,out h,out m);

			if(tile != null)
				h.DrawCard(tile.Clone());

			// Now that we have data we can mess with as much as we want, let's search for melds
			// The easiest (and most unlikely) way to make a mahjong is to use 14 tiles in a special hand
			if(hand.CardsInHand == 14) // This can only happen if there are no melds, so we don't have the edge case to worry about
				if(new MahjongMeld(hand.Cards,true).Valid)
					return true;



			return false;
		}

		/// <summary>
		/// Takes a hand and a set of fixed melds and returns the highest scoring winning hand possible.
		/// This algorithm does not clobber data.
		/// </summary>
		/// <param name="hand">The set of tiles free to make melds as desired.</param>
		/// <param name="melds">The set of melds that cannot be changed.</param>
		/// <returns>The best mahjong that can be formed from the given tiles or null if no mahjong can be formed.</returns>
		public static List<MahjongMeld> BestMahjong(Hand hand, List<MahjongMeld> melds)
		{
			if(!HasMahjong(hand,melds))
				return null;



			return null;
		}
		
		/// <summary>
		/// Creates a deep copy in the manner expected.
		/// </summary>
		private static void CopyData(Hand hand, List<MahjongMeld> melds, out Hand h, out List<MahjongMeld> m)
		{
			h = new StandardHand(hand.Cards);
			m = new List<MahjongMeld>();

			foreach(MahjongMeld meld in melds)
				m.Add(meld.Clone());

			return;
		}
	}
}
