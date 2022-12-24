using System;

namespace BattleBoatsProject
{
    class BattleBoatsProject
    {

        static int BoardSize = 8;
        static int[] boatLengths = { 2, 2, 3, 4 };


        static void Main(string[] args)
        {

            int[,,][] renderBoard = new int[64, 80, 4][];

            Renderer.intro(renderBoard);

            Console.ReadLine();

            int choice = Menu(renderBoard);

            if (choice == 0) { NewGame(renderBoard); }
            if (choice == 1) { ResumeGame(); }
            if (choice == 2) { Instructions(); }
            if (choice == 3) { Quit(); return; }


            return;

            Game currentGame = new Game();

            return;

            //create a new array to store the players boatrs and fill it with -
            char[,] playerBoatsArray = NewArray();

            //get the player to place the boats
            playerBoatsArray = PlayerBoatsPlacement(playerBoatsArray);

            //create a new array to store the computers boats
            char[,] computerBoatsArray = NewArray();

            //generate the computers boats
            computerBoatsArray = GenerateBoats(computerBoatsArray);

            //display for debug purposes
            //CreatePlacementView(computerBoatsArray);
            //Console.ReadLine();

            //create arrays to store each players shots
            char[,] playerShotsArray = NewArray();
            char[,] computerShotsArray = NewArray();

            //show the game board
            createDebugView(playerBoatsArray, playerShotsArray, computerBoatsArray, computerShotsArray);

            //declare a boolean to check if the game is over
            bool winner = false;

            //variables for the computer to decide its moves
            bool compSearching = false;
            int[] compLastShot = new int[2] { -1, -1 };
            int compDirection = 0;


            while (!winner)
            {
                ComputerTurn.Turn(computerShotsArray, playerBoatsArray, ref compSearching, ref compLastShot, ref compDirection);

                createDebugView(playerBoatsArray, playerShotsArray, computerBoatsArray, computerShotsArray);

                winner = CheckWin(computerShotsArray);
                if (winner)
                {
                    Console.Clear();
                    Console.WriteLine("The computer wins :(");
                    Console.ReadLine();
                    return;
                }

                PlayerTurn(playerShotsArray, computerBoatsArray, playerBoatsArray, computerShotsArray);

                winner = CheckWin(playerShotsArray);
                if (winner)
                {
                    Console.Clear();
                    Console.WriteLine("You win! :)");
                    Console.ReadLine();
                    return;
                }
            }

            Console.ReadLine();
        }


        static int Menu(int[,,][] renderBoard)
        {
            Renderer.menuInit(renderBoard);

            int selection = 0;
            Renderer.menuOption(renderBoard, selection);

            ConsoleKey key = new ConsoleKey();

            while ((key = Console.ReadKey().Key) != ConsoleKey.Enter)
            {
                if (key == ConsoleKey.UpArrow && selection > 0)
                {
                    selection--;
                }
                if (key == ConsoleKey.DownArrow && selection < 3)
                {
                    selection++;
                }
                Renderer.menuOption(renderBoard, selection);
                System.Console.WriteLine(selection);
            }

            return selection;
        }
        static void NewGame(int[,,][] renderBoard)
        {
            Game newGame = new Game(8);


        }
        static void ResumeGame() { }
        static void PlayGame() { }
        static void Instructions() { }
        static void Quit()
        {
            Console.Clear();
            System.Environment.Exit(1);
        }
        static char[,] NewArray(int Size, char Symbol)
        {
            char[,] returnArray = new char[Size, Size];
            for (int i = 0; i < returnArray.GetLength(0); i++)
            {
                for (int j = 0; j < returnArray.GetLength(1); j++)
                {
                    returnArray[j, i] = Symbol;
                }
            }

            return returnArray;
        }
        static void CreatePlacementView(char[,] board)
        {
            Console.Clear();
            //outputs the grid
            Console.WriteLine("╔══════════════════╗");
            Console.WriteLine("║ place your boats ║");
            Console.WriteLine("║    ╔════════╗    ║");
            Console.WriteLine("║    ║++++++++║    ║");
            Console.WriteLine("║    ║++++++++║    ║");
            Console.WriteLine("║    ║++++++++║    ║");
            Console.WriteLine("║    ║++++++++║    ║");
            Console.WriteLine("║    ║++++++++║    ║");
            Console.WriteLine("║    ║++++++++║    ║");
            Console.WriteLine("║    ║++++++++║    ║");
            Console.WriteLine("║    ║++++++++║    ║");
            Console.WriteLine("║    ╚════════╝    ║");
            Console.WriteLine("╚══════════════════╝");

            //loops through the gameboard
            for (int X = 0; X < board.GetLength(0); X++)
            {
                for (int Y = 0; Y < board.GetLength(1); Y++)
                {
                    //if there is a boat
                    if (board[X, Y] == '#')
                    {
                        //place a red +
                        Console.SetCursorPosition(X + 6, Y + 3);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("+");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }
        static char[,] PlayerBoatsPlacement(char[,] board)
        {
            //loops through each boat
            for (int i = 0; i < boatLengths.Length; i++)
            {
                //output the board
                CreatePlacementView(board);
                //gets the boats length
                int boatLength = boatLengths[i];

                int boatX = 0;
                int boatY = 0;
                bool boatR = false;
                bool overlap = true;

                //loops as long as the enter key isnt pressed
                ConsoleKey key = new ConsoleKey();
                while ((key = Console.ReadKey().Key) != ConsoleKey.Enter || overlap == true)
                {
                    int tempboatX = boatX;
                    int tempboatY = boatY;
                    bool tempboatR = boatR;

                    //navigate with the arrow keys
                    if (key == ConsoleKey.R) { tempboatR = !boatR; }
                    else if (key == ConsoleKey.RightArrow) { tempboatX++; }
                    else if (key == ConsoleKey.LeftArrow) { tempboatX--; }
                    else if (key == ConsoleKey.UpArrow) { tempboatY--; }
                    else if (key == ConsoleKey.DownArrow) { tempboatY++; }

                    //check that the placement isnt out of bounds
                    if (tempboatX >= 0 && tempboatR && tempboatX + boatLength <= BoardSize) { boatX = tempboatX; }
                    else if (tempboatX >= 0 && !tempboatR && tempboatX <= 7) { boatX = tempboatX; }
                    if (tempboatY >= 0 && !tempboatR && tempboatY + boatLength <= BoardSize) { boatY = tempboatY; }
                    else if (tempboatY >= 0 && tempboatR && tempboatY <= 7) { boatY = tempboatY; }

                    if (!boatR && boatX < 9 - boatLength) { boatR = tempboatR; }
                    else if (boatR && boatY < 9 - boatLength) { boatR = tempboatR; }

                    //check that the placement doesnt overlap with another boat
                    overlap = false;
                    for (int j = 0; j < boatLength; j++)
                    {
                        if (boatR) { if (board[boatX + j, boatY] == '#') { overlap = true; } }
                        else { if (board[boatX, boatY + j] == '#') { overlap = true; } }
                    }

                    //output the board
                    CreatePlacementView(board);

                    //draw the current boat position on the board
                    for (int j = 0; j < boatLength; j++)
                    {
                        //place a blue +
                        if (boatR) { Console.SetCursorPosition(boatX + 6 + j, boatY + 3); }
                        else { Console.SetCursorPosition(boatX + 6, boatY + 3 + j); }
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("+");
                        Console.ForegroundColor = ConsoleColor.White;

                        //put the cursor back
                        Console.SetCursorPosition(0, 12);
                    }

                }

                for (int j = 0; j < boatLength; j++)
                {
                    if (boatR) { board[boatX + j, boatY] = '#'; }
                    else { board[boatX, boatY + j] = '#'; }
                }

            }
            return board;
        }
        static char[,] GenerateBoats(char[,] board)
        {
            Random random = new Random();
            for (int i = 0; i < boatLengths.Length; i++)
            {
                bool valid = false;

                int boatX = 0;
                int boatY = 0;
                bool boatR = true;
                int boatLength = boatLengths[i];


                while (!valid)
                {
                    boatR = ((int)random.Next(0, 2)) == 0;
                    if (boatR)
                    {
                        boatX = random.Next(0, BoardSize - boatLength);
                        boatY = random.Next(0, 7);
                    }
                    else
                    {
                        boatX = random.Next(0, 7);
                        boatY = random.Next(0, BoardSize - boatLength);
                    }

                    valid = true;
                    for (int j = 0; j < boatLength; j++)
                    {
                        if ((boatR && board[boatX + j, boatY] == '#') || (!boatR && board[boatX, boatY + j] == '#'))
                        {
                            valid = false;
                        }
                    }
                }
                for (int j = 0; j < boatLength; j++)
                {
                    if (boatR) { board[boatX + j, boatY] = '#'; }
                    else if (!boatR) { board[boatX, boatY + j] = '#'; }
                }

            }
            return board;
        }
        static void createPlayView(char[,] boatsBoard, char[,] shotsBoard)
        {
            Console.Clear();
            //outputs the grid
            Console.WriteLine("╔════════════════════════════╗");
            Console.WriteLine("║    your boats  your shots  ║");
            Console.WriteLine("║    ╔════════╗  ╔════════╗  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ╚════════╝  ╚════════╝  ║");
            Console.WriteLine("╚════════════════════════════╝");

            //loops through the boats board to place boats
            for (int X = 0; X < boatsBoard.GetLength(0); X++)
            {
                for (int Y = 0; Y < boatsBoard.GetLength(1); Y++)
                {
                    //if there is a boat
                    if (boatsBoard[X, Y] == '#')
                    {
                        //place a red +
                        Console.SetCursorPosition(X + 6, Y + 3);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("+");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    //if there is a hit boat
                    if (boatsBoard[X, Y] == '*')
                    {
                        //place a yellow +
                        Console.SetCursorPosition(X + 6, Y + 3);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("+");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }

            //loops through the shots board to place shots
            for (int X = 0; X < shotsBoard.GetLength(0); X++)
            {
                for (int Y = 0; Y < shotsBoard.GetLength(1); Y++)
                {
                    //if there is a miss
                    if (shotsBoard[X, Y] == '~')
                    {
                        //place a cyan ~ for a miss
                        Console.SetCursorPosition(X + 18, Y + 3);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("~");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    //if there is a hit
                    if (shotsBoard[X, Y] == '*')
                    {
                        //place a red # for a hit
                        Console.SetCursorPosition(X + 18, Y + 3);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("#");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }
        static void createDebugView(char[,] boatsBoard, char[,] shotsBoard, char[,] compBoatsBoard, char[,] compShotsBoard)
        {
            Console.Clear();
            //outputs the grid
            Console.WriteLine("╔════════════════════════════╗");
            Console.WriteLine("║    your boats  your shots  ║");
            Console.WriteLine("║    ╔════════╗  ╔════════╗  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ╚════════╝  ╚════════╝  ║");
            Console.WriteLine("╚════════════════════════════╝");
            Console.WriteLine("╔════════════════════════════╗");
            Console.WriteLine("║    comp boats  comp shots  ║");
            Console.WriteLine("║    ╔════════╗  ╔════════╗  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ║++++++++║  ║++++++++║  ║");
            Console.WriteLine("║    ╚════════╝  ╚════════╝  ║");
            Console.WriteLine("╚════════════════════════════╝");

            //loops through the boats board to place boats
            for (int X = 0; X < boatsBoard.GetLength(0); X++)
            {
                for (int Y = 0; Y < boatsBoard.GetLength(1); Y++)
                {
                    //if there is a boat
                    if (boatsBoard[X, Y] == '#')
                    {
                        //place a red +
                        Console.SetCursorPosition(X + 6, Y + 3);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("+");
                        Console.ForegroundColor = ConsoleColor.White;
                    }


                    //if there is a hit boat
                    if (boatsBoard[X, Y] == '*')
                    {
                        //place a red *
                        Console.SetCursorPosition(X + 6, Y + 3);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("*");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }

            //loops through the shots board to place shots
            for (int X = 0; X < shotsBoard.GetLength(0); X++)
            {
                for (int Y = 0; Y < shotsBoard.GetLength(1); Y++)
                {
                    //if there is a miss
                    if (shotsBoard[X, Y] == '~')
                    {
                        //place a cyan ~ for a miss
                        Console.SetCursorPosition(X + 18, Y + 3);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("~");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    //if there is a hit
                    if (shotsBoard[X, Y] == '*')
                    {
                        //place a red # for a hit
                        Console.SetCursorPosition(X + 18, Y + 3);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("#");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }

            //loops through the computers boats board to place boats
            for (int X = 0; X < compBoatsBoard.GetLength(0); X++)
            {
                for (int Y = 0; Y < compBoatsBoard.GetLength(1); Y++)
                {
                    //if there is a boat
                    if (compBoatsBoard[X, Y] == '#')
                    {
                        //place a red +
                        Console.SetCursorPosition(X + 6, Y + 16);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("+");
                        Console.ForegroundColor = ConsoleColor.White;
                    }


                    //if there is a hit boat
                    if (boatsBoard[X, Y] == '*')
                    {
                        //place a yellow +
                        Console.SetCursorPosition(X + 6, Y + 16);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("+");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }

            //loops through the computers shots board to place shots
            for (int X = 0; X < compShotsBoard.GetLength(0); X++)
            {
                for (int Y = 0; Y < compShotsBoard.GetLength(1); Y++)
                {
                    //if there is a miss
                    if (compShotsBoard[X, Y] == '~')
                    {
                        //place a cyan ~ for a miss
                        Console.SetCursorPosition(X + 18, Y + 16);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("~");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    //if there is a hit
                    if (compShotsBoard[X, Y] == '*')
                    {
                        //place a red # for a hit
                        Console.SetCursorPosition(X + 18, Y + 16);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("#");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }
        static void PlayerTurn(char[,] shotsArray, char[,] boatsArray, /*just here for rendering*/ char[,] playerBoatsArray, char[,] computerShotsArray)
        {
            ConsoleKey key = new ConsoleKey();
            int shotX = 0;
            int shotY = 0;

            while ((key = Console.ReadKey().Key) != ConsoleKey.Enter)
            {
                if (key == ConsoleKey.UpArrow && shotY > 0) { shotY--; }
                if (key == ConsoleKey.DownArrow && shotY < 7) { shotY++; }

                if (key == ConsoleKey.LeftArrow && shotX > 0) { shotX--; }
                if (key == ConsoleKey.RightArrow && shotX < 7) { shotX++; }

                createDebugView(playerBoatsArray, shotsArray, boatsArray, computerShotsArray);
                Console.SetCursorPosition(shotX + 18, shotY + 3);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write('+');
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (boatsArray[shotX, shotY] == '#') { shotsArray[shotX, shotY] = '*'; }
            else { shotsArray[shotX, shotY] = '~'; }
        }
        static bool CheckWin(char[,] shotsArray)
        {
            int hitcount = 0;
            int hitTarget = 0;

            for (int i = 0; i < boatLengths.Length; i++)
            {
                hitTarget += boatLengths[i];
            }

            for (int i = 0; i < shotsArray.GetLength(0); i++)
            {
                for (int j = 0; j < shotsArray.GetLength(1); j++)
                {
                    if (shotsArray[i, j] == '*') { hitcount++; }
                }
            }

            if (hitcount >= hitTarget) { return true; }
            else { return false; }
        }
    }

    //contains the algorithm for the computers turns
    class ComputerTurn
    {
        static int BoardSize = 8;
        public static void Turn(char[,] shotsBoard, char[,] boatsBoard, ref bool searching, ref int[] lastShot, ref int direction)
        {
            //if the algorithm currently is tracking a boat
            if (searching)
            {
                //if the location is below 0 it is still searching for the location
                if (direction <= 0 && direction > -5)
                {
                    int[] shot = new int[2];

                    bool valid = false;
                    //get the next valid shot working anticlockwise round the current boat
                    do
                    {
                        direction--;
                        shot = DirectionToCoord(lastShot, direction * -1);

                        valid = validShot(shotsBoard, shot[0], shot[1]) != 1;
                    }
                    while (valid && direction != -5);

                    //if all directions have been tested and there is no boat, take a random shot
                    if (direction == -5)
                    {
                        randomShot(shotsBoard, boatsBoard, ref searching, ref lastShot);
                        direction = 0;
                        return;
                    }

                    //check if the shot hits and mark the arrays appropriately
                    else if (checkHit(boatsBoard, shot[0], shot[1]))
                    {
                        direction = direction * -1;
                        shotsBoard[shot[0], shot[1]] = '*';
                        boatsBoard[shot[0], shot[1]] = '*';
                        lastShot = shot;
                    }
                    else
                    {
                        shotsBoard[shot[0], shot[1]] = '~';
                    }
                }
                //if the direction of the boat is known
                else
                {
                    bool flipped = false;
                    bool foundShot = false;
                    //move in the direction
                    int[] shot = DirectionToCoord(lastShot, direction);

                    while (!foundShot)
                    {
                        //if there is an already shot boat in the way, skip over it
                        if (validShot(shotsBoard, shot[0], shot[1]) == 3)
                        {
                            shot = DirectionToCoord(shot, direction);
                            continue;
                        }
                        //if the shot is not valid for any other reason, the end of the boat has been reached to head in the opposite direction
                        else if (validShot(shotsBoard, shot[0], shot[1]) != 1 && !flipped)
                        {
                            //makes it only go back and forth once to prevent infinite loop
                            flipped = true;

                            if (direction < 3) { direction += 2; }
                            else if (direction > 2) { direction -= 2; }

                            shot = DirectionToCoord(shot, direction);
                            continue;
                        }
                        //if its a valid shot take the shot and mark boards appropriately with hit or miss
                        else if (validShot(shotsBoard, shot[0], shot[1]) == 1)
                        {
                            foundShot = true;
                            if (checkHit(boatsBoard, shot[0], shot[1]))
                            {
                                shotsBoard[shot[0], shot[1]] = '*';
                                boatsBoard[shot[0], shot[1]] = '*';
                            }
                            else
                            {
                                shotsBoard[shot[0], shot[1]] = '~';
                            }
                        }
                        //if none of these work (it you have flipped and there is no valid shot) take a random shot
                        else
                        {
                            randomShot(shotsBoard, boatsBoard, ref searching, ref lastShot);
                            direction = 0;
                            searching = false;
                            return;
                        }
                    }
                }
            }
            else
            {
                //if the algorithm is not tracking a boat shoot randomly
                randomShot(shotsBoard, boatsBoard, ref searching, ref lastShot);
            }
        }


        //checks if the shot is a hit or not
        static bool checkHit(char[,] boatsBoard, int X, int Y)
        {
            if (boatsBoard[X, Y] == '#')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //checks if the shot is valid, returns 1 for valid, 0 for out of bounds, 2 for already a miss or 3 for already a hit
        static int validShot(char[,] shotsBoard, int X, int Y)
        {
            if (X > 7 || X < 0 || Y > 7 || Y < 0) { return -1; }
            if (shotsBoard[X, Y] == '~') { return 2; }
            if (shotsBoard[X, Y] == '*') { return 3; }
            if (shotsBoard[X, Y] == '-') { return 1; }
            else { return -1; }
        }

        //generates and takes a valid random shot
        static void randomShot(char[,] shotsBoard, char[,] boatsBoard, ref bool searching, ref int[] lastShot)
        {
            Random random = new Random();

            int X = 0;
            int Y = 0;

            do
            {
                X = random.Next(0, BoardSize);
                Y = random.Next(0, BoardSize);
            }
            while (validShot(shotsBoard, X, Y) != 1);

            if (checkHit(boatsBoard, X, Y))
            {
                shotsBoard[X, Y] = '*';
                boatsBoard[X, Y] = '*';

                searching = true;
            }
            else
            {
                shotsBoard[X, Y] = '~';
            }

            lastShot = new int[] { X, Y };
        }

        //generates new coordinates from old coordinates and a direction
        static int[] DirectionToCoord(int[] lastShot, int direction)
        {
            if (direction == 1) { return new int[] { lastShot[0] + 1, lastShot[1] }; }
            else if (direction == 2) { return new int[] { lastShot[0], lastShot[1] + 1 }; }
            else if (direction == 3) { return new int[] { lastShot[0] - 1, lastShot[1] }; }
            else if (direction == 4) { return new int[] { lastShot[0], lastShot[1] - 1 }; }
            else { return new int[] { -1, -1 }; }
        }
    }

    class Renderer
    {
        static void Render(int[,,][] renderBoard)
        {
            int layers = renderBoard.GetLength(2);
            int width = renderBoard.GetLength(1);
            int height = renderBoard.GetLength(0);

            int[,][] flatBoard = new int[height, width][];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int layer = 0;
                    for (int k = 0; k < layers; k++)
                    {
                        if (renderBoard[i, j, k] != null && renderBoard[i, j, k][3] != 0)
                        {
                            layer = k;
                        }
                    }
                    flatBoard[i, j] = renderBoard[i, j, layer];
                    //System.Console.WriteLine(layer);
                    //Console.ReadLine();
                    //Console.Write("\x1b[48;2;" + pixel[0] + ";" + pixel[1] + ";" + pixel[2] + "m  \u001b[0m");
                }
            }

            if (height % 2 == 1) { Console.WriteLine("screen must be an even number of pixels high"); return; }

            string output = "";
            for (int i = 0; i < height; i += 2)
            {
                for (int j = 0; j < width; j++)
                {

                    int[] topPixel = flatBoard[i, j];
                    int[] bottomPixel = flatBoard[i + 1, j];
                    output += ("\x1b[48;2;" + topPixel[0] + ";" + topPixel[1] + ";" + topPixel[2] + "m" + "\x1b[38;2;" + bottomPixel[0] + ";" + bottomPixel[1] + ";" + bottomPixel[2] + "m▄\u001b[0m");
                }
                output += "\n";
            }
            Console.Clear();
            Console.WriteLine(output);

        }
        static int[,][] LoadObject(string Filename)
        {
            BinaryReader Br = new BinaryReader(File.Open(("Assets/" + Filename), FileMode.Open));

            int imgWidth = Br.ReadInt32();
            int imgHeight = Br.ReadInt32();

            int[,][] ObjectArray = new int[imgWidth, imgHeight][];

            for (int i = 0; i < imgWidth; i++)
            {
                for (int j = 0; j < imgHeight; j++)
                {
                    byte R = Br.ReadByte();
                    byte G = Br.ReadByte();
                    byte B = Br.ReadByte();
                    byte A = Br.ReadByte();
                    ObjectArray[i, j] = new int[] { R, G, B, A };
                }
            }
            Br.Close();

            return ObjectArray;
        }
        static void PlaceObject(int[,][] Object, int[,,][] renderBoard, int X, int Y, int Layer)
        {
            int width = Object.GetLength(1);
            int height = Object.GetLength(0);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i + Y > renderBoard.GetLength(0) || j + X > renderBoard.GetLength(1)) { continue; }
                    renderBoard[i + Y, j + X, Layer] = Object[i, j];
                }
            }

        }
        static void ClearLayer(int[,,][] renderBoard, int layer)
        {
            for (int i = 0; i < renderBoard.GetLength(0); i++)
            {
                for (int j = 0; j < renderBoard.GetLength(1); j++)
                {
                    renderBoard[i, j, layer] = new int[] { 0, 0, 0, 0 };
                }
            }
        }
        static void ClearAllObj(int[,,][] renderBoard)
        {
            ClearLayer(renderBoard, 1);
            ClearLayer(renderBoard, 2);
            ClearLayer(renderBoard, 3);
        }
        public static void intro(int[,,][] renderBoard)
        {
            Console.WriteLine("make the console full screen then hit Enter");
            Console.ReadLine();

            int[,][] background = LoadObject("Background.bin");
            PlaceObject(background, renderBoard, 0, 0, 0);
            Render(renderBoard);

            Console.WriteLine("if the image takes up the whole console try zooming out");
            Console.ReadLine();

            int[,][] title = LoadObject("Title.bin");
            PlaceObject(title, renderBoard, 12, 12, 2);


            int[,][] titleBorder = LoadObject("TitleBorder.bin");
            PlaceObject(titleBorder, renderBoard, 9, 9, 1);

            Render(renderBoard);
        }
        public static void menuInit(int[,,][] renderBoard)
        {
            ClearAllObj(renderBoard);
            PlaceObject(LoadObject("menu.bin"), renderBoard, 9, 12, 1);

            Render(renderBoard);
        }
        public static void menuOption(int[,,][] renderBoard, int selection)
        {
            ClearLayer(renderBoard, 2);

            if (selection == 0) { PlaceObject(LoadObject("menu newgame.bin"), renderBoard, 12, 15, 2); ; }
            if (selection == 1) { PlaceObject(LoadObject("menu resumegame.bin"), renderBoard, 12, 21, 2); }
            if (selection == 2) { PlaceObject(LoadObject("menu instructions.bin"), renderBoard, 12, 27, 2); }
            if (selection == 3) { PlaceObject(LoadObject("menu quit.bin"), renderBoard, 12, 33, 2); }

            Render(renderBoard);
        }
        public static void splashText(int[,,][] renderBoard, string file, int X, int Y)
        {
            ClearLayer(renderBoard, 3);

            PlaceObject(LoadObject(file), renderBoard, X, Y, 3);

            Render(renderBoard);
        }
    }

    //create a struct to store the games state
    struct Game
    {
        public char[,] PlayerBoats;
        public char[,] PlayerShots;
        public char[,] ComputerBoats;
        public char[,] ComputerShots;
        public bool compSearching = false;
        public int[] compLastShot = new int[2] { -1, -1 };
        public int compDirection = 0;
        public bool PlayerTurn = true;
        public string fileName = "newGame.bin";

        public Game(int BoardSize)
        {
            PlayerBoats = new char[BoardSize, BoardSize];
            PlayerShots = new char[BoardSize, BoardSize];
            ComputerBoats = new char[BoardSize, BoardSize];
            ComputerShots = new char[BoardSize, BoardSize];
        }
    }
}

// \x18[#C;1m

// ~ means missed shot
// * means hit shot
// - means blank
// # means unhit boat
// @ means hit boat