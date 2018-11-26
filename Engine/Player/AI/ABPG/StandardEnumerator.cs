using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Cards;
using Engine.Game;

namespace Engine.Player.AI.ABPG
{
    /// <summary>
    /// Enumerates moves for a game.
    /// </summary>
    /// <remarks>This enumerator is valid even if the game state has changes made to it.</remarks>
    public abstract class StandardEnumerator<M> : IEnumerator<Tuple<GameState<M>,M>>
    {
        /// <summary>
        /// Creates a new enumerator for a game.
        /// </summary>
        /// <param name="state">The current state to enumerate from.</param>
        /// <param name="apply_func">An optional specialized apply function. Disregarded if the related undo function is null.</param>
        /// <param name="undo_func">An optional specialized undo function. Disregarded if the related apply function is null.</param>
        public StandardEnumerator(GameState<M> state, Apply<M> apply_func = null, Undo<M> undo_func = null)
        {
            current = -1;
            player_index = state.ActivePlayer;

            cards = new List<Card>();
				
		    foreach(Card c in state.GetPlayer(state.ActivePlayer).CardsInHand.Cards)
			    cards.Add(c.Clone());

		    this.state = state;
		    has_undo = false;

            apply = apply_func;
            undo = undo_func;

            return;
        }

        /// <summary>
        /// Moves to the next element.
        /// </summary>
        /// <returns>Returns true if there was a next element and the enumerator successfully moved to it and false otherwise.</returns>
        public bool MoveNext()
        {
		    if(current >= cards.Count)
			    throw new InvalidOperationException("Passed end of enumerator.");

		    if(has_undo)
		    {
                // We always have a move to undo
			    if(undo != null)
                    undo(state);
                else
                    state.UndoMove();

			    has_undo = false;
		    }
				
            if(++current < cards.Count)
		    {
                bool valid = false;

                // We want the state already changed in Current when other things call it so apply the next move
                if(apply != null)
                    valid = apply(state,Current.Item2);
                else
                    valid = state.ApplyMove(Current.Item2);

			    if(!valid)
			    {
				    // This move didn't work out so try the next one
				    bool ret = false;

				    try
				    {ret = MoveNext();} // Try moving to the next element
				    catch
				    {return false;} // This is no fault of the user so no exception should be thrown

				    return ret;
			    }

			    has_undo = true; // We made a move successfully
                return true;
		    }

            return false;
        }

        /// <summary>
        /// Resets the enumerator.
        /// </summary>
        public void Reset()
        {
		    if(has_undo)
		    {
			    // We always have a move to undo
			    if(undo != null)
                    undo(state);
                else
                    state.UndoMove();

			    has_undo = false;
		    }
				
		    current = -1;
		    return;
	    }

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        public void Dispose()
        {
		    // Someone's going to a lot of effort to get rid of this object so might as well null our references
		    state = null;
		    cards = null;
				
		    return;
	    }

        /// <summary>
        /// Gets the move for the current item.
        /// </summary>
        /// <param name="consider">The card to consider playing.</param>
        /// <param name="player">The player considering the move.</param>
        /// <returns>Returns the move for the current item.</returns>
        protected abstract M GetCurrentMove(Card consider, int player);

        /// <summary>
        /// The current move.
        /// </summary>
	    /// <exception cref="InvalidOperationException">Thrown if we try to call this before moving to the first element or after moving past the last element.</exception>
        public Tuple<GameState<M>,M> Current
        {
            get
            {
			    if(current < 0 || current >= cards.Count)
				    throw new InvalidOperationException("Passed end of enumerator.");
					
			    return new Tuple<GameState<M>,M>(state,GetCurrentMove(cards[current],player_index));
		    }
        }

        /// <summary>
        /// The current element.
        /// </summary>
        object IEnumerator.Current // No idea what the modifier is but modifierless works and is the default
        {
            get
            {return Current;}
        }

        /// <summary>
        /// The current card to play.
        /// </summary>
        protected int current;

        /// <summary>
        /// The index of the player.
        /// </summary>
	    /// <remarks>We keep this for convenience since ActivePlayer will change in state.</remarks>
        protected int player_index;

        /// <summary>
        /// The cards in the hand of the current player.
        /// </summary>
	    /// <remarks>We keep this for convenience.</remarks>
        protected IList<Card> cards;

	    /// <summary>
	    /// The game state we are thinking about.
	    /// </summary>
	    protected GameState<M> state;

	    /// <summary>
	    /// True if we have a move to undo.
	    /// </summary>
	    protected bool has_undo;

        /// <summary>
        /// An optional specialized apply function.
        /// </summary>
        protected Apply<M> apply;

        /// <summary>
        /// An optional specialized undo function.
        /// </summary>
        protected Undo<M> undo;
    }

    /// <summary>
    /// Applies the given move to a game state.
    /// </summary>
    /// <param name="state">The state to apply the move to.</param>
    /// <param name="m">The move to apply.</param>
    /// <returns>Returns true if the move could be applied and false otherwise.</returns>
    public delegate bool Apply<M>(GameState<M> state, M m);

    /// <summary>
    /// Undoes the last move from a game state.
    /// </summary>
    /// <param name="state">The state to undo the move from.</param>
    /// <returns>Returns true if the last move could be undone and false otherwise.</returns>
    public delegate bool Undo<M>(GameState<M> state);
}
