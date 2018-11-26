using System;

namespace TheGame.Utility
{
	public class Pair<T,S>
	{
		public Pair(T a, S b)
		{
			val1 = a;
			val2 = b;

			return;
		}

		public Pair(Pair<T,S> p)
		{
			val1 = p.val1;
			val2 = p.val2;
			
			return;
		}

		public static bool operator ==(Pair<T,S> p1, Pair<T,S> p2)
		{
			// If we have the same reference or we have two nulls return true
			if(System.Object.ReferenceEquals(p1,p2))
				return true;

			// If only one is null then return false
			if(p1 == null || p2 == null)
				return false;
			
			bool v1 = false;
			bool v2 = false;

			if(p1.val1 != null)
				v1 = p1.val1.Equals(p2.val1);
			else if(p2.val1 == null)
				v1 = true;
			
			if(p1.val2 != null)
				v2 = p1.val2.Equals(p2.val2);
			else if(p2.val2 == null)
				v2 = true;

			return v1 && v2;
		}

		public static bool operator !=(Pair<T,S> p1, Pair<T,S> p2)
		{return !(p1 == p2);}
		
		public static Pair<int,int> Add(Pair<int,int> a, Pair<int,int> b)
		{return new Pair<int,int>(a.val1 + b.val1,a.val2 + b.val2);}

		public static Pair<int,int> Subtract(Pair<int,int> a, Pair<int,int> b)
		{return new Pair<int,int>(a.val1 - b.val1,a.val2 - b.val2);}

		public static int Distance(Pair<int,int> a, Pair<int,int> b)
		{return Math.Abs(a.val1 - b.val1) + Math.Abs(a.val2 - b.val2);}

		public override bool Equals(object obj)
		{
			if(obj.GetType() != typeof(Pair<T,S>))
				return false;
			
			return Equals((Pair<T,S>)obj);
		}

		public bool Equals(Pair<T,S> p)
		{
			if(val1.Equals(p.val1) && val2.Equals(p.val2))
				return true;
			
			return false;
		}

		public override int GetHashCode()
		{return val1.GetHashCode() + val2.GetHashCode();}

		public override string ToString()
		{return "(" + val1.ToString() + "," + val2.ToString() + ")";}

		public T val1
		{get; protected set;}

		public S val2
		{get; protected set;}
	}
}
