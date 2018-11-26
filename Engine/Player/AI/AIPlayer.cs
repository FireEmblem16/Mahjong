using Engine.Game;

namespace Engine.Player.AI
{
	/// <summary>
	/// Represents what it means to be an AI player.
	/// </summary>
	public interface AIPlayer<M> : Player<M>
	{
		/// <summary>
		/// This AI player will take its turn based on the provided game state and return the move it wishes to make.
		/// </summary>
		/// <param name="state">The current state of the game.</param>
		M TakeTurn(GameState<M> state);

		/// <summary>
		/// The way this AI player will actually make decisions on what to do.
		/// </summary>
		AIBehavior<M> AI
		{get; set;}
	}
}
