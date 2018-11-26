using Engine.Game;

namespace Engine.Player.AI
{
	/// <summary>
	/// Represents the basics of how an AI decides what to do.
	/// </summary>
	public interface AIBehavior<M>
	{
		/// <summary>
		/// Determines the next move for this AI to make.
		/// </summary>
		/// <param name="state">The state of the game. This will be cloned so that the AI does not affect the actual game state.</param>
		/// <returns>Returns the next move to make based on the current game state.</returns>
		M GetNextMove(GameState<M> state);

		/// <summary>
		/// Creates a deep copy of this AIBehavior.
		/// </summary>
		/// <returns>Returns a copy of this AIBehavior.</returns>
		AIBehavior<M> Clone();
	}
}
