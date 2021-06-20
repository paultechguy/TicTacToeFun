namespace TicTacToeFun
{
	using System;
	using System.Linq;

	public class TicTacToe
	{
		// handy lil' guy for private and public
		public readonly int BoardSize;

		// board is a set of tuples; the player (0,1) and has it been played (bool)
		private readonly (GamePlayerType Player, bool Played)[,] board;

		// misc
		private delegate (GamePlayerType Player, bool Played)[] DirectionalWinStrategy(int columnNumber);

		/// <summary>
		/// Created a Tic Tac Tow game board
		/// </summary>
		/// <param name="n">nxn dimension for the game board</param>
		public TicTacToe(int n)
		{
			this.BoardSize = n;
			this.board = new (GamePlayerType, bool)[n, n]; // initialized as all false values
		}

		/// <summary>
		/// Place a piece on the game board
		/// </summary>
		/// <param name="row">row to place a piece</param>
		/// <param name="col">column to place a piece</param>
		/// <param name="player">the player (1 or 2) the piece is for</param>
		/// <returns>0 = no winner, 1 = player 1 won, 2 = player 2 won</returns>
		public GameMoveResult PlacePiece(int row, int col, GamePlayerType player)
		{
			// ok, we'll do this simple error check
			if (player == GamePlayerType.None)
			{
				throw new InvalidOperationException($"Found invalid {nameof(player)}, {player}");
			}

			(GamePlayerType Player, bool Played) cell = this.board[row, col]; // like I said, no error checking :-)

			// make sure this cell is not already played
			if (cell.Played)
			{
				throw new InvalidOperationException($"Cell already played, row:{row}, col:{col}");
			}
			else if (cell.Player != GamePlayerType.None)
			{
				throw new InvalidOperationException($"Invalid cell state, indicates already played by player {cell.Player}");
			}

			// play this cell
			board[row, col].Played = true;
			board[row, col].Player = player;

			// find out if this player just won
			GamePlayerType winner = this.DetermineIfWon(player);

			return new GameMoveResult
			{
				Game = this,
				Winner = winner,
				Moves = 0, // caller is responsible
			};
		}

		public void PrintGame()
		{
			// KISS
			for (int row = 0; row < BoardSize; row++)
			{
				for (int col = 0; col < BoardSize; col++)
				{
					var cell = this.board[row, col];
					string cellState = cell.Played ? $" {(int)cell.Player}" : " -";
					Console.Write(cellState);
				}

				// new row
				Console.WriteLine();
			}
		}

		private GamePlayerType DetermineIfWon(GamePlayerType player)
		{
			// check each direction;  if horizonal wins, the vertical statement should short-circuit and not call the check
			bool winner = this.CheckDirectionForWin(this.GetRow, player) != GamePlayerType.None;
			winner |= this.CheckDirectionForWin(this.GetCol, player) != GamePlayerType.None;
			winner |= this.CheckDiagonalForWin(player) != GamePlayerType.None;

			return winner ? player : GamePlayerType.None;
		}

		private GamePlayerType CheckDiagonalForWin(GamePlayerType player)
		{
			// top-left to bottom-right
			if (Enumerable.Range(0, this.BoardSize)
				.Select(x => this.board[x, x])
				.Where(x => x.Played && x.Player == player)
				.Count() == this.BoardSize)
			{
				return player;
			}

			// top-right to bottom-left
			if (Enumerable.Range(0, this.BoardSize)
				.Select(x => this.board[this.BoardSize - 1 - x, x])
				.Where(x => x.Played && x.Player == player)
				.Count() == this.BoardSize)
			{
				return player;
			}

			return GamePlayerType.None;
		}

		private GamePlayerType CheckDirectionForWin(DirectionalWinStrategy strategy, GamePlayerType player)
		{
			for (int i = 0; i < this.BoardSize; i++)
			{
				// if all cells in this direction are played by player, getty-up!
				if (strategy(i)
					.Where(r => r.Player == player & r.Played)
					.Count() == this.BoardSize)
				{
					return player; // I win
				}
			}

			return GamePlayerType.None;
		}

		private (GamePlayerType Player, bool Played)[] GetCol(int columnNumber)
		{
			return Enumerable.Range(0, this.BoardSize)
					.Select(x => this.board[x, columnNumber])
					.ToArray();
		}

		private (GamePlayerType Player, bool Played)[] GetRow(int rowNumber)
		{
			return Enumerable.Range(0, this.BoardSize)
					.Select(x => this.board[rowNumber, x])
					.ToArray();
		}
	}
}
