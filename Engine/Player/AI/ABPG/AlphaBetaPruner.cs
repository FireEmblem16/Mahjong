using System;
using System.Collections.Generic;
using Engine.Game;

namespace Engine.Player.AI.ABPG
{
	/// <summary>
	/// The basics of an alpha-beta pruning algorithm.
	/// Everything is implemented except the evaluation algorithm and move iteration.
	/// </summary>
	public abstract class AlphaBetaPruner<M> : AIBehavior<M>
	{
		/// <summary>
		/// Determines the next move for this AI to make.
		/// </summary>
		/// <param name="state">The state of the game. This will be cloned so that the AI does not affect the actual game state.</param>
		/// <returns>Returns the next move to make based on the current game state.</returns>
		public M GetNextMove(GameState<M> state)
		{
			AlphaBetaPruneEntry(state.Clone(),state.ActivePlayer,SearchDepth);
			return SelectedMove;
		}

		/// <summary>
		/// The entry point for the alpha beta pruning algorithm.
		/// The selected move will be stored in SelectedMove.
		/// </summary>
		/// <param name="state">The state to evaluate.</param>
		/// <param name="active_player">The active player. This is the index of the player in the game state.</param>
		/// <param name="depth">The depth down we are. The original value for this should be the number of moves that can be explored.</param>
		/// <param name="alpha">The alpha value. First call should be the lowest value possible.</param>
		/// <param name="beta">The beta value. First call should be the highest value possible.</param>
		/// <returns>Returns a pruned score for the evaluated branch of moves.</returns>
		protected uint AlphaBetaPruneEntry(GameState<M> state, int active_player, uint depth, uint alpha = uint.MinValue, uint beta = uint.MaxValue)
		{
			// If we should go no further down, evaluate and return
			if(depth == 0)
				return Evaluate(state,active_player);

			IEnumerator<Tuple<GameState<M>,M>> next_moves = GetMoveEnumerator(state);

			//////////////////////////////////////////////////////////////////////////////////////////////
			//////////////// Some magic is happening by having these next four statements ////////////////
			//////////// If we take them out we start selecting moves that we can't even play ////////////
			////////// There's very little overhead from this magic so we'll just leave it here //////////
			List<Tuple<GameState<M>,M>> valids = new List<Tuple<GameState<M>,M>>();

			while(next_moves.MoveNext())
				valids.Add(next_moves.Current);

			next_moves = GetMoveEnumerator(state);
			///////////////////////////////////////// End Magic //////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////

			// If this is a terminal node we should just evaluate it and return
			if(!next_moves.MoveNext())
				return Evaluate(state,active_player);

			while(true)
			{
				Tuple<GameState<M>,M> play = next_moves.Current;
				uint move_val = AlphaBetaPrune(play.Item1,active_player,depth - 1,alpha,beta);

				if(alpha < move_val) // Must be < or else the last move is always picked
				{
					alpha = move_val;
					SelectedMove = play.Item2;
				} // Beta is always infinity here so no pruning can happen yet

				if(!next_moves.MoveNext())
					break;
			}

			return alpha;
		}

		/// <summary>
		/// Performs the actual alpha beta pruning algorithm.
		/// </summary>
		/// <param name="state">The state to evaluate.</param>
		/// <param name="active_player">The active player. This is the index of the player in the game state.</param>
		/// <param name="depth">The depth down we are. The original value for this should be the number of moves that can be explored.</param>
		/// <param name="alpha">The alpha value. First call should be the lowest value possible.</param>
		/// <param name="beta">The beta value. First call should be the highest value possible.</param>
		/// <returns>Returns a pruned score for the evaluated branch of moves.</returns>
		protected uint AlphaBetaPrune(GameState<M> state, int active_player, uint depth, uint alpha = uint.MinValue, uint beta = uint.MaxValue)
		{
			// If we should go no further down, evaluate and return
			if(depth == 0)
				return Evaluate(state,active_player);

			IEnumerator<Tuple<GameState<M>,M>> next_moves = GetMoveEnumerator(state);

			// If this is a terminal node we should just evaluate it and return
			if(!next_moves.MoveNext())
				return Evaluate(state,active_player);

			if(active_player == state.ActivePlayer) // maximize
			{
				while(true)
				{
					Tuple<GameState<M>,M> play = next_moves.Current;
					alpha = Math.Max(alpha,AlphaBetaPrune(play.Item1,active_player,depth - 1,alpha,beta));

					if(beta <= alpha || !next_moves.MoveNext())
						break;
				}

				return alpha;
			}
			
			// minimize
			while(true)
			{
				Tuple<GameState<M>,M> play = next_moves.Current;
				beta = Math.Min(beta,AlphaBetaPrune(play.Item1,active_player,depth - 1,alpha,beta));

				if(beta <= alpha || !next_moves.MoveNext())
					break;
			}

			return beta;
		}

		/// <summary>
		/// Obtains an enumerator for all possible moves on the current state.
		/// </summary>
		/// <param name="state">The state to enumerate moves for.</param>
		/// <returns>Returns an enumerator for all possible moves.</returns>
		/// <remarks>If better moves are returned first the pruning will be quicker.</remarks>
		protected abstract IEnumerator<Tuple<GameState<M>,M>> GetMoveEnumerator(GameState<M> state);

		/// <summary>
		/// Assigns a score to the given game state.
		/// Lower values states are worse states than higher value states.
		/// </summary>
		/// <param name="state">The state to evaluate.</param>
		/// <param name="active_player">The player we want to maximize for.</param>
		/// <returns>Returns a score for the given state.</returns>
		/// <remarks>Zero is not an acceptable value.</remarks>
		protected abstract uint Evaluate(GameState<M> state, int active_player);

		/// <summary>
		/// Creates a deep copy of this AIBehavior.
		/// </summary>
		/// <returns>Returns a copy of this AIBehavior.</returns>
		public abstract AIBehavior<M> Clone();

		/// <summary>
		/// The search depth of the alpha-beta pruning algorithm.
		/// </summary>
		/// <remarks>Must be at least one.</remarks>
		public uint SearchDepth
		{
			get
			{return search_depth;}
			set
			{
				if(value < 1)
					return;

				search_depth = value;
				return;
			}
		}

		/// <summary>
		/// The value for the search depth of the alpha-beta pruning algorithm.
		/// </summary>
		protected uint search_depth = 1;

		/// <summary>
		/// The move that the alpha beta pruning has selected as the best play.
		/// </summary>
		protected M SelectedMove
		{get; set;}
	}
}
