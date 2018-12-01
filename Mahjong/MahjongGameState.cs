using System;
using System.Collections.Generic;
using Engine.Cards;
using Engine.Cards.CardTypes;
using Engine.Cards.DeckTypes;
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
		/// All players default to human until replaced by an AI.
		/// </summary>
		public MahjongGameState()
		{
			// Create the deck
			Deck = new MahjongDeck();

			// Set up misc data
			Round = 1;
			Hand = 1;
			BonusHand = 0;

			// Create the players
			players = new Player<MahjongMove>[4];
			player_moves = new MahjongMove[4];

			for(int i = 0;i < 4;i++)
				players[i] = new MahjongHumanPlayer(new List<Card>(Deck.Draw(13)));

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
		{throw new NotImplementedException();} // We don't need this here, so let's not bother

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
			if(index < 0 || index > 4)
				return null;

			return players[index];
		}

		/// <summary>
		/// If a player leaves the game then they are replaced with an AI player.
		/// </summary>
		/// <param name="index">The index of the player that left.</param>
		/// <param name="replacement">The replacement AI. If null then a default behavior will be used.</param>
		/// <remarks>An AI player can leave the game to be replaced by a new AI player.</remarks>
		public void PlayerLeft(int index, AIBehavior<MahjongMove> replacement = null)
		{
			if(index < 0 || index > 4)
				return;

			players[index] = new MahjongAIPlayer(players[index].CardsInHand.Cards,replacement);
			return;
		}

		/// <summary>
		/// Called to let a player join the game replacing an AI player.
		/// </summary>
		/// <param name="index">The index of the AI player to replace.</param>
		/// <returns>Returns true if the player joined the game and false otherwise.</returns>
		/// <remarks>Only AI players can be booted for a human player to join. If you want to replace the AI behaviour indirectly, use PlayerLeft instead.</remarks>
		public bool PlayerJoined(int index)
		{
			if(index < 0 || index > 4 || !players[index].IsAI)
				return false;

			players[index] = new MahjongHumanPlayer(players[index].CardsInHand.Cards);
			return true;
		}

		/// <summary>
		/// Clones this game state. All events will be null in the returned copy.
		/// </summary>
		/// <returns>Returns a deep copy of this state.</returns>
		public GameState<MahjongMove> Clone()
		{
			MahjongGameState ret = new MahjongGameState();



			return ret;
		}

		/// <summary>
		/// Serializes the game state.
		/// </summary>
		/// <returns>Returns the game state in string form.</returns>
		public string Serialize()
		{throw new NotImplementedException();} // This isn't a useful notion for the implementation of this game as we don't have networking

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
		/// The set of tiles.
		/// </summary>
		public Deck Deck
		{get; protected set;}

		/// <summary>
		/// The current prevailing wind.
		/// </summary>
		public SuitIdentifier PrevailingWind
		{
			get
			{
				switch(Round)
				{
				case 1:
					return SuitIdentifier.EAST_WIND;
				case 2:
					return SuitIdentifier.SOUTH_WIND;
				case 3:
					return SuitIdentifier.WEST_WIND;
				case 4:
					return SuitIdentifier.NORTH_WIND;
				}

				return SuitIdentifier.EAST_WIND; // Should never happen
			}
		}
		
		/// <summary>
		/// The current round number.
		/// </summary>
		public uint Round
		{get; protected set;}

		/// <summary>
		/// The current hand number (without extra hands, there are 4 per round).
		/// </summary>
		public uint Hand
		{get; protected set;}
		
		/// <summary>
		/// The current bonus hand of the current East.
		/// This value is 0 if gameplay is not currently in a bonus hand.
		/// </summary>
		public uint BonusHand
		{get; protected set;}

		/// <summary>
		/// The players in the game.
		/// </summary>
		protected Player<MahjongMove>[] players;

		/// <summary>
		/// The current moves of the four players for the current trick/tile.
		/// </summary>
		protected MahjongMove[] player_moves;
	}
}
