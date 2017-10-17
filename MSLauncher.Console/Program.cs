#define GAME

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MineSweeper;

namespace MSLauncher.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			GameMain game = new GameMain(20, 25);

			while (true)
			{
				System.Console.Clear();

				System.Console.ForegroundColor = ConsoleColor.Yellow;
				System.Console.Write(string.Empty.PadLeft(3));
				for (int column = 0; column < game.Column; column++)
					System.Console.Write((column + 1).ToString().PadLeft(3));
				System.Console.WriteLine();

				for (int row = 0; row < game.Row; row++)
				{
					System.Console.ForegroundColor = ConsoleColor.Yellow;
					System.Console.Write((row + 1).ToString().PadLeft(3));
					for (int column = 0; column < game.Column; column++)
					{
						System.Console.ResetColor();

#if GAME
						if (game.mask[row, column] == true)
						{
							string str;
							if (game.maps[row, column] == -1)
							{
								System.Console.ForegroundColor = ConsoleColor.Red;
								str = "*".PadLeft(3);
							}
							else
							{
								System.Console.ForegroundColor = ConsoleColor.Green;

								if (game.maps[row, column] == 0)
									str = string.Empty.PadLeft(3);
								else
									str = game.maps[row, column].ToString().PadLeft(3);
							}

							System.Console.Write(str);
						}
						else if (game.mask[row, column] == null)
						{
							System.Console.ForegroundColor = ConsoleColor.White;

							System.Console.Write("□".PadLeft(2));
						}
						else if (game.mask[row, column] == false)
						{
							System.Console.ForegroundColor = ConsoleColor.Red;

							System.Console.Write("！".PadLeft(2));
						}
#else
					if (game.maps[row, column] == -1)
						System.Console.ForegroundColor = ConsoleColor.Red;
					else if (game.maps[row, column] == 0)
						System.Console.ForegroundColor = ConsoleColor.Gray;
					else
						System.Console.ForegroundColor = ConsoleColor.Green;

					System.Console.Write(game.maps[row, column].ToString().PadLeft(3));
#endif
					}
					System.Console.WriteLine();
				}


				System.Console.WriteLine();
				System.Console.ForegroundColor = ConsoleColor.Blue;
				System.Console.Write("MS>>");

				System.Console.ForegroundColor = ConsoleColor.Yellow;
				string line = System.Console.ReadLine();

				string[] ss = line.Split(new[] { ' ', '\t', '　' }, StringSplitOptions.RemoveEmptyEntries);
				if (ss.Length == 1 && ss[0].ToLower() == "newgame")
				{
					game = new GameMain(game.Row, game.Column);
				}
				else
				{
					if (ss.Length != 3) continue;
					else
					{
						int[] position = new int[2];
						bool f = true;
						for (int i = 0; i < position.Length; i++)
						{
							try { position[i] = int.Parse(ss[i + 1]); }
							catch (Exception)
							{
								f = false;
								break;
							}
						}
						if (ss[0].ToLower() != "newgame" && ss[0].ToLower() != "ng")
						{
							if (position[0] <= 0 || position[0] > game.Row) f = false;
							if (position[1] <= 0 || position[1] > game.Column) f = false;
						}

						if (f == false) continue;
						else
						{
							switch (ss[0].ToLower())
							{
								case "d":
								case "dig":
									game.Dig(position[0] - 1, position[1] - 1, false);
									break;
								case "da":
								case "digarea":
									game.Dig(position[0] - 1, position[1] - 1, true);
									break;
								case "f":
								case "flag":
									game.Flag(position[0] - 1, position[1] - 1);
									break;
								case "ng":
								case "newgame":
									game = new GameMain(position[0], position[1]);
									break;
								default:
									f = false;
									break;
							}

							if (f == false) continue;
						}
					}
				}

				System.Console.ResetColor();
			}

			System.Console.ReadLine();
		}
	}
}
