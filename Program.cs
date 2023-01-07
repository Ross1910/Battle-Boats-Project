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

            Renderer.Intro(renderBoard);

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
        //displays the menu and returns the selected option
        static int Menu(int[,,][] renderBoard)
        {
            Renderer.MenuInit(renderBoard);

            int selection = 0;
            Renderer.MenuOption(renderBoard, selection);

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
                Renderer.MenuOption(renderBoard, selection);
                System.Console.WriteLine(selection);
            }

            Renderer.MenuClose(renderBoard);

            return selection;
        }
        //creates a new game and places the boats
        static void NewGame(int[,,][] renderBoard)
        {
            //create a new game object
            Game newGame = new Game(BoardSize);

            //get an input of the file name for the game
            Renderer.SplashText(renderBoard, "NameInput.bin", 17, 15);

            newGame.fileName = Renderer.TextInput(21, 16, 36);

            //show a grid to place the boats
            Renderer.PlaceBoatsGrid(renderBoard);

            //loop though each boat
            for (int i = 0; i < boatLengths.Length; i++)
            {
                bool valid = true;
                int X = 0;
                int Y = 0;
                bool Rotated = false;
                int length = boatLengths[i];
                do
                {
                    valid = true;
                    X = 0;
                    Y = 0;
                    Rotated = false;
                    length = boatLengths[i];

                    Renderer.PlaceBoatsTargets(renderBoard, X, Y, Rotated, length);

                    //while the enter key isnt hit
                    ConsoleKey key = new ConsoleKey();
                    while ((key = Console.ReadKey().Key) != ConsoleKey.Enter)
                    {
                        //move around the board
                        if (key == ConsoleKey.UpArrow && Y > 0) { Y--; }
                        if (key == ConsoleKey.DownArrow && ((Rotated && Y < 7) || (!Rotated && Y + length <= 7))) { Y++; }
                        if (key == ConsoleKey.LeftArrow && X > 0) { X--; }
                        if (key == ConsoleKey.RightArrow && ((!Rotated && X < 7) || (Rotated && X + length <= 7))) { X++; }
                        if (key == ConsoleKey.R && ((Rotated && Y + length <= 8) || (!Rotated && X + length <= 8))) { Rotated = !Rotated; }
                        //show the current position
                        Renderer.PlaceBoatsTargets(renderBoard, X, Y, Rotated, length);
                    }

                    //check it doesnt overlap any other boats
                    for (int j = 0; j < length; j++)
                    {
                        if (!Rotated && (newGame.PlayerBoats[X, Y + j] != new char())) { valid = false; };
                        if (Rotated && (newGame.PlayerBoats[X + j, Y] != new char())) { valid = false; };
                    }
                }
                while (!valid);
                for (int j = 0; j < length; j++)
                {
                    //place the boat in the array
                    if (!Rotated) { newGame.PlayerBoats[X, Y + j] = alphabet[i]; };
                    if (Rotated) { newGame.PlayerBoats[X + j, Y] = alphabet[i]; };
                }
                //display the placed boats
                Renderer.PlaceBoats(renderBoard, newGame.PlayerBoats, 24, 16);
            }

            //generate the computers boats
            Console.ReadLine();
            Random random = new Random();
            //loop througheach boat
            for (int i = 0; i < boatLengths.Length; i++)
            {
                bool valid = false;

                int boatX = 0;
                int boatY = 0;
                bool boatR = true;
                int boatLength = boatLengths[i];


                while (!valid)
                {
                    //generate a random rotation
                    boatR = ((int)random.Next(0, 2)) == 0;
                    //generate a random valid position
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
                    //check it does not overlap
                    valid = true;
                    for (int j = 0; j < boatLength; j++)
                    {
                        if ((boatR && newGame.ComputerBoats[boatX + j, boatY] != new char()) || (!boatR && newGame.ComputerBoats[boatX, boatY + j] != new char()))
                        {
                            valid = false;
                        }
                    }
                }
                //place the boat into the array
                for (int j = 0; j < boatLength; j++)
                {
                    if (boatR) { newGame.ComputerBoats[boatX + j, boatY] = alphabet[i]; }
                    else if (!boatR) { newGame.ComputerBoats[boatX, boatY + j] = alphabet[i]; }
                }

            }
            //save the game as a file
            SaveGame(newGame);
            //start playing the game
            PlayGame(newGame.fileName, renderBoard);

        }
        //gets a games name and loads it
        static void ResumeGame(int[,,][] renderBoard)
        {

            Renderer.SplashText(renderBoard, "NameInput.bin", 17, 15);

            string fileName = Renderer.TextInput(21, 16, 36);

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
        //saves the game under the filename attached
        static void SaveGame(Game game)
        {
            //declare a new binary writer
            BinaryWriter writer = new BinaryWriter(File.Open(("SavedGames/" + game.fileName + ".BattleBoats"), FileMode.Create));
            //write the boardsize into the file
            writer.Write(game.BoardSize);
            //write the value in each tile of the file
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
            //write the other parameters of the game object
            writer.Write(game.compSearching);

            for (int i = 0; i < 2; i++)
            {
                writer.Write(game.compLastShot[i]);
            }

            writer.Write(game.compDirection);

            //close the file
            writer.Close();
        }
        //loads the game from a specified filename and returns it
        static Game LoadGame(string Filename)
        {
            try
            {
                //create a new binary reader
                BinaryReader reader = new BinaryReader(File.Open(("SavedGames/" + Filename + ".BattleBoats"), FileMode.Open));
                //get the size of the arrays
                int boardSize = reader.ReadInt32();

                //create a new game object using the arrray size
                Game returnGame = new Game(boardSize);

                //get the values for the array
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

                //get the other parameters
                returnGame.compSearching = reader.ReadBoolean();

                for (int i = 0; i < 2; i++)
                {
                    returnGame.compLastShot[i] = reader.ReadInt32();
                }

                returnGame.compDirection = reader.ReadInt32();

                returnGame.fileName = Filename;

                //close the file and return the object
                reader.Close();
                return returnGame;

            }
            catch
            {
                //if the file is not found tell the user
                Console.Clear();
                Console.WriteLine("File not found");
                Console.ReadLine();
                //throw an error
                throw;
            }
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
            Renderer.PlaceBattleGrid(renderBoard);


            int X = 0;
            int Y = 0;

            //loop until someone wins
            while (win == -1)
            {
                //load the game from a file
                Game currentGame = LoadGame(Filename);

                //show the players boats
                Renderer.PlaceShotsAndBoats(renderBoard, currentGame.PlayerShots, currentGame.PlayerBoats, currentGame.ComputerShots);

                //run the players turn
                bool validShot = false;
                do
                {
                    //show where the player is currently aiming
                    Renderer.PlaceShotsTarget(renderBoard, X, Y);

                    //let the player navigate around the board
                    ConsoleKey key = new ConsoleKey();
                    while ((key = Console.ReadKey().Key) != ConsoleKey.Enter)
                    {
                        if (key == ConsoleKey.UpArrow && Y > 0) { Y--; }
                        if (key == ConsoleKey.DownArrow && Y < 7) { Y++; }
                        if (key == ConsoleKey.LeftArrow && X > 0) { X--; }
                        if (key == ConsoleKey.RightArrow && X < 7) { X++; }
                        //show where the player is currently aiming
                        Renderer.PlaceShotsTarget(renderBoard, X, Y);
                    }
                    //checks if the position has been tried before
                    if (currentGame.PlayerShots[X, Y] == new char()) { validShot = true; }
                }
                while (!validShot);
                //if the position hasnt been tried check if its a hit or a mixx
                // mark * for hit and ~ for miss
                if (currentGame.ComputerBoats[X, Y] != new char()) { currentGame.PlayerShots[X, Y] = '*'; }
                else { currentGame.PlayerShots[X, Y] = '~'; }

                //run the computers turn
                ComputerTurn.Turn(currentGame.ComputerShots, currentGame.PlayerBoats, ref currentGame.compSearching, ref currentGame.compLastShot, ref currentGame.compDirection);

                int playerHits = 0;
                int computerHits = 0;
                //check the number of hits for both players
                for (int i = 0; i < currentGame.PlayerShots.GetLength(0); i++)
                {
                    for (int j = 0; j < currentGame.PlayerShots.GetLength(1); j++)
                    {
                        if (currentGame.PlayerShots[i, j] == '*') { playerHits++; }
                        if (currentGame.ComputerShots[i, j] == '*') { computerHits++; }
                    }
                }
                //if either player has hit all boats they have won
                if (playerHits >= hitTarget) { win = 1; }
                else if (computerHits >= hitTarget) { win = 2; }


                //save the game at the end of the turn
                SaveGame(currentGame);
            }

            //display the win message
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
        //displays the instructions
        static void Instructions()
        {
            //clears the console
            Console.Clear();
            //outputs all lines of the instructions
            StreamReader reader = new StreamReader(File.Open("instructions.txt", FileMode.Open));
            string line = "";
            while ((line = reader.ReadLine()) != null) { Console.WriteLine(line); }
            //once the user hits enter, clears the instructions
            Console.ReadLine();
            Console.Clear();
        }
        //quits the game
        static void Quit()
        {
            //clear the console and quit the program
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
            //if the algorithm is not tracking a boat shoot randomly
            if (!searching)
            {
                RandomShot(shotsBoard, boatsBoard, ref searching, ref lastShot);
            }
            //if the algorithm is currently tracking a boat
            else
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

                        valid = ValidShot(shotsBoard, shot[0], shot[1]) != 1;
                    }
                    while (valid && direction != -5);

                    //if all directions have been tested and there is no boat, take a random shot
                    if (direction == -5)
                    {
                        RandomShot(shotsBoard, boatsBoard, ref searching, ref lastShot);
                        direction = 0;
                        return;
                    }

                    //check if the shot hits and mark the arrays appropriately
                    else if (CheckHit(boatsBoard, shot[0], shot[1]))
                    {
                        direction = direction * -1;
                        shotsBoard[shot[0], shot[1]] = '*';
                        lastShot = shot;
                        return;
                    }
                    else
                    {
                        shotsBoard[shot[0], shot[1]] = '~';
                        return;
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
                        if (ValidShot(shotsBoard, shot[0], shot[1]) == 3)
                        {
                            shot = DirectionToCoord(shot, direction);
                            continue;
                        }
                        //if the shot is not valid for any other reason, the end of the boat has been reached to head in the opposite direction
                        else if (ValidShot(shotsBoard, shot[0], shot[1]) != 1 && !flipped)
                        {
                            //makes it only go back and forth once to prevent infinite loop
                            flipped = true;

                            if (direction < 3) { direction += 2; }
                            else if (direction > 2) { direction -= 2; }

                            shot = DirectionToCoord(shot, direction);
                            continue;
                        }
                        //if its a valid shot take the shot and mark boards appropriately with hit or miss
                        else if (ValidShot(shotsBoard, shot[0], shot[1]) == 1)
                        {
                            foundShot = true;
                            if (CheckHit(boatsBoard, shot[0], shot[1]))
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
                            RandomShot(shotsBoard, boatsBoard, ref searching, ref lastShot);
                            direction = 0;
                            searching = false;
                            return;
                        }
                    }
                }
            }
        }
        //checks if the shot is a hit or not
        static bool CheckHit(char[,] boatsBoard, int X, int Y)
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
        static int ValidShot(char[,] shotsBoard, int X, int Y)
        {
            if (X > 7 || X < 0 || Y > 7 || Y < 0) { return -1; }
            if (shotsBoard[X, Y] == '~') { return 2; }
            if (shotsBoard[X, Y] == '*') { return 3; }
            if (shotsBoard[X, Y] == new char()) { return 1; }
            else { return -1; }
        }
        //generates and takes a valid random shot
        static void RandomShot(char[,] shotsBoard, char[,] boatsBoard, ref bool searching, ref int[] lastShot)
        {
            Random random = new Random();

            int X = 0;
            int Y = 0;
            //generates a random shot and checks if its valid
            do
            {
                X = random.Next(0, BoardSize);
                Y = random.Next(0, BoardSize);
            }
            while (ValidShot(shotsBoard, X, Y) != 1);
            //checks if the shot hits
            if (CheckHit(boatsBoard, X, Y))
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
            //gets the dimensions of the array
            int layers = renderBoard.GetLength(2);
            int width = renderBoard.GetLength(1);
            int height = renderBoard.GetLength(0);

            //checks that the height is even
            if (height % 2 == 1) { Console.WriteLine("screen must be an even number of pixels high"); return; }

            //creates a new 2d array
            int[,][] flatBoard = new int[height, width][];

            //loops through each pixel and takes the highest non transparent pixel
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
                }
            }

            //converts the 2d array into a string
            string output = "";
            for (int i = 0; i < height; i += 2)
            {
                for (int j = 0; j < width; j++)
                {
                    //the takes 2 rows, the top one is set as the backgroung colour and the bottom as the foreground
                    int[] topPixel = flatBoard[i, j];
                    int[] bottomPixel = flatBoard[i + 1, j];
                    output += ("\x1b[48;2;" + topPixel[0] + ";" + topPixel[1] + ";" + topPixel[2] + "m" + "\x1b[38;2;" + bottomPixel[0] + ";" + bottomPixel[1] + ";" + bottomPixel[2] + "m▄\u001b[0m");
                }
                //adds a new line character at the end of every line
                output += "\n";
            }
            //moves the cursor to the top corner and writes the whole image
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(output);

        }
        static int[,][] LoadObject(string Filename)
        {
            //creates a new binary reader
            BinaryReader Br = new BinaryReader(File.Open(("Assets/" + Filename), FileMode.Open));

            //reads the images dimensions
            int imgWidth = Br.ReadInt32();
            int imgHeight = Br.ReadInt32();

            //creates an array to store the image
            int[,][] ObjectArray = new int[imgWidth, imgHeight][];

            //gets the rgba values for every pixel of the image
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

            //close the file and return the array
            Br.Close();
            return ObjectArray;
        }
        static void PlaceObject(int[,][] Object, int[,,][] renderBoard, int X, int Y, int Layer)
        {
            //get the sizes of the object
            int width = Object.GetLength(1);
            int height = Object.GetLength(0);

            //loop though each pixel and place it on the renderboard
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
            //loop through each pixel in a layer
            for (int i = 0; i < renderBoard.GetLength(0); i++)
            {
                for (int j = 0; j < renderBoard.GetLength(1); j++)
                {
                    //replace the pixel with a black transparent pixel
                    renderBoard[i, j, layer] = new int[] { 0, 0, 0, 0 };
                }
            }
        }
        static void ClearAllObj(int[,,][] renderBoard)
        {
            //clear layers 1, 2 and 3
            ClearLayer(renderBoard, 1);
            ClearLayer(renderBoard, 2);
            ClearLayer(renderBoard, 3);
        }
        public static void Intro(int[,,][] renderBoard)
        {
            //write the introductory info
            Console.WriteLine("make the console full screen then hit Enter");
            Console.WriteLine("warning this can get quite flickery, especially on less powerful computers");
            Console.WriteLine("as the console doesnt like writing bulk quickly");
            Console.ReadLine();

            //load and place the background
            int[,][] background = LoadObject("Background.bin");
            PlaceObject(background, renderBoard, 0, 0, 0);
            Render(renderBoard);

            //check the user can see the whole image
            Console.WriteLine("if the image takes up the whole console try zooming out");
            Console.ReadLine();

            //display the title
            int[,][] title = LoadObject("Title.bin");
            PlaceObject(title, renderBoard, 12, 12, 2);
            int[,][] titleBorder = LoadObject("TitleBorder.bin");
            PlaceObject(titleBorder, renderBoard, 9, 9, 1);
            Render(renderBoard);
        }
        public static void MenuInit(int[,,][] renderBoard)
        {
            //clear all objects and display the menu
            ClearAllObj(renderBoard);
            PlaceObject(LoadObject("menu.bin"), renderBoard, 9, 12, 1);
            Render(renderBoard);
        }
        public static void MenuOption(int[,,][] renderBoard, int selection)
        {
            //clear the previous option
            ClearLayer(renderBoard, 2);

            //highlight the current selection
            if (selection == 0) { PlaceObject(LoadObject("menu newgame.bin"), renderBoard, 12, 15, 2); ; }
            if (selection == 1) { PlaceObject(LoadObject("menu resumegame.bin"), renderBoard, 12, 21, 2); }
            if (selection == 2) { PlaceObject(LoadObject("menu instructions.bin"), renderBoard, 12, 27, 2); }
            if (selection == 3) { PlaceObject(LoadObject("menu quit.bin"), renderBoard, 12, 33, 2); }
            Render(renderBoard);
        }
        public static void MenuClose(int[,,][] renderBoard)
        {
            //clear all objects for the menu
            ClearAllObj(renderBoard);
        }
        public static void SplashText(int[,,][] renderBoard, string file, int X, int Y)
        {
            //clear the top layer and draw the popup
            ClearLayer(renderBoard, 3);
            PlaceObject(LoadObject(file), renderBoard, X, Y, 3);
            Render(renderBoard);
        }
        public static string TextInput(int X, int Y, int maxLength = -1)
        {
            //put the cursor at the start position for the text
            Console.SetCursorPosition(X, Y);

            //make the text appear in the correct colours
            int[] background = new int[] { 170, 170, 170 };
            int[] foreground = new int[] { 68, 73, 84 };
            Console.Write("\x1b[48;2;" + background[0] + ";" + background[1] + ";" + background[2] + "m" + "\x1b[38;2;" + foreground[0] + ";" + foreground[1] + ";" + foreground[2] + "m");

            int length = 0;
            string output = "";

            ConsoleKeyInfo key = new ConsoleKeyInfo();

            while ((key = Console.ReadKey()).Key != ConsoleKey.Enter)
            {
                //if the character is backspace
                if (key.Key == ConsoleKey.Backspace)
                {
                    //remove one character from the string and decrease the length
                    output = output.Substring(0, output.Length - 1);
                    length--;
                }
                //if the character is a number or letter
                else if ((key.KeyChar >= 'a' && key.KeyChar <= 'z') || (key.KeyChar >= 'A' && key.KeyChar <= 'Z') || (key.KeyChar >= '0' && key.KeyChar <= '9'))
                {
                    //add the character to the output and incrememnt the length
                    output += key.KeyChar;
                    length++;
                    //if the length is greater than the maximum remove the character 
                    if (length > maxLength && maxLength != -1)
                    {
                        Console.Write("\b \b");
                    }
                }
                //if it is not a number or letter remove it
                else
                {
                    Console.Write("\b \b");
                }
            }
            //write the ansii reset code and return the output string
            Console.Write("\u001b[0m");
            return output;
        }
        public static void PlaceBoatsGrid(int[,,][] renderBoard)
        {
            //clears all objects and displays the boat placement grid
            ClearAllObj(renderBoard);
            PlaceObject(LoadObject("placementGrid.bin"), renderBoard, 20, 12, 1);
            Render(renderBoard);
        }
        public static void PlaceShotsTarget(int[,,][] renderBoard, int X, int Y)
        {
            //clears the layer and places the target at the specified coordinates
            ClearLayer(renderBoard, 2);
            int[,][] target = LoadObject("target.bin");
            PlaceObject(target, renderBoard, 44 + X * 4, 16 + Y * 4, 2);
            Render(renderBoard);
        }
        public static void PlaceBoatsTargets(int[,,][] renderBoard, int X, int Y, bool R, int length)
        {
            //clears the layer
            ClearLayer(renderBoard, 2);
            int[,][] target = LoadObject("target.bin");
            //places the target from the specified position, in the spefified direction for the specified length 
            for (int i = 0; i < length; i++)
            {
                if (R) { PlaceObject(target, renderBoard, 24 + X * 4 + i * 4, 16 + Y * 4, 2); }
                else if (!R) { PlaceObject(target, renderBoard, 24 + X * 4, 16 + Y * 4 + i * 4, 2); }
            }
            Render(renderBoard);
        }
        public static void PlaceBoats(int[,,][] renderBoard, char[,] boatsBoard, int X, int Y)
        {
            //cleats the layer
            ClearLayer(renderBoard, 3);
            //gets the boat object
            int[,][] boat = LoadObject("1 boat.bin");
            //loops throught the boat array and places a boat object anywhere where there is a boat in the array
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
        public static void PlaceShots(int[,,][] renderBoard, char[,] shotsBoard, int X, int Y)
        {
            //clear the layer
            ClearLayer(renderBoard, 3);
            //get the hit and miss icon objects
            int[,][] hit = LoadObject("hit.bin");
            int[,][] miss = LoadObject("miss.bin");
            //loop through the array
            for (int i = 0; i < shotsBoard.GetLength(0); i++)
            {
                for (int j = 0; j < shotsBoard.GetLength(0); j++)
                {
                    //place a hit or miss where needed
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
        public static void PlaceShotsAndBoats(int[,,][] renderBoard, char[,] shotsBoard, char[,] boatsBoard, char[,] enemyShotsBoard)
        {
            //clear the layer and get the hit and miss objects
            ClearLayer(renderBoard, 3);
            int[,][] hit = LoadObject("hit.bin");
            int[,][] miss = LoadObject("miss.bin");
            //loop through the array placing hit or miss where needed
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
            //get the boat object
            int[,][] boat = LoadObject("1 boat.bin");
            //loop through the array placing boats where needed
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
        public static void PlaceBattleGrid(int[,,][] renderBoard)
        {
            //clear all objects and place the boat and shots board
            ClearAllObj(renderBoard);
            PlaceObject(LoadObject("placementGrid.bin"), renderBoard, 0, 12, 1);
            PlaceObject(LoadObject("placementGrid.bin"), renderBoard, 40, 12, 1);
            Render(renderBoard);
        }
    }

    //declares a struct to store the games state
    struct Game
    {
        //declares all the variables in the struct 
        public int BoardSize;
        public char[,] PlayerBoats;
        public char[,] PlayerShots;
        public char[,] ComputerBoats;
        public char[,] ComputerShots;
        public bool compSearching = false;
        public int[] compLastShot = new int[2] { -1, -1 };
        public int compDirection = 0;
        public string fileName = "newGame.bin";

        public Game(int boardSizeParam)
        {
            //generates the boards at the specified size
            BoardSize = boardSizeParam;
            PlayerBoats = new char[boardSizeParam, boardSizeParam];
            PlayerShots = new char[boardSizeParam, boardSizeParam];
            ComputerBoats = new char[boardSizeParam, boardSizeParam];
            ComputerShots = new char[boardSizeParam, boardSizeParam];
        }
    }
}