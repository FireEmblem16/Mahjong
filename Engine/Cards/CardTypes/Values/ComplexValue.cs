using System.Collections.Generic;

namespace Engine.Cards.CardTypes.Values
{
	/// <summary>
	/// A value for cards that can have more than one value.
	/// </summary>
	public class ComplexValue : CardValue
	{
		/// <summary>
		/// Creates a new card value.
		/// </summary>
		public ComplexValue(List<double> vals, string name)
		{
			values = new List<double>(vals);
			Name = name;

			int min_index = 0;
			int max_index = 0;

			for(int i = 0;i < values.Count;i++)
			{
				if(values[i] < values[min_index])
					min_index = i;

				if(values[i] > values[max_index])
					max_index = i;

				MaxValue += values[i];
			}
			
			MinValue = values[min_index];
			MaxSingleValue = values[max_index];

			return;
		}

		/// <summary>
		/// Creates a deep copy of this value.
		/// </summary>
		/// <returns>Returns a copy of this value.</returns>
		public CardValue Clone()
		{return new ComplexValue(values,Name);}

		/// <summary>
		/// Returns true if this value is the same value as the provided value.
		/// </summary>
		/// <param name="val">The value to compare with.</param>
		/// <returns>Returns true if the two values are the same and false otherwise.</returns>
		public bool Equals(CardValue val)
		{
			foreach(double d in val.Value)
				if(!values.Contains(d))
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
			{return values.AsReadOnly();}
		}

		/// <summary>
		/// The lowest value this value can have.
		/// </summary>
		public double MinValue
		{get; protected set;}

		/// <summary>
		/// The highest value this value can have.
		/// </summary>
		public double MaxValue
		{get; protected set;}

		/// <summary>
		/// The maximum single value this value can have.
		/// </summary>
		public double MaxSingleValue
		{get; protected set;}

		/// <summary>
		/// The values that this card value has.
		/// </summary>
		protected List<double> values;

		/// <summary>
		/// The name of this value.
		/// </summary>
		protected readonly string Name;
	}
}
