using System;
using System.Collections.Generic;
using Engine.Cards;
using Engine.Player;

namespace Mahjong
{
	/// <summary>
	/// A human Mahjong player.
	/// </summary>
	public class MahjongHumanPlayer : StandardPlayer<MahjongMove>
	{
		/// <summary>
		/// Creates a new player for a Mahjong game.
		/// </summary>
		/// <param name="cards">The cards this player starts with.</param>
		public MahjongHumanPlayer(IEnumerable<Card> cards) : base(cards)
		{
			

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
	}
}
