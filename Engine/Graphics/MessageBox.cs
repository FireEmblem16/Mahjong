using System;
using System.Collections.Generic;
using TheGame.Utility;

namespace TheGame.Graphics
{
	/// <summary>
	/// Delivers a message...through time and space.
	/// </summary>
	public class MessageBox
	{
		/// <summary>
		/// Creates a message box.
		/// </summary>
		/// <param name="msg">The message to write.</param>
		/// <param name="area">The area to write in.</param>
		public MessageBox(string msg = "", Rectangle area = null)
		{
			Message = msg;
			Bounds = area;

			return;
		}

		/// <summary>
		/// Delivers the message.
		/// </summary>
		/// <param name="win">The window to draw on.</param>
		public void Show(Window win)
		{
			// Hide the cursor
			bool visible = Console.CursorVisible;
			Console.CursorVisible = false;

			// Set the color
			win.SetColor(ConsoleColor.White,ConsoleColor.Black);

			// Temporary bounds for convenience
			Rectangle bounds = Bounds == null? new Rectangle(0,0,win.Width,win.Height) : Bounds;

			// Clear our writing area
			win.ClearBox(bounds);
			win.CursorPosition = new Pair<int,int>(bounds.Left,bounds.Top);
			
			// Some state variables
			bool quoted = false, dquoted = false, tildaed = false;
			
			for(int i = 0;true;i++)
			{
				// Check if we should print the end statement
				if(i == Message.Length)
				{
					win.CursorPosition = new Pair<int,int>(bounds.Left + bounds.Width - 25,bounds.Top + bounds.Height - 1);
					win.SetColor(ConsoleColor.White,ConsoleColor.Black);

					win.Write("Press Enter to return...");
					WaitForInput(ConsoleKey.Enter);

					win.ClearBox(bounds);
					break;
				}

				// Check if we have printed a full page
				if(win.CursorY == bounds.Top + bounds.Height - 1)
				{
					win.CursorPosition = new Pair<int,int>(bounds.Left + bounds.Width - 27,bounds.Top + bounds.Height - 1);
					win.SetColor(ConsoleColor.White,ConsoleColor.Black);

					win.Write("Press Enter to continue...");
					WaitForInput(ConsoleKey.Enter);

					win.ClearBox(bounds);
					win.CursorPosition = new Pair<int,int>(bounds.Left,bounds.Top);
				}
				else // Print more
				{
					switch(Message[i])
					{
					case '[':
						win.SetColor(ConsoleColor.Green,ConsoleColor.Black);
						break;
					case '{':
						win.SetColor(ConsoleColor.Blue,ConsoleColor.Black);
						break;
					case '\"':
						if(dquoted)
						{
							win.SetColor(ConsoleColor.White,ConsoleColor.Black);

							dquoted = false;
							quoted = false;
							tildaed = false;
						}
						else
						{
							win.SetColor(ConsoleColor.Red,ConsoleColor.Black);

							dquoted = true;
							quoted = false;
							tildaed = false;
						}

						break;
					case '\'':
						if(quoted)
						{
							win.SetColor(ConsoleColor.White,ConsoleColor.Black);

							dquoted = false;
							quoted = false;
							tildaed = false;
						}
						else
						{
							win.SetColor(ConsoleColor.Magenta,ConsoleColor.Black);

							dquoted = false;
							quoted = true;
							tildaed = false;
						}

						break;
					case '~':
						if(tildaed)
						{
							win.SetColor(ConsoleColor.White,ConsoleColor.Black);

							dquoted = false;
							quoted = false;
							tildaed = false;
						}
						else
						{
							win.SetColor(ConsoleColor.Yellow,ConsoleColor.Black);

							dquoted = false;
							quoted = false;
							tildaed = true;
						}

						break;
					case ']':
					case '}':
						win.SetColor(ConsoleColor.White,ConsoleColor.Black);
						break;
					case '`':
						win.Write("\'");
						break;
					case '\n':
						win.CursorY = win.CursorY + 1;
						win.CursorX = bounds.Left;
						break;
					case '\r':
						break; // Screw that
					default:
						win.Write(Message[i].ToString(),new Pair<int,int>(win.CursorX - bounds.Left,win.CursorY - bounds.Top),bounds);
						break;
					}
				}
			}


			// Restory cursor visibility
			Console.CursorVisible = visible;

			return;
		}

		/// <summary>
		/// Delivers the message.
		/// </summary>
		/// <param name="win">The window to draw on.</param>
		public void ShowBoundless(Window win)
		{
			// Hide the cursor
			bool visible = Console.CursorVisible;
			Console.CursorVisible = false;

			// Set the color
			win.SetColor(ConsoleColor.White,ConsoleColor.Black);

			// Clear our writing area
			win.Clear();
			win.CursorPosition = new Pair<int,int>(0,0);
			
			// Some state variables
			bool quoted = false, dquoted = false, tildaed = false;
			
			for(int i = 0;true;i++)
			{
				// Check if we should print the end statement
				if(i == Message.Length)
				{
					win.CursorPosition = new Pair<int,int>(win.Width - 25,win.Height - 1);
					win.SetColor(ConsoleColor.White,ConsoleColor.Black);

					win.Write("Press Enter to return...");
					WaitForInput(ConsoleKey.Enter);

					win.Clear();
					break;
				}

				// Check if we have printed a full page
				if(win.CursorY == win.Height - 1)
				{
					win.CursorPosition = new Pair<int,int>(win.Width - 27,win.Height - 1);
					win.SetColor(ConsoleColor.White,ConsoleColor.Black);

					win.Write("Press Enter to continue...");
					WaitForInput(ConsoleKey.Enter);

					win.Clear();
					win.CursorPosition = new Pair<int,int>(0,0);
				}
				else // Print more
				{
					switch(Message[i])
					{
					case '[':
						win.SetColor(ConsoleColor.Green,ConsoleColor.Black);
						break;
					case '{':
						win.SetColor(ConsoleColor.Blue,ConsoleColor.Black);
						break;
					case '\"':
						if(dquoted)
						{
							win.SetColor(ConsoleColor.White,ConsoleColor.Black);

							dquoted = false;
							quoted = false;
							tildaed = false;
						}
						else
						{
							win.SetColor(ConsoleColor.Red,ConsoleColor.Black);

							dquoted = true;
							quoted = false;
							tildaed = false;
						}

						break;
					case '\'':
						if(quoted)
						{
							win.SetColor(ConsoleColor.White,ConsoleColor.Black);

							dquoted = false;
							quoted = false;
							tildaed = false;
						}
						else
						{
							win.SetColor(ConsoleColor.Magenta,ConsoleColor.Black);

							dquoted = false;
							quoted = true;
							tildaed = false;
						}

						break;
					case '~':
						if(tildaed)
						{
							win.SetColor(ConsoleColor.White,ConsoleColor.Black);

							dquoted = false;
							quoted = false;
							tildaed = false;
						}
						else
						{
							win.SetColor(ConsoleColor.Yellow,ConsoleColor.Black);

							dquoted = false;
							quoted = false;
							tildaed = true;
						}

						break;
					case ']':
					case '}':
						win.SetColor(ConsoleColor.White,ConsoleColor.Black);
						break;
					case '`':
						win.Write("\'");
						break;
					default:
						win.Write(Message[i].ToString());
						break;
					}
				}
			}


			// Restory cursor visibility
			Console.CursorVisible = visible;

			return;
		}

		/// <summary>
		/// Waits until input is provided.
		/// </summary>
		protected void WaitForInput()
		{
			Console.ReadKey(true);
			return;
		}

		/// <summary>
		/// Waits until specific input is provided.
		/// </summary>
		/// <param name="key">The key to wait for.</param>
		protected void WaitForInput(ConsoleKey key)
		{
			while(Console.ReadKey(true).Key != key);
			return;
		}

		/// <summary>
		/// Waits until specific input is provided.
		/// </summary>
		/// <param name="key">The keys to wait for.</param>
		protected void WaitForInput(List<ConsoleKey> keys)
		{
			while(!keys.Contains(Console.ReadKey(true).Key));
			return;
		}

		/// <summary>
		/// The message to deliver.
		/// </summary>
		public string Message
		{get; set;}

		/// <summary>
		/// The bounds to draw in.
		/// </summary>
		public Rectangle Bounds
		{get; set;}
	}
}
