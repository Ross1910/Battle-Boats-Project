using System;

namespace BattleBoatsProject
{
    class BattleBoatsProject
    {
        static int[] boatLengths = { 1, 1, 2, 2, 3 };

        static void Main(string[] args)
        {
            //create a new array to store the players boatrs and fill it with -
            char[,] playerBoatsArray = NewArray(8, 8);

            //get the player to place the boats
            playerBoatsArray = PlayerBoatsPlacement(playerBoatsArray);

            //create a new array to store the computers boats
            char[,] computerBoatsArray = NewArray(8, 8);

            //generate the computers boats
            computerBoatsArray = GenerateBoats(computerBoatsArray);


            //display for debug purposes
            CreatePlacementView(computerBoatsArray);


            Console.ReadLine();
        }

        static char[,] NewArray(int X, int Y)
        {
            char[,] returnArray = new char[X, Y];
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
    }
}