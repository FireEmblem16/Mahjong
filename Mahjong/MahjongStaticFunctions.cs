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
		/// Counts the number of fan in the given hand. This only works on completed hands.
		/// Returns -1 if the melds do not form a valid hand.
		/// </summary>
		/// <param name="melds">The melds of the hand.</param>
		/// <returns>Returns the fan of the provided hand.</returns>
		public static int HandFan(List<MahjongMeld> melds)
		{
			// If we don't have a hand, abandon ship
			if(!IsMahjong(melds))
				return -1;



			return -1;
		}

		/// <summary>
		/// Determines if the given melds form a winning hand.
		/// </summary>
		/// <param name="melds">The melds to use.</param>
		/// <returns>Returns true if the given melds are a winning hand and false otherwise.</returns>
		public static bool IsMahjong(List<MahjongMeld> melds)
		{


			return false;
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

			// There's really not much runtime difference between finding A and finding ALL winning hands, but this is much easier to program
			return GetAllMahjongs(h,m).Count > 0;
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
			List<MahjongMeld> ret = null;
			int max = -1;

			// Note that if the list is empty, we default to returning null as desired
			foreach(List<MahjongMeld> mahjong in GetAllMahjongs(hand,melds))
			{
				int fan = HandFan(mahjong);

				if(fan > max)
				{
					ret = mahjong;
					max = fan;
				}
			}

			return ret;
		}
		
		/// <summary>
		/// Gets all of the possible mahjong hands made from the given tiles.
		/// </summary>
		/// <param name="hand">The set of tiles free to make melds as desired.</param>
		/// <param name="melds">The set of melds that cannot be changed.</param>
		/// <returns>A list containing all available winning hands that can be made. The list is empty if no such hands exist.</returns>
		public static List<List<MahjongMeld>> GetAllMahjongs(Hand hand, List<MahjongMeld> melds)
		{
			List<List<MahjongMeld>> ret = new List<List<MahjongMeld>>();
			GetAllMahjongs(hand,melds,ret);

			return ret;
		}

		/// <summary>
		/// The recursive version of the GetAllMahjongs function.
		/// </summary>
		/// <param name="hand">The set of tiles free to make melds as desired.</param>
		/// <param name="melds">The set of melds that cannot be changed.</param>
		/// <param name="ret">The set to add new winning hands to.</param>
		private static void GetAllMahjongs(Hand hand, List<MahjongMeld> melds, List<List<MahjongMeld>> ret)
		{
			if(!HasMahjong(hand,melds))
				return;







			/*
			// Copy all the data so we don't clobber it
			Hand h;
			List<MahjongMeld> m;

			CopyData(hand,melds,out h,out m);

			if(tile != null)
				h.DrawCard(tile.Clone());

			// Now that we have data we can mess with as much as we want, let's search for melds
			// The easiest (and most unlikely) way to make a mahjong is to use 14 tiles in a special hand
			if(hand.CardsInHand == 14) // This can only happen if there are no melds, so we don't have the edge case to worry about
				if(FormsMeld(hand))
					return true;

			// The other extreme is that the melds already contain everything and we need to make sure they're not batshit insane
			 */

			return;
		}

		/// <summary>
		/// Determines if the given tiles forms a chow, pong, or kong.
		/// </summary>
		/// <param name="tiles">The set of tiles to meld.</param>
		public static bool FormsCPK(Hand tiles)
		{
			if(tiles.CardsInHand < 3 || tiles.CardsInHand > 4)
				return false; // The meld construction catches this, of course, but this is a quick and dirty yes/no

			MahjongMeld m = new MahjongMeld(tiles.Cards);
			return m.Chow || m.Pung || m.Kong;
		}

		/// <summary>
		/// Determines if the given tiles forms an eye.
		/// </summary>
		/// <param name="tiles">The set of tiles to meld.</param>
		public static bool FormsEye(Hand tiles)
		{
			if(tiles.CardsInHand != 2)
				return false; // The meld construction catches this, of course, but this is a quick and dirty yes/no

			MahjongMeld m = new MahjongMeld(tiles.Cards);
			return m.Eye;
		}

		/// <summary>
		/// Returns true if the provided tiles forms a meld.
		/// </summary>
		/// <param name="tiles">The set of tiles to make a meld of.</param>
		public static bool FormsMeld(Hand tiles)
		{return new MahjongMeld(tiles.Cards).Valid;}

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
