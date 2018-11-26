using System.Collections.Generic;

namespace Engine.Cards.CardTypes
{
	/// <summary>
	/// Represents the value of a card.
	/// </summary>
	public interface CardValue
	{
		/// <summary>
		/// Creates a deep copy of this value.
		/// </summary>
		/// <returns>Returns a copy of this value.</returns>
		CardValue Clone();

		/// <summary>
		/// Returns true if this value is the same value as the provided value.
		/// </summary>
		/// <param name="val">The value to compare with.</param>
		/// <returns>Returns true if the two values are the same and false otherwise.</returns>
		bool Equals(CardValue val);

		/// <summary>
		/// Returns a string that represents this value.
		/// </summary>
		/// <returns>Returns a string the represents this value.</returns>
		string ToString();

		/// <summary>
		/// The values that the card this is attached to can have.
		/// </summary>
		IList<double> Value
		{get;}
		
		/// <summary>
		/// The lowest value this value can have.
		/// </summary>
		double MinValue
		{get;}

		/// <summary>
		/// The highest value this value can have.
		/// </summary>
		double MaxValue
		{get;}

		/// <summary>
		/// The maximum single value this value can have.
		/// </summary>
		double MaxSingleValue
		{get;}

	}
}
