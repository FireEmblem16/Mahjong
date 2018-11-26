using System;
using System.Collections.Generic;
using TheGame.Utility;

namespace TheGame.Graphics
{
	/// <summary>
	/// A wrapper class that manages console graphics.
	/// </summary>
	public class Window
	{
		/// <summary>
		/// Constructs a new window with the given dimensions.
		/// </summary>
		/// <param name="title">The title to put into the window title bar.</param>
		/// <param name="x">The x position to put the window at relative to the screen buffer.</param>
		/// <param name="y">The y position to put the window at relative to the screen buffer.</param>
		/// <param name="width">The width of the window.</param>
		/// <param name="height">The height of the window.</param>
		public Window(string title, int width, int height, int x = 0, int y = 0)
		{
			Console.TreatControlCAsInput = true;
			
			Title = title;
			SetWindowPosition(x,y);
			Width = width;
			Height = height;

			settings = new Stack<WindowSettings>();
			return;
		}

		/// <summary>
		/// Moves the window to the specificed position relative to the screen buffer.
		/// </summary>
		/// <param name="x">The x position to put the window at.</param>
		/// <param name="y">The y position to put the window at.</param>
		public void SetWindowPosition(int x, int y)
		{
			Console.SetWindowPosition(x,y);
			return;
		}

		/// <summary>
		/// Writes the given string to the window.
		/// </summary>
		/// <param name="str">The string to write.</param>
		public void Write(string str)
		{
			Console.Write(str);
			return;
		}

		/// <summary>
		/// Writes the given string to the window.
		/// </summary>
		/// <param name="str">The string to write.</param>
		/// <param name="position">The position to write the string at.</param>
		public void Write(string str, Pair<int,int> position)
		{
			CursorPosition = position;
			Write(str);

			return;
		}

		/// <summary>
		/// Writes the given string inside the area starting at the top left.
		/// For convienence all tabs will be replaced with 5 spaces.
		/// </summary>
		/// <param name="str">The string to write.</param>
		/// <param name="area">The box to write in.</param>
		/// <remarks>If the string is longer than can fit into the box the extra characters will be truncated.</remarks>
		public void Write(string str, Rectangle area)
		{
			Write(str,new Pair<int,int>(0,0),area);
			return;
		}

		/// <summary>
		/// Writes the given string inside the area at the given position relative to the top left of the area.
		/// For convienence all tabs will be replaced with 5 spaces.
		/// </summary>
		/// <param name="str">The string to write.</param>
		/// <param name="pos">The position to start at in the box.</param>
		/// <param name="area">The box to write in.</param>
		/// <remarks>If the string is longer than can fit into the box the extra characters will be truncated.</remarks>
		public void Write(string str, Pair<int,int> pos, Rectangle area)
		{
			if(!area.IsInside(new Pair<int,int>(pos.val1 + area.Position.val1,pos.val2 + area.Position.val2)))
				return;

			CursorPosition = new Pair<int,int>(pos.val1 + area.Position.val1,pos.val2 + area.Position.val2);

			Scanner lines = new Scanner(str.Replace("\t","     "),new string[] {"\n"});

			if(!lines.HasNextItem)
				return;

			// First line/line-fragment is special
			string first = lines.Next<string>();
			string next = null; // Use null instead of empty string due to empty lines

			if(first.Length + pos.val1 >= area.Width)
			{
				Write(first.Substring(0,area.Width - pos.val1));
				next = first.Substring(area.Width - pos.val1);
			}
			else
				Write(first); // next is already null

			while(next != null && CursorY < area.Top + area.Height - 1) // Minus one since we move the cursor after entering the loop
			{
				// If we are at position zero for x then we did a wrap around on the console
				if(CursorX == 0)
					CursorPosition = new Pair<int,int>(area.Left,CursorY);
				else
					CursorPosition = new Pair<int,int>(area.Left,CursorY + 1);

				if(next.Length > area.Width)
				{
					Write(next.Substring(0,area.Width));
					next = next.Substring(area.Width);
				}
				else
				{
					Write(next);
					next = null;

					// If we still have stuff to write and we want to write an empty line we may have to drop the line here
					if(next == "" && CursorX == 0 && lines.HasNextItem)
						CursorY++;
				}
			}

			bool last_line_empty = false;

			while(lines.HasNextItem)
			{
				next = lines.Next<string>();
				
				while(next != null && CursorY < area.Top + area.Height - 1)
				{
					// If we are at position zero for x then we did a wrap around on the console
					if(CursorX == 0 && !last_line_empty)
						CursorPosition = new Pair<int,int>(area.Left,CursorY);
					else
						CursorPosition = new Pair<int,int>(area.Left,CursorY + 1);

					last_line_empty = false;

					if(next.Length > area.Width)
					{
						Write(next.Substring(0,area.Width));
						next = next.Substring(area.Width);
					}
					else
					{
						last_line_empty = next == "";

						Write(next);
						next = null;
					}
				}
			}

			return;
		}

		/// <summary>
		/// Clears the given rectangle to the current foreground and background color.
		/// Cursor position will be maintained. A line return will be forced on the buffer if the last character in the buffer is written to or cleared.
		/// </summary>
		/// <param name="r">The box to clear. It is assumed that the box is valid.</param>
		public void ClearBox(Rectangle r)
		{
			Pair<int,int> current = CursorPosition;

			CursorPosition = new Pair<int,int>(r.Left,r.Top);
			SetColor(ConsoleColor.White,ConsoleColor.Black);

			string clear_line = "";

			for(int i = 0;i < r.Width;i++)
				clear_line += " ";

			for(int i = 0;i < r.Height;i++)
			{
				CursorPosition = new Pair<int,int>(r.Left,r.Top + i);
				Write(clear_line);
			}

			CursorPosition = current;
			return;
		}

		/// <summary>
		/// Clears the window.
		/// </summary>
		public void Clear()
		{
			Console.Clear();
			return;
		}

		/// <summary>
		/// Returns the position of the cursor if moved back one space when typing.
		/// </summary>
		/// <returns>Returns the previous position of the cursor.</returns>
		public Pair<int,int> GetLastCursorPosition()
		{
			return new Pair<int,int>(GetLastCursorX(),GetLastCursorY());
		}

		/// <summary>
		/// Returns the position of the cursor if moved back one space when typing withing a subset of the window.
		/// </summary>
		/// <param name="bounds">The subset of the window to consider.</param>
		/// <returns>Returns the previous position of the cursor.</returns>
		public Pair<int,int> GetLastCursorPosition(Rectangle bounds)
		{
			return new Pair<int,int>(GetLastCursorX(bounds),GetLastCursorY(bounds));
		}

		/// <summary>
		/// Returns what the cursor position would be after the specified number of strokes.
		/// </summary>
		/// <param name="strokes">The number of strokes that would be preformed.</param>
		/// <returns>Returns the cursor position advanced by the given number.</returns>
		public Pair<int,int> GetCursorPositionAfter(int strokes)
		{
			int new_x = CursorX + strokes;
			int new_y = CursorY;

			while(new_x >= Width)
			{
				new_x -= Width;
				new_y++;
			}

			return new Pair<int,int>(new_x,new_y);
		}

		/// <summary>
		/// Returns what the cursor position would be after the specified number of strokes starting from the specified location.
		/// </summary>
		/// <param name="strokes">The number of strokes that would be preformed.</param>
		/// <param name="x">The x position to start the cursor from.</param>
		/// <param name="y">The y position to start the cursor from.</param>
		/// <returns>Returns the cursor position advanced by the given number.</returns>
		public Pair<int,int> GetCursorPositionAfter(int strokes, int x, int y)
		{
			int new_x = x + strokes;
			int new_y = y;

			while(new_x >= Width)
			{
				new_x -= Width;
				new_y++;
			}

			return new Pair<int,int>(new_x,new_y);
		}

		/// <summary>
		/// Returns what the cursor position would be after the specified number of strokes within a specified subwindow.
		/// </summary>
		/// <param name="strokes">The number of strokes that would be preformed.</param>
		/// <param name="bounds">The subwindow to consider.</param>
		/// <returns>Returns the cursor position advanced by the given number.</returns>
		public Pair<int,int> GetCursorPositionAfter(int strokes, Rectangle bounds)
		{
			int new_x = CursorX + strokes;
			int new_y = CursorY;

			while(new_x >= bounds.Width + bounds.Left)
			{
				new_x -= bounds.Width;
				new_y++;
			}

			return new Pair<int,int>(new_x,new_y);
		}

		/// <summary>
		/// Returns what the cursor position would be after the specified number of strokes within a specified subwindow starting from the specified location.
		/// </summary>
		/// <param name="strokes">The number of strokes that would be preformed.</param>
		/// <param name="bounds">The subwindow to consider.</param>
		/// <param name="x">The x position to start the cursor from.</param>
		/// <param name="y">The y position to start the cursor from.</param>
		/// <returns>Returns the cursor position advanced by the given number.</returns>
		public Pair<int,int> GetCursorPositionAfter(int strokes, Rectangle bounds, int x, int y)
		{
			int new_x = x + strokes;
			int new_y = y;

			while(new_x >= bounds.Width + bounds.Left)
			{
				new_x -= bounds.Width;
				new_y++;
			}

			return new Pair<int,int>(new_x,new_y);
		}

		/// <summary>
		/// Returns the number of strokes required to reach the given end point from the given start point.
		/// The start point is inclusive and the end point is exclusive.
		/// </summary>
		/// <param name="start">The point to start at.</param>
		/// <param name="end">The point to end at.</param>
		/// <returns>Returns the number of strokes required to get from the start to the end.</returns>
		public int GetStrokeDifference(Pair<int,int> start, Pair<int,int> end)
		{
			if(end.val2 == start.val2)
				return end.val1 - start.val1;

			if(end.val2 < start.val2)
				return (end.val2 - start.val2 + 1) * Width + (Width - end.val1) + start.val1;
				
			return (end.val2 - start.val2 - 1) * Width + end.val1 + (Width - start.val1);
		}

		/// <summary>
		/// Returns the number of strokes required to reach the given end point from the given start point within a given subwindow.
		/// The start point is inclusive and the end point is exclusive.
		/// </summary>
		/// <param name="start">The point to start at.</param>
		/// <param name="end">The point to end at.</param>
		/// <param name="bounds">The subwindow to consider.</param>
		/// <returns>Returns the number of strokes required to get from the start to the end.</returns>
		public int GetStrokeDifference(Pair<int,int> start, Pair<int,int> end, Rectangle bounds)
		{
			if(end.val2 == start.val2)
				return end.val1 - start.val1;

			if(end.val2 < start.val2)
				return (end.val2 - start.val2 + 1) * bounds.Width - (bounds.Width - (end.val1 - bounds.Left)) - (start.val1 - bounds.Left);
				
			return (end.val2 - start.val2 - 1) * bounds.Width + (end.val1 - bounds.Left) - (bounds.Width - (start.val1 - bounds.Left));
		}

		/// <summary>
		/// Returns the x position of the cursor if moved back one space when typing.
		/// </summary>
		/// <returns>Returns the x position of the previous cursor position.</returns>
		protected int GetLastCursorX()
		{
			if(CursorX == 0 && CursorY == 0)
				return 0;

			if(CursorX > 0)
				return CursorX - 1;

			return Width - 1;
		}

		/// <summary>
		/// Returns the x position of the cursor if moved back one space when typing within a subset of the window.
		/// </summary>
		/// <param name="bounds">The subset of the window to consider.</param>
		/// <returns>Returns the x position of the previous cursor position.</returns>
		protected int GetLastCursorX(Rectangle bounds)
		{
			if(CursorX == bounds.Left && CursorY == bounds.Top)
				return bounds.Left;

			if(CursorX > bounds.Left)
				return CursorX - 1;
			
			return bounds.Width - 1;
		}

		/// <summary>
		/// Returns the y position of the cursor if moved back one space when typing.
		/// </summary>
		/// <returns>Returns the y position of the previous cursor position.</returns>
		protected int GetLastCursorY()
		{
			if(CursorX == 0 && CursorY != 0)
				return CursorY - 1;

			return CursorY;
		}

		/// <summary>
		/// Returns the y position of the cursor if moved back one space when typing within a subset of the window.
		/// </summary>
		/// <param name="bounds">The subset of the window to consider.</param>
		/// <returns>Returns the y position of the previous cursor position.</returns>
		protected int GetLastCursorY(Rectangle bounds)
		{
			if(CursorX == bounds.Left && CursorY != bounds.Top)
				return CursorY - 1;

			return CursorY;
		}

		/// <summary>
		/// Sets the current window color for new output.
		/// </summary>
		/// <param name="fore">The foreground color.</param>
		/// <param name="back">The background color.</param>
		public void SetColor(ConsoleColor fore, ConsoleColor back)
		{
			Console.ForegroundColor = fore;
			Console.BackgroundColor = back;

			return;
		}

		/// <summary>
		/// Sets the visibility of the cursor.
		/// </summary>
		/// <param name="visible">If true, the cursor will be visible. If false, it will not be.</param>
		public void SetVisible(bool visible = true)
		{
			Console.CursorVisible = visible;
			return;
		}

		/// <summary>
		/// Saves the current window settings to the stack to be restored later.
		/// </summary>
		public void PushSettings()
		{
			settings.Push(new WindowSettings(CursorPosition,Width,Height,Console.CursorVisible,Console.ForegroundColor,Console.BackgroundColor));
			return;
		}

		/// <summary>
		/// Restores the most recently saved window settings (if any).
		/// </summary>
		/// <returns>Returns true if settings were restored and false otherwise.</returns>
		public bool PopSettings()
		{
			if(settings.Count == 0)
				return false;

			WindowSettings ws = settings.Pop();

			CursorPosition = ws.Position;
			
			Width = ws.Width;
			Height = ws.Height;

			Console.CursorVisible = ws.Visible;

			SetColor(ws.ForegroundColor,ws.BackgroundColor);
			return true;
		}

		/// <summary>
		/// The title of the window.
		/// </summary>
		public string Title
		{
			get
			{return Console.Title;}
			
			set
			{
				Console.Title = value;
				return;
			}
		}

		/// <summary>
		/// The position of the cursor in the window.
		/// </summary>
		public Pair<int,int> CursorPosition
		{
			get
			{return new Pair<int,int>(CursorX,CursorY);}

			set
			{
				CursorX = value.val1;
				CursorY = value.val2;

				return;
			}
		}

		/// <summary>
		/// The x position of the cursor in the window.
		/// </summary>
		public int CursorX
		{
			get
			{return Console.CursorLeft;}

			set
			{
				if(value < 0)
					Console.CursorLeft = 0;
				else if(value >= Width)
					Console.CursorLeft = Width - 1;
				else
					Console.CursorLeft = value;

				return;
			}
		}

		/// <summary>
		/// The y position of the cursor in the window.
		/// </summary>
		public int CursorY
		{
			get
			{return Console.CursorTop;}

			set
			{
				if(value < 0)
					Console.CursorTop = 0;
				else if(value >= Height)
					Console.CursorTop = Height - 1;
				else
					Console.CursorTop = value;

				return;
			}
		}

		/// <summary>
		/// The width of the window.
		/// </summary>
		public int Width
		{
			get
			{return w;}
			
			set
			{
				if(value > Console.LargestWindowWidth)
					w = Console.LargestWindowWidth;
				else if(value < 1)
					w = 1;
				else
					w = value;

				Console.SetWindowSize(Width,Height);
				Console.SetBufferSize(Width,Height);
				return;
			}
		}

		/// <summary>
		/// How window width is recorded.
		/// </summary>
		protected int w = 1;

		/// <summary>
		/// The height of the window.
		/// </summary>
		public int Height
		{
			get
			{return h;}

			set
			{
				if(value > Console.LargestWindowHeight)
					h = Console.LargestWindowHeight;
				else if(value < 1)
					h = 1;
				else
					h = value;

				Console.SetWindowSize(Width,Height);
				Console.SetBufferSize(Width,Height);
				return;
			}
		}

		/// <summary>
		/// How window height is recorded.
		/// </summary>
		protected int h = 1;

		/// <summary>
		/// A stack of window settings.
		/// </summary>
		protected Stack<WindowSettings> settings;

		/// <summary>
		/// Contains a complete copy of window settings.
		/// </summary>
		protected class WindowSettings
		{
			public WindowSettings(Pair<int,int> position, int width, int height, bool visible, ConsoleColor fore, ConsoleColor back)
			{
				Position = new Pair<int,int>(position);
				
				Width = width;
				Height = height;

				Visible = visible;

				ForegroundColor = fore;
				BackgroundColor = back;

				return;
			}

			public Pair<int,int> Position
			{get; protected set;}

			public int Width
			{get; protected set;}

			public int Height
			{get; protected set;}

			public bool Visible
			{get; protected set;}

			public ConsoleColor ForegroundColor
			{get; protected set;}

			public ConsoleColor BackgroundColor
			{get; protected set;}
		}
	}
}
