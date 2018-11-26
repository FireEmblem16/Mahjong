using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Cards.DeckTypes
{
	/// <summary>
	/// Creates a new deck that contains n copies of the standard deck of 52 (or 54) playing cards.
    /// Aces are high.
    /// Both the draw pile and discard pile are ordered.
	/// </summary>
	public class MultiDeck : StandardDeck
	{
		/// <summary>
		/// Creates a new standard deck of playing cards.
		/// </summary>
		/// <param name="n">The number of </param>
		/// <param name="jokers">If true then the big and little joker cards will be included.</param>
		/// <exception cref="ArgumentException">Thrown when the number of decks to create this deck with is zero.</exception>
		public MultiDeck(uint n = 2, bool jokers = false) : base(jokers)
		{
			if(n == 0)
				throw new ArgumentException("A multi deck must contain at least one copy of the standard deck of cards.");
			
			num_decks = n;

			// The standard deck has created a deck of cards for us but we need to add more
			for(uint i = 1;i < n;i++)
			{
				deck.AddRange(CreateDeck());
				draw_pile.AddRange(CreateDeck());
			}

			Shuffle();
			return;
		}

		/// <summary>
		/// The number of copies of the standard deck this deck was created with.
		/// </summary>
		public uint NumberOfDecks
		{
			get
			{return num_decks;}
		}

		/// <summary>
		/// Contains the number of copies of the standard deck this deck was created with.
		/// </summary>
		protected readonly uint num_decks;
	}
}
