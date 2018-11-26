using System;
using System.Collections.Generic;
using Engine.Game;

namespace Engine.Player.AI.DecisionTree
{
	/// <summary>
	/// A randomizer node in a decision tree.
	/// Used only as a decorator to randomize the order of the children of the node it decorates at each time of evaluation.
	/// </summary>
	public class Randomizer<M> : Node<M>
	{
		/// <summary>
		/// Initializes this randomizer.
		/// Randomizes the order of the children of the inner node each time evaluation is called.
		/// </summary>
		/// <param name="to_decorate">This node must not be null. This node is purely a decorator node for randomization.</param>
		public Randomizer(Node<M> to_decorate) : base(to_decorate,false)
		{
			if(to_decorate == null)
				throw new ArgumentException("Randomizers must decorate a node.");

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
			IEnumerable<Node<M>> chldrn = Children;
			InnerNode.RemoveAllChildren();
			InnerNode.AddChildren(chldrn);

			return InnerNode.Evaluate(state,ref move);
		}

		/// <summary>
		/// Creates a deep copy of this AIBehavior.
		/// </summary>
		/// <returns>Returns a copy of this AIBehavior.</returns>
		public override AIBehavior<M> Clone()
		{
			Randomizer<M> ret = new Randomizer<M>(InnerNode);
			List<Node<M>> children = new List<Node<M>>();

			foreach(Node<M> n in base.Children)
				children.Add(n.Clone() as Node<M>);

			ret.AddChildren(children);
			return ret;
		}

		/// <summary>
		/// The children of this node.
		/// </summary>
		/// <remarks>If this is a strictly terminal node Children should be null.</remarks>
		public override IEnumerable<Node<M>> Children
		{
			get
			{
				List<Node<M>> randomized = new List<Node<M>>(InnerNode.Children); // There must be an inner node

				for(int i = 0;i < randomized.Count - 1;i++) // Last element is by definition already randomly placed
				{
					int swap_index = rand.Next(i,randomized.Count); // No harm done if this index is also i
					
					Node<M> ntemp = randomized[swap_index];
					randomized[swap_index] = randomized[i];
					randomized[i] = ntemp;
				}

				return randomized;
			}
		}

		/// <summary>
		/// A random number generator for this randomizer.
		/// </summary>
		/// <remarks>Pray to it.</remarks>
		protected Random rand = new Random();
	}
}
