namespace TheGame.Utility
{
	/// <summary>
	/// A simple rectangle class with width, height, left and top.
	/// </summary>
	public class Rectangle
	{
		public Rectangle(int l, int t, int width, int height)
		{
			Left = l;
			Top = t;
			Width = width;
			Height = height;

			return;
		}

		public bool IsInside(int x, int y)
		{
			if(x < Left || x >= Left + Width || y < Top || y >= Top + Height)
				return false;

			return true;
		}

		public bool IsInside(Pair<int,int> loc)
		{return IsInside(loc.val1,loc.val2);}

		public Pair<int,int> Position
		{
			get
			{return new Pair<int,int>(Left,Top);}
		}

		public int Width;
		public int Height;
		public int Left;
		public int Top;
	}
}
