namespace TicTacToeFun
{
	using CommandLine;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	class Program
	{
		private readonly int EXIT_SUCCESS = 0;
		private readonly int EXIT_FAIL = 1;

		// this lil guy is just a hack to ensure the game print to the console
		// isn't mixed with other threads returning at the same time
		private object gamePrintSync = new object();

		static int Main(string[] args)
		{
			int exit = 1;
			ParserResult<CommandLineOptions> result = Parser.Default.ParseArguments<CommandLineOptions>(args)
			.WithParsed(options =>  // options is an instance of Options type
			{
				exit = new Program().Run(options);
			})
			.WithNotParsed(errors =>  // errors is a sequence of type IEnumerable<Error>
			{
				exit = 0;
			});

			return exit;
		}

		private int Run(CommandLineOptions options)
		{
			try
			{
				CheckOptions(options);

				int gameCount = 0;
				DateTime startTime = DateTime.Now;
				gameCount = this.PlayGames(options, gameCount);
				TimeSpan durationTime = DateTime.Now - startTime;

				Console.WriteLine($"We played {gameCount} game(s) before a winner was found.");
				Console.WriteLine($"Total duration, {durationTime}\n");

				return EXIT_SUCCESS;
			}
			catch(Exception ex)
			{
				Console.Error.WriteLine($"Exception detected, {ex}");
			}

			return EXIT_FAIL;
		}

		private static void CheckOptions(CommandLineOptions options)
		{
			if (options.GridSize < 3)
			{
				throw new InvalidOperationException("Error: game grid size must be 3 or greater");
			}

			if (options.Threads > Environment.ProcessorCount || options.Threads < 0)
			{
				throw new InvalidOperationException($"Error: thread count must be from 1 to {Environment.ProcessorCount}");
			}
		}

		private int PlayGames(CommandLineOptions options, int gameCount)
		{
			// thread completion and timing may vary, caused by CPU context switching;
			// results should be somewhat similar though, where the game count differs
			// by no more than the processor count

			// double randomization so we can track what seed caused the win;
			// this could be initalized using boardSize to attempt a better repeatable test
			int startingSeed = options.StartSeed > 0 ? options.StartSeed : new Random().Next();
			var random = new Random(startingSeed);


			var cts = new CancellationTokenSource();
			CancellationToken cancelToken = cts.Token;
			var parallelOptions = new ParallelOptions
			{
				MaxDegreeOfParallelism = options.Threads > 0 ?  options.Threads : Environment.ProcessorCount,
			};

			// let folks know some of the starting info
			Console.WriteLine($"Starting seed: {startingSeed}");
			Console.WriteLine($"Thead count: {parallelOptions.MaxDegreeOfParallelism}");

			// threading optimization: during testing, we observed that a 20 board size game
			// took ~61 seconds to complete using a single thread; when we used TPL using
			// the max processors, the game was won in ~17 seconds
			//
			// so paralleliism does appear to help significantly

			int maxTheads = parallelOptions.MaxDegreeOfParallelism;
			int[] threads = Enumerable.Range(0, maxTheads).ToArray();
			Parallel.ForEach(threads, parallelOptions, i =>
			{
				while (true)
				{
					++gameCount; // should be thread-safe

					var gameRandom = new Random(random.Next()); // use a seed for each game
					var game = new TicTacToe(options.GridSize);


					GameMoveResult moveResult = this.PlayRandomGame(game, gameRandom, cancelToken);
					if (moveResult.Winner != GamePlayerType.None)
					{
						// cancel other foreach threads
						cts.Cancel();

						this.PrintGame(moveResult);
					}

					// has the game been cancelled by any thread (including ourselves)
					if (cancelToken.IsCancellationRequested)
					{
						break;
					}
				}
			});

			return gameCount;
		}

		private void PrintGame(GameMoveResult moveResult)
		{
			lock (this.gamePrintSync)
			{
				string winnerName = moveResult.Winner == GamePlayerType.Player1 ? "Laurel" : "Hardy";
				Console.WriteLine($"{winnerName} wins after {moveResult.Moves} move(s)");
				moveResult.Game.PrintGame();
				Console.WriteLine();
			}
		}

		private GameMoveResult PlayRandomGame(TicTacToe game, Random random, CancellationToken cancelToken)
		{
			IEnumerable<int> cells = Enumerable
				.Range(0, game.BoardSize * game.BoardSize)
				.OrderBy(c => random.Next()) // randomize the order we'll play the cells
				.ToArray();

			int moveCount = 0;

			// randomly choose a starting player
			GamePlayerType player = random.Next(0, 2) == 0 ? GamePlayerType.Player1 : GamePlayerType.Player2;

			GameMoveResult moveResult = null;
			foreach (int cell in cells)
			{
				// cancel?
				if (cancelToken.IsCancellationRequested)
				{
					break;
				}

				++moveCount;

				int col = cell % game.BoardSize;
				int row = cell / game.BoardSize;
				moveResult = game.PlacePiece(row, col, player);
				if (moveResult.Winner != GamePlayerType.None)
				{
					moveResult.Moves = moveCount;

					return moveResult;
				}

				// swap players
				player = player == GamePlayerType.Player1 ? GamePlayerType.Player2 : GamePlayerType.Player1;
			}

			// watch for incomplete games caused by cancellation
			if (moveResult is null)
			{
				moveResult = new GameMoveResult(game)
				{
				};
			}

			return moveResult;
		}
	}
}
