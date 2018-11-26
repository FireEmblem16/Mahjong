using System.Collections.Generic;
using Engine.Cards;
using Engine.Cards.Hands;

namespace Engine.Player
{
	/// <summary>
	/// A standard human player for a game.
	/// </summary>
	public abstract class StandardPlayer<M> : Player<M>
	{
		/// <summary>
		/// Creates a new player for a game.
		/// </summary>
		/// <param name="cards">The cards this player starts with.</param>
		public StandardPlayer(IEnumerable<Card> cards)
		{
			CardsInHand = new StandardHand(cards);
			return;
		}

		/// <summary>
		/// Given a move encoded in m, will perform all the necessary work to make the player reflect the move occuring.
		/// </summary>
		/// <param name="m">The move to make.</param>
		/// <returns>Returns true if the move is valid and false otherwise.</returns>
		public abstract bool MakePlay(M m);

		/// <summary>
		/// Given a move encoded in m, will reverse any changes that were made to this class from performing the move.
		/// </summary>
		/// <param name="m">The move to undo.</param>
		public abstract void UndoMove(M m);

		/// <summary>
		/// Creates a deep copy of this player.
		/// </summary>
		/// <returns>Returns a deep copy of this player.</returns>
		public abstract Player<M> Clone();

		/// <summary>
		/// This player's hand.
		/// </summary>
		public Hand CardsInHand
		{get; protected set;}

		/// <summary>
		/// If true then this player is an AI player.
		/// </summary>
		public virtual bool IsAI
		{
			get
			{return false;}
		}
	}
}
