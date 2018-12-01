using System;
using System.Collections.Generic;
using Engine.Cards.CardTypes;
using Engine.Cards.CardTypes.Suits;
using Engine.Cards.CardTypes.Values;
using Engine.Exceptions;

namespace Engine.Cards.DeckTypes
{
	/// <summary>
	/// A standard deck of 52 (or 54) cards.
	/// Aces are high.
	/// Both the draw pile and discard pile are ordered.
	/// </summary>
	public class StandardDeck : Deck
	{
		/// <summary>
		/// Creates a new standard deck of playing cards.
		/// </summary>
		/// <param name="jokers">If true then the big and little joker cards will be included.</param>
		/// <param name="refill_on_empty">If true then when the draw pile is empty and a card is requested from it then the discard pile is shuffled into the draw pile.</param>
		public StandardDeck(bool jokers = false, bool refill_on_empty = false)
		{
			RefillDrawPile = refill_on_empty;
			has_jokers = jokers;

			// Setup the RNG
			seed = DateTime.Now.Millisecond;
			rand = new Random(seed);
			rand_calls = 0;

			deck.AddRange(CreateDeck());
			draw_pile.AddRange(CreateDeck()); // Have deep copies everywhere in case we do something crazy; besides, they take very little memory
			
			Shuffle();
			return;
		}

		/// <summary>
		/// Clones the deck.
		/// </summary>
		public virtual Deck Clone()
		{
			StandardDeck ret = new StandardDeck(HasJokers,RefillDrawPile);
			
			ret.rand = new Random(seed);
			ret.seed = seed;
			ret.rand_calls = rand_calls;

			for(uint i = 0;i < rand_calls;i++)
				ret.rand.Next();
			
			if(last_discard == null)
				ret.last_discard = null;
			else
				ret.last_discard = last_discard.Clone();

			ret.deck.Clear();
			ret.draw_pile.Clear();
			ret.discard_pile.Clear();
			ret.missing_cards.Clear();

			foreach(Card c in deck)
				ret.deck.Add(c.Clone());

			foreach(Card c in draw_pile)
				ret.draw_pile.Add(c.Clone());

			foreach(Card c in discard_pile)
				ret.discard_pile.Add(c.Clone());

			foreach(Card c in missing_cards)
				ret.missing_cards.Add(c.Clone());

			return ret;
		}
		
		/// <summary>
		/// Shuffles the draw pile.
		/// </summary>
		public void Shuffle()
		{
			// Since we need to preserve the order in the next we must actually rearrange the cards here
			for(int i = 0;i < draw_pile.Count - 1;i++) // Last card is already where it belongs
			{
				Card temp = draw_pile[i];
				int steal = rand.Next(0,draw_pile.Count - i) + i;
				rand_calls++; // Yes, this is painful, but we have no choice

				draw_pile[i] = draw_pile[steal];
				draw_pile[steal] = temp;
			}

			return;
		}

		/// <summary>
		/// Shuffles the draw pile together with the discard pile.
		/// The discard pile is cleared after and the cards are placed in the draw pile.
		/// </summary>
		public void Reshuffle()
		{
			draw_pile.AddRange(discard_pile); // No need to clone cards since we will empty the discard pile
			discard_pile.Clear();

			Shuffle();
			return;
		}

		/// <summary>
		/// Shuffles the deck completely including the draw pile, the discard pile and any missing cards.
		/// All the cards will be placed in the draw pile.
		/// The discard pile is cleared and no cards are missing.
		/// </summary>
		public void Reset()
		{
			// We won't just dump the discard and missing cards into the draw pile just in case cards have been permenantly removed from one of those three piles while the deck itself remembers those cards
			draw_pile.Clear();
			discard_pile.Clear();
			missing_cards.Clear();

			foreach(Card c in deck)
				draw_pile.Add(c.Clone());
			
			Shuffle();
			return;
		}

		/// <summary>
		/// Draws a card.
		/// </summary>
		/// <param name="top">If true then the card will come from the top of the draw pile and if false it will come from the bottom.</param>
		/// <returns>Returns a card that remains in the draw pile.</returns>
		/// <exception cref="OutOfCards">Thrown when there are not enough cards in the draw pile to draw the requested number of cards.</exception>
		public Card Draw(bool top = true)
		{
			if(draw_pile.Count == 0)
				if(!RefillDrawPile || discard_pile.Count == 0)
					throw new OutOfCardsException("No cards left in the deck.");
				else
					Reshuffle();

			Card ret;

			if(top)
			{
				ret = draw_pile[draw_pile.Count - 1]; // Top of the draw pile is the end of the list

				missing_cards.Add(ret); // We will clone the return value so we can reuse this one
				draw_pile.RemoveAt(draw_pile.Count - 1);
			}
			else
			{
				ret = draw_pile[0]; // Bottom of the draw pile is the beginning of the list

				missing_cards.Add(ret); // We will clone the return value so we can reuse this one
				draw_pile.RemoveAt(0);
			}

			return ret.Clone();
		}

		/// <summary>
		/// Returns the given card to the appropriate position in the deck.
		/// </summary>
		/// <param name="c">The card to put back.</param>
		/// <param name="top">If true, then the card will be put back on the top of the draw pile and at the bottom if false.</param>
		/// <returns>Returns true if the card was missing and could be undrawn and false otherwise.</returns>
		public bool Undraw(Card c, bool top = true)
		{
			if(c == null || missing_cards.FindLastIndex(cc => c.Equals(cc)) < 0)
				return false;

			if(top)
				draw_pile.Add(c.Clone()); // The top of the pile is the end of the list
			else
				draw_pile.Insert(0,c.Clone());
			
			missing_cards.RemoveAt(missing_cards.FindLastIndex(cc => c.Equals(cc)));
			return true;
		}

		/// <summary>
		/// Draws a specified number of cards that remain in the draw pile.
		/// </summary>
		/// <param name="num">The number of cards to draw.</param>
		/// <param name="top">If true then the card will come from the top of the draw pile and if false it will come from the bottom.</param>
		/// <returns>Returns a list of cards that are the first <paramref name="num"/> cards from the draw pile.</returns>
		/// <exception cref="OutOfCards">Thrown when there are not enough cards in the draw pile to draw the requested number of cards.</exception>
		public IList<Card> Draw(uint num, bool top = true)
		{
			if(draw_pile.Count < num) // We do this here instead of draw because we don't want to affect the deck at all if we throw an error
				if(!RefillDrawPile || draw_pile.Count + discard_pile.Count < num)
					throw new OutOfCardsException("No cards left in the deck."); // No need for else clause as Draw will call Reshuffle once the draw pile is actually empty; this preserves order
			
			List<Card> ret = new List<Card>((int)num);

			for(uint i = 0;i < num;i++)
				ret.Add(Draw(top)); // If we need to throw an error the Draw call will throw it for us though it should never happen

			return ret;
		}

		/// <summary>
		/// Takes the ith card from the discard pile.
		/// </summary>
		/// <param name="c">The index of the card to take.</param>
		/// <returns>Returns the ith card in the discard pile which is removed from the discard pile.</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown when attempting to take a card that doesn't exist.</exception>
		public Card TakeFromDiscard(int c)
		{
			if(c < 0 || c > discard_pile.Count)
				throw new IndexOutOfRangeException("No such card exists.");

			Card ret = discard_pile[c];
			discard_pile.RemoveAt(c);
			missing_cards.Add(ret);

			return ret;
		}

		/// <summary>
		/// Discards a card.
		/// </summary>
		/// <param name="c">The card to discard.</param>
		/// <param name="top">If true then the card will be discarded to the top of the discard pile and if false it will be sent to the bottom.</param>
		/// <exception cref="CardNotInDeckException">Thrown when an attempt to discard a card that is not missing from the deck is made.</exception>
		public void Discard(Card c, bool top = true)
		{
			if(!missing_cards.Contains(c))
				throw new CardNotInDeckException("Attempted to discard a card that was not part of the deck.");
			
			if(top)
			{
				LastDiscard = c; // LastDiscard will clone the card for us
				discard_pile.Add(c.Clone());
			}
			else
				discard_pile.Insert(0,c.Clone());

			missing_cards.Remove(c); // We don't care about order in missing cards so any one will do, including the first one
			return;
		}
		
		/// <summary>
		/// Discards all the given cards.
		/// </summary>
		/// <param name="c">The cards to discard.</param>
		/// <param name="top">If true then the cards will be discarded to the top of the discard pile and if false they will be sent to the bottom.</param>
		/// <exception cref="CardNotInDeckException">Thrown when an attempt to discard a card that is not missing from the deck is made.</exception>
		/// <remarks>Cards are discarded in front to back order.</remarks>
		public void Discard(IList<Card> c, bool top = true)
		{
			for(int i = 0;i < c.Count;i++)
				if(!missing_cards.Remove(c[i])) // Trial by error is probably faster on average than checking for contents (especially for multiple copies of a card)
				{
					for(int j = 0;j < i;j++)
						missing_cards.Add(c[j].Clone());

					throw new CardNotInDeckException("Attempted to discard a card that was not part of the deck.");
				}

			if(top)
			{
				discard_pile.AddRange(c); // We discard in the order they arrive and the top of the pile is at the end
				LastDiscard = c[c.Count - 1];
			}
			else
				for(int i = 0;i < c.Count;i++)
					discard_pile.Insert(0,c[i].Clone());

			return;
		}
		
		/// <summary>
		/// Determines if the draw pile contains the given card.
		/// </summary>
		/// <param name="c">The card to search for.</param>
		/// <returns>Returns true if the draw pile contains the card and false otherwise.</returns>
		public bool HasCardDrawPile(Card c)
		{return draw_pile.Contains(c);}

		/// <summary>
		/// Counts the number of copies of the given card in the draw pile.
		/// </summary>
		/// <param name="c">The card to count.</param>
		/// <returns>Returns the number of copies of the provided card in the draw pile.</returns>
		public uint CountCardDrawPile(Card c)
		{return (uint)draw_pile.FindAll(x => x.Equals(c)).Count;} // See lambda expressions

		/// <summary>
		/// Determines if the discard pile contains the given card.
		/// </summary>
		/// <param name="c">The card to search for.</param>
		/// <returns>Returns true if the discard pile contains the card and false otherwise.</returns>
		public bool HasCardDiscardPile(Card c)
		{return discard_pile.Contains(c);}

		/// <summary>
		/// Counts the number of copies of the given card in the discard pile.
		/// </summary>
		/// <param name="c">The card to count.</param>
		/// <returns>Returns the number of copies of the provided card in the discard pile.</returns>
		public uint CountCardDiscardPile(Card c)
		{return (uint)discard_pile.FindAll(x => x.Equals(c)).Count;} // See lambda expressions

		/// <summary>
		/// Determines if the given card is missing from the deck.
		/// </summary>
		/// <param name="c">The card to search for.</param>
		/// <returns>Returns true if the card is missing from the deck and false otherwise.</returns>
		public bool HasCardMissing(Card c)
		{return missing_cards.Contains(c);}

		/// <summary>
		/// Counts the number of copies of the given card missing from the deck.
		/// </summary>
		/// <param name="c">The card to count.</param>
		/// <returns>Returns the number of copies of the provided card missing from the deck.</returns>
		public uint CountCardMissing(Card c)
		{return (uint)missing_cards.FindAll(x => x.Equals(c)).Count;} // See lambda expressions

		/// <summary>
		/// Determines if the deck contains the given card.
		/// </summary>
		/// <param name="c">The card to search for.</param>
		/// <returns>Returns true if the deck contains the card and false otherwise.</returns>
		public bool HasCardDeck(Card c)
		{return deck.Contains(c);}

		/// <summary>
		/// Counts the number of copies of the given card in the deck.
		/// </summary>
		/// <param name="c">The card to count.</param>
		/// <returns>Returns the number of copies of the provided card in the deck.</returns>
		public uint CountCardDeck(Card c)
		{return (uint)deck.FindAll(x => x.Equals(c)).Count;} // See lambda expressions

		/// <summary>
		/// Adds the given card to the deck.
		/// </summary>
		/// <param name="c">The card to add.</param>
		/// <param name="from">Subpiles of the deck that the cards are allowed to be added to. If more than one is specified then piles have priority as follows: draw pile then the discard pile. Cards can not be added to the missing pile and if specified it will be ignored.</param>
		public void AddCard(Card c, DeckPile to = DeckPile.DRAW_PILE)
		{
			if((to & DeckPile.DRAW_PILE) == DeckPile.DRAW_PILE) // Bitwise operations have low priority
				draw_pile.Add(c.Clone());
			else if((to & DeckPile.DISCARD_PILE) == DeckPile.DISCARD_PILE)
				discard_pile.Add(c.Clone());
			else
				return; // We need to skip the deck add

			deck.Add(c.Clone());
			return;
		}

		/// <summary>
		/// Adds the given cards to the deck.
		/// </summary>
		/// <param name="c">The cards to add.</param>
		/// <param name="from">Subpiles of the deck that the cards are allowed to be added to. If more than one is specified then piles have priority as follows: draw pile then the discard pile. Cards can not be added to the missing pile and if specified it will be ignored.</param>
		public void AddCards(IList<Card> c, DeckPile to = DeckPile.DRAW_PILE)
		{
			foreach(Card card in c)
				AddCard(card,to);

			return;
		}

		/// <summary>
		/// Removes the given card from the deck.
		/// </summary>
		/// <param name="c">The card to remove.</param>
		/// <param name="num">The number of the card to remove. If the number is negative then all copies of the provided card will be removed.</param>
		/// <param name="from">Subpiles of the deck that the cards are allowed to be withdrawn from. If more than one is specified then cards will be removed starting from the missing pile, then the discard pile, then the draw pile (assuming the associated pile is allowed).</param>
		/// <returns>Returns the number of cards actually removed from the deck.</returns>
		public uint RemoveCard(Card c, int num = 1, DeckPile from = DeckPile.ALL_PILES)
		{
			if(num < 0)
				num = deck.Count;

			uint ret = 0;

			if((from & DeckPile.MISSING_PILE) == DeckPile.MISSING_PILE)
				while(ret < num)
					if(missing_cards.Remove(c))
					{
						deck.Remove(c); // We found the card in a subpile so it must be in the deck
						ret++;
					}
					else
						break;
			
			if((from & DeckPile.DISCARD_PILE) == DeckPile.DISCARD_PILE)
				while(ret < num)
					if(discard_pile.Remove(c))
					{
						deck.Remove(c);
						ret++;
					}
					else
						break;
			
			if((from & DeckPile.DRAW_PILE) == DeckPile.DRAW_PILE)
				while(ret < num)
					if(draw_pile.Remove(c))
					{
						deck.Remove(c);
						ret++;
					}
					else
						break;

			return ret;
		}

		/// <summary>
		/// Removes the given cards from the deck.
		/// </summary>
		/// <param name="c">The cards to remove.</param>
		/// <param name="num">The numbers of the cards to remove. If a number is negative then all copies of the appropriate card will be removed.</param>
		/// <param name="from">Subpiles of the deck that the cards are allowed to be withdrawn from. If more than one is specified then cards will be removed starting from the missing pile, then the discard pile, then the draw pile (assuming the associated pile is allowed).</param>
		/// <returns>Returns the number of cards actually removed from the deck for each card.</returns>
		/// <exception cref="ArgumentException">Thrown if the length of card list and number list are different.</exception>
		public IList<uint> RemoveCards(IList<Card> c, IList<int> num, DeckPile from = DeckPile.ALL_PILES)
		{
			if(c.Count != num.Count)
				throw new ArgumentException("List lengths not equal.");

			List<uint> ret = new List<uint>(num.Count);

			for(int i = 0;i < c.Count;i++)
				ret.Add(RemoveCard(c[i],num[i],from));

			return ret;
		}

		/// <summary>
		/// Returns a string that represents the contents of this deck.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{return ToString(DeckStringFormat.DECK_CONTENTS);}

		/// <summary>
		/// Returns a string that represents this deck.
		/// </summary>
		/// <param name="format">The formatting to apply to the output string.</param>
		/// <returns>Returns a string that represents this deck with the provided formatting options.</returns>
		/// <exception cref="ArgumentException">Thrown when the provided format is invalid.</exception>
		public string ToString(DeckStringFormat format)
		{
			string ret = "";

			if((format & DeckStringFormat.DECK_CONTENTS) == DeckStringFormat.DECK_CONTENTS)
			{
				ret += "Deck Contents [";

				foreach(Card c in deck)
					ret += c.ToString() + ", ";

				if(deck.Count != 0)
					ret = ret.Substring(0,ret.Length - 2); // Chop off the extra space and comma
				
				ret += "]";

				if((format & ~DeckStringFormat.DECK_CONTENTS) != DeckStringFormat.NOTHING)
					ret += "\n"; // We have something else to output so add a new line
			}

			if((format & DeckStringFormat.DRAW_PILE_CONTENTS) == DeckStringFormat.DRAW_PILE_CONTENTS)
			{
				ret += "Draw Pile [";

				// Reverse order as the top of the draw pile is at the end of the list
				for(int i = draw_pile.Count - 1;i >= 0;i--)
					ret += draw_pile[i].ToString() + ", ";

				if(draw_pile.Count != 0)
					ret = ret.Substring(0,ret.Length - 2); // Chop off the extra space and comma

				ret += "]";

				if((format & ~(DeckStringFormat.DECK_CONTENTS | DeckStringFormat.DRAW_PILE_CONTENTS)) != DeckStringFormat.NOTHING)
					ret += "\n"; // We have something else to output so add a new line
			}

			if((format & DeckStringFormat.DISCARD_PILE_CONTENTS) == DeckStringFormat.DISCARD_PILE_CONTENTS)
			{
				ret += "Discard Pile [";

				// Reverse order as the top of the discard pile is at the end of the list
				for(int i = discard_pile.Count - 1;i >= 0;i--)
					ret += discard_pile[i].ToString() + ", ";
				
				if(discard_pile.Count != 0)
					ret = ret.Substring(0,ret.Length - 2); // Chop off the extra space and comma

				ret += "]";

				if((format & ~(DeckStringFormat.DECK_CONTENTS | DeckStringFormat.DRAW_PILE_CONTENTS | DeckStringFormat.DISCARD_PILE_CONTENTS)) != DeckStringFormat.NOTHING)
					ret += "\n"; // We have something else to output so add a new line
			}

			if((format & DeckStringFormat.MISSING_CARDS) == DeckStringFormat.MISSING_CARDS)
			{
				ret += "Dealt Cards [";

				foreach(Card c in missing_cards)
					ret += c.ToString() + ", ";
				
				if(missing_cards.Count != 0)
					ret = ret.Substring(0,ret.Length - 2); // Chop off the extra space and comma

				ret += "]";
			}

			return ret;
		}

		/// <summary>
		/// Creates a standard deck of cards.
		/// </summary>
		/// <returns>Returns a list of cards that contains a standard deck.</returns>
		protected virtual List<Card> CreateDeck()
		{
			List<Card> ret = new List<Card>(HasJokers ? 54 : 52);
			string[] value_names = {"Duece","Three","Four","Five","Six","Seven","Eight","Nine","Ten","Knave","Queen","King","Ace"};

			for(uint i = 2;i < 15;i++)
			{
				ret.Add(new StandardCard(new Spades(),new ValueN((double)i,value_names[i - 2])));
				ret.Add(new StandardCard(new Hearts(),new ValueN((double)i,value_names[i - 2])));
				ret.Add(new StandardCard(new Clubs(),new ValueN((double)i,value_names[i - 2])));
				ret.Add(new StandardCard(new Dimonds(),new ValueN((double)i,value_names[i - 2])));
			}

			if(HasJokers)
			{
				ret.Add(new StandardCard(new RedSuit(),new ValueN(15.0,""),"Little Joker"));
				ret.Add(new StandardCard(new RedSuit(),new ValueN(16.0,""),"Big Joker"));
			}

			return ret;
		}

		/// <summary>
		/// The cards currently in the draw pile.
		/// </summary>
		/// <remarks>List.AsReadOnly is O(1).</remarks>
		public IList<Card> DrawPile
		{
			get
			{return draw_pile.AsReadOnly();}
		}

		/// <summary>
		/// The draw pile.
		/// </summary>
		/// <remarks>Using List over LinkedList due to AI having a heavier requirements for look up than for the game having removals (we seperate deck and discard so removal can not be done in O(1)). Add is O(1) in either case.</remarks>
		protected List<Card> draw_pile = new List<Card>();

		/// <summary>
		/// The cards currently in the discard pile.
		/// </summary>
		/// <remarks>List.AsReadOnly is O(1).</remarks>
		public IList<Card> DiscardPile
		{
			get
			{return discard_pile.AsReadOnly();}
		}

		/// <summary>
		/// The discard pile.
		/// </summary>
		/// <remarks>Using List over LinkedList due to AI having a heavier requirements for look up than for the game having removals (we seperate deck and discard so removal can not be done in O(1)). Add is O(1) in either case.</remarks>
		protected List<Card> discard_pile = new List<Card>();

		/// <summary>
		/// The cards currently missing from the deck.
		/// </summary>
		/// <remarks>List.AsReadOnly is O(1).</remarks>
		public IList<Card> MissingCards
		{
			get
			{return missing_cards.AsReadOnly();}
		}

		/// <summary>
		/// The cards missing from the deck.
		/// </summary>
		/// <remarks>Using List over LinkedList due to AI having a heavier requirements for look up than for the game having removals (we seperate deck and discard so removal can not be done in O(1)). Add is O(1) in either case.</remarks>
		protected List<Card> missing_cards = new List<Card>();

		/// <summary>
		/// The cards originally in the deck.
		/// </summary>
		/// <remarks>List.AsReadOnly is O(1).</remarks>
		public IList<Card> Deck
		{
			get
			{return deck.AsReadOnly();}
		}

		/// <summary>
		/// The full deck.
		/// </summary>
		/// <remarks>Using List over LinkedList due to AI having a heavier requirements for look up than for the game having removals (we seperate deck and discard so removal can not be done in O(1)). Add is O(1) in either case.</remarks>
		protected List<Card> deck = new List<Card>();

		/// <summary>
		/// The top card of the discard pile.
		/// </summary>
		/// <remarks>Creates a deep copy on get and set.</remarks>
		public Card LastDiscard
		{
			get
			{return last_discard == null ? null : last_discard.Clone();}
			protected set
			{
				last_discard = value.Clone();
				return;
			}
		}

		/// <summary>
		/// The top card of the discard pile.
		/// Available even if the discard pile is empty unless no card has ever been discarded.
		/// </summary>
		protected Card last_discard = null;

		/// <summary>
		/// The number of cards in the draw pile.
		/// </summary>
		/// <remarks>List.Count is O(1)</remarks>
		public uint CountDrawPile
		{
			get
			{return (uint)draw_pile.Count;}
		}

		/// <summary>
		/// The number of cards in the discard pile.
		/// </summary>
		/// <remarks>List.Count is O(1)</remarks>
		public uint CountDiscardPile
		{
			get
			{return (uint)discard_pile.Count;}
		}

		/// <summary>
		/// The number of cards currently missing from the deck.
		/// </summary>
		/// <remarks>List.Count is O(1)</remarks>
		public uint CountMissingCards
		{
			get
			{return (uint)missing_cards.Count;}
		}

		/// <summary>
		/// The orginal number of cards in the deck before removal or discarding.
		/// </summary>
		/// <remarks>List.Count is O(1)</remarks>
		public uint CountDeckSize
		{
			get
			{return (uint)deck.Count;}
		}

		/// <summary>
		/// If true then when this deck runs out of cards in the draw pile it will shuffle the discard pile into the draw pile.
		/// </summary>
		public bool RefillDrawPile
		{get; protected set;}
		
		/// <summary>
		/// The name of the deck.
		/// </summary>
		public virtual string DeckName
		{
			get
			{return "Standard Deck";}
		}

		/// <summary>
		/// True if this deck was created with jokers and false if it was not.
		/// </summary>
		public bool HasJokers
		{
			get
			{return has_jokers;}
		}

		/// <summary>
		/// If true then jokers were added when this deck was created.
		/// </summary>
		protected readonly bool has_jokers;

		/// <summary>
		/// A random number generator for shuffling.
		/// Pray to it.
		/// </summary>
		protected Random rand;

		/// <summary>
		/// The RNG seed.
		/// </summary>
		protected int seed;

		/// <summary>
		/// The number of times the RNG has been used.
		/// </summary>
		protected uint rand_calls = 0;
	}
}
