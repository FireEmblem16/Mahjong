using Engine.Game;
using System.Collections.Generic;
using System;

namespace Engine.Player.AI.DecisionTree
{
	/// <summary>
	/// A basic node in the decision tree.
	/// </summary>
	/// <remarks>All nodes are decorators.</remarks>
	public abstract class Node<M> : AIBehavior<M>
	{
		/// <summary>
		/// Initiaizes the node base.
		/// </summary>
		/// <param name="to_decorate">If this is not null then this node will decorate the given node for evaluation.</param>
		/// <param name="terminal">If true this is a terminal node and has no children.</param>
		/// <remarks>In some cases it would make sense to decorate a terminal node so such functionality is preserved. User is required to ensure that does not break anything.</remarks>
		protected Node(Node<M> to_decorate = null, bool terminal = true)
		{
			InnerNode = to_decorate;
			Terminal = terminal;

			if(Terminal || InnerNode != null)
				children = null;
			else
				children = new List<Node<M>>();

			return;
		}

		/// <summary>
		/// Determines the next move for this AI to make.
		/// </summary>
		/// <param name="state">The state of the game. This will be cloned so that the AI does not affect the actual game state.</param>
		/// <returns>Returns the next move to make based on the current game state.</returns>
		/// <remarks>A complete AI will never return a null move. However an incomplete AI can return null.</remarks>
		public M GetNextMove(GameState<M> state)
		{
			M ret = default(M);

			if(Evaluate(state.Clone(),ref ret)) // A complete AI should never return false
				return ret;

			return default(M);
		}

		/// <summary>
		/// Evaluates this node in the decision tree.
		/// If the return value is true then move will contain the desired move (if any).
		/// </summary>
		/// <param name="state">The current state of the game.</param>
		/// <param name="move">If this node wants to make a move then it will be placed in this parameter.</param>
		/// <returns>Returns true if this node is satisfied and false otherwise.</returns>
		/// <remarks>Although move must be initialized, that does not mean it is not null.</remarks>
		protected internal abstract bool Evaluate(GameState<M> state, ref M move);

		/// <summary>
		/// Adds a child node to this node.
		/// </summary>
		/// <param name="n">The node to add.</param>
		/// <returns>Returns true if the node was added or false if nodes can not be added to this node.</returns>
		public virtual bool AddChild(Node<M> n)
		{
			if(InnerNode != null)
				return InnerNode.AddChild(n); // Children should always be added to the inner most node (note that decoration happens at construction so children can only be on the inner most node)

			if(!Terminal)
				children.Add(n);

			return !Terminal;
		}

		/// <summary>
		/// Adds all the nodes given as children of this node.
		/// </summary>
		/// <param name="nodes">The nodes to add.</param>
		/// <returns>The number of nodes actually added.</returns>
		/// <exception cref="ArgumentException">Thrown if called on a terminal node.</exception>
		public virtual int AddChildren(IEnumerable<Node<M>> nodes)
		{
			if(InnerNode != null)
				return InnerNode.AddChildren(nodes);

			if(Terminal)
				throw new ArgumentException("Terminal nodes have no children.");

			int ret = 0;

			foreach(Node<M> n in nodes)
				if(AddChild(n))
					ret++;

			return ret;
		}

		/// <summary>
		/// Removes the child at the given index.
		/// </summary>
		/// <param name="index">The index of the child to remove.</param>
		/// <returns>Returns the node that is removed.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the index provided is out of bounds.</exception>
		public virtual Node<M> RemoveChild(int index)
		{
			if(InnerNode != null)
				return InnerNode.RemoveChild(index);

			if(Terminal)
				throw new ArgumentOutOfRangeException("Terminal nodes have no children.");

			Node<M> ret = children[index];
			children.RemoveAt(index);

			return ret;
		}

		/// <summary>
		/// Removes all children from this node.
		/// </summary>
		public virtual void RemoveAllChildren()
		{
			if(InnerNode != null)
				InnerNode.RemoveAllChildren();
			else if(!Terminal)
				children.Clear();

			return;
		}

		/// <summary>
		/// Creates a deep copy of this AIBehavior.
		/// </summary>
		/// <returns>Returns a copy of this AIBehavior.</returns>
		public abstract AIBehavior<M> Clone();

		/// <summary>
		/// If this node is decorating another node, the inner node will be located here.
		/// </summary>
		/// <remarks>If there is no inner node this value is null.</remarks>
		public Node<M> InnerNode
		{get; protected set;}
		
		/// <summary>
		/// If true then this is a terminal node.
		/// If false then this node may have children (although it does not necessarily).
		/// </summary>
		public bool Terminal
		{get; protected set;}

		/// <summary>
		/// The children of this node.
		/// </summary>
		/// <remarks>If this is a strictly terminal node Children should be null.</remarks>
		public virtual IEnumerable<Node<M>> Children
		{
			get
			{
				if(InnerNode != null)
					return InnerNode.Children;

				return children;
			}
		}

		/// <summary>
		/// The actual copies of the children of this node.
		/// </summary>
		protected List<Node<M>> children;

		/// <summary>
		/// The number of children this node has.
		/// </summary>
		public virtual int NumberOfChildren
		{
			get
			{
				if(InnerNode != null)
					return InnerNode.NumberOfChildren;

				return children.Count;
			}
		}
	}
}
