using System;
using System.Collections.Generic;
using TheGame.Utility;

namespace TheGame.Graphics
{
	/// <summary>
	/// A popup menu of sorts that allows the user to select one of a list of options.
	/// </summary>
	public class Menu
	{
		/// <summary>
		/// Creates a new menu.
		/// </summary>
		/// <param name="title">The title of the menu to use.</param>
		/// <param name="items">The times that should be displayed.</param>
		/// <param name="selected_index">The index to select.</param>
		/// <param name="num_items">The number of items the menu will display</param>
		public Menu(string title, IEnumerable<string> items, int selected_index, int num_items)
		{
			Title = title;
			Items = new List<string>(items);
			ToolTips = null;

			SelectedIndex = selected_index;
			ItemsDisplayed = num_items;
			TopIndex = SelectedIndex - ItemsDisplayed / 2;

			if(Items.Count - TopIndex < ItemsDisplayed)
				TopIndex -= ItemsDisplayed - (Items.Count - TopIndex);

			if(TopIndex < 0)
				TopIndex = 0;

			TextColor = ConsoleColor.White;
			BackColor = ConsoleColor.Black;
			CursorColor = ConsoleColor.Magenta;

			IsDisplaying = false;
			return;
		}

		/// <summary>
		/// Creates a new menu.
		/// </summary>
		/// <param name="title">The title of the menu to use.</param>
		/// <param name="items">The times that should be displayed.</param>
		/// <param name="tooltips">The tooltips for the menu items.</param>
		/// <param name="selected_index">The index to select.</param>
		/// <param name="num_items">The number of items the menu will display</param>
		public Menu(string title, IEnumerable<string> items, IEnumerable<string> tooltips, int selected_index, int num_items)
		{
			Title = title;
			Items = new List<string>(items);
			ToolTips = new List<string>(tooltips);

			SelectedIndex = selected_index;
			ItemsDisplayed = num_items;
			TopIndex = SelectedIndex - ItemsDisplayed / 2;

			if(Items.Count - TopIndex < ItemsDisplayed)
				TopIndex -= ItemsDisplayed - (Items.Count - TopIndex);

			if(TopIndex < 0)
				TopIndex = 0;

			TextColor = ConsoleColor.White;
			BackColor = ConsoleColor.Black;
			CursorColor = ConsoleColor.Magenta;

			IsDisplaying = false;
			return;
		}

		/// <summary>
		/// Creates a new menu.
		/// </summary>
		/// <param name="title">The title of the menu to use.</param>
		/// <param name="items">The times that should be displayed.</param>
		/// <param name="tooltips">The tooltips for the menu items.</param>
		/// <param name="selected_index">The index to select.</param>
		/// <param name="num_items">The number of items the menu will display</param>
		/// <param name="top_index">The top index to use for the menu. The user is responsible for making sure this value makes sense.</param>
		public Menu(string title, IEnumerable<string> items, IEnumerable<string> tooltips, int selected_index, int num_items, int top_index)
		{
			Title = title;
			Items = new List<string>(items);
			ToolTips = new List<string>(tooltips);
			
			SelectedIndex = selected_index;
			ItemsDisplayed = num_items;
			TopIndex = top_index;

			TextColor = ConsoleColor.White;
			BackColor = ConsoleColor.Black;
			CursorColor = ConsoleColor.Magenta;

			IsDisplaying = false;
			return;
		}

		/// <summary>
		/// Displays the menu at the given coordinates.
		/// </summary>
		/// <param name="win">The window to use to display this menu.</param>
		/// <param name="x">The x coordinate to begin displaying the menu at.</param>
		/// <param name="y">The y coordinate to begin displaying the menu at.</param>
		/// <returns>Returns the index of the item selected by the user or -1 if no item is selected.</returns>
		/// <exception cref="ArgumentException">Thrown if the display location is out of bounds.</exception>
		public int Display(Window win, int x, int y)
		{
			if(x < 0 || x >= win.Width || y < 0 || y >= win.Height)
				throw new ArgumentException("Display coordinates out of bounds.");
			
			IsDisplaying = true;

			bool old_cursor_visible = Console.CursorVisible;
			Console.CursorVisible = false;

			win.CursorPosition = new Pair<int,int>(x,y);
			win.SetColor(TextColor,BackColor);
			
			win.Write(Title);

			int longest_str = Width;
			string underscore = "";

			for(int i = 0;i < longest_str;i++)
				underscore += "-";

			win.CursorPosition = new Pair<int,int>(x,y+1);
			win.Write(underscore);

			// Get the first key input
			ConsoleKeyInfo next_key;
			InputType key_type = InputType.OTHER;

			while(key_type != InputType.TERMINATE_LINE && key_type != InputType.ESCAPE)
			{
				WriteMenuItems(win,TopIndex,ItemsDisplayed,x,y + 2);

				next_key = Console.ReadKey(true);
				key_type = DetermineInputType(next_key);

				if(key_type == InputType.ARROW)
					switch(next_key.Key)
					{
					case ConsoleKey.UpArrow:
						if(SelectedIndex != 0)
						{
							if(SelectedIndex == TopIndex)
								TopIndex--;

							SelectedIndex--;
						}
						else
						{
							SelectedIndex = Items.Count - 1;
							TopIndex = Items.Count - ItemsDisplayed;

							if(TopIndex < 0)
								TopIndex = 0;
						}

						break;
					case ConsoleKey.DownArrow:
						if(SelectedIndex != Items.Count - 1)
						{
							if(SelectedIndex == TopIndex + ItemsDisplayed - 1)
								TopIndex++;

							SelectedIndex++;
						}
						else
						{
							SelectedIndex = 0;
							TopIndex = 0;
						}

						break;
					}
			}
			
			IsDisplaying = false;
			win.ClearBox(new Rectangle(x,y,longest_str,ItemsDisplayed + 2));

			Console.CursorVisible = old_cursor_visible;

			if(Items.Count == 0)
				return -1;

			if(key_type == InputType.ESCAPE)
				return -1;

			return SelectedIndex;
		}

		/// <summary>
		/// Displays the menu at the given coordinates with tooltips if they are given.
		/// </summary>
		/// <param name="win">The window to use to display this menu.</param>
		/// <param name="x">The x coordinate to begin displaying the menu at.</param>
		/// <param name="y">The y coordinate to begin displaying the menu at.</param>
		/// <param name="tips_area">The area to write the tooltips in. Writing will begin at the top left position.</param>
		/// <returns>Returns the index of the item selected by the user or -1 if no item is selected.</returns>
		/// <exception cref="ArgumentException"></exception>
		public int DisplayWithTips(Window win, int x, int y, Rectangle tips_area)
		{
			// If we have no tool tips just use the other function
			if(ToolTips == null)
				return Display(win,x,y);

			if(x < 0 || x >= win.Width || y < 0 || y >= win.Height)
				throw new ArgumentException("Display coordinates out of bounds.");
			
			IsDisplaying = true;

			bool old_cursor_visible = Console.CursorVisible;
			Console.CursorVisible = false;

			win.CursorPosition = new Pair<int,int>(x,y);
			win.SetColor(TextColor,BackColor);
			
			win.Write(Title);

			int longest_str = Width;
			string underscore = "";

			for(int i = 0;i < longest_str;i++)
				underscore += "-";

			win.CursorPosition = new Pair<int,int>(x,y+1);
			win.Write(underscore);

			// Get the first key input
			ConsoleKeyInfo next_key;
			InputType key_type = InputType.OTHER;

			while(key_type != InputType.TERMINATE_LINE && key_type != InputType.ESCAPE)
			{
				WriteMenuItems(win,TopIndex,ItemsDisplayed,x,y + 2);

				win.ClearBox(tips_area);
				win.Write(ToolTips[SelectedIndex],new Pair<int,int>(0,0),tips_area);

				next_key = Console.ReadKey(true);
				key_type = DetermineInputType(next_key);

				if(key_type == InputType.ARROW)
					switch(next_key.Key)
					{
					case ConsoleKey.UpArrow:
						if(SelectedIndex != 0)
						{
							if(SelectedIndex == TopIndex)
								TopIndex--;

							SelectedIndex--;
						}
						else
						{
							SelectedIndex = Items.Count - 1;
							TopIndex = Items.Count - ItemsDisplayed;

							if(TopIndex < 0)
								TopIndex = 0;
						}

						break;
					case ConsoleKey.DownArrow:
						if(SelectedIndex != Items.Count - 1)
						{
							if(SelectedIndex == TopIndex + ItemsDisplayed - 1)
								TopIndex++;

							SelectedIndex++;
						}
						else
						{
							SelectedIndex = 0;
							TopIndex = 0;
						}

						break;
					}
			}
			
			IsDisplaying = false;

			win.ClearBox(tips_area);
			win.ClearBox(new Rectangle(x,y,longest_str,ItemsDisplayed + 2));

			Console.CursorVisible = old_cursor_visible;

			if(Items.Count == 0)
				return -1;

			if(key_type == InputType.ESCAPE)
				return -1;

			return SelectedIndex;
		}

		/// <summary>
		/// Writes the menu items to be displayed.
		/// </summary>
		/// <param name="win">The window to display to.</param>
		/// <param name="top">The index of the first item to display.</param>
		/// <param name="num">The number of items to display.</param>
		/// <param name="x">The x coordinate to begin displaying at.</param>
		/// <param name="y">The y coordinate to begin displaying at.</param>
		protected void WriteMenuItems(Window win, int top, int num, int x, int y)
		{
			win.ClearBox(new Rectangle(x,y,LongestLength(),num));

			for(int i = 0;i < num;i++)
			{
				if(top + i >= Items.Count)
					break;

				if(top + i == SelectedIndex)
				{
					win.CursorPosition = new Pair<int,int>(x,y + i);
					win.SetColor(TextColor,CursorColor);
				}
				else
				{
					win.CursorPosition = new Pair<int,int>(x,y + i);
					win.SetColor(TextColor,BackColor);
				}

				win.Write(Items[top + i]);
			}

			return;
		}

		/// <summary>
		/// Determines the input type of the provided key stroke.
		/// </summary>
		/// <param name="key">The key stroke to consider.</param>
		/// <returns>Returns the type of input that the key stroke is.</returns>
		protected InputType DetermineInputType(ConsoleKeyInfo key)
		{
			if((key.Modifiers & (ConsoleModifiers.Control | ConsoleModifiers.Alt)) != 0)
				return InputType.COMMAND;

			if(key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.RightArrow)
				return InputType.ARROW;

			if(key.Key == ConsoleKey.Home)
				return InputType.HOME;

			if(key.Key == ConsoleKey.End)
				return InputType.END;

			if(key.KeyChar == '\u0000')
				return InputType.OTHER;

			if(key.Key == ConsoleKey.Tab)
				return InputType.AUTO_COMPLETE;

			if(key.Key == ConsoleKey.Backspace)
				return InputType.BACKSPACE;

			if(key.Key == ConsoleKey.Enter)
				return InputType.TERMINATE_LINE;

			if(key.Key == ConsoleKey.Escape)
				return InputType.ESCAPE;

			return InputType.TEXT;
		}

		/// <summary>
		/// Returns the length of the longest item (or title) in the menu.
		/// </summary>
		/// <returns>Returns the length of the longest item to be displayed.</returns>
		protected int LongestLength()
		{
			int max = Title.Length;

			foreach(string str in Items)
				if(str.Length > max)
					max = str.Length;

			return max;
		}

		/// <summary>
		/// Represents the type of key that is a character represents.
		/// </summary>
		protected enum InputType
		{
			COMMAND = 0,
			TEXT = 1,
			BACKSPACE = 2,
			AUTO_COMPLETE = 3,
			TERMINATE_LINE = 4,
			ARROW = 5,
			ESCAPE = 6,
			HOME = 7,
			END = 8,
			OTHER = 9
		}

		/// <summary>
		/// The title of the menu.
		/// </summary>
		public string Title
		{get; set;}

		/// <summary>
		/// The items being displayed by this menu.
		/// </summary>
		public List<string> Items
		{get; protected set;}

		/// <summary>
		/// Tooltips for the items.
		/// </summary>
		public List<string> ToolTips
		{get; protected set;}

		/// <summary>
		/// If true then the menu is currently being displayed.
		/// </summary>
		public bool IsDisplaying
		{get; protected set;}

		/// <summary>
		/// The index of the currently selected item.
		/// </summary>
		public int SelectedIndex
		{
			get
			{return selected_index;}

			protected set
			{
				if(value < 0 || value >= Items.Count)
					return;

				selected_index = value;
				return;
			}
		}

		/// <summary>
		/// The number of items displayed at a time by the menu.
		/// </summary>
		public int ItemsDisplayed
		{get; protected set;}


		public int TopIndex
		{get; protected set;}

		/// <summary>
		/// How many characters the menu is wide at it's widest point.
		/// </summary>
		public int Width
		{
			get
			{return LongestLength();}
		}

		/// <summary>
		/// The color to write text with.
		/// </summary>
		public ConsoleColor TextColor
		{get; set;}

		/// <summary>
		/// The color to use for the background of text.
		/// </summary>
		public ConsoleColor BackColor
		{get; set;}

		/// <summary>
		/// The highlight to use (in the background) to indicate the selected item.
		/// </summary>
		public ConsoleColor CursorColor
		{get; set;}

		/// <summary>
		/// Contains the index of the selected item.
		/// </summary>
		protected int selected_index = 0;
	}
}
