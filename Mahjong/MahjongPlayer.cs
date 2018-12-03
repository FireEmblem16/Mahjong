using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Cards;
using Engine.Cards.CardTypes;

namespace Mahjong
{
	/// <summary>
	/// An interface that allows easy access to data inside Mahjong players.
	/// </summary>
	public interface MahjongPlayer
	{
		/// <summary>
		/// Causes the player's wind to shift to the next direction.
		/// </summary>
		void RotateWind();

		/// <summary>
		/// Copies the data in the given player into this one.
		/// </summary>
		/// <param name="mp">The player data to copy.</param>
		void CopyData(MahjongPlayer mp);

		/// <summary>
		/// The revealed melds of the player.
		/// </summary>
		List<MahjongMeld> Melds
		{get;}

		/// <summary>
		/// The player's current score.
		/// </summary>
		int Score
		{get; set;}

		/// <summary>
		/// The bonus tiles currently in the player's possession.
		/// </summary>
		Hand BonusTiles
		{get;}

		/// <summary>
		/// The current wind of the seat the player is at.
		/// </summary>
		SuitIdentifier SeatWind
		{get; set;}

		/// <summary>
		/// This is totally coupling and bad, but it's here anyway. This is the current prevailing wind.
		/// </summary>
		SuitIdentifier PrevailingWind
		{get; set;}
	}
}
