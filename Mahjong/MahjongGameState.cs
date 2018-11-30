using System;
using Engine.Game;
using Engine.Player;
using Engine.Player.AI;

namespace Mahjong
{
	/// <summary>
	/// The game state for a Mahjong game.
	/// </summary>
	public class MahjongGameState : GameState<MahjongMove>
	{
		/// <summary>
		/// Creates a new game state.
		/// </summary>
		public MahjongGameState()
		{


			return;
		}
		
		/// <summary>
		/// Makes whatever changes are necessary to the game state by making the provided move.
		/// </summary>
		/// <param name="move">The move to make.</param>
		/// <returns>Returns true if the move is valid and false if it is not.</returns>
		public bool ApplyMove(MahjongMove move)
		{


			return true;
		}

		/// <summary>
		/// Undoes the last move that occured to the game state.
		/// </summary>
		/// <returns>Returns true if the last move could be undone and false otherwise.</returns>
		public bool UndoMove()
		{


			return true;
		}

        /// <summary>
        /// Determines if the provided move is valid.
        /// </summary>
        /// <param name="move">The move to check.</param>
        /// <returns>Returns true if the move is valid and false otherwise.</returns>
        public bool IsValid(MahjongMove move)
		{


			return true;
		}

        /// <summary>
		/// Gets the player with the specified index.
		/// </summary>
		/// <param name="index">The index to check. This value should be between zero and one less than the number of players.</param>
		/// <returns>Returns the player at the specified index.</returns>
		public Player<MahjongMove> GetPlayer(int index)
		{


			return null;
		}

		/// <summary>
		/// If a player leaves the game then they are replaced with an AI player.
		/// </summary>
		/// <param name="index">The index of the player that left.</param>
		/// <param name="replacement">The replacement AI. If null then a default behavior will be used.</param>
		/// <remarks>An AI player can leave the game to be replaced by a new AI player.</remarks>
		public void PlayerLeft(int index, AIBehavior<MahjongMove> replacement = null)
		{


			return;
		}

		/// <summary>
		/// Called to let a player join the game replacing an AI player.
		/// </summary>
		/// <param name="index">The index of the AI player to replace.</param>
		/// <returns>Returns true if the player joined the game and false otherwise.</returns>
		/// <remarks>Only AI players can be booted for a human player to join.</remarks>
		public bool PlayerJoined(int index)
		{


			return true;
		}

		/// <summary>
		/// Clones this game state. All events will be null in the returned copy.
		/// </summary>
		/// <returns>Returns a deep copy of this state.</returns>
		public GameState<MahjongMove> Clone()
		{


			return null;
		}

		/// <summary>
		/// Serializes the game state.
		/// </summary>
		/// <returns>Returns the game state in string form.</returns>
		public string Serialize()
		{return "";} // This isn't a useful notion for the implementation of this game as we don't have networking

		/// <summary>
		/// If true then play proceeds clockwise (or from low index to high index).
		/// If false then play proceeds counter-clockwise (or from high index to low index).
		/// </summary>
		public bool Clockwise
		{
			get
			{return false;}
		}

		/// <summary>
		/// The index of the active player.
		/// </summary>
		public int ActivePlayer
		{get; protected set;}

		/// <summary>
		/// The number of participating players in this game state.
		/// </summary>
		public int NumberOfPlayers
		{
			get
			{return 4;}
		}

		/// <summary>
		/// If true then the game is over.
		/// </summary>
		public bool GameFinished
		{
			get
			{


				return false;
			}
		}

		/// <summary>
		/// Fired when this game state changes.
		/// </summary>
		public event GameStateChanged<MahjongMove> StateChanged;

		/// <summary>
		/// Fired when this game state reaches a finish state.
		/// </summary>
		public event GameOver<MahjongMove> Finished;
	}
}
