using Engine.Player;
using Engine.Player.AI;

namespace Engine.Game
{
	/// <summary>
	/// The bare bones of the state of a game.
	/// </summary>
	public interface GameState<M>
	{
		/// <summary>
		/// Makes whatever changes are necessary to the game state by making the provided move.
		/// </summary>
		/// <param name="move">The move to make.</param>
		/// <returns>Returns true if the move is valid and false if it is not.</returns>
		bool ApplyMove(M move);

		/// <summary>
		/// Undoes the last move that occured to the game state.
		/// </summary>
		/// <returns>Returns true if the last move could be undone and false otherwise.</returns>
		bool UndoMove();

        /// <summary>
        /// Determines if the provided move is valid.
        /// </summary>
        /// <param name="move">The move to check.</param>
        /// <returns>Returns true if the move is valid and false otherwise.</returns>
        bool IsValid(M move);

        /// <summary>
		/// Gets the player with the specified index.
		/// </summary>
		/// <param name="index">The index to check. This value should be between zero and one less than the number of players.</param>
		/// <returns>Returns the player at the specified index.</returns>
		Player<M> GetPlayer(int index); // Something weird is forcing me to use Player.Player

		/// <summary>
		/// If a player leaves the game then they are replaced with an AI player.
		/// </summary>
		/// <param name="index">The index of the player that left.</param>
		/// <param name="replacement">The replacement AI. If null then a default behavior will be used.</param>
		/// <remarks>An AI player can leave the game to be replaced by a new AI player.</remarks>
		void PlayerLeft(int index, AIBehavior<M> replacement = null);

		/// <summary>
		/// Called to let a player join the game replacing an AI player.
		/// </summary>
		/// <param name="index">The index of the AI player to replace.</param>
		/// <returns>Returns true if the player joined the game and false otherwise.</returns>
		/// <remarks>Only AI players can be booted for a human player to join.</remarks>
		bool PlayerJoined(int index);

		/// <summary>
		/// Clones this game state. All events will be null in the returned copy.
		/// </summary>
		/// <returns>Returns a deep copy of this state.</returns>
		GameState<M> Clone();

		/// <summary>
		/// Serializes the game state.
		/// </summary>
		/// <returns>Returns the game state in string form.</returns>
		string Serialize();

		/// <summary>
		/// If true then play proceeds clockwise (or from low index to high index).
		/// If false then play proceeds counter-clockwise (or from high index to low index).
		/// </summary>
		bool Clockwise
		{get;}

		/// <summary>
		/// The index of the active player.
		/// </summary>
		int ActivePlayer
		{get;}

		/// <summary>
		/// The number of participating players in this game state.
		/// </summary>
		int NumberOfPlayers
		{get;}

		/// <summary>
		/// If true then the game is over.
		/// </summary>
		bool GameFinished
		{get;}

		/// <summary>
		/// Fired when this game state changes.
		/// </summary>
		event GameStateChanged<M> StateChanged;

		/// <summary>
		/// Fired when this game state reaches a finish state.
		/// </summary>
		event GameOver<M> Finished;
	}

	/// <summary>
	/// An event fired on game state changes.
	/// </summary>
	/// <param name="state">The state that changed.</param>
	/// <param name="m">The last move or null if the state change was not move related.</param>
	/// <param name="undo">If true then the move provided has been undone.</param>
	public delegate void GameStateChanged<M>(GameState<M> state, M m, bool undo = false);

	/// <summary>
	/// An event fired when a game state reaches a finish state.
	/// </summary>
	/// <param name="state">The game state in its final state.</param>
	public delegate void GameOver<M>(GameState<M> state);
}
