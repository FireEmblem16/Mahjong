using System.Collections.Generic;
using Engine.Cards;

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
		/// </summary>
		/// <param name="hand">The tiles in the hand.</param>
		/// <param name="melds">The melds already formed.</param>
		/// <param name="tile">The fourteenth tile (+- extras from kongs). If null, then the algorithm assumes that the hand and melds contain the full tileset necessary to go out.</param>
		/// <returns>Returns true if the given data represents a mahjong and false otherwise.</returns>
		public static bool HasMahjongWithTile(Hand hand, List<MahjongMeld> melds, Card tile = null)
		{


			return false;
		}
	}
}
