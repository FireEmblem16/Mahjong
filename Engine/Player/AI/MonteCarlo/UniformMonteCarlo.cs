using System;
using System.Collections.Generic;

namespace Engine.Player.AI.MonteCarlo
{
	/// <summary>
	/// Performs a uniformly weighted monte carlo search.
	/// </summary>
	/// <typeparam name="S">The type of state to be evaluated.</typeparam>
	/// <typeparam name="A">The type of action to be taken.</typeparam>
	public static class UniformMonteCarloSearch<S,A>
	{
		/// <summary>
		/// Given a start state, performs a random sampling of it to terminal states and determines which available action is best to take.
		/// </summary>
		/// <param name="state">The state to start in.</param>
		/// <param name="samples">The number of samples to take for each actino.</param>
		/// <param name="cloner">The means by which duplicate states may be generated.</param>
		/// <param name="action_enumerator">Enumerates the actions available in any given state.</param>
		/// <param name="updater">Takes a state and an action and applies one to the other.</param>
		/// <param name="state_hueristic">Determines how good a terminal state is.</param>
		/// <returns>Returns the best action to take.</returns>
		public static A Search(S state, int samples, StateCloner<S> cloner, ActionEnumerator<A,S> action_enumerator, ActionApplier<A,S> updater, StateEvaluator<S> state_hueristic)
		{
			List<A> actions = new List<A>();
			List<int> expected_values = new List<int>();

			// Get all the actions
			IEnumerator<A> acts = action_enumerator(state);

			while(acts.MoveNext())
				actions.Add(acts.Current);

			// If we have no actions, we can't do anything
			if(actions.Count == 0)
				return default(A);

			// If we have one action, don't waste time
			if(actions.Count == 1)
				return actions[0];

			// Obtain the expected value of each action
			foreach(A a in actions)
				expected_values.Add(Explore(updater(cloner(state),a),samples,cloner,action_enumerator,updater,state_hueristic));

			// Find the best action
			int max = expected_values[0];

			List<A> best = new List<A>();
			best.Add(actions[0]);

			for(int i = 1;i < expected_values.Count;i++)
				if(expected_values[i] > max)
				{
					max = expected_values[i];

					best.Clear();
					best.Add(actions[i]);
				}
				else if(expected_values[i] == max)
					best.Add(actions[i]);
			
			return best[rand.Next(0,best.Count)];
		}

		/// <summary>
		/// Assigns the RNG.
		/// </summary>
		/// <param name="r">The random number generator to use.</param>
		public static void AssignRNG(Random r)
		{
			rand = r;
			return;
		}

		/// <summary>
		/// Takes a state and returns the expected value (pre-division) of its outcome.
		/// </summary>
		/// <param name="state">The state to start in.</param>
		/// <param name="samples">The number of samples to take for each actino.</param>
		/// <param name="cloner">The means by which duplicate states may be generated.</param>
		/// <param name="action_enumerator">Enumerates the actions available in any given state.</param>
		/// <param name="updater">Takes a state and an action and applies one to the other.</param>
		/// <param name="state_hueristic">Determines how good a terminal state is.</param>
		/// <returns>Returns the expected value (pre-division) of the state's outcome.</returns>
		private static int Explore(S state, int samples, StateCloner<S> cloner, ActionEnumerator<A,S> action_enumerator, ActionApplier<A,S> updater, StateEvaluator<S> state_hueristic)
		{
			int ret = 0;

			for(int i = 0;i < samples;i++)
				ret += PlayToCompletion(cloner(state),action_enumerator,updater,state_hueristic);

			return ret;
		}

		/// <summary>
		/// Takes a state and executes it to completion randomly and returns a value representing how good the outcome is.
		/// </summary>
		/// <param name="state">The state to start in.</param>
		/// <param name="action_enumerator">Enumerates the actions available in any given state.</param>
		/// <param name="updater">Takes a state and an action and applies one to the other.</param>
		/// <param name="state_hueristic">Determines how good a terminal state is.</param>
		/// <returns>A value representing how good a random terminal node of the given state is.</returns>
		private static int PlayToCompletion(S state, ActionEnumerator<A,S> action_enumerator, ActionApplier<A,S> updater, StateEvaluator<S> state_hueristic)
		{
			List<A> actions = new List<A>();

			// Get all the actions
			IEnumerator<A> acts = action_enumerator(state);

			while(acts.MoveNext())
				actions.Add(acts.Current);

			// If we have no actions left, we're in a terminal state
			if(actions.Count == 0)
				return state_hueristic(state);

			// Pick a random action and proceed further
			return PlayToCompletion(updater(state,actions[rand.Next(0,actions.Count)]),action_enumerator,updater,state_hueristic);
		}

		/// <summary>
		/// A random number generator. Pray to it, and may it grant you fortune.
		/// </summary>
		private static Random rand = new Random();
	}

	/// <summary>
	/// Takes a state and turns it into a number representing how good it is.
	/// </summary>
	/// <typeparam name="S">The type of the state to evaluate.</typeparam>
	/// <param name="state">The state to evaluate.</param>
	/// <returns>Returns an integer representing the 'goodness' of the state. Higher values are superior, and negative values are very bad.</returns>
	public delegate int StateEvaluator<S>(S state);

	/// <summary>
	/// Clones a given state.
	/// </summary>
	/// <typeparam name="S">The type of state to clone.</typeparam>
	/// <param name="state">The state to clone.</param>
	/// <returns>Returns a deep copy of the provided parameter.</returns>
	public delegate S StateCloner<S>(S state);

	/// <summary>
	/// Takes a state and enumerates the available actions in it. When no actions can be enumerated, the parameter represents a terminal state.
	/// </summary>
	/// <typeparam name="A">The type of action.</typeparam>
	/// <typeparam name="S">The type of state.</typeparam>
	/// <param name="state">The state to enumerate available actions in.</param>
	/// <returns>Returns an enumerator that iterates through all of the available actions in the given state.</returns>
	public delegate IEnumerator<A> ActionEnumerator<A,S>(S state);

	/// <summary>
	/// Takes a state and an action and applies the action to the state (the original state data may be altered).
	/// </summary>
	/// <typeparam name="A">The type of action.</typeparam>
	/// <typeparam name="S">The type of state.</typeparam>
	/// <param name="state">The state to update.</param>
	/// <param name="action">The action to take.</param>
	/// <returns>Returns the new state after the given action has been undertaken.</returns>
	public delegate S ActionApplier<A,S>(S state, A action);
}
