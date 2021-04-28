using System;

namespace Minesweeper
{
    class Program
    {
        static readonly int sizeOfBoard = ChooseLevel();
        static string[,] initialBoard = UnrevealedBoard();
        static int[] firstCellPosition = GetCell();
        static int[,] realBoard = SetMines(firstCellPosition);
        static void Main(string[] args)
        {
            bool gameOver = false;
            int[] cellPosition;
            string option;
            Console.Clear();
            string[,] playingBoard = ChangeDisplayBoard(firstCellPosition, initialBoard);
            Console.Clear();
            PrintNumberOfMines();
            PrintBoard(playingBoard);

            while (!gameOver)
            {
                cellPosition = GetCell();
                option = RevealOrFlag();
                playingBoard = ChangeDisplayBoard(cellPosition, playingBoard, option);
                Console.Clear();
                PrintNumberOfMines();
                PrintBoard(playingBoard);
                IsGameOver(out gameOver, cellPosition, playingBoard, option);
            }
        }
        static int ChooseLevel()
        {
            int sizeOfBoard = 0;
            Console.WriteLine($"Choose the level of difficulty: ");
            Console.WriteLine("Level One: ");
            Console.WriteLine("Level Two: ");
            bool parseBoardSize = Int32.TryParse(Console.ReadLine(), out int level);
            if ((!parseBoardSize) || (level < -1) || (level > 4))
            {
                Console.WriteLine("Not a level. Try again");
                level = ChooseLevel();
            }
            if (level == 1)
            {
                sizeOfBoard = 5;
            }
            if (level == 2)
            {
                sizeOfBoard = 10;
            }
            return sizeOfBoard;
        }
        static int[] GetCell()
        {
            int cellRow;
            int cellColumn;

            Console.WriteLine($"Enter the cell you want to reveal or flag");

            Console.WriteLine($"Row of cell: ");
            bool rowParsed = Int32.TryParse(Console.ReadLine(), out cellRow);
            Console.WriteLine($"Column of cell: ");
            bool columnParsed = Int32.TryParse(Console.ReadLine(), out cellColumn);
            int[] cellPosition = { cellRow, cellColumn };
            if ((cellColumn >= sizeOfBoard) || (cellRow >= sizeOfBoard) || (!rowParsed) || (!columnParsed))
            {
                Console.WriteLine($"{cellColumn} or {cellRow} is out of bounds or is not an integer. Please try again.");
                Console.WriteLine($"Dont enter any number above {sizeOfBoard - 1}");

                cellPosition = GetCell();
            }
            return cellPosition;
        }

        static string[,] UnrevealedBoard()
        {
            Console.Clear();
            string[,] displayBoard = new string[sizeOfBoard, sizeOfBoard];
            for (int i = 0; i < sizeOfBoard; i++)
            {
                for (int j = 0; j < sizeOfBoard; j++)
                {
                    displayBoard[i, j] = "[ ]";
                }
            }
            PrintBoard(displayBoard);
            return displayBoard;
        }

        static string RevealOrFlag()
        {
            string option = "r";
            Console.WriteLine($"Do you want to reveal or flag (f / r): ");
            option = Console.ReadLine().ToLower();
            if ((option == "f") || (option == "flag"))
            {
                option = "f";
            }
            else if ((option == "reveal") || (option == "r"))
            {
                option = "r";
            }
            else
            {
                Console.WriteLine($"Option is not valid");
                option = RevealOrFlag();
            }
            return option;
        }


        static string[,] ChangeDisplayBoard(int[] cellPosition, string[,] displayBoard, string option = "r")
        {
            int i = cellPosition[0];
            int j = cellPosition[1];
            if (option == "r")
            {
                int NORTH = i - 1;
                int SOUTH = i + 1;
                int EAST = j + 1;
                int WEST = j - 1;

                displayBoard[i, j] = $"[{realBoard[i, j].ToString()}]";
                if ((isCellValid(NORTH)) && (isCellValid(j)) && (realBoard[NORTH, j] > -1))
                {
                    displayBoard[NORTH, j] = $"[{realBoard[NORTH, j].ToString()}]";
                }
                if ((isCellValid(SOUTH)) && (isCellValid(j)) && (realBoard[SOUTH, j] > -1))
                {
                    displayBoard[SOUTH, j] = $"[{realBoard[SOUTH, j].ToString()}]";
                }
                if ((isCellValid(i)) && (isCellValid(EAST)) && (realBoard[i, EAST] > -1))
                {
                    displayBoard[i, EAST] = $"[{realBoard[i, EAST].ToString()}]";
                }
                if ((isCellValid(i)) && (isCellValid(WEST)) && (realBoard[i, WEST] > -1))
                {
                    displayBoard[i, WEST] = $"[{realBoard[i, WEST].ToString()}]";
                }
                if ((isCellValid(NORTH)) && (isCellValid(EAST)) && (realBoard[NORTH, EAST] > -1))
                {
                    displayBoard[NORTH, EAST] = $"[{realBoard[NORTH, EAST].ToString()}]";
                }
                if ((isCellValid(NORTH)) && (isCellValid(WEST)) && (realBoard[NORTH, WEST] > -1))
                {
                    displayBoard[NORTH, WEST] = $"[{realBoard[NORTH, WEST].ToString()}]";
                }
                if ((isCellValid(SOUTH)) && (isCellValid(EAST)) && (realBoard[SOUTH, EAST] > -1))
                {
                    displayBoard[SOUTH, EAST] = $"[{realBoard[SOUTH, EAST].ToString()}]";
                }
                if ((isCellValid(SOUTH)) && (isCellValid(WEST)) && (realBoard[SOUTH, WEST] > -1))
                {
                    displayBoard[SOUTH, WEST] = $"[{realBoard[SOUTH, WEST].ToString()}]";
                }
            }
            else if (option == "f")
            {
                displayBoard[i, j] = "[#]";
            }
            return displayBoard;
        }
        static int[,] SetMines(int[] cellPosition)
        {
            int numberOfMines = sizeOfBoard;
            var randomCell = new Random();
            int[,] realBoard = new int[sizeOfBoard, sizeOfBoard];
            for (int i = 0; i < sizeOfBoard; i++)
            {
                for (int j = 0; j < sizeOfBoard; j++)
                {
                    int state = randomCell.Next(30);
                    if ((state <= numberOfMines) && (i != firstCellPosition[0]) && (j != firstCellPosition[1]))
                    {
                        realBoard[i, j] = -1;
                    }

                    int NORTH = i - 1;
                    int SOUTH = i + 1;
                    int EAST = j + 1;
                    int WEST = j - 1;

                    // Count the number of mines and modify the non-mine cells around them
                    if (realBoard[i, j] == -1)
                    {
                        if (isCellValid(NORTH))
                        {
                            realBoard[NORTH, j] = CountAdjancentMines(realBoard[NORTH, j]);
                        }
                        if (isCellValid(SOUTH))
                        {
                            realBoard[SOUTH, j] = CountAdjancentMines(realBoard[SOUTH, j]);
                        }
                        if (isCellValid(EAST))
                        {
                            realBoard[i, EAST] = CountAdjancentMines(realBoard[i, EAST]);
                        }
                        if (isCellValid(WEST))
                        {
                            realBoard[i, WEST] = CountAdjancentMines(realBoard[i, WEST]);
                        }
                        if ((isCellValid(NORTH)) && (isCellValid(EAST)))
                        {
                            realBoard[NORTH, EAST] = CountAdjancentMines(realBoard[NORTH, EAST]);
                        }
                        if ((isCellValid(NORTH)) && (isCellValid(WEST)))
                        {
                            realBoard[NORTH, WEST] = CountAdjancentMines(realBoard[NORTH, WEST]);
                        }
                        if ((isCellValid(SOUTH)) && (isCellValid(EAST)))
                        {
                            realBoard[SOUTH, EAST] = CountAdjancentMines(realBoard[SOUTH, EAST]);
                        }
                        if ((isCellValid(SOUTH)) && (isCellValid(WEST)))
                        {
                            realBoard[SOUTH, WEST] = CountAdjancentMines(realBoard[SOUTH, WEST]);
                        }
                    }
                }
            }
            return realBoard;
        }
        static string[,] Board()
        {
            string[,] board = new string[sizeOfBoard, sizeOfBoard];
            for (var i = 0; i < sizeOfBoard; i++)
            {
                for (var j = 0; j < sizeOfBoard; j++)
                {
                    board[i, j] = realBoard[i, j].ToString();
                }
            }
            return board;
        }
        static void PrintBoard(string[,] playingBoard)
        {
            for (var i = 0; i < sizeOfBoard; i++)
            {
                Console.Write($"\n");
                Console.Write($"\n");
                for (var j = 0; j < sizeOfBoard; j++)
                {
                    Console.Write($"{playingBoard[i, j]}   ");
                }
            }
            Console.Write($"\n");
        }
        static int CountAdjancentMines(int cellValue)
        {
            if (cellValue > -1)
            {
                cellValue++;
            }
            return cellValue;
        }
        static bool isCellValid(int value)
        {
            bool valid = true;
            if ((value >= sizeOfBoard) || (value < 0))
            {
                valid = false;
            }
            return valid;
        }
        static void IsGameOver(out bool gameOver, int[] cellPosition, string[,] board, string option)
        {
            gameOver = false;
            int row = cellPosition[0];
            int column = cellPosition[1];
            if ((realBoard[row, column] == -1) && (option == "r"))
            {
                Console.Clear();
                Console.WriteLine($"Game Over!!");
                gameOver = true;
                RevealMinePosition(board);
            }
        }

        static void PrintNumberOfMines()
        {
            int numberOfMines = 0;
            for (var i = 0; i < sizeOfBoard; i++)
            {
                for (var j = 0; j < sizeOfBoard; j++)
                {
                    if (realBoard[i, j] == -1)
                    {
                        numberOfMines++;
                    }
                }
            }
            Console.WriteLine($"There are {numberOfMines} mines.");
        }

        static void RevealMinePosition(string[,] board)
        {
            for (var i = 0; i < sizeOfBoard; i++)
            {
                Console.Write($"\n");
                Console.Write($"\n");
                for (var j = 0; j < sizeOfBoard; j++)
                {
                    if (realBoard[i, j] == -1)
                    {
                        board[i, j] = "[*]";
                        Console.Write($"{board[i, j]}   ");
                    }
                    else
                    {
                        Console.Write($"{board[i, j]}   ");
                    }
                }
            }
            Console.Write($"\n");
        }
    }
}
