using System;
using System.Collections.Generic;
using Engine.Cards;
using Engine.Cards.CardTypes;
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
		/// <param name="seat_wind">The player's seat wind.</param>
		/// <param name="prevailing_wind">The round's prevailing wind.</param>
		/// <param name="self_pick">If true, then the player won by drawing from the wall.</param>
		/// <param name="last_catch">If true, then the player won by drawing the last tile from the wall.</param>
		/// <param name="win_by_replacement">If true, then the player won by drawing a replacement tile from a kong or bonus tile.</param>
		/// <param name="double_replacement">If true, then the player won on a replacement tile for a replacement tile.</param>
		/// <param name="robbing_kong">If true, then the player won by robbing the kong.</param>
		/// <param name="heavenly_hand">If true, then the player won on their starting hand (only applies to east).</param>
		/// <param name="earthly_hand">If true, then the player won on east's discard.</param>
		/// <returns>Returns the fan of the provided hand.</returns>
		public static int HandFan(List<MahjongMeld> melds, SuitIdentifier seat_wind, SuitIdentifier prevailing_wind, bool self_pick = false, bool last_catch = false, bool win_by_replacement = false, bool double_replacement = false, bool robbing_kong = false, bool heavenly_hand = false, bool earthly_hand = false)
		{
			// If we don't have a hand, abandon ship
			if(!IsMahjong(melds))
				return -1;
			
			// We know we have a winning hand, so ensure we get the limit value
			if(melds.Count == 1 || double_replacement || heavenly_hand || earthly_hand)
				return int.MaxValue;

			// For convenience
			MahjongMeld eye = null;
			List<MahjongMeld> leye = new List<MahjongMeld>();
			List<MahjongMeld> cpk_melds = new List<MahjongMeld>();

			foreach(MahjongMeld meld in melds)
				if(meld.Eye)
					eye = meld.Clone(); // There is exactly one of these since we know we have a winning hand that isn't a special hand
				else
					cpk_melds.Add(meld); // Since we have a winning hand, everything but the eye is a chow, pung, or kong
			
			leye.Add(eye);

			// Check for limit hands first
			int winds = ContainsSuit(cpk_melds,SuitIdentifier.EAST_WIND) + ContainsSuit(cpk_melds,SuitIdentifier.SOUTH_WIND) + ContainsSuit(cpk_melds,SuitIdentifier.WEST_WIND) + ContainsSuit(cpk_melds,SuitIdentifier.NORTH_WIND);

			// Great winds
			if(winds == 4)
				return int.MaxValue;

			// Small winds
			if(winds == 3 && ContainsSuit(leye,SuitIdentifier.EAST_WIND) + ContainsSuit(leye,SuitIdentifier.SOUTH_WIND) + ContainsSuit(leye,SuitIdentifier.WEST_WIND) + ContainsSuit(leye,SuitIdentifier.NORTH_WIND) == 1)
				return int.MaxValue;

			// All kongs and self triplets
			bool all_kongs = true;
			bool self_pks = self_pick; // Self triplets must be won by self pick

			foreach(MahjongMeld meld in cpk_melds)
			{
				if(!meld.Kong)
					all_kongs = false;

				if(!meld.Concealed || !(meld.Pung || meld.Kong))
					self_pks = false;
			}

			if(all_kongs || self_pks)
				return int.MaxValue;

			// All orphans
			bool ophans = true;
			
			foreach(MahjongMeld meld in cpk_melds) // Here we accept melds of ones and nines only
				if(!meld.Pung && !meld.Kong || !ApproxEqual(meld.Cards[0].Value.MaxValue,1.0) && !ApproxEqual(meld.Cards[0].Value.MaxValue,9.0))
					ophans = false;

			if(ophans)
				return int.MaxValue;

			// We have a boring hand, so calculate its points
			int ret = 0;
			
			// The below all have their points reduced so that the presence of the dragons/winds will bump up their point values as appropriate later
			if(ContainsSuit(cpk_melds,SuitIdentifier.GREEN_DRAGON) + ContainsSuit(cpk_melds,SuitIdentifier.RED_DRAGON) + ContainsSuit(cpk_melds,SuitIdentifier.WHITE_DRAGON) == 3)
				ret = 5; // Great dragon: 8 (5 + 3)
			else if(ContainsSuit(cpk_melds,SuitIdentifier.GREEN_DRAGON) + ContainsSuit(cpk_melds,SuitIdentifier.RED_DRAGON) + ContainsSuit(cpk_melds,SuitIdentifier.WHITE_DRAGON) == 2 && ContainsSuit(leye,SuitIdentifier.GREEN_DRAGON) + ContainsSuit(leye,SuitIdentifier.RED_DRAGON) + ContainsSuit(leye,SuitIdentifier.WHITE_DRAGON) == 1)
				ret = 3; // Small dragon: 5 (3 + 2)
			else if(ContainsTileType(cpk_melds,SuitColor.HONOURS) + ContainsTileType(leye,SuitColor.HONOURS) == 5)
				ret = 10 - ContainsSuit(cpk_melds,SuitIdentifier.GREEN_DRAGON) - ContainsSuit(cpk_melds,SuitIdentifier.RED_DRAGON) - ContainsSuit(cpk_melds,SuitIdentifier.WHITE_DRAGON); // All honours: 10 (10 - drags + drags)
			else if(ContainsSuit(cpk_melds,SuitIdentifier.BAMBOO) + ContainsSuit(leye,SuitIdentifier.BAMBOO) == 5 || ContainsSuit(cpk_melds,SuitIdentifier.CHARACTER) + ContainsSuit(leye,SuitIdentifier.CHARACTER) == 5 || ContainsSuit(cpk_melds,SuitIdentifier.DOT) + ContainsSuit(leye,SuitIdentifier.DOT) == 5)
				ret = 7; // All one suit: 7
			else if(ContainsSuit(cpk_melds,SuitIdentifier.BAMBOO) + ContainsSuit(leye,SuitIdentifier.BAMBOO) + ContainsTileType(cpk_melds,SuitColor.HONOURS) + ContainsTileType(leye,SuitColor.HONOURS) == 5 ||
					ContainsSuit(cpk_melds,SuitIdentifier.CHARACTER) + ContainsSuit(leye,SuitIdentifier.CHARACTER) + ContainsTileType(cpk_melds,SuitColor.HONOURS) + ContainsTileType(leye,SuitColor.HONOURS) == 5 ||
					ContainsSuit(cpk_melds,SuitIdentifier.DOT) + ContainsSuit(leye,SuitIdentifier.DOT) + ContainsTileType(cpk_melds,SuitColor.HONOURS) + ContainsTileType(leye,SuitColor.HONOURS) == 5)
				ret = 3; // Mixed one suit: 3
			else
			{
				bool all_chow = true;
				bool all_in_triplet = true;

				foreach(MahjongMeld meld in cpk_melds)
					if(meld.Chow)
						all_in_triplet = false;
					else
						all_chow = false;

				if(all_in_triplet)
					ret = 3; // All in triplet
				else if(all_chow)
					ret = 1; // All chow
			}
			
			// The above are calculated so that we can add these special points without worrying over what type of hand we had for double counting
			if(ContainsSuit(cpk_melds,seat_wind) > 0)
				ret++;

			if(ContainsSuit(cpk_melds,prevailing_wind) > 0)
				ret++;

			if(ContainsSuit(cpk_melds,SuitIdentifier.RED_DRAGON) > 0)
				ret++;

			if(ContainsSuit(cpk_melds,SuitIdentifier.GREEN_DRAGON) > 0)
				ret++;

			if(ContainsSuit(cpk_melds,SuitIdentifier.WHITE_DRAGON) > 0)
				ret++;

			// All oprhans is a limit hand, so the mixed orphans bonus is fine to put here like this
			bool mixed_ophans = true;
			
			foreach(MahjongMeld meld in cpk_melds) // Here we accept melds of honours, ones, and nines only
				if(!meld.Pung && !meld.Kong || !ApproxEqual(meld.Cards[0].Value.MaxValue,1.0) && !ApproxEqual(meld.Cards[0].Value.MaxValue,9.0) && meld.Cards[0].Suit.Color != SuitColor.HONOURS)
					mixed_ophans = false;

			if(mixed_ophans)
				ret++;

			// Self pick is drawing the winning tile
			if(self_pick)
				ret++;

			if(robbing_kong)
				ret++;

			if(last_catch)
				ret++;

			if(win_by_replacement)
				ret++;

			// Win from wall is if you never melded from a discard, that is all melds are concealed
			bool all_concealed = true;

			for(int i = 0;i < melds.Count;i++)
				if(melds[i].Exposed)
				{
					all_concealed = false;
					break;
				}

			if(all_concealed)
				ret++;
			
			return ret;
		}
		
		private static int ContainsSuit(List<MahjongMeld> melds, SuitIdentifier suit)
		{
			int ret = 0;

			foreach(MahjongMeld meld in melds)
				if(meld.Cards[0].Suit.ID == suit) // Melds can only be made of uniform suits, so this is fine
					ret++;

			return ret;
		}

		private static int ContainsTileType(List<MahjongMeld> melds, SuitColor type)
		{
			int ret = 0;

			foreach(MahjongMeld meld in melds)
				if(meld.Cards[0].Suit.Color == type) // Melds can only be made of uniform suits, which implies uniform type, so this is fine
					ret++;

			return ret;
		}
		
		private static bool ApproxEqual(double d1, double d2, double epsilon = 0.0001)
		{return Math.Abs(d1 - d2) < epsilon;}

		/// <summary>
		/// Converts hand fan into base points.
		/// </summary>
		/// <param name="fan">The number of fan.</param>
		public static int FanToBasePoints(int fan)
		{
			if(fan > 9)
				return 128; // This is the limit

			switch(fan)
			{
			case 3:
				return 8;
			case 4:
				return 16;
			case 5:
				return 24;
			case 6:
				return 32;
			case 7:
				return 48;
			case 8:
				return 64;
			case 9:
				return 96;
			default:
				break;
			}

			return 0;
		}

		/// <summary>
		/// Determines if the given melds form a winning hand.
		/// </summary>
		/// <param name="melds">The melds to use.</param>
		/// <returns>Returns true if the given melds are a winning hand and false otherwise.</returns>
		public static bool IsMahjong(List<MahjongMeld> melds)
		{
			// If we only have one meld, we better have a special hand
			if(melds.Count == 1)
				if(melds[0].SevenPair || melds[0].NineGates || melds[0].ThirteenOrphans) // Any of these being true automaticaly makes the meld valid, so we don't need to check that here
					return true;
				else
					return false;

			// We don't have a special hand, so we better have exactly five melds
			if(melds.Count != 5)
				return false;

			int eyes = 0;
			int sanity_check = 0;

			// If any meld is invalid, we're done
			foreach(MahjongMeld meld in melds)
				if(!meld.Valid)
					return false;
				else if(meld.Eye)
					eyes++;
				else if(meld.Chow || meld.Pung || meld.Kong)
					sanity_check++;

			// We need exactly one eye and we need to pass the sanity check on our meld types
			if(eyes != 1 || sanity_check != 4)
				return false;

			// At this point nothing more can go wrong, so we're done
			return true;
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
		/// <param name="seat_wind">The player's seat wind.</param>
		/// <param name="prevailing_wind">The round's prevailing wind.</param>
		/// <param name="self_pick">If true, then the player won by drawing from the wall.</param>
		/// <returns>The best mahjong that can be formed from the given tiles or null if no mahjong can be formed.</returns>
		public static List<MahjongMeld> BestMahjong(Hand hand, List<MahjongMeld> melds, SuitIdentifier seat_wind, SuitIdentifier prevailing_wind, bool self_pick = false)
		{
			List<MahjongMeld> ret = null;
			int max = -1;
			
			// Note that if the list is empty, we default to returning null as desired
			foreach(List<MahjongMeld> mahjong in GetAllMahjongs(hand,melds))
			{
				int fan = HandFan(mahjong,seat_wind,prevailing_wind,self_pick);

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
		/// This algorithm does not clobber data.
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
		/// This algorithm does not clobber data.
		/// </summary>
		/// <param name="hand">The set of tiles free to make melds as desired.</param>
		/// <param name="melds">The set of melds that cannot be changed.</param>
		/// <param name="ret">The set to add new winning hands to.</param>
		private static void GetAllMahjongs(Hand hand, List<MahjongMeld> melds, List<List<MahjongMeld>> ret)
		{
			// If we have less than 2 tiles left, something has gone horribly wrong
			if(hand.CardsInHand < 2)
				return;
			
			// If we have a limit hand, then get it out of the way right away
			if(hand.CardsInHand == 14) // Note that this can only happen if we have no fixed melds
			{
				MahjongMeld meld = new MahjongMeld(hand.Cards,true);
				
				if(meld.Valid)
				{
					List<MahjongMeld> temp = new List<MahjongMeld>();
					temp.Add(meld);

					ret.Add(temp);
				}
			}

			// If we have two tiles left, then we need them to be an eye
			if(hand.CardsInHand == 2)
			{
				MahjongMeld meld = new MahjongMeld(hand.Cards,true);

				// If we don't have an eye, this won't form a winning hand
				if(!meld.Eye)
					return;

				// If we have an eye, then we need to make sure what we have is valid
				// Copying the data into a new list here is very important, because we're going to return a whole bunch of lists that need to all be distinct
				List<MahjongMeld> melds2 = new List<MahjongMeld>();

				foreach(MahjongMeld m in melds)
					melds2.Add(m.Clone());

				melds2.Add(meld);

				if(IsMahjong(melds2)) // It should be, but it's possible we were originally passed weird melds
					ret.Add(melds2);

				return; // We have nothing further to do, so return
			}

			// A simple check for failure
			// We must have an eye and melds of size 3 as we don't have extra tiles to make kongs
			if((hand.CardsInHand - 2) % 3 != 0)
				return;

			// This call for BRUTE FORCE
			for(int i = 0;i < hand.CardsInHand - 2;i++)
				for(int j = i + 1;j < hand.CardsInHand - 1;j++)
					for(int k = j + 1;k < hand.CardsInHand;k++)
					{
						// Check if (i,j,k) forms a meld
						List<Card> l = new List<Card>();

						l.Add(hand.Cards[i]);
						l.Add(hand.Cards[j]);
						l.Add(hand.Cards[k]);

						MahjongMeld meld = new MahjongMeld(l,true);

						// If we do have a meld, meld it and go deeper
						if(meld.Valid)
						{
							// Add the meld
							melds.Add(meld);

							// We want to preserve card order in the hand, and the easiest way to do that is to just make a new hand
							Hand temp = new StandardHand(hand.Cards);
							
							// i < j < k, so this is fine
							temp.PlayCard(k);
							temp.PlayCard(j);
							temp.PlayCard(i);

							// Go deeper
							GetAllMahjongs(temp,melds,ret);

							// Remove the meld
							melds.RemoveAt(melds.Count - 1);
						}
					}
			
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
