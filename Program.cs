using System;

namespace BattleBoatsProject
{
    class BattleBoatsProject
    {
        static int[] boatLengths = { 1, 1, 2, 2, 3 };

        static void Main(string[] args)
        {
            char[,] playerBoatsArray = new char[8, 8];
            //output the grid for the player to place boats
            CreatePlacementView();
            //get the player to place the boats
            PlayerBoatsPlacement(playerBoatsArray);
        }

        static void CreatePlacementView()
        {
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

        }

        static char[,] PlayerBoatsPlacement(char[,] playerBoatsArray)
        {
            for (int i = 0; i < boatLengths.Length; i++)
            {

            }
            return playerBoatsArray;
        }
    }
}