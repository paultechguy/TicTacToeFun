# Tic-Tac-Toe Fun

This is just a tiny little project I did recently to simulate playing a tic-tac-toe (TTT) game.  Normally a TTT game is played using a 3x3 grid; this program will automatically play the game between two computer generated players on any size grid that you specify on the command-line.  By default it will play as many parallel games as your computer can suppport.

* Command-line accepts game grid size (e.g. 12)
* Plays multiple games in parallel until a winner if found
* Winning *seed* value is provided if you want to repeat the same game
* Displays total games played before a winner was found
* Displays total game time until a winning game was found

Be careful as larger game grid sizes may take a long time to detect a winning game, depending on the number of cores on your computer.  My recommendation is to start small, say, 10, and work the grid size up until you grow impatient waiting for game completion.

*Command-Line Help*

	.\TicTacToeFun.exe --help


*Example Output*

	.\TicTacToeFun.exe --grid 18
	Starting seed: 137560196
	Thead count: 8
	Player 1 wins after 310 move(s)
	 1 1 1 2 1 1 2 2 2 1 2 1 1 2 1 2 2 2
	 2 1 2 1 2 1 1 1 2 1 2 2 2 2 2 2 1 2
	 1 1 2 1 2 1 2 1 2 1 1 1 1 2 1 2 2 1
	 2 2 2 1 2 1 1 1 1 1 1 2 2 2 2 2 2 2
	 1 2 1 1 1 1 1 1 2 2 2 2 1 2 2 2 1 2
	 1 1 1 2 2 1 1 2 1 - 2 2 1 2 1 2 2 1
	 2 - 2 2 2 2 1 2 2 - 1 1 2 1 2 1 1 -
	 2 2 2 2 - 2 1 1 2 1 2 2 1 2 1 2 1 2
	 1 2 2 2 2 - 1 2 1 2 1 2 1 2 1 2 2 1
	 1 1 1 1 1 2 1 2 1 1 1 1 2 2 1 2 1 2
	 1 1 - 1 1 1 1 2 2 2 - 2 2 2 2 2 2 2
	 2 2 1 1 2 2 2 2 2 2 1 2 1 1 2 2 1 2
	 1 2 2 1 1 1 1 2 1 2 2 2 2 - 2 1 1 1
	 2 1 2 - 2 1 1 2 2 2 2 2 1 2 1 1 1 2
	 1 1 2 1 2 2 2 2 1 1 2 1 2 1 2 2 2 1
	 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1
	 2 1 1 2 2 1 2 1 1 - - 1 1 2 1 1 1 2
	 1 - 2 2 2 2 1 1 2 1 2 1 2 - 1 2 1 1

	We played 8071 game(s) before a winner was found.
	Total duration, 00:00:13.8080797