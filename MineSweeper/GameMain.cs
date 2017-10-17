using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineSweeper
{
	public class GameMain
	{
		private bool playing = false;
		
		public int Row { get; private set; }
		public int Column { get; private set; }

		public int MineCount { get; set; }

        /// <summary>
        /// 获取一个值，指示游戏是否结束。
        /// </summary>
        /// <value>一个值，指示游戏是否结束。</value>
        public bool IsGameOver => !this.playing;

		/// <summary>
		/// 获取或设置游戏蒙板。
		/// </summary>
		/// <value>游戏蒙板。</value>
		public bool?[,] mask { get; private set; }
		/// <summary>
		/// 获取或设置游戏地图。
		/// </summary>
		/// <value>游戏地图。</value>
		public sbyte[,] maps { get; private set; }

		public GameMain(int row, int column)
		{
			if (row < 0) throw new ArgumentOutOfRangeException(nameof(row), row, string.Format("{0}不能小于0。", nameof(row)));
			if (column < 0) throw new ArgumentOutOfRangeException(nameof(column), column, string.Format("{0}不能小于0。", nameof(column)));

			this.Row = row;
			this.Column = column;

			this.MineCount = (this.Row * this.Column) / 5;
			this.GameStart();
		}

		public void GameStart()
		{
			this.maps = new sbyte[this.Row, this.Column];
			this.mask = new bool?[this.Row, this.Column];
			this.MakeMines(this.MineCount);

			this.playing = true;
		}

		private void MakeMines(int count)
		{
			Random r = new Random();
			int index = 0;
			while (index < count)
			{
				int i = r.Next(0, this.Row * this.Column);
				int column = i % this.Column;
				int row = (i - column) / this.Column;
				if (this.maps[row, column] == 0)
				{
					this.maps[row, column] = -1;
					index++;
				}
			}

			this.MakeHint();
		}

		private void MakeHint()
		{
			for (int row = 0; row < this.Row; row++)
				for (int column = 0; column < this.Column; column++)
				{
					if (this.maps[row, column] == -1) continue;

					sbyte count = 0;
					for (int _row = row - 1; _row <= row + 1; _row++)
						for (int _column = column - 1; _column <= column + 1; _column++)
						{
							if (_row == row && _column == column) continue;

							if (
								((_row >= 0 && _row < this.Row) && (_column >= 0 && _column < this.Column)) &&
								this.maps[_row, _column] == -1
							)
								count++;
						}

					this.maps[row, column] = count;
				}
		}

		public void Flag(int row, int column)
		{
			if (!this.playing) return;
			
			if (this.mask[row, column] == null)
				this.mask[row, column] = false;
			else if (this.mask[row, column] == false)
				this.mask[row, column] = null;
		}

		public void Dig(int row, int column, bool area)
		{
			if (!this.playing) return;
			
			if (area ^ this.mask[row, column] != null) return;

			if (this.maps[row, column] == -1) this.GameOver(row, column);

			if (area)
			{
				this.mask[row, column] = true;

				if (this.maps[row, column] == 0) return;
				else
				{
					int count = 0;
					for (int _row = row - 1; _row <= row + 1; _row++)
						for (int _column = column - 1; _column <= column + 1; _column++)
						{
							if (_row == row && _column == column) continue;

							if (
								((_row >= 0 && _row < this.Row) && (_column >= 0 && _column < this.Column)) &&
								((this.maps[_row, _column] == -1 && this.mask[_row, _column] == true) ||
									this.mask[_row, _column] == false)
							)
								count++;
						}

					if (this.maps[row, column] == count)
					{
						for (int _row = row - 1; _row <= row + 1; _row++)
							for (int _column = column - 1; _column <= column + 1; _column++)
							{
								if (_row == row && _column == column) continue;

								if (!((_row >= 0 && _row < this.Row) && (_column >= 0 && _column < this.Column))) continue;

								if (this.maps[_row, _column] == -1)
								{
									if (this.mask[_row, _column] == null) this.GameOver(_row, _column);
								}
								else
								{
									if (this.mask[_row, _column] == false) this.GameOver(_row, _column);
									else this.DigInternal(_row, _column);
								}
							}
					}
				}
			}
			else
			{
				this.DigInternal(row, column);
			}

			if (this.playing && this.CheckWin()) this.GameWin();
		}

		private void DigInternal(int row, int column)
		{
			if (this.mask[row, column] != null) return;
			if (this.maps[row, column] > 0) this.mask[row, column] = true;
			else if (this.maps[row, column] == 0)
			{
				this.mask[row, column] = true;

				for (int _row = row - 1; _row <= row + 1; _row++)
					for (int _column = column - 1; _column <= column + 1; _column++)
					{
						if (_row == row && _column == column) continue;

						if ((_row >= 0 && _row < this.Row) && (_column >= 0 && _column < this.Column))
							DigInternal(_row, _column);
					}
			}
		}

		private void GameOver(int row, int column)
		{
			for (int _row = 0; _row < this.Row; _row++)
				for (int _column = 0; _column < this.Column; _column++)
				{
					this.mask[_row, _column] = true;
				}

			this.playing = false;
		}

		private bool CheckWin()
		{
			for (int row = 0; row < this.Row; row++)
				for (int column = 0; column < this.Column; column++)
				{
					if (this.maps[row, column] >= 0 && this.mask[row, column] != true) return false;
				}

			return true;
		}

		private void GameWin()
		{
			for (int _row = 0; _row < this.Row; _row++)
				for (int _column = 0; _column < this.Column; _column++)
				{
					if (this.maps[_row, _column] == -1) this.mask[_row, _column] = false;
				}

			this.playing = false;
		}
	}
}
