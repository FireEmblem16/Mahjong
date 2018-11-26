using System;
using System.Collections.Generic;
using System.Diagnostics;
using Engine.Game;

namespace Engine.Player.AI.DecisionTree
{
	/// <summary>
	/// A thinker node in a decision tree.
	/// Used only to waste time to make it seem like an AI is actually thinking hard.
	/// </summary>
	/// <remarks>Multiple uses of shorter thinking times can be more realistic than one long one at the top of a decision tree.</remarks>
	public class Thinker<M> : Node<M>
	{
		/// <summary>
		/// Initializes this thinker.
		/// Wastes time to make the AI seem like it's doing more work.
		/// </summary>
		/// <param name="to_decorate">This node must not be null. This node is purely a decorator node for wasting time.</param>
		/// <param name="randomize">If true then the wait time will be randomized between the min and max time.</param>
		/// <param name="min_sec">The minimum time to wait. If randomization is off then if evaluation finished before this time, the return value will wait to return until this time has been reached.</param>
		/// <param name="max_sec">The maximum time to wait. Going over this time will not cause the AI to break and force return.</param>
		public Thinker(Node<M> to_decorate, double min_sec = 0.5f, bool randomize = true, double max_sec = 3.0f) : base(to_decorate,false)
		{
			if(to_decorate == null)
				throw new ArgumentException("Thinkers must decorate a node.");

			randomized = randomize;

			// Make sure things make sense
			if(randomized && min_time < max_time)
			{
				min_time = max_sec;
				max_time = min_sec;
			}
			else // Even if we are not randomized we still get the min assignment so we don't care
			{
				min_time = min_sec;
				max_time = max_sec;
			}

			return;
		}

		/// <summary>
		/// Evaluates this node in the decision tree.
		/// If the return value is true then move will contain the desired move (if any).
		/// </summary>
		/// <param name="state">The current state of the game.</param>
		/// <param name="move">If this node wants to make a move then it will be placed in this parameter.</param>
		/// <returns>Returns true if this node is satisfied and false otherwise.</returns>
		/// <remarks>Although move must be initialized, that does not mean it is not null.</remarks>
		protected internal override bool Evaluate(GameState<M> state, ref M move)
		{
			watch.Restart();
			bool ret = InnerNode.Evaluate(state,ref move);
			watch.Stop();
			
			if(randomized && watch.Elapsed.TotalSeconds < max_time)
				System.Threading.Thread.Sleep((int)((rand.NextDouble() * (max_time - min_time) + min_time - watch.Elapsed.TotalSeconds) * 1000.0));
			else if(watch.Elapsed.TotalSeconds < min_time) // If we are randomized but greater than max we certainly won't be less than min
				System.Threading.Thread.Sleep((int)((min_time - watch.Elapsed.TotalSeconds) * 1000.0));

			return ret;
		}

		/// <summary>
		/// Creates a deep copy of this AIBehavior.
		/// </summary>
		/// <returns>Returns a copy of this AIBehavior.</returns>
		public override AIBehavior<M> Clone()
		{
			Thinker<M> ret = new Thinker<M>(InnerNode,min_time,randomized,max_time);
			List<Node<M>> children = new List<Node<M>>();

			foreach(Node<M> n in Children)
				children.Add(n.Clone() as Node<M>);

			ret.AddChildren(children);
			return ret;
		}

		/// <summary>
		/// The minimum wait time for the evaluation to finish.
		/// </summary>
		protected double min_time;

		/// <summary>
		/// The maximum wait time for the evaluation to finish.
		/// </summary>
		/// <remarks>If the evaluation takes longer it will not be forced to break.</remarks>
		protected double max_time;

		/// <summary>
		/// If true then wait time will be randomized between the min and max time.
		/// </summary>
		/// <remarks>If the actual execution time exceeds the max time no further waiting will occur.</remarks>
		protected bool randomized;

		/// <summary>
		/// A random number generator for this node.
		/// </summary>
		protected Random rand = new Random();

		/// <summary>
		/// A watch for timing execution.
		/// </summary>
		protected Stopwatch watch = new Stopwatch();
	}
}