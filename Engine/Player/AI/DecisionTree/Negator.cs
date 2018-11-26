using System;
using System.Collections.Generic;
using Engine.Game;

namespace Engine.Player.AI.DecisionTree
{
	/// <summary>
	/// A negator node in a decision tree.
	/// Used only as a decorator to inverse the evaluation of what it decorates.
	/// </summary>
	/// <remarks>Like a not gate.</remarks>
	public class Negator<M> : Node<M>
	{
		/// <summary>
		/// Initializes this negator.
		/// Negates the value of Evaluate for its inner node. The return value of move for Evaluate will not be affected.
		/// </summary>
		/// <param name="to_decorate">This node must not be null. This node is purely a decorator node that will inverse the result of Evaluate for what it decorates.</param>
		public Negator(Node<M> to_decorate) : base(to_decorate,false)
		{
			if(to_decorate == null)
				throw new ArgumentException("Negators must decorate a node.");

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
		{return !InnerNode.Evaluate(state,ref move);}

		/// <summary>
		/// Creates a deep copy of this AIBehavior.
		/// </summary>
		/// <returns>Returns a copy of this AIBehavior.</returns>
		public override AIBehavior<M> Clone()
		{
			Negator<M> ret = new Negator<M>(InnerNode.Clone() as Node<M>);
			List<Node<M>> children = new List<Node<M>>();

			foreach(Node<M> n in Children)
				children.Add(n.Clone() as Node<M>);

			ret.AddChildren(children);
			return ret;
		}
	}
}
