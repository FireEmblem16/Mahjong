using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engine.Cards
{
	/// <summary>
	/// Represents a deck of cards.
	/// </summary>
	public interface Deck
	{
		/// <summary>
		/// Shuffles the draw pile.
		/// </summary>
		void Shuffle();

		/// <summary>
		/// Shuffles the draw pile together with the discard pile.
		/// The discard pile is cleared after and the cards are placed in the draw pile.
		/// </summary>
		void Reshuffle();

		/// <summary>
		/// Shuffles the deck completely including the draw pile, the discard pile and any missing cards.
		/// All the cards will be placed in the draw pile.
		/// The discard pile is cleared and no cards are missing.
		/// </summary>
		void Reset();

		/// <summary>
		/// Draws a card.
		/// </summary>
		/// <param name="top">If true then the card will come from the top of the draw pile and if false it will come from the bottom.</param>
		/// <returns>Returns a card that remains in the draw pile.</returns>
		/// <exception cref="OutOfCards">Thrown when there are not enough cards in the draw pile to draw the requested number of cards.</exception>
		Card Draw(bool top = true);

		/// <summary>
		/// Draws a specified number of cards that remain in the draw pile.
		/// </summary>
		/// <param name="num">The number of cards to draw.</param>
		/// <param name="top">If true then the card will come from the top of the draw pile and if false it will come from the bottom.</param>
		/// <returns>Returns a list of cards that are the first <paramref name="num"/> cards from the draw pile.</returns>
		/// <exception cref="OutOfCards">Thrown when there are not enough cards in the draw pile to draw the requested number of cards.</exception>
		IList<Card> Draw(uint num, bool top = true);

		/// <summary>
		/// Takes the ith card from the discard pile.
		/// </summary>
		/// <param name="c">The index of the card to take.</param>
		/// <returns>Returns the ith card in the discard pile which is removed from the discard pile.</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown when attempting to take a card that doesn't exist.</exception>
		Card TakeFromDiscard(int c);

		/// <summary>
		/// Discards a card.
		/// </summary>
		/// <param name="c">The card to discard.</param>
		/// <param name="top">If true then the card will be discarded to the top of the discard pile and if false it will be sent to the bottom.</param>
		/// <exception cref="CardNotInDeckException">Thrown when an attempt to discard a card that is not missing from the deck is made.</exception>
		void Discard(Card c, bool top = true);

		/// <summary>
		/// Discards all the given cards.
		/// </summary>
		/// <param name="c">The cards to discard.</param>
		/// <param name="top">If true then the cards will be discarded to the top of the discard pile and if false they will be sent to the bottom.</param>
		/// <exception cref="CardNotInDeckException">Thrown when an attempt to discard a card that is not missing from the deck is made.</exception>
		/// <remarks>Cards are discarded in front to back order.</remarks>
		void Discard(IList<Card> c, bool top = true);
		
		/// <summary>
		/// Determines if the draw pile contains the given card.
		/// </summary>
		/// <param name="c">The card to search for.</param>
		/// <returns>Returns true if the draw pile contains the card and false otherwise.</returns>
		bool HasCardDrawPile(Card c);

		/// <summary>
		/// Counts the number of copies of the given card in the draw pile.
		/// </summary>
		/// <param name="c">The card to count.</param>
		/// <returns>Returns the number of copies of the provided card in the draw pile.</returns>
		uint CountCardDrawPile(Card c);

		/// <summary>
		/// Determines if the discard pile contains the given card.
		/// </summary>
		/// <param name="c">The card to search for.</param>
		/// <returns>Returns true if the discard pile contains the card and false otherwise.</returns>
		bool HasCardDiscardPile(Card c);

		/// <summary>
		/// Counts the number of copies of the given card in the discard pile.
		/// </summary>
		/// <param name="c">The card to count.</param>
		/// <returns>Returns the number of copies of the provided card in the discard pile.</returns>
		uint CountCardDiscardPile(Card c);

		/// <summary>
		/// Determines if the given card is missing from the deck.
		/// </summary>
		/// <param name="c">The card to search for.</param>
		/// <returns>Returns true if the card is missing from the deck and false otherwise.</returns>
		bool HasCardMissing(Card c);

		/// <summary>
		/// Counts the number of copies of the given card missing from the deck.
		/// </summary>
		/// <param name="c">The card to count.</param>
		/// <returns>Returns the number of copies of the provided card missing from the deck.</returns>
		uint CountCardMissing(Card c);

		/// <summary>
		/// Determines if the deck contains the given card.
		/// </summary>
		/// <param name="c">The card to search for.</param>
		/// <returns>Returns true if the deck contains the card and false otherwise.</returns>
		bool HasCardDeck(Card c);

		/// <summary>
		/// Counts the number of copies of the given card in the deck.
		/// </summary>
		/// <param name="c">The card to count.</param>
		/// <returns>Returns the number of copies of the provided card in the deck.</returns>
		uint CountCardDeck(Card c);

		/// <summary>
		/// Adds the given card to the deck.
		/// </summary>
		/// <param name="c">The card to add.</param>
		/// <param name="from">Subpiles of the deck that the cards are allowed to be added to. If more than one is specified then piles have priority as follows: draw pile then the discard pile. Cards can not be added to the missing pile and if specified it will be ignored.</param>
		void AddCard(Card c, DeckPile to = DeckPile.DRAW_PILE);

		/// <summary>
		/// Adds the given cards to the deck.
		/// </summary>
		/// <param name="c">The cards to add.</param>
		/// <param name="from">Subpiles of the deck that the cards are allowed to be added to. If more than one is specified then piles have priority as follows: draw pile then the discard pile. Cards can not be added to the missing pile and if specified it will be ignored.</param>
		void AddCards(IList<Card> c, DeckPile to = DeckPile.DRAW_PILE);

		/// <summary>
		/// Removes the given card from the deck.
		/// </summary>
		/// <param name="c">The card to remove.</param>
		/// <param name="num">The number of the card to remove. If the number is negative then all copies of the provided card will be removed.</param>
		/// <param name="from">Subpiles of the deck that the cards are allowed to be withdrawn from. If more than one is specified then cards will be removed starting from the missing pile, then the discard pile, then the draw pile (assuming the associated pile is allowed).</param>
		/// <returns>Returns the number of cards actually removed from the deck.</returns>
		uint RemoveCard(Card c, int num = 1, DeckPile from = DeckPile.ALL_PILES);

		/// <summary>
		/// Removes the given cards from the deck.
		/// </summary>
		/// <param name="c">The cards to remove.</param>
		/// <param name="num">The numbers of the cards to remove. If a number is negative then all copies of the appropriate card will be removed.</param>
		/// <param name="from">Subpiles of the deck that the cards are allowed to be withdrawn from. If more than one is specified then cards will be removed starting from the missing pile, then the discard pile, then the draw pile (assuming the associated pile is allowed).</param>
		/// <returns>Returns the number of cards actually removed from the deck for each card.</returns>
		/// <exception cref="ArgumentException">Thrown if the length of card list and number list are different.</exception>
		IList<uint> RemoveCards(IList<Card> c, IList<int> num, DeckPile from = DeckPile.ALL_PILES);

		/// <summary>
		/// Returns a string that represents this deck.
		/// </summary>
		/// <param name="format">The formatting to apply to the output string.</param>
		/// <returns>Returns a string that represents this deck with the provided formatting options.</returns>
		/// <exception cref="ArgumentException">Thrown when the provided format is invalid.</exception>
		string ToString(DeckStringFormat format = DeckStringFormat.DECK_CONTENTS);

		/// <summary>
		/// The cards currently in the draw pile.
		/// </summary>
		/// <remarks>Returns a read only copy of the draw pile.</remarks>
		IList<Card> DrawPile
		{get;}

		/// <summary>
		/// The cards currently in the discard pile.
		/// </summary>
		/// <remarks>Returns a read only copy of the discard pile.</remarks>
		IList<Card> DiscardPile
		{get;}

		/// <summary>
		/// The cards currently missing from the deck.
		/// </summary>
		/// <remarks>Returns a read only copy of the discard pile.</remarks>
		IList<Card> MissingCards
		{get;}

		/// <summary>
		/// The cards originally in the deck.
		/// </summary>
		/// <remarks>Returns a read only copy of the deck.</remarks>
		IList<Card> Deck
		{get;}

		/// <summary>
		/// The top card of the discard pile.
		/// </summary>
		Card LastDiscard
		{get;}

		/// <summary>
		/// The number of cards in the draw pile.
		/// </summary>
		uint CountDrawPile
		{get;}

		/// <summary>
		/// The number of cards in the discard pile.
		/// </summary>
		uint CountDiscardPile
		{get;}

		/// <summary>
		/// The number of cards currently missing from the deck.
		/// </summary>
		uint CountMissingCards
		{get;}

		/// <summary>
		/// The orginal number of cards in the deck before removal or discarding.
		/// </summary>
		uint CountDeckSize
		{get;}

		/// <summary>
		/// If true then when this deck runs out of cards in the draw pile it will shuffle the discard pile into the draw pile.
		/// </summary>
		bool RefillDrawPile
		{get;}
		
		/// <summary>
		/// The name of the deck.
		/// </summary>
		string DeckName
		{get;}
	}

	/// <summary>
	/// A formatting enum used with objects implementing Deck for the ToString function.
	/// </summary>
	[Flags]
	public enum DeckStringFormat
	{
		NOTHING = 0x0,
		DECK_CONTENTS = 0x1,
		DRAW_PILE_CONTENTS = 0x2,
		DISCARD_PILE_CONTENTS = 0x4,
		MISSING_CARDS = 0x8,
		ALL_PILES = 0xF
	}

	/// <summary>
	/// Bitflags for indicating which piles in the deck to be concerned with.
	/// </summary>
	[Flags]
	public enum DeckPile
	{
		DRAW_PILE = 0x1,
		DISCARD_PILE = 0x2,
		MISSING_PILE = 0x4,
		ALL_PILES = 0x7
	}
}
