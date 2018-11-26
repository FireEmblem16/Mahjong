using System.Collections.Generic;
using Engine.Cards;
using Engine.Game;

namespace Engine.Player.AI
{
	/// <summary>
	/// A standard AI for a game.
	/// </summary>
	public abstract class StandardAI<M> : StandardPlayer<M>, AIPlayer<M>
	{
		/// <summary>
		/// Creates a new AI for a game.
		/// </summary>
		/// <param name="cards">The cards that this player starts with.</param>
		/// <param name="behavior">The way the AI makes decisions.</param>
		public StandardAI(IEnumerable<Card> cards, AIBehavior<M> behavior) : base(cards)
		{
			AI = behavior.Clone();
			return;
		}

		/// <summary>
		/// This AI player will take its turn based on the provided game state and return the move it wishes to make.
		/// </summary>
		/// <param name="state">The current state of the game.</param>
		public M TakeTurn(GameState<M> state)
		{return AI.GetNextMove(state);}

		/// <summary>
		/// The way this AI player will actually make decisions on what to do.
		/// </summary>
		public AIBehavior<M> AI
		{get; set;}

		/// <summary>
		/// If true then this player is an AI player.
		/// </summary>
		public override bool IsAI
		{
			get
			{return true;}
		}
	}
}
