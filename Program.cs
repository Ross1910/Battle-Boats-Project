using System;

namespace BattleBoatsProject
{
    class BattleBoatsProject
    {

        static int BoardSize = 8;
        static int[] boatLengths = { 2, 2, 3, 4 };
        static string alphabet = "abcdefghijklmnopqrstuvwxyz";


        static void Main(string[] args)
        {

            int[,,][] renderBoard = new int[64, 80, 4][];

            Renderer.intro(renderBoard);

            Console.ReadLine();
            int choice = 0;

            while (choice != 4)
            {
                choice = Menu(renderBoard);

                if (choice == 0) { NewGame(renderBoard); }
                else if (choice == 1) { ResumeGame(renderBoard); }
                else if (choice == 2) { Instructions(); }
                else if (choice == 3) { Quit(); return; }
            }
            return;
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

            Renderer.MenuClose(renderBoard);

            return selection;
        }
        static void NewGame(int[,,][] renderBoard)
        {
            Game newGame = new Game(8);

            Renderer.splashText(renderBoard, "NameInput.bin", 17, 15);

            newGame.fileName = Renderer.textInput(21, 16, 36);

            Renderer.placeBoatsGrid(renderBoard);

            for (int i = 0; i < boatLengths.Length; i++)
            {
                bool valid = true;
                int X = 0;
                int Y = 0;
                bool R = false;
                int length = boatLengths[i];
                do
                {
                    valid = true;
                    X = 0;
                    Y = 0;
                    R = false;
                    length = boatLengths[i];

                    Renderer.boatsPlacementTarget(renderBoard, X, Y, R, length);

                    ConsoleKey key = new ConsoleKey();
                    while ((key = Console.ReadKey().Key) != ConsoleKey.Enter)
                    {
                        if (key == ConsoleKey.UpArrow && Y > 0) { Y--; }
                        if (key == ConsoleKey.DownArrow && ((R && Y < 7) || (!R && Y + length <= 7))) { Y++; }
                        if (key == ConsoleKey.LeftArrow && X > 0) { X--; }
                        if (key == ConsoleKey.RightArrow && ((!R && X < 7) || (R && X + length <= 7))) { X++; }
                        if (key == ConsoleKey.R && ((R && Y + length <= 8) || (!R && X + length <= 8))) { R = !R; }

                        Renderer.boatsPlacementTarget(renderBoard, X, Y, R, length);
                    }

                    for (int j = 0; j < length; j++)
                    {
                        if (!R && (newGame.PlayerBoats[X, Y + j] != new char())) { valid = false; };
                        if (R && (newGame.PlayerBoats[X + j, Y] != new char())) { valid = false; };
                    }
                }
                while (!valid);
                for (int j = 0; j < length; j++)
                {
                    if (!R) { newGame.PlayerBoats[X, Y + j] = alphabet[i]; };
                    if (R) { newGame.PlayerBoats[X + j, Y] = alphabet[i]; };
                }
                Renderer.placeBoats(renderBoard, newGame.PlayerBoats, 24, 16);
            }

            Console.ReadLine();
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
                        if ((boatR && newGame.ComputerBoats[boatX + j, boatY] != new char()) || (!boatR && newGame.ComputerBoats[boatX, boatY + j] != new char()))
                        {
                            valid = false;
                        }
                    }
                }
                for (int j = 0; j < boatLength; j++)
                {
                    if (boatR) { newGame.ComputerBoats[boatX + j, boatY] = alphabet[i]; }
                    else if (!boatR) { newGame.ComputerBoats[boatX, boatY + j] = alphabet[i]; }
                }

            }


            Renderer.placeBoats(renderBoard, newGame.ComputerBoats, 24, 16);
            SaveGame(newGame);

            PlayGame(newGame.fileName, renderBoard);

        }
        static void SaveGame(Game game)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(("SavedGames/" + game.fileName + ".BattleBoats"), FileMode.Create));
            writer.Write(game.BoardSize);
            for (int i = 0; i < game.BoardSize; i++)
            {
                for (int j = 0; j < game.BoardSize; j++)
                {
                    writer.Write(game.PlayerBoats[i, j]);
                    writer.Write(game.PlayerShots[i, j]);
                    writer.Write(game.ComputerBoats[i, j]);
                    writer.Write(game.ComputerShots[i, j]);
                }
            }
            writer.Write(game.compSearching);

            for (int i = 0; i < 2; i++)
            {
                writer.Write(game.compLastShot[i]);
            }

            writer.Write(game.compDirection);
            writer.Write(game.PlayerTurn);

            writer.Close();
        }
        static Game LoadGame(string Filename)
        {
            try
            {
                BinaryReader reader = new BinaryReader(File.Open(("SavedGames/" + Filename + ".BattleBoats"), FileMode.Open));
                int boardSize = reader.ReadInt32();

                Game returnGame = new Game(boardSize);

                for (int i = 0; i < BoardSize; i++)
                {
                    for (int j = 0; j < BoardSize; j++)
                    {
                        returnGame.PlayerBoats[i, j] = reader.ReadChar();
                        returnGame.PlayerShots[i, j] = reader.ReadChar();
                        returnGame.ComputerBoats[i, j] = reader.ReadChar();
                        returnGame.ComputerShots[i, j] = reader.ReadChar();
                    }
                }

                returnGame.compSearching = reader.ReadBoolean();

                for (int i = 0; i < 2; i++)
                {
                    returnGame.compLastShot[i] = reader.ReadInt32();
                }

                returnGame.compDirection = reader.ReadInt32();
                returnGame.PlayerTurn = reader.ReadBoolean();

                returnGame.fileName = Filename;

                reader.Close();

                return returnGame;

            }
            catch
            {
                Console.Clear();
                Console.WriteLine("File not found");
                Console.ReadLine();
                throw;
            }
        }
        static void ResumeGame(int[,,][] renderBoard)
        {

            Renderer.splashText(renderBoard, "NameInput.bin", 17, 15);

            string fileName = Renderer.textInput(21, 16, 36);

            try
            {
                LoadGame(fileName);
            }
            catch
            {
                return;
            }
            PlayGame(fileName, renderBoard);
        }
        //loops through the game until it is quit or someone wins
        static void PlayGame(string Filename, int[,,][] renderBoard)
        {
            //set the win to false
            int win = -1;
            //set up the win condition
            int hitTarget = 0;
            for (int i = 0; i < boatLengths.Length; i++)
            {
                hitTarget += boatLengths[i];
            }

            //set up the screen for the default display
            Renderer.placeBattleGrid(renderBoard);


            int X = 0;
            int Y = 0;

            //loop until someone wins
            while (win == -1)
            {
                //load the game from a file
                Game currentGame = LoadGame(Filename);

                //show the players boats
                Renderer.placeShotsAndBoats(renderBoard, currentGame.PlayerShots, currentGame.PlayerBoats, currentGame.ComputerShots);

                //run the players turn
                bool validShot = false;
                Renderer.placeShotsTarget(renderBoard, X, Y);
                do
                {
                    Renderer.placeShotsTarget(renderBoard, X, Y);

                    ConsoleKey key = new ConsoleKey();
                    while ((key = Console.ReadKey().Key) != ConsoleKey.Enter)
                    {
                        if (key == ConsoleKey.UpArrow && Y > 0) { Y--; }
                        if (key == ConsoleKey.DownArrow && Y < 7) { Y++; }
                        if (key == ConsoleKey.LeftArrow && X > 0) { X--; }
                        if (key == ConsoleKey.RightArrow && X < 7) { X++; }
                        Renderer.placeShotsTarget(renderBoard, X, Y);
                    }

                    if (currentGame.PlayerShots[X, Y] == new char()) { validShot = true; }
                }
                while (!validShot);

                if (currentGame.ComputerBoats[X, Y] != new char()) { currentGame.PlayerShots[X, Y] = '*'; }
                else { currentGame.PlayerShots[X, Y] = '~'; }

                //run the computers turn
                ComputerTurn.Turn(currentGame.ComputerShots, currentGame.PlayerBoats, ref currentGame.compSearching, ref currentGame.compLastShot, ref currentGame.compDirection);

                int playerHits = 0;
                int computerHits = 0;
                //check for a win
                for (int i = 0; i < currentGame.PlayerShots.GetLength(0); i++)
                {
                    for (int j = 0; j < currentGame.PlayerShots.GetLength(1); j++)
                    {
                        if (currentGame.PlayerShots[i, j] == '*') { playerHits++; }
                        if (currentGame.ComputerShots[i, j] == '*') { computerHits++; }
                    }
                }

                if (playerHits >= hitTarget) { win = 1; }
                else if (computerHits >= hitTarget) { win = 2; }


                //save the game at the end of the turn
                SaveGame(currentGame);

                //win = 0;
            }

            if (win == 1)
            {
                Console.Clear();
                Console.WriteLine("Congratulations, you win");
                Console.ReadLine();
                return;
            }
            else if (win == 2)
            {
                Console.Clear();
                Console.WriteLine("The Computer won :(");
                Console.ReadLine();
                return;
            }
        }
        static void Instructions() { }
        static void Quit()
        {
            Console.Clear();
            System.Environment.Exit(1);
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
            if (boatsBoard[X, Y] != new char())
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
            if (shotsBoard[X, Y] == new char()) { return 1; }
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
            Console.WriteLine("warning this can get quite flickery, especially on less powerful computers");
            Console.WriteLine("as the console doesnt like writing bulk quickly");
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
        public static void MenuClose(int[,,][] renderBoard)
        {
            ClearAllObj(renderBoard);
        }
        public static void splashText(int[,,][] renderBoard, string file, int X, int Y)
        {
            ClearLayer(renderBoard, 3);

            PlaceObject(LoadObject(file), renderBoard, X, Y, 3);

            Render(renderBoard);
        }
        public static string textInput(int X, int Y, int maxLength = -1)
        {
            Console.SetCursorPosition(X, Y);

            int[] background = new int[] { 170, 170, 170 };
            int[] foreground = new int[] { 68, 73, 84 };
            Console.Write("\x1b[48;2;" + background[0] + ";" + background[1] + ";" + background[2] + "m" + "\x1b[38;2;" + foreground[0] + ";" + foreground[1] + ";" + foreground[2] + "m");

            int length = 0;
            string output = "";

            ConsoleKeyInfo key = new ConsoleKeyInfo();

            while ((key = Console.ReadKey()).Key != ConsoleKey.Enter)
            {
                length++;

                if (length > maxLength && maxLength != -1)
                {
                    Console.Write("\b \b");
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    output = output.Substring(0, output.Length - 1);
                }
                else
                {
                    output += key.KeyChar;
                }
            }

            Console.Write("\u001b[0m");

            return output;
        }
        public static void placeBoatsGrid(int[,,][] renderBoard)
        {
            ClearAllObj(renderBoard);
            PlaceObject(LoadObject("placementGrid.bin"), renderBoard, 20, 12, 1);
            Render(renderBoard);
        }
        public static void placeShotsTarget(int[,,][] renderBoard, int X, int Y)
        {
            ClearLayer(renderBoard, 2);
            int[,][] target = LoadObject("target.bin");
            PlaceObject(target, renderBoard, 44 + X * 4, 16 + Y * 4, 2);
            Render(renderBoard);
        }
        public static void boatsPlacementTarget(int[,,][] renderBoard, int X, int Y, bool R, int length)
        {
            ClearLayer(renderBoard, 2);
            int[,][] target = LoadObject("target.bin");
            for (int i = 0; i < length; i++)
            {
                if (R) { PlaceObject(target, renderBoard, 24 + X * 4 + i * 4, 16 + Y * 4, 2); }
                else if (!R) { PlaceObject(target, renderBoard, 24 + X * 4, 16 + Y * 4 + i * 4, 2); }
            }
            Render(renderBoard);
        }
        public static void placeBoats(int[,,][] renderBoard, char[,] boatsBoard, int X, int Y)
        {
            ClearLayer(renderBoard, 3);
            int[,][] boat = LoadObject("1 boat.bin");
            for (int i = 0; i < boatsBoard.GetLength(0); i++)
            {
                for (int j = 0; j < boatsBoard.GetLength(0); j++)
                {
                    if (boatsBoard[j, i] != new char())
                    {
                        PlaceObject(boat, renderBoard, X + 4 * j, Y + 4 * i, 3);
                    }
                }
            }
            Render(renderBoard);
        }
        public static void placeShots(int[,,][] renderBoard, char[,] shotsBoard, int X, int Y)
        {
            ClearLayer(renderBoard, 3);
            int[,][] hit = LoadObject("hit.bin");
            int[,][] miss = LoadObject("miss.bin");
            for (int i = 0; i < shotsBoard.GetLength(0); i++)
            {
                for (int j = 0; j < shotsBoard.GetLength(0); j++)
                {
                    if (shotsBoard[j, i] == '*')
                    {
                        PlaceObject(hit, renderBoard, X + 4 * j, Y + 4 * i, 3);
                    }
                    if (shotsBoard[j, i] == '~')
                    {
                        PlaceObject(miss, renderBoard, X + 4 * j, Y + 4 * i, 3);
                    }
                }
            }
            Render(renderBoard);
        }
        public static void placeShotsAndBoats(int[,,][] renderBoard, char[,] shotsBoard, char[,] boatsBoard, char[,] enemyShotsBoard)
        {
            ClearLayer(renderBoard, 3);
            int[,][] hit = LoadObject("hit.bin");
            int[,][] miss = LoadObject("miss.bin");
            for (int i = 0; i < shotsBoard.GetLength(0); i++)
            {
                for (int j = 0; j < shotsBoard.GetLength(0); j++)
                {
                    if (shotsBoard[j, i] == '*')
                    {
                        PlaceObject(hit, renderBoard, 44 + 4 * j, 16 + 4 * i, 3);
                    }
                    if (shotsBoard[j, i] == '~')
                    {
                        PlaceObject(miss, renderBoard, 44 + 4 * j, 16 + 4 * i, 3);
                    }
                }
            }
            int[,][] boat = LoadObject("1 boat.bin");
            for (int i = 0; i < boatsBoard.GetLength(0); i++)
            {
                for (int j = 0; j < boatsBoard.GetLength(0); j++)
                {
                    if (boatsBoard[j, i] != new char() && enemyShotsBoard[j, i] == '*')
                    {
                        PlaceObject(hit, renderBoard, 4 + 4 * j, 16 + 4 * i, 3);
                    }
                    else if (boatsBoard[j, i] != new char())
                    {
                        PlaceObject(boat, renderBoard, 4 + 4 * j, 16 + 4 * i, 3);
                    }
                    else if (boatsBoard[j, i] == new char() && enemyShotsBoard[j, i] == '~')
                    {
                        PlaceObject(miss, renderBoard, 4 + 4 * j, 16 + 4 * i, 3);
                    }
                }
            }
            Render(renderBoard);
        }
        public static void placeBattleGrid(int[,,][] renderBoard)
        {
            ClearAllObj(renderBoard);
            PlaceObject(LoadObject("placementGrid.bin"), renderBoard, 0, 12, 1);
            PlaceObject(LoadObject("placementGrid.bin"), renderBoard, 40, 12, 1);
            Render(renderBoard);
        }
    }

    //create a struct to store the games state
    struct Game
    {
        public int BoardSize;
        public char[,] PlayerBoats;
        public char[,] PlayerShots;
        public char[,] ComputerBoats;
        public char[,] ComputerShots;
        public bool compSearching = false;
        public int[] compLastShot = new int[2] { -1, -1 };
        public int compDirection = 0;
        public bool PlayerTurn = true;
        public string fileName = "newGame.bin";

        public Game(int boardSizeParam)
        {
            BoardSize = boardSizeParam;
            PlayerBoats = new char[boardSizeParam, boardSizeParam];
            PlayerShots = new char[boardSizeParam, boardSizeParam];
            ComputerBoats = new char[boardSizeParam, boardSizeParam];
            ComputerShots = new char[boardSizeParam, boardSizeParam];
        }
    }
}

// ~ means missed shot
// * means hit shot 
//TODO: use the boats A-Z render boats all fancy