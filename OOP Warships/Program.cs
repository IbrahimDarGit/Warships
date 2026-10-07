
//Version Number 1.0

using System;
using System.IO;
namespace OOP_Warships
{
    class Program //Controls the overall game flow and user interaction(Run the Game)
    {

        const string TrainingGame = "Training.txt";// Constant string for the training game file name

        private static void GetRowColumn(ref int Row, ref int Column, ref string Mtype)
        {
            Console.WriteLine();
            Console.Write("Please enter type (M) missile, (B) Bomb: ");
            Mtype = (Console.ReadLine().ToUpper());

            Console.Write("Please enter column: ");
            Column = Convert.ToInt32(Console.ReadLine());
            Console.Write("Please enter row: ");
            Row = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            if (Row < 0 || Row > 9) // Checks input is valid: M/B and row/column between 0-9.
            {
                Console.WriteLine("Invalid row. Please enter a row between 0 and 9.");
                GetRowColumn(ref Row, ref Column, ref Mtype);
            }
            else if (Column < 0 || Column > 9)
            {
                Console.WriteLine("Invalid column. Please enter a column between 0 and 9.");
                GetRowColumn(ref Row, ref Column, ref Mtype);
            }
            else if (Mtype != "M" && Mtype != "B")
            {
                Console.WriteLine("Invalid type. Please enter 'M' for missile or 'B' for bomb.");
                GetRowColumn(ref Row, ref Column, ref Mtype);
            }
        }

        private static void DisplayMenu()
        {
            Console.WriteLine("MAIN MENU");
            Console.WriteLine("");
            Console.WriteLine("1. Start new game");
            Console.WriteLine("2. Load training game");
            Console.WriteLine("3. Quit");
            Console.WriteLine("4. Save game");
            Console.WriteLine("5. Load saved game");
            Console.WriteLine();
        }

        private static int GetMainMenuChoice()
        {
            int Choice = 0;
            Console.Write("Please enter your choice: ");
            Choice = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();
            return Choice;
        }

        private static void PlayGame(ref GameBoard Board)
        {
            bool GameWon = false;
            int row=0;
            int col=0;
            string Wtype="";
            while (GameWon == false)
            {
                Board.PrintBoard();
                GetRowColumn(ref row, ref col, ref Wtype);
                if (Wtype == "M")
                {
                    Missile MyMissile = new Missile();
                    MyMissile.Fire(row, col, Board);
                }
                if (Wtype == "B")
                {
                    Bomb MyBomb = new Bomb();
                    MyBomb.Fire(row, col, Board);
                }
                if (Board.CheckWin() == true)
                    {
                        Console.WriteLine("All ships sunk!");
                        Console.WriteLine();
                    }
            }
        }


        static void Main(string[] args)
        {
           

            GameBoard Board = new GameBoard();// Create a new game board

            int MenuOption = 0;
            while (MenuOption != 9)
            {
                DisplayMenu();
                MenuOption = GetMainMenuChoice();
                if (MenuOption == 1)
                {
                    Board.PlaceRandomShips();
                    PlayGame(ref Board);
                }
                else if (MenuOption == 2)
                {
                    Board.LoadBoard(TrainingGame);
                    PlayGame(ref Board);
                }
                else if (MenuOption == 3) // Validated Menu option to quit the game
                {
                    Console.WriteLine("Quitting the game...");
                    break;
                }
                else // Invalid menu option
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }
        }
    }



    class Missile //Represents a normal single-square weapon(Normal shot)
    {
        protected int startRow;// Store the starting row of the missile
        protected int startCol;// Store the starting column of the missile

        public virtual void Fire(int row, int col, GameBoard Board)
        {
            startRow = row;
            startCol = col;
            Board.CheckLocation(startRow,startCol);
        }

    }



    class Bomb : Missile //A specialised Missile that attacks several squares(Area attack)
    {
        private int blastRadius;

        public Bomb()
        {
            blastRadius = 1;
        }

        public override void Fire(int row, int col, GameBoard Board)
        {
            for (int startRow = row - blastRadius; startRow <= row + blastRadius; startRow++)
            {
                for (int startCol = col - blastRadius; startCol <= col + blastRadius; startCol++)
                {
                    if (startCol >= 0 && startCol < 10 && startRow >= 0 && startRow < 10)
                    {
                        
                        Board.CheckLocation(startRow,startCol);
                        
                    }
                }
            }
        }
    }



    class GameBoard //Stores and manages the board, ships, hits, misses and game rules(Controls the battlefield)
    {
        private char[,] Board = new char[10, 10];// 2D array to hold the board
        public Ship[] Ships = new Ship[5];// Array to hold the ships

        private int Hits = 0; // Strike Rate and hit and miss calculation
        private int Misses = 0;


        public GameBoard()
        {

            for (int Row = 0; Row < 10; Row++)
            {
                for (int Column = 0; Column < 10; Column++)
                {
                    Board[Row, Column] = '-';
                }
            }
            
            Ships[0] = new Ship("Aircraft Carrier", 5);
            Ships[1] = new Ship("Battleship", 4);
            Ships[2] = new Ship("Submarine", 3);
            Ships[3] = new Ship("Destroyer", 3);
            Ships[4] = new Ship("Patrol Boat", 2);
        }

        public void PrintBoard()
        {
            Console.WriteLine();
            Console.WriteLine("The board looks like this: ");
            Console.WriteLine();
            Console.Write(" ");
            for (int Column = 0; Column < 10; Column++)
            {
                Console.Write(" " + Column + "  ");
            }
            Console.WriteLine();
            for (int Row = 0; Row < 10; Row++)
            {
                Console.Write(Row + " ");
                for (int Column = 0; Column < 10; Column++)
                {
                    if (Board[Row, Column] == '-')
                    {
                        Console.Write(" ");
                    }
                    else if (Board[Row, Column] == 'A' || Board[Row, Column] == 'B' || Board[Row, Column] == 'S' || Board[Row, Column] == 'D' || Board[Row, Column] == 'P')
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.Write(Board[Row, Column]);
                    }
                    if (Column != 9)
                    {
                        Console.Write(" | ");
                    }
                }
                Console.WriteLine();
            }
        }

        public void CheckLocation(int Row, int Column)
        {
            if (Board[Row, Column] == '-')
            {
                if (NearMiss(Row, Column))
                {
                    Board[Row, Column] = 'n';
                    Misses++;
                    Console.WriteLine("Near miss! A ship is nearby.");
                }
                else
                {
                    Board[Row, Column] = 'm';
                    Misses++;
                    Console.WriteLine("Sorry, this is a miss.");
                }
            }
            else if (Board[Row, Column] == 'm' ||
                     Board[Row, Column] == 'h' ||
                     Board[Row, Column] == 'n')
            {
                Console.WriteLine("You have already fired at this location.");
            }
            else
            {
                foreach (Ship ThisShip in Ships)
                {
                    if (ThisShip.GetShipType() == Board[Row, Column])
                    {
                        Console.WriteLine("Hit a " + ThisShip.GetName() +
                                          " at (" + Column + "," + Row + ").");

                        ThisShip.AddHit();

                        if (ThisShip.GetHitCount() == ThisShip.GetSize())
                        {
                            Console.WriteLine("You have sunk the " + ThisShip.GetName() + "!");
                        }
                    }
                }

                Board[Row, Column] = 'h';
                Hits++;
            }

            if (Hits + Misses > 0)
            {
                double StrikeRate = (double)Hits / (Hits + Misses) * 100;
                Console.WriteLine("Strike Rate: {0:F2}%", StrikeRate);
            }
        }
        public void SaveBoard()// Save the current state of the board to a text file
        {
            StreamWriter writer = new StreamWriter("save.txt");

            for (int Row = 0; Row < 10; Row++)
            {
                for (int Column = 0; Column < 10; Column++)
                {
                    writer.Write(Board[Row, Column]);
                }

                writer.WriteLine();
            }

            writer.Close();
        }
        public void LoadBoardFromFile(string filename)// Load the state of the board from a text file
        {
            StreamReader reader = new StreamReader(filename);
            for (int Row = 0; Row < 10; Row++)
            {
                string line = reader.ReadLine();
                for (int Column = 0; Column < 10; Column++)
                {
                    Board[Row, Column] = line[Column];
                }
            }
            reader.Close();
        }
        public bool NearMiss(int Row, int Column)// Check if the location is near a ship
        {
            for (int r = Row - 1; r <= Row + 1; r++)
            {
                for (int c = Column - 1; c <= Column + 1; c++)
                {
                    if (r >= 0 && r < 10 && c >= 0 && c < 10)
                    {
                        if (Board[r, c] == 'A' || Board[r, c] == 'B' || Board[r, c] == 'S' || Board[r, c] == 'D' || Board[r, c] == 'P')
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void SetLocation(int Row, int Column, char Value)
        {
            Board[Row, Column] = Value;
        }

        public char GetLocation(int Row, int Column)
        {
            return Board[Row, Column];
        }

        public void LoadBoard(string TrainingGame)
        {
            string Line = "";
            StreamReader BoardFile = new StreamReader(TrainingGame);
            for (int Row = 0; Row < 10; Row++)
            {
                Line = BoardFile.ReadLine();
                for (int Column = 0; Column < 10; Column++)
                {
                    Board[Row, Column] = Line[Column];
                }
            }
            BoardFile.Close();
        }

        public void PlaceRandomShips()
        {
            Random RandomNumber = new Random();
            bool Valid;
            char Orientation = ' ';
            int Row = 0;
            int Column = 0;
            int HorV = 0;
            foreach (var Ship in Ships)
            {
                Valid = false;
                while (Valid == false)
                {
                    Row = RandomNumber.Next(0, 10);
                    Column = RandomNumber.Next(0, 10);
                    HorV = RandomNumber.Next(0, 2);
                    if (HorV == 0)
                    {
                        Orientation = 'v';
                    }
                    else
                    {
                        Orientation = 'h';
                    }
                    Valid = ValidateBoatPosition(Ship, Row, Column, Orientation);
                }
                Console.WriteLine("Computer placing the " + Ship.GetName());
                PlaceShip(Ship, Row, Column, Orientation);
            }
        }

        private bool ValidateBoatPosition(Ship Ship, int Row, int Column, char Orientation)
        {
            if (Orientation == 'v' && Row + Ship.GetSize() > 10)
            {
                return false;
            }
            else if (Orientation == 'h' && Column + Ship.GetSize() > 10)
            {
                return false;
            }
            else
            {
                if (Orientation == 'v')
                {
                    for (int Scan = 0; Scan < Ship.GetSize(); Scan++)
                    {
                        if (Board[Row + Scan, Column] != '-')
                        {
                            return false;
                        }
                    }
                }
                else if (Orientation == 'h')
                {
                    for (int Scan = 0; Scan < Ship.GetSize(); Scan++)
                    {
                        if (Board[Row, Column + Scan] != '-')
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void PlaceShip(Ship Ship, int Row, int Column, char Orientation)
        {
            if (Orientation == 'v')
            {
                for (int Scan = 0; Scan < Ship.GetSize(); Scan++)
                {
                    Board[Row + Scan, Column] = Ship.GetName()[0];
                }
            }
            else if (Orientation == 'h')
            {
                for (int Scan = 0; Scan < Ship.GetSize(); Scan++)
                {
                    Board[Row, Column + Scan] = Ship.GetName()[0];
                }
            }
        }

        public bool CheckWin()
        {
            for (int Row = 0; Row < 10; Row++)
            {
                for (int Column = 0; Column < 10; Column++)
                {
                    if (Board[Row, Column] == 'A' || Board[Row, Column] == 'B' || Board[Row, Column] == 'S' || Board[Row, Column] == 'D' || Board[Row, Column] == 'P')
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }


    class Ship //Stores information about one individual ship(Represents one ship)
    {
        private string _Name;// Store the name of the ship
        private int _Size;// Store the size of the ship
        private int _HitCount = 0;// Store the number of hits the ship has taken

        public string GetName()
        {
            return _Name;
        }

        public int GetSize()
        {
            return _Size;
        }


        public char GetShipType()
        {
            return _Name[0]; // Uses the first letter of the ship name as its board symbol
        }

        public int GetHitCount()
        {
            return _HitCount;
        }

        public void AddHit()
        {
            _HitCount++;
        }

        public Ship()
        {
        }

        public Ship(string ShipName, int ShipSize)
        {
            _Name = ShipName;
            _Size = ShipSize;
        }

    }

}


