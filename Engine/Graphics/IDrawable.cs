using TheGame.Utility;

namespace TheGame.Graphics
{
	/// <summary>
	/// Contains the basic definition of a drawable object.
	/// </summary>
	public interface IDrawable
	{
		/// <summary>
		/// Draws the object.
		/// This draw call is garunteed not to change the current window settings (i.e. cursor color, cursor position, etc).
		/// <param name="win">The window to draw on.</param>
		/// </summary>
		void Draw(Window win);

		/// <summary>
		/// Draws the object.
		/// This draw call is garunteed not to change the current window settings (i.e. cursor color, cursor position, etc).
		/// </summary>
		/// <param name="win">The window to draw on.</param>
		/// <param name="pos">The position to draw at.</param>
		void Draw(Window win, Pair<int,int> pos);

		/// <summary>
		/// The position of the object
		/// </summary>
		Pair<int,int> Position
		{get; set;}

		/// <summary>
		/// The x position of the object
		/// </summary>
		int X
		{get; set;}

		/// <summary>
		/// The y position of the object
		/// </summary>
		int Y
		{get; set;}
	}
}
