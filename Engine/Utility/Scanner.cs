using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TheGame.Utility
{
	/// <summary>
	/// A class that parses data.
	/// </summary>
	public class Scanner
	{
		/// <summary>
		/// Creates a new parser that takes a stream.
		/// </summary>
		/// <param name="src">The input source.</param>
		/// <param name="delineators">The characters that split inputs.</param>
		/// <param name="ignore_empty">Determines if empty tokens are ignored.</param>
		public Scanner(Stream src, string[] delineators = null, StringSplitOptions ignore_empty = StringSplitOptions.RemoveEmptyEntries)
		{
			if(delineators == null)
				Delineators = new string[] {" ","\t","\n"};
			else
				Delineators = delineators;

			IgnoresEmptyTokens = ignore_empty;

			StreamReader srtemp = new StreamReader(src);
			parser = srtemp.ReadToEnd().Split(Delineators,IgnoresEmptyTokens).Select<string,Func<Type,object>>((s => {return t => (s as IConvertible).ToType(t,System.Globalization.CultureInfo.InvariantCulture);})).GetEnumerator();
			srtemp.Close();

			HasNextItem = parser.MoveNext();
			return;
		}

		/// <summary>
		/// Creates a new parser that takes a string.
		/// </summary>
		/// <param name="src">The input source.</param>
		/// <param name="delineators">The characters that split inputs.</param>
		/// <param name="ignore_empty">Determines if empty tokens are ignored.</param>
		public Scanner(string src, string[] delineators = null, StringSplitOptions ignore_empty = StringSplitOptions.RemoveEmptyEntries)
		{
			if(delineators == null)
				Delineators = new string[] {" ","\t","\n"};
			else
				Delineators = delineators;

			IgnoresEmptyTokens = ignore_empty;

			parser = src.Split(Delineators,IgnoresEmptyTokens).Select<string,Func<Type,object>>((s => {return t => (s as IConvertible).ToType(t,System.Globalization.CultureInfo.InvariantCulture);})).GetEnumerator();

			HasNextItem = parser.MoveNext();
			return;
		}

		/// <summary>
		/// Peeks at the next item in the scanner.
		/// </summary>
		/// <typeparam name="T">The type we are expecting.</typeparam>
		/// <returns>Returns the default value of type T if the next item is not the provided type or if there are no more items.</returns>
		public T PeekNext<T>()
		{
			T ret = default(T);

			if(!HasNextItem)
				return ret;

			try
			{ret = (T)parser.Current(typeof(T));}
			catch
			{}

			return ret;
		}

		/// <summary>
		/// Gets the next item in the scanner.
		/// </summary>
		/// <typeparam name="T">The type we are expecting.</typeparam>
		/// <returns>Returns the default value of type T if the next item is not the provided type or if there are no more items.</returns>
		public T Next<T>()
		{
			T ret = default(T);

			if(!HasNextItem)
				return ret;

			try
			{ret = (T)parser.Current(typeof(T));}
			catch
			{}

			HasNextItem = parser.MoveNext();
			return ret;
		}

		/// <summary>
		/// Determines if the scanner's next item (if any) is of the given type.
		/// </summary>
		/// <typeparam name="T">The type to check for.</typeparam>
		/// <returns>Returns true if the next item is of the given type and false otherwise.</returns>
		public bool HasNext<T>()
		{
			if(!HasNextItem)
				return false;

			try
			{parser.Current(typeof(T));}
			catch
			{return false;}

			return true;
		}

		/// <summary>
		/// If true then there are more inputs in the scanner.
		/// </summary>
		public bool HasNextItem
		{get; protected set;}

		/// <summary>
		/// The characters that split input.
		/// </summary>
		public string[] Delineators
		{get; protected set;}

		/// <summary>
		/// If true then empty tokens are ignored in this scanner.
		/// </summary>
		public StringSplitOptions IgnoresEmptyTokens
		{get; protected set;}

		/// <summary>
		/// The parser for this scanner.
		/// </summary>
		protected IEnumerator<Func<Type,object>> parser;
	}
}
