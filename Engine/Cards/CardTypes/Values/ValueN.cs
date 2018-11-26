using System.Collections.Generic;

namespace Engine.Cards.CardTypes.Values
{
	/// <summary>
	/// A card value with value of n.
	/// </summary>
	public class ValueN : CardValue
	{
		/// <summary>
		/// Creates a new card value.
		/// </summary>
		/// <param name="n">The value of this card value.</param>
		/// <param name="name">The name of this value.</param>
		public ValueN(double n, string name)
		{
			val = n;
			Name = name;

			return;
		}

		/// <summary>
		/// Creates a deep copy of this value.
		/// </summary>
		/// <returns>Returns a copy of this value.</returns>
		public CardValue Clone()
		{return new ValueN(val,Name);}

		/// <summary>
		/// Returns true if this value is the same value as the provided value.
		/// </summary>
		/// <param name="val">The value to compare with.</param>
		/// <returns>Returns true if the two values are the same and false otherwise.</returns>
		public bool Equals(CardValue val)
		{
			if(val.Value.Count != 1 || this.val != val.Value[0])
				return false;

			return true;
		}

		/// <summary>
		/// Returns a string that represents this value.
		/// </summary>
		/// <returns>Returns a string the represents this value.</returns>
		public override string ToString()
		{return Name;}

		/// <summary>
		/// The values that the card this is attached to can have.
		/// </summary>
		public IList<double> Value
		{
			get
			{
				List<double> ret = new List<double>(1);
				ret.Add(val);

				return ret.AsReadOnly();
			}
		}

		/// <summary>
		/// The lowest value this value can have.
		/// </summary>
		public double MinValue
		{
			get
			{return val;}
		}

		/// <summary>
		/// The highest value this value can have.
		/// </summary>
		public double MaxValue
		{
			get
			{return val;}
		}

		/// <summary>
		/// The maximum single value this value can have.
		/// </summary>
		public double MaxSingleValue
		{
			get
			{return val;}
		}

		/// <summary>
		/// The value of this card value.
		/// </summary>
		protected readonly double val;

		/// <summary>
		/// The name of this value.
		/// </summary>
		protected readonly string Name;
	}
}
