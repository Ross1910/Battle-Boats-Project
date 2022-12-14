using System;

namespace BattleBoatsProject
{
    class BattleBoatsProject
    {
        static int[] boatLengths = { 1, 1, 2, 2, 3 };

        static int BoardSize = 8;

        static void Main(string[] args)
        {
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

                createPlayView(playerBoatsArray, computerShotsArray);
                Console.ReadLine();
            }

            Console.ReadLine();
        }

        static char[,] NewArray()
        {
            char[,] returnArray = new char[BoardSize, BoardSize];
            for (int i = 0; i < returnArray.GetLength(0); i++)
            {
                for (int j = 0; j < returnArray.GetLength(1); j++)
                {
                    returnArray[j, i] = '-';
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
                    if (tempboatX >= 0 && tempboatR && tempboatX + boatLength <= 8) { boatX = tempboatX; }
                    else if (tempboatX >= 0 && !tempboatR && tempboatX <= 7) { boatX = tempboatX; }
                    if (tempboatY >= 0 && !tempboatR && tempboatY + boatLength <= 8) { boatY = tempboatY; }
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
                        boatX = random.Next(0, 8 - boatLength);
                        boatY = random.Next(0, 7);
                    }
                    else
                    {
                        boatX = random.Next(0, 7);
                        boatY = random.Next(0, 8 - boatLength);
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

        /*static void ComputerTurn(char[,] shotsArray, char[,] boatsArray)
        {
            bool searching = false;
            int direction = 0;
            int[] lastShot = new int[2];

            while (true)
            {
                Random random = new Random();
                createPlayView(boatsArray, shotsArray);

                int shotX = 0;
                int shotY = 0;
                bool shoot = false;

                if (false)
                {
                    if (direction == 0)
                    {
                        while (!shoot)
                        {
                            direction++;

                            if (direction == 1) { shotX = lastShot[0]; shotY = lastShot[1] + 1; }
                            if (direction == 2) { shotX = lastShot[0] + 1; shotY = lastShot[1]; }
                            if (direction == 3) { shotX = lastShot[0]; shotY = lastShot[1] - 1; }
                            if (direction == 4) { shotX = lastShot[0] - 1; shotY = lastShot[1]; }
                            if (direction == 5)
                            {
                                direction = 0;
                                searching = false;

                                do
                                {
                                    shotX = random.Next(0, 8);
                                    shotY = random.Next(0, 8);

                                    if (shotsArray[shotX, shotY] != '-') { shoot = false; }

                                } while (!shoot);

                                if (boatsArray[shotX, shotY] == '#')
                                {
                                    shotsArray[shotX, shotY] = '*';
                                    searching = true;
                                    lastShot = new int[] { shotX, shotY };
                                }
                                else
                                {
                                    shotsArray[shotX, shotY] = '~';
                                }

                            }

                            if (shotX < 0 || shotX > 7 || shotY < 0 || shotY > 7) { continue; }

                            if (shotsArray[shotX, shotY] != '-') { continue; }

                            shoot = true;
                        }

                        lastShot = new int[] { shotX, shotY };
                        if (boatsArray[shotX, shotY] == '-') { shotsArray[shotX, shotY] = '~'; direction = 0; }
                        else if (boatsArray[shotX, shotY] == '#')
                        {
                            shotsArray[shotX, shotY] = '*';
                        }
                    }
                    else
                    {
                        int counter = 0;

                        int ends = 0;

                        while (!shoot)
                        {
                            if (ends >= 2)
                            {
                                direction = 0;
                                searching = false;

                                do
                                {
                                    shotX = random.Next(0, 8);
                                    shotY = random.Next(0, 8);

                                    if (shotsArray[shotX, shotY] != '-') { shoot = false; }

                                } while (!shoot);

                                if (boatsArray[shotX, shotY] == '#')
                                {
                                    shotsArray[shotX, shotY] = '*';
                                    searching = true;
                                    lastShot = new int[] { shotX, shotY };
                                }
                                else
                                {
                                    shotsArray[shotX, shotY] = '~';
                                }
                            }

                            counter++;

                            if (direction == 1) { shotX = lastShot[0]; shotY = lastShot[1] + counter; }
                            if (direction == 2) { shotX = lastShot[0] + counter; shotY = lastShot[1]; }
                            if (direction == 3) { shotX = lastShot[0]; shotY = lastShot[1] - counter; }
                            if (direction == 4) { shotX = lastShot[0] - counter; shotY = lastShot[1]; }

                            if (shotsArray[shotX, shotY] == '~') { ends++; continue; }

                            if (shotsArray[shotX, shotY] != '-') { continue; }

                            if (shotX < 0 || shotX > 7 || shotY < 0 || shotY > 7)
                            {
                                if (direction <= 2) { direction += 2; }
                                if (direction >= 3) { direction -= 2; }
                                ends++;
                                continue;
                            }

                        }
                    }
                }
                else
                {
                    do
                    {
                        shotX = random.Next(0, 8);
                        shotY = random.Next(0, 8);

                        if (shotsArray[shotX, shotY] != '-') { shoot = false; }

                    } while (!shoot);

                    if (boatsArray[shotX, shotY] == '#')
                    {
                        shotsArray[shotX, shotY] = '*';
                        searching = true;
                        lastShot = new int[] { shotX, shotY };
                    }
                    else
                    {
                        shotsArray[shotX, shotY] = '~';
                    }

                }

                Console.ReadLine();
            }
        }*/
    }

    class ComputerTurn
    {
        public static void Turn(char[,] shotsBoard, char[,] boatsBoard, ref bool searching, ref int[] lastShot, ref int direction)
        {
            System.Console.WriteLine("A");
            if (searching)
            {
                searching = false;
            }
            else
            {
                int[] shot = randomShot(shotsBoard);

                if (checkHit(boatsBoard, shot[0], shot[1]))
                {
                    shotsBoard[shot[0], shot[1]] = '*';
                    boatsBoard[shot[0], shot[1]] = '*';
                    System.Console.WriteLine("ih");

                    searching = true;
                    lastShot = shot;
                }
                else
                {
                    shotsBoard[shot[0], shot[1]] = '~';
                    System.Console.WriteLine("hi");
                }
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

        //generates a valid random shot to take
        static int[] randomShot(char[,] shotsBoard)
        {
            Random random = new Random();

            int X = 0;
            int Y = 0;

            do
            {
                X = random.Next(0, 8);
                Y = random.Next(0, 8);
            }
            while (validShot(shotsBoard, X, Y) != 1);

            return new int[] { X, Y };
        }

        static int[] DirectionToCoord(int[] lastShot, int direction)
        {
            if (direction == 1) { return new int[] { lastShot[0] + 1, lastShot[1] }; }
            else if (direction == 2) { return new int[] { lastShot[0], lastShot[1] + 1 }; }
            else if (direction == 3) { return new int[] { lastShot[0] - 1, lastShot[1] }; }
            else if (direction == 4) { return new int[] { lastShot[0], lastShot[1] - 1 }; }
            else { return new int[] { -1, -1 }; }
        }
    }
}