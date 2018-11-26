using System;
using System.Collections.Generic;

namespace TheGame.AI.ABP
{
	/// <summary>
	/// Performs an alpha-beta search.
	/// </summary>
	/// <typeparam name="S">The type of action to be taken.</typeparam>
	/// <typeparam name="T">The type of state to be evaluated.</typeparam>
	public static class AlphaBetaPruner<A,S>
	{
		/// <summary>
		/// Given a state and a maximum search depth, this performs a search using the alpha-beta algorithm to find the optimal action to take.
		/// </summary>
		/// <param name="state">The current state.</param>
		/// <param name="max_depth">The maximum number of sequential actions that may be taken.</param>
		/// <param name="state_hueristic">The means of determining how good a state is.</param>
		/// <param name="action_enumerator">The means of proceeding to a new state.</param>
		/// <param name="updater">The means by which a state is updated with new actions.</param>
		/// <param name="maximizing">Determines if we are in a maximizing or minimizing state.</param>
		/// <returns>Returns the optimal action to take in the given state or default(A) if something went wrong.</returns>
		public static A Search(S state, int max_depth, StateEvaluator<S> state_hueristic, ActionEnumerator<A,S> action_enumerator, ActionApplier<A,S> updater, Maximising<S> maximising)
		{
			// Sanity check
			if(max_depth < 1 || state == null || state_hueristic == null || action_enumerator == null || maximising == null)
				return default(A);

			// Bookkeeping
			List<A> best_actions = new List<A>();
			int best_val = int.MinValue;

			// Enumerate the available moves in the given state
			IEnumerator<A> actions = action_enumerator(state);
			
			// If there are no moves, we've reached a terminal state and can't do anything
			if(!actions.MoveNext())
				return default(A);
			
			// If there's only one move (a forced move) then just take it
			// Note that this is only a useful case in the inital call as later one we're trying to compute the values of each move we could take
			if(!actions.MoveNext())
			{
				actions.Reset();
				actions.MoveNext();

				return actions.Current;
			}

			// There's more than one move, so check them all out
			actions.Reset();
			actions.MoveNext();

			// We assume we start with the maximising player
			do
			{
				// Check out how good the current action is
				int val = RSearch(state,updater(state,actions.Current),max_depth - 1,state_hueristic,action_enumerator,updater,maximising,best_val,int.MaxValue);

				// If the current action is trash, ignore it
				if(val < best_val)
					continue;

				// If the current action is okay, add it to the list
				if(val == best_val)
				{
					best_actions.Add(actions.Current);
					continue;
				}

				// The current action is crazy, so rock on
				best_actions.Clear();
				best_actions.Add(actions.Current);

				best_val = val;
			}
			while(actions.MoveNext());
			
			// We're garunteed to have at least one action to take, so this list will never be empty
			return best_actions[rand.Next() % best_actions.Count];
		}
		
		/// <summary>
		/// Given a state and a maximum search depth, this performs a recursive search using the alpha-beta algorithm to find the optimal action to take.
		/// </summary>
		/// <param name="init">The initial state.</param>
		/// <param name="state">The current state.</param>
		/// <param name="max_depth">The maximum number of sequential actions that may be taken.</param>
		/// <param name="state_hueristic">The means of determining how good a state is.</param>
		/// <param name="action_enumerator">The means of proceeding to a new state.</param>
		/// <param name="updater">The means by which a state is updated with new actions.</param>
		/// <param name="maximising">True if the algorithm should maximise at this step and false if it should minimise.</param>
		/// <param name="alpha">The best maximisation value so far.</param>
		/// <param name="beta">The best minimisation value so far.</param>
		/// <returns>Returns the value of the optimal action to take in the given state.</returns>
		private static int RSearch(S init, S state, int depth, StateEvaluator<S> state_hueristic, ActionEnumerator<A,S> action_enumerator, ActionApplier<A,S> updater, Maximising<S> maximising, int alpha, int beta)
		{
			// If we've reached our terminal depth, evaluate and return
			if(depth == 0)
				return state_hueristic(state);

			// Enumerate the available moves in the given state
			IEnumerator<A> actions = action_enumerator(state);

			// If there are no moves, we've reached a terminal state and need to evaluate it and return
			if(!actions.MoveNext())
				return state_hueristic(state);
			
			if(maximising(init,state))
			{
				int max = int.MinValue;

				do
				{
					max = Math.Max(max,RSearch(init,updater(state,actions.Current),depth - 1,state_hueristic,action_enumerator,updater,maximising,alpha,beta));
					alpha = alpha > max ? alpha : max;

					if(alpha >= beta)
						break;
				}
				while(actions.MoveNext());

				return max;
			}
			
			int min = int.MaxValue;

			do
			{
				min = Math.Min(min,RSearch(init,updater(state,actions.Current),depth - 1,state_hueristic,action_enumerator,updater,maximising,alpha,beta));
				beta = beta < min ? beta : min;

				if(alpha >= beta)
					break;
			}
			while(actions.MoveNext());

			return min;
		}

		/// <summary>
		/// A random number generator to use.
		/// </summary>
		private static Random rand = new Random();
	}

	/// <summary>
	/// Takes a state and turns it into a number representing how good it is.
	/// </summary>
	/// <typeparam name="T">The type of the state to evaluate.</typeparam>
	/// <param name="state">The state to evaluate.</param>
	/// <returns>Returns an integer representing the 'goodness' of the state. Higher values are superior, and negative values are very bad.</returns>
	public delegate int StateEvaluator<T>(T state);

	/// <summary>
	/// Takes a state and enumerates the available actions in it.
	/// </summary>
	/// <typeparam name="A">The type of action.</typeparam>
	/// <typeparam name="S">The type of state.</typeparam>
	/// <param name="state">The state to enumerate available actions in.</param>
	/// <returns>Returns an enumerator that iterates through all of the available actions in the given state.</returns>
	public delegate IEnumerator<A> ActionEnumerator<A,S>(S state);

	/// <summary>
	/// Takes a state and an action and applies the action to the state (without altering the original state data).
	/// </summary>
	/// <typeparam name="A">The type of action.</typeparam>
	/// <typeparam name="S">The type of state.</typeparam>
	/// <param name="state">The state to update.</param>
	/// <param name="action">The action to take.</param>
	/// <returns>Returns the new state after the given action has been undertaken.</returns>
	public delegate S ActionApplier<A,S>(S state, A action);

	/// <summary>
	/// Determines if the minimax algorithm should be minimising or maximising in the given state.
	/// </summary>
	/// <typeparam name="S">The type of state.</typeparam>
	/// <param name="initial_state">The initial state which is assume to represent the maximising player.</param>
	/// <param name="current_state">The current state</param>
	/// <returns>True if the algorithm should maximise and false otherwise</returns>
	public delegate bool Maximising<S>(S initial_state, S current_state);
}
