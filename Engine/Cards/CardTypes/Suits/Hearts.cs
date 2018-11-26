using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Cards.CardTypes.Suits
{
	/// <summary>
	/// The hearts suit.
	/// </summary>
	public class Hearts : CardSuit
	{
		/// <summary>
		/// Creates a new suit of hearts.
		/// </summary>
		public Hearts()
		{return;}

		/// <summary>
		/// Creates a deep copy of this suit.
		/// </summary>
		/// <returns>Returns a copy of this suit.</returns>
		public CardSuit Clone()
		{return new Hearts();}

		/// <summary>
		/// Returns true if this suit is the same suit as the provided suit.
		/// </summary>
		/// <param name="s">The suit to compare with.</param>
		/// <returns>Returns true if the two suits are the same and false otherwise.</returns>
		public bool Equals(CardSuit s)
		{return ToString() == s.ToString();}

		/// <summary>
		/// Returns a string that represents this suit.
		/// The suit is pluralized.
		/// </summary>
		/// <returns>Returns a string the represents this suit.</returns>
		public override string ToString()
		{return "Hearts";}

		/// <summary>
		/// A way to identify a suit's actually suits.
		/// That is, a card suit can have more than one basic suit.
		/// </summary>
		public SuitIdentifier ID
		{
			get
			{return SuitIdentifier.HEART;}
		}

		/// <summary>
		/// The color of the suit.
		/// </summary>
		public SuitColor Color
		{
			get
			{return SuitColor.RED;}
		}
	}
}
