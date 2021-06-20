namespace TicTacToeFun
{
	public class GameMoveResult
	{
		public TicTacToe Game { get; set; }

		public GamePlayerType Winner { get; set; }

		public int Moves { get; set; }

		public GameMoveResult()
		{
			this.Winner = GamePlayerType.None;
		}

		public GameMoveResult(TicTacToe game)
		{
			this.Winner = GamePlayerType.None;
		}
	}
}
