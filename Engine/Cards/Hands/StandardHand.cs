#pragma warning disable 0162

using System;
using System.Collections.Generic;

namespace Engine.Cards.Hands
{
	/// <summary>
	/// A normal hand with nothing special to it.
	/// </summary>
	public class StandardHand : Hand
	{
		/// <summary>
		/// Creates a new empty hand.
		/// </summary>
		public StandardHand()
		{
			hand = new List<Card>();
			return;
		}

		/// <summary>
		/// Creates a new hand with the given cards in it.
		/// </summary>
		/// <param name="cards">The cards to add to the hand.</param>
		public StandardHand(IEnumerable<Card> cards)
		{
			hand = new List<Card>();
			DrawCards(cards); // These will all be cloned

			return;
		}

		/// <summary>
		/// Plays the card at the given index.
		/// </summary>
		/// <param name="index">The index the card is at.</param>
		/// <returns>Returns the card played.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the provided index is out or range.</exception>
		/// <remarks>Playing a card consists of removing it from the hand and returning it.</remarks>
		public Card PlayCard(int index)
		{
			Card ret = hand[index];
			hand.RemoveAt(index);

			return ret;
		}

		/// <summary>
		/// Plays the given card.
		/// </summary>
		/// <param name="c">The card to play.</param>
		/// <returns>Returns the card played.</returns>
		/// <exception cref="ArgumentException">Thrown when the provided card does not exist.</exception>
		/// <remarks>Playing a card consists of removing it from the hand and returning it.</remarks>
		public Card PlayCard(Card c)
		{
			int index = hand.FindIndex(handc => handc.Equals(c)); // Can't binary search as the hand may not be sorted
			
			try
			{
				Card ret = PlayCard(index);
				return ret;
			}
			catch(ArgumentOutOfRangeException e)
			{throw new ArgumentException(c + " is not in this hand.",e);}

			return null;
		}

		/// <summary>
		/// Draws the given card.
		/// The new card will be added to the end of the hand.
		/// </summary>
		/// <param name="c">The card to add to the hand.</param>
		public void DrawCard(Card c)
		{
			hand.Add(c.Clone());
			return;
		}

		/// <summary>
		/// Draws the given cards.
		/// The new cards will be added to the end of the hand.
		/// </summary>
		/// <param name="cards">The cards to add to the hand.</param>
		public void DrawCards(IEnumerable<Card> cards)
		{
			foreach(Card c in cards)
				hand.Add(c.Clone());
			
			return;
		}

		/// <summary>
		/// Moves the card at the specified index to the new specified location.
		/// </summary>
		/// <param name="index">The location of the card to move.</param>
		/// <param name="where">The new location to put the card at (Index after the card is removed).</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if either index is out or range.</exception>
		public void MoveCard(int index, int where)
		{
			Card c = hand[index];
			hand.RemoveAt(index);
			hand.Insert(where,c);

			return;
		}

		/// <summary>
		/// Sorts the hand by suit and then by value.
		/// </summary>
		public void SortBySuit()
		{
			Sort((c1,c2) => {if(c1.Suit.ID == c2.Suit.ID) {if(c1.Value.MaxValue == c2.Value.MaxValue) return 0; else if(c1.Value.MaxValue < c2.Value.MaxValue) return -1; else return 1;} else return c1.Suit.ID - c2.Suit.ID;});
			return;
		}

		/// <summary>
		/// Sorts the hand by card value and then by suit.
		/// </summary>
		public void SortByValue()
		{
			Sort((c1,c2) => {if(c1.Value.MaxValue == c2.Value.MaxValue) return c1.Suit.ID - c2.Suit.ID; else if(c1.Value.MaxValue < c2.Value.MaxValue) return -1; else return 1;});
			return;
		}

		/// <summary>
		/// Sorts the hand using the provided comparison.
		/// </summary>
		/// <param name="comp">How to compare cards.</param>
		public void Sort(Comparison<Card> comp)
		{
			hand.Sort(comp);
			return;
		}

		/// <summary>
		/// The cards in this hand.
		/// </summary>
		public IList<Card> Cards
		{
			get
			{return hand.AsReadOnly();}
		}

		/// <summary>
		/// Returns the number of cards in this hand.
		/// </summary>
		public uint CardsInHand
		{
			get
			{return (uint)hand.Count;}
		}

		/// <summary>
		/// The physical copies of the cards in this hand.
		/// </summary>
		protected List<Card> hand;
	}
}
