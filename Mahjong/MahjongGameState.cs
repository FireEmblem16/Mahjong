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
			
			// State variables
			AvailableTile = null;
			OnReplacement = 0;
			
			Heavenly = true;
			Earthly = true;
			
			// Set up hand data
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
			
			// Set the initial seat winds (prevailing winds default to east as desired)
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
		/// Initialises a hand but otherwise leaves all data untouched.
		/// Assumes that the hand and bonus hand values are already appropriately assigned.
		/// </summary>
		protected void InitHand()
		{
			// State variables
			AvailableTile = null;
			OnReplacement = 0;
			
			Heavenly = true;
			Earthly = true;

			HandFinished = true;

			// Resets
			ResetMoves();
			Deck.Reset();

			// Deal each player a new hand
			MahjongPlayer[] mp = new MahjongPlayer[4];

			for(int i = 0;i < 4;i++)
			{
				mp[i] = players[i] as MahjongPlayer;

				// Discard old cards
				while(players[i].CardsInHand.CardsInHand > 0)
					players[i].CardsInHand.PlayCard(0);

				while(mp[i].BonusTiles.CardsInHand > 0)
					mp[i].BonusTiles.PlayCard(0);

				mp[i].Melds.Clear();

				// Draw the initial thireen tiles
				players[i].CardsInHand.DrawCards(Deck.Draw(13));
				
				// If a player has bonus tiles, we need to replace them
				RemoveBonusTiles(players[i],mp[i]);

				// Set the new seat winds and prevailing wind
				mp[i].SeatWind = PlayerToWind(i); // We do have that rotate wind function, but we don't know if this is a bonus hand or not, so let's not chance it
				mp[i].PrevailingWind = PrevailingWind;
			}
			
			// Set the active player to be east
			for(int i = 0;i < 4;i++)
				if(mp[i].SeatWind == SuitIdentifier.EAST_WIND)
				{
					ActivePlayer = i;
					SubActivePlayer = i;

					break;
				}

			// Give east their first draw, and make sure the draw wasn't a bonus tile
			DrawFromWall(players[ActivePlayer],mp[ActivePlayer]);

			return;
		}
		
		protected SuitIdentifier PlayerToWind(int player)
		{
			switch(player)
			{
			case 0:
				switch(Hand % 4)
				{
				case 1:
					return SuitIdentifier.EAST_WIND;
				case 2:
					return SuitIdentifier.NORTH_WIND;
				case 3:
					return SuitIdentifier.WEST_WIND;
				case 0:
					return SuitIdentifier.SOUTH_WIND;
				}

				break;
			case 1:
				switch(Hand % 4)
				{
				case 2:
					return SuitIdentifier.EAST_WIND;
				case 3:
					return SuitIdentifier.NORTH_WIND;
				case 0:
					return SuitIdentifier.WEST_WIND;
				case 1:
					return SuitIdentifier.SOUTH_WIND;
				}

				break;
			case 2:
				switch(Hand % 4)
				{
				case 3:
					return SuitIdentifier.EAST_WIND;
				case 0:
					return SuitIdentifier.NORTH_WIND;
				case 1:
					return SuitIdentifier.WEST_WIND;
				case 2:
					return SuitIdentifier.SOUTH_WIND;
				}

				break;
			case 3:
				switch(Hand % 4)
				{
				case 0:
					return SuitIdentifier.EAST_WIND;
				case 1:
					return SuitIdentifier.NORTH_WIND;
				case 2:
					return SuitIdentifier.WEST_WIND;
				case 3:
					return SuitIdentifier.SOUTH_WIND;
				}

				break;
			}

			return SuitIdentifier.SEASON; // Something has gone horribly wrong
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
		protected bool DrawFromWall(Player<MahjongMove> player, MahjongPlayer mp, bool from_kong = false)
		{
			if(!from_kong)
				OnReplacement = 0;

			if(Deck.CountDrawPile == 0)
				return false;

			Card c = Deck.Draw();

			while(c.Suit.Color == SuitColor.BONUS)
			{
				mp.BonusTiles.DrawCard(c);
				OnReplacement++;

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

			// This is false as soon as literally anything happens
			HandFinished = false;

			// Now add the move to our collection
			player_moves[SubActivePlayer] = move;
			SubActivePlayer = NextSubActivePlayer;

			if(!AllMovesGathered())
				return true;

			// Check the heavenly hand state
			// This will trigger immediately after the first trick unless east wins immediately
			if(Heavenly && !player_moves[ActivePlayer].Mahjong)
				Heavenly = false;

			// Check the earthly hand state
			// To get this, you must win on east's first discard (or rob the kong before a discard, but earthly hands are limit hands, so who cares)
			if(Earthly && player_moves[ActivePlayer].Discard && !(player_moves[GetNextPlayer(ActivePlayer,1)].Mahjong || player_moves[GetNextPlayer(ActivePlayer,2)].Mahjong || player_moves[GetNextPlayer(ActivePlayer,3)].Mahjong))
				Earthly = false;

			// For convenience
			MahjongMove active_move = player_moves[ActivePlayer];

			// We have all four sub moves ready, we can figure out which one (or ones, since the active player usually is discarding) takes priority
			if(!active_move.Discard)
			{
				// The player could be going out on their own draw here (and has THE highest priority possible) or melding
				// The meld can only be affected by other players if it's a kong (although why the active player would want to meld otherwise is beyond me)
				// If a meld occurs, then the active player will still need to discard, but a new tile will need to be drawn if the meld is a kong
				if(active_move.Meld)
				{
					// Check for robbing the kong first
					if(active_move.Kong && !active_move.Mahjong && (player_moves[GetNextPlayer(ActivePlayer,1)].Mahjong && player_moves[GetNextPlayer(ActivePlayer,1)].Meld || player_moves[GetNextPlayer(ActivePlayer,2)].Mahjong && player_moves[GetNextPlayer(ActivePlayer,2)].Meld || player_moves[GetNextPlayer(ActivePlayer,3)].Mahjong && player_moves[GetNextPlayer(ActivePlayer,3)].Meld))
					{
						// The kong is robbed, so the active player loses their tile
						GetPlayer(ActivePlayer).CardsInHand.PlayCard(active_move.DiscardedTile);

						// We don't really need to do this, as we won't apply the move, but it does make life easier
						active_move.Mahjong = false;

						// Now handle mahjong logic for whoever stole from the kong
						// And we ARE stealing from the kong no matter what, so in the bizzare situation when 2+ players are trying, we can just pick whoever is first
						for(int i = 1;i < 4;i++)
							if(player_moves[GetNextPlayer(ActivePlayer,i)].Mahjong)
							{
								// Apply the move
								GetPlayer(GetNextPlayer(ActivePlayer,i)).MakePlay(player_moves[GetNextPlayer(ActivePlayer,i)]);

								// End this hand
								Mahjong(GetNextPlayer(ActivePlayer,i),true);
								return true;
							}

						// If we get here, something has gone horribly wrong
						return false;
					}

					// If a kong isn't being robbed, we can just let the meld go forward
					GetPlayer(ActivePlayer).MakePlay(active_move);

					// If the active move was a kong, we need a replacement tile
					if(!DrawFromWall(GetPlayer(ActivePlayer),GetPlayer(ActivePlayer) as MahjongPlayer,true))
					{
						Goulash(); // We ran out of tiles, so no one wins
						return true;
					}
				}
				
				// This should be mutually exclusive with melding for the active player, but we'll put it as only an if statement anyway just in case
				if(active_move.Mahjong)
				{
					Mahjong(ActivePlayer);
					return true;
				}

				// The active player did not discard, so no one else can do anything (robbing the kong was already taken care of)
				// If mahjong is declared, then we've already returned
				// If mahjong is not declared, then we must have performed some action (melding a kong) that doesn't advance the turn order, so the active player is unchanged
				// Note that the sub active player has already been advanced and so must once again be the active player already
				ResetMoves();
				return true;
			}
			else if(!GetPlayer(ActivePlayer).MakePlay(active_move)) // Perform the actual discard
				return false; // Something has gone horribly wrong

			// The active player has discarded, so there are two cases
			// First, no one takes the tile and play continues uninterrupted
			// Second, someone melds the discarded tile, and we have to determine who has priority

			// First, let's handle the simple case where everyone passes
			if(player_moves[GetNextPlayer(ActivePlayer,1)].Pass && player_moves[GetNextPlayer(ActivePlayer,2)].Pass && player_moves[GetNextPlayer(ActivePlayer,3)].Pass)
			{
				ActivePlayer = NextPlayer;
				SubActivePlayer = ActivePlayer;

				// Add the tile to the discard pile (we can only get here if the active player discards and no one takes it, and this is the only place where no one takes the tile or goes out)
				Deck.Discard(player_moves[ActivePlayer].DiscardedTile);

				// The active player needs to draw a tile
				if(!DrawFromWall(GetPlayer(ActivePlayer),GetPlayer(ActivePlayer) as MahjongPlayer,true))
				{
					Goulash(); // We ran out of tiles, so no one wins
					return true;
				}

				ResetMoves();
				return true;
			}
			
			// Next, let's handle the case where someone wants to declare mahjong
			// Priority goes to whoever comes first
			for(int i = 1;i < 4;i++)
				if(player_moves[GetNextPlayer(ActivePlayer,i)].Mahjong)
				{
					// Apply the move
					if(!GetPlayer(GetNextPlayer(ActivePlayer,i)).MakePlay(player_moves[GetNextPlayer(ActivePlayer,i)]))
						return false; // Something has gone horribly wrong

					// End this hand
					Mahjong(GetNextPlayer(ActivePlayer,i),true);
					return true;
				}

			// The next highest priority goes to pungs/kongs
			// Subpriority goes to whoever comes first
			for(int i = 1;i < 4;i++)
				if(player_moves[GetNextPlayer(ActivePlayer,i)].Pung || player_moves[GetNextPlayer(ActivePlayer,i)].Kong)
				{
					// Apply the move
					if(!GetPlayer(GetNextPlayer(ActivePlayer,i)).MakePlay(player_moves[GetNextPlayer(ActivePlayer,i)]))
						return false; // Something has gone horribly wrong

					// Update the state
					ActivePlayer = GetNextPlayer(ActivePlayer,i);
					SubActivePlayer = ActivePlayer;
					
					// No one else gets to do anything, so press on
					ResetMoves();
					return true;
				}

			// Lastly, we must have someone trying to chow, and there's only one player who can
			// If this isn't the case, then something has gone horribly wrong and we flee immediately
			if(!player_moves[NextPlayer].Chow)
				return false;

			if(!GetPlayer(NextPlayer).MakePlay(player_moves[NextPlayer]))
				return false; // Something has gone horribly wrong

			ActivePlayer = NextPlayer;
			SubActivePlayer = ActivePlayer;

			// No one else has done anything, so press on
			ResetMoves();
			return true;
		}
		
		/// <summary>
		/// Handles all the logic necessary for a declaration of mahjong, including scoring and setting up the next round, and, if necessary, delcaring the game finished.
		/// </summary>
		protected void Mahjong(int player_index, bool stolen_from_kong = false)
		{
			MahjongPlayer[] mp = new MahjongPlayer[4];

			for(int i = 0;i < 4;i++)
				mp[i] = players[i] as MahjongPlayer;

			// Score the hand
			int fan = MahjongStaticFunctions.HandFan(mp[player_index].Melds,mp[player_index].SeatWind,PrevailingWind,player_moves[player_index].SelfPick,Deck.CountDrawPile == 0,OnReplacement > 0,OnReplacement > 1,stolen_from_kong,Heavenly,Earthly);
			int base_points = MahjongStaticFunctions.FanToBasePoints(fan);

			// Payout the score
			for(int i = 0;i < 4;i++)
				if(i != player_index)
				{
					int factor = payment_factor(player_index,i);

					mp[i].Score -= factor * base_points;
					mp[player_index].Score += factor * base_points;
				}
			
			// Go to the next hand
			if(mp[player_index].SeatWind == SuitIdentifier.EAST_WIND)
			{
				// East won, so we have a bonus hand
				BonusHand++;

				if(BonusHand >= MaxBonusHand)
				{
					BonusHand = 0;
					Hand++;
				}
			}
			else
			{
				// East didn't win, so no bonus hand
				BonusHand = 0;
				Hand++;
			}
			
			if(!GameFinished)
				InitHand();

			return;
		}

		/// <summary>
		/// Determines what multiplier is on the payment made to the winner by the loser.
		/// </summary>
		protected int payment_factor(int winner, int loser)
		{
			int ret = 1;

			// Self picks are worth double the points from everyone
			if(winner == ActivePlayer)
				ret *= 2;

			// Whoever discarded the winning tile pays double
			if(loser == ActivePlayer)
				ret *= 2;

			// Everyone pays east double
			if((GetPlayer(winner) as MahjongPlayer).SeatWind == SuitIdentifier.EAST_WIND)
				ret *= 2;

			// East pays double
			if((GetPlayer(loser) as MahjongPlayer).SeatWind == SuitIdentifier.EAST_WIND)
				ret *= 2;

			return ret;
		}

		/// <summary>
		/// Hands all the logic necessary for a hand in which no one wins.
		/// </summary>
		protected void Goulash()
		{
			// Everyone is a failure, so reset everything and increment the bonus hand
			BonusHand++;

			// Check if there have been too many bonus hands
			if(BonusHand >= MaxBonusHand)
			{
				BonusHand = 0;
				Hand++;
			}

			if(!GameFinished)
				InitHand();

			return;
		}
		
		/// <summary>
		/// Returns the next player in a circle after delta turns.
		/// </summary>
		protected int GetNextPlayer(int index, int delta = 1)
		{return (index + delta) % 4;}

		/// <summary>
		/// Determines if all 4 moves are in.
		/// </summary>
		protected bool AllMovesGathered()
		{
			for(int i = 0;i < 4;i++)
				if(player_moves[i] == null)
					return false;

			return true;
		}

		/// <summary>
		/// Nulls all player moves.
		/// </summary>
		protected void ResetMoves()
		{
			for(int i = 0;i < 4;i++)
				player_moves[i] = null;

			return;
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
			// The below might not be comprehensive, which is a problem, but it definitely covers most scenarios

			// Obviously, if the game is done, no moves are valid
			if(GameFinished)
				return false;

			// The active player cannot pass or randomly meld
			if(SubActivePlayer == ActivePlayer && (move.Pass || move.Meld && !move.Kong))
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
			
			ret.Hand = Hand;
			ret.BonusHand = BonusHand;

			ret.HandFinished = HandFinished;
			ret.AvailableTile = AvailableTile.Clone();

			ret.OnReplacement = OnReplacement;
			ret.Heavenly = Heavenly;
			ret.Earthly = Earthly;

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
		{
			get
			{return Hand > 16;}
		}

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
		{
			get
			{
				if(Hand < 5)
					return 1;

				if(Hand < 9)
					return 2;

				if(Hand < 13)
					return 3;

				return 4;
			}
		}

		/// <summary>
		/// The current hand number (not counting extra hands, there are 4 per round).
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
		/// The maximum number of sequential bonus hands.
		/// </summary>
		public uint MaxBonusHand
		{
			get
			{return 16;}
		}

		/// <summary>
		/// The number of replacement tiles drawn before an actual tile is available to consider.
		/// </summary>
		/// <remarks>This value is automatically managed in DrawFromWall and should never be touched elsewhere except to read.</remarks>
		public int OnReplacement
		{get; protected set;}

		/// <summary>
		/// If true, then there is currently the possibility for a heavenly hand.
		/// </summary>
		protected bool Heavenly
		{get; set;}

		/// <summary>
		/// If true, then there is currently the possibility for an earthly hand.
		/// </summary>
		protected bool Earthly
		{get; set;}

		/// <summary>
		/// The players in the game.
		/// Note that anything that might be in here will be a MahjongPlayer as well.
		/// </summary>
		protected Player<MahjongMove>[] players;

		/// <summary>
		/// The current moves of the four players for the current trick/tile.
		/// DO NOT EDIT THIS OUTSIDE THE CLASS. READONLY!!!!!!!
		/// </summary>
		public MahjongMove[] player_moves;
	}
}
