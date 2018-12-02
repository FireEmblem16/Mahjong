using System;
using System.Collections.Generic;
using Engine.Cards;
using Engine.Cards.CardTypes;
using Engine.Cards.DeckTypes;
using Engine.Cards.Hands;
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
			
			AvailableTile = null;

			// Set up misc data
			Round = 1;
			Hand = 1;
			BonusHand = 0;

			// Create the players
			ActivePlayer = 0;
			SubActivePlayer = 0;

			players = new Player<MahjongMove>[4];
			player_moves = new MahjongMove[4];
			
			MahjongPlayer[] mp = new MahjongPlayer[4];

			for(int i = 0;i < 4;i++)
			{
				players[i] = new MahjongHumanPlayer(new List<Card>(Deck.Draw(13)));
				player_moves[i] = null;

				mp[i] = players[i] as MahjongPlayer;
				mp[i].Score = 0;
				
				// If a player has bonus tiles, we need to replace them
				RemoveBonusTiles(players[i],mp[i]);
			}
			
			// Set the initial seat winds
			// Note that we don't need to randomise the seat winds; a user can just put the players in a different order if they want something different
			mp[0].SeatWind = SuitIdentifier.EAST_WIND;
			mp[1].SeatWind = SuitIdentifier.SOUTH_WIND;
			mp[2].SeatWind = SuitIdentifier.WEST_WIND;
			mp[3].SeatWind = SuitIdentifier.NORTH_WIND;

			// Give east their first draw, and make sure the draw wasn't a bonus tile
			DrawFromWall(players[0],mp[0]);

			return;
		}

		/// <summary>
		/// Removes all the bonus tiles from the player's hand AND replaces them.
		/// </summary>
		protected void RemoveBonusTiles(Player<MahjongMove> player, MahjongPlayer mp)
		{
			for(int i = 0;i < player.CardsInHand.CardsInHand;i++)
				if(player.CardsInHand.Cards[i].Suit.Color == SuitColor.BONUS)
				{
					mp.BonusTiles.DrawCard(player.CardsInHand.PlayCard(i));
					player.CardsInHand.DrawCard(Deck.Draw());
					i--;
				}

			return;
		}

		/// <summary>
		/// Draws a tile from the wall. Automatically replaces bonus tiles.
		/// </summary>
		protected bool DrawFromWall(Player<MahjongMove> player, MahjongPlayer mp)
		{
			if(Deck.CountDrawPile == 0)
				return false;

			Card c = Deck.Draw();

			while(c.Suit.Color == SuitColor.BONUS)
			{
				mp.BonusTiles.DrawCard(c);

				if(Deck.CountDrawPile == 0)
					return false; // It's okay to draw bonus tiles without ever getting a proper tile

				c = Deck.Draw();
			}

			player.CardsInHand.DrawCard(c);
			return true;
		}

		/// <summary>
		/// Makes whatever changes are necessary to the game state by making the provided move.
		/// </summary>
		/// <param name="move">The move to make.</param>
		/// <returns>Returns true if the move is valid and false if it is not.</returns>
		public bool ApplyMove(MahjongMove move)
		{
			// If the move is outright invalid, ignore it
			if(!IsValid(move))
				return false;

			// Now add the move to our collection
			player_moves[SubActivePlayer] = move;
			SubActivePlayer = NextSubActivePlayer;

			if(!AllMovesGathered())
				return true;

			// We have all four sub moves ready, we can figure out which one (or ones, since the active player usually is discarding) takes priority
			if(!player_moves[ActivePlayer].Discard)
			{
				// The player could be going out on their own draw here (and has THE highest priority possible) or melding
				// The meld can only be affected by other players if it's a kong (although why the active player would want to meld otherwise is beyond me)
				// If a meld occurs, then the active player will still need to discard, but a new tile will need to be drawn if the meld is a kong

			}

			// 
			

			return true;
		}

		/// <summary>
		/// Handles all the logic necessary for a declaration of mahjong, including scoring and setting up the next round, and, if necessary, delcaring the game finished.
		/// </summary>
		/// <param name="player_index">The player declaring mahjong.</param>
		protected void Mahjong(int player_index)
		{


			return;
		}

		/// <summary>
		/// Hands all the logic necessary for a hand in which no one wins.
		/// </summary>
		protected void Goulash()
		{


			return;
		}

		protected bool AllMovesGathered()
		{
			for(int i = 0;i < 4;i++)
				if(player_moves[i] == null)
					return false;

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
			// First check if the move obeys basic game mechancis
			// The below aren't exhaustive, probably, but they do cover most scenarios

			// The active player cannot pass
			if(SubActivePlayer == ActivePlayer && move.Pass)
				return false;

			// Only the active player may discard, and if a meld is being made from the discard pile, it better use that tile
			if(SubActivePlayer != ActivePlayer && (move.Discard || !move.Pass && !move.DiscardedTile.Equals(AvailableTile)))
				return false;

			// Only the player after the active player can chow unless it's to go out
			if(move.Chow && SubActivePlayer != NextPlayer && !move.Mahjong)
				return false;

			// Eyes and special hands can only claim a discarded tile to go out
			if(SubActivePlayer != ActivePlayer && move.Meld && !(move.Chow || move.Pung || move.Kong) && !move.Mahjong)
				return false;

			// Next check if the active player can actually perform the mvoe
			Player<MahjongMove> temp = GetPlayer(SubActivePlayer).Clone();
			return temp.MakePlay(move);
		}

        /// <summary>
		/// Gets the player with the specified index between 0 and 3 inclusive.
		/// Note that the player returned will also be of type MahjongPlayer.
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

			MahjongPlayer old = players[index] as MahjongPlayer;

			players[index] = new MahjongAIPlayer(players[index].CardsInHand.Cards,replacement);
			(players[index] as MahjongPlayer).CopyData(old);

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

			MahjongPlayer old = players[index] as MahjongPlayer;

			players[index] = new MahjongHumanPlayer(players[index].CardsInHand.Cards);
			(players[index] as MahjongPlayer).CopyData(old);
			
			return true;
		}

		/// <summary>
		/// Clones this game state. All events will be null in the returned copy.
		/// </summary>
		/// <returns>Returns a deep copy of this state.</returns>
		public GameState<MahjongMove> Clone()
		{
			MahjongGameState ret = new MahjongGameState();

			ret.ActivePlayer = ActivePlayer;
			ret.Deck = Deck.Clone();

			ret.Round = Round;
			ret.Hand = Hand;
			ret.BonusHand = BonusHand;

			for(int i = 0;i < 4;i++)
			{
				ret.players[i] = players[i].Clone();
				ret.player_moves[i] = player_moves[i].Clone();
			}

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
		/// <remarks>Without a visual representation of the game, this means nothing.</remarks>
		public bool Clockwise
		{
			get
			{return false;}
		}

		/// <summary>
		/// The index of the active player. This is the player who has drawn a tile and now needs to discard, go out, or declare a concealed kong.
		/// </summary>
		public int ActivePlayer
		{get; protected set;}
		
		/// <summary>
		/// The index of the player immediately after the active player (assuming a play order isn't interrupted).
		/// </summary>
		public int NextPlayer
		{
			get
			{
				if(ActivePlayer == 3)
					return 0;
				
				return ActivePlayer++;
			}
		}

		/// <summary>
		/// The index of the player's whose move is being waited upon.
		/// </summary>
		public int SubActivePlayer
		{get; protected set;}

		/// <summary>
		/// The index of the player immediately after the subactive player.
		/// </summary>
		public int NextSubActivePlayer
		{
			get
			{
				if(SubActivePlayer == 3)
					return 0;
				
				return SubActivePlayer++;
			}
		}

		/// <summary>
		/// The number of participating players in this game state.
		/// </summary>
		public int NumberOfPlayers
		{
			get
			{return 4;}
		}

		/// <summary>
		/// If true, then the last hand is over and a new hand has started. At all other times, this is false.
		/// </summary>
		public bool HandFinished
		{get; protected set;}
		
		/// <summary>
		/// If true then the game is over.
		/// </summary>
		public bool GameFinished
		{get; protected set;}

		/// <summary>
		/// The set of tiles.
		/// </summary>
		public Deck Deck
		{get; protected set;}

		/// <summary>
		/// The tile that is currently being discarded or is available to be robbed from a kong
		/// </summary>
		public Card AvailableTile
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
		/// Note that anything that might be in here will be a MahjongPlayer as well.
		/// </summary>
		protected Player<MahjongMove>[] players;

		/// <summary>
		/// The current moves of the four players for the current trick/tile.
		/// </summary>
		protected MahjongMove[] player_moves;
	}
}
