using System;
using System.Collections.Generic;
using Engine.Cards;
using Engine.Cards.CardTypes;
using Engine.Cards.Hands;
using Engine.Player;

namespace Mahjong
{
	/// <summary>
	/// A human Mahjong player.
	/// </summary>
	public class MahjongHumanPlayer : StandardPlayer<MahjongMove>, MahjongPlayer
	{
		/// <summary>
		/// Creates a new player for a Mahjong game.
		/// </summary>
		/// <param name="cards">The cards this player starts with.</param>
		public MahjongHumanPlayer(IEnumerable<Card> cards) : base(cards)
		{
			Score = 0;
			BonusTiles = new StandardHand();
			SeatWind = SuitIdentifier.EAST_WIND;

			Melds = new List<MahjongMeld>();
			return;
		}

		/// <summary>
		/// Given a move encoded in m, will perform all the necessary work to make the player reflect the move occuring.
		/// </summary>
		/// <param name="m">The move to make.</param>
		/// <returns>Returns true if the move is valid and false otherwise.</returns>
		public override bool MakePlay(MahjongMove m)
		{
			

			return true;
		}

		/// <summary>
		/// Given a move encoded in m, will reverse any changes that were made to this class from performing the move.
		/// </summary>
		/// <param name="m">The move to undo.</param>
		public override void UndoMove(MahjongMove m)
		{throw new NotImplementedException();} // We don't need this here, so let's not bother

		/// <summary>
		/// Creates a deep copy of this player.
		/// </summary>
		/// <returns>Returns a deep copy of this player.</returns>
		public override Player<MahjongMove> Clone()
		{


			return null;
		}

		/// <summary>
		/// Copies the data in the given player into this one.
		/// </summary>
		/// <param name="mp">The player data to copy.</param>
		public void CopyData(MahjongPlayer mp)
		{
			Score = mp.Score;
			SeatWind = mp.SeatWind;
			
			while(BonusTiles.CardsInHand > 0)
				BonusTiles.PlayCard(0);
			
			Melds.Clear();

			foreach(MahjongMeld meld in mp.Melds)
				Melds.Add(meld.Clone());

			BonusTiles.DrawCards(mp.BonusTiles.Cards);
			return;
		}

		/// <summary>
		/// The revealed melds of the player.
		/// </summary>
		public List<MahjongMeld> Melds
		{get; protected set;}

		/// <summary>
		/// The player's current score.
		/// </summary>
		public int Score
		{get; set;}

		/// <summary>
		/// The bonus tiles currently in the player's possession.
		/// </summary>
		public Hand BonusTiles
		{get; protected set;}

		/// <summary>
		/// The current wind of the seat the player is at.
		/// </summary>
		public SuitIdentifier SeatWind
		{get; set;}
	}
}
