namespace TicTacToeFun
{
	using CommandLine;

	public class CommandLineOptions
	{
		[Option('g', "grid", Required = true, HelpText = "Size of the game grid.")]
		public int GridSize { get; set; }

		[Option('t', "threads", Required = false, Default = 0, HelpText = "Number of parallel threads; use 0 for maximum.")]
		public int Threads{ get; set; }

		[Option('s', "seed", Required = false, Default = 0, HelpText = "Seed for starting a repeatable game.")]
		public int StartSeed { get; set; }
	}
}
