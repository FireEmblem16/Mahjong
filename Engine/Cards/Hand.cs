using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engine.Cards
{
	/// <summary>
	/// Represents a player's hand.
	/// </summary>
	public interface Hand
	{
		/// <summary>
		/// Plays the card at the given index.
		/// </summary>
		/// <param name="index">The index the card is at.</param>
		/// <returns>Returns the card played.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the provided index is out or range.</exception>
		/// <remarks>Playing a card consists of removing it from the hand and returning it.</remarks>
		Card PlayCard(int index);

		/// <summary>
		/// Plays the given card.
		/// </summary>
		/// <param name="c">The card to play.</param>
		/// <returns>Returns the card played.</returns>
		/// <exception cref="ArgumentException">Thrown when the provided card does not exist.</exception>
		/// <remarks>Playing a card consists of removing it from the hand and returning it.</remarks>
		Card PlayCard(Card c);

		/// <summary>
		/// Draws the given card.
		/// </summary>
		/// <param name="c">The card to add to the hand.</param>
		void DrawCard(Card c);

		/// <summary>
		/// Draws the given cards.
		/// </summary>
		/// <param name="cards">The cards to add to the hand.</param>
		void DrawCards(IEnumerable<Card> cards);

		/// <summary>
		/// Moves the card at the specified index to the new specified location.
		/// </summary>
		/// <param name="index">The location of the card to move.</param>
		/// <param name="where">The new location to put the card at (Index after the card is removed).</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if either index is out or range.</exception>
		void MoveCard(int index, int where);

		/// <summary>
		/// Sorts the hand by suit and then by value.
		/// </summary>
		void SortBySuit();

		/// <summary>
		/// Sorts the hand by card value and then by suit.
		/// </summary>
		void SortByValue();

		/// <summary>
		/// Sorts the hand using the provided comparison.
		/// </summary>
		/// <param name="comp">How to compare cards.</param>
		void Sort(Comparison<Card> comp);

		/// <summary>
		/// The cards in this hand.
		/// </summary>
		IList<Card> Cards
		{get;}

		/// <summary>
		/// Returns the number of cards in this hand.
		/// </summary>
		uint CardsInHand
		{get;}
	}
}
