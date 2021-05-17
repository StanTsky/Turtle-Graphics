using System;

namespace TurtleGraphics
{
    class Program
    {
        static int RoomWidth = 20;
        static int RoomHeight = 20;

        static Turtle player;  // turtle object
        static int[,] room = new int[RoomWidth, RoomHeight];  // initialize room
        static string[] programArgs;

        static void Main(string[] args)
        {
            try
            {
                if (args.Length>0)
                {
                    Console.Write("Load pre-defined arguments?(Y/N) ");
                    string Answer = Console.ReadLine();
                    if (Answer.Trim().ToUpper().StartsWith("Y"))
                    {
                        programArgs = args;
                    }               
                }
                Console.WriteLine("Hints: Scripts you can try");
                Console.WriteLine("Draw 6: 1 5 12 3 3 2 5 12 4 5 12 4 5 12 4 5 6 4 5 12 1 6");
                Console.WriteLine("Draw 7: 2 5 12 3 5 12 1 6");
                Console.WriteLine();
                Console.Write("Choose room width (best sizes are 20-50): ");
                RoomWidth = Convert.ToInt32(Console.ReadLine());
                player = new Turtle('E', 0, 0, false, RoomWidth);     // initialize turtle (face East at 0,0 with pen up, and set room width)
                room = (int[,])ResizeArray(room, new int[] { player.RoomWidth, RoomHeight });  // resize room as requested
                ClearRoom();    // clear room
                NextCommand();  // get ready to accept commands
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                Console.WriteLine();
            }
        }

        public static Array ResizeArray(Array arr, int[] newSizes)  // from MSDN example for Array.Resize<T> Method (T[], Int32)
        {
            var temp = Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
            int length = arr.Length <= temp.Length ? arr.Length : temp.Length;
            Array.ConstrainedCopy(arr, 0, temp, 0, length);
            return temp;
        }
        public static void ClearRoom()
        {
            for (int y = 0; y < room.GetLength(1); y++)
            {
                for (int x = 0; x < room.GetLength(0); x++)
                { room[x, y] = 0; }
            }
        }
        public static void RefreshScreen()
        {
            player.DrawRoom(room);                         // draw room
        }
        public static void NextCommand()
        {
            Turtle.menu(player, room, programArgs);                     // show menu
        }
    }

    class Turtle
    {
        private char direction;
        public char Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        private int roomWidth=0;
        public int RoomWidth
        {
            get { return roomWidth; }
            set
            {  if (value > 50) roomWidth = 50;          // limit to width of 50
                else roomWidth = value;
            }
        }

        private int x;
        public int X
        {
            get { return x; }
            set
            {
                if (value > roomWidth) x = roomWidth;   // X can't be greater than room width
                else x = value;
            }
        }
        private int y;
        public int Y
        {
            get { return y; }
            set
            {
                if (value > 20) y = 20;                 // Y can't be greater than room height
                else y = value;
            }
        }

        private bool pen;
        public bool Pen
        {
            get { return pen; }
            set { pen = value; }
        }
        public Turtle(char direction, int x, int y, bool pen, int roomWidth)
        {
            this.direction = direction;
            this.x = x;
            this.y = y;
            this.pen = pen;
            RoomWidth = roomWidth;
        }
        public void PenUp()
        { Pen = false; }
        public void PenDown()
        { Pen = true; }
        public void TurnRight(int[,] room)
        {
            switch (direction)
            {
                case 'N': direction = 'W'; break;
                case 'S': direction = 'E'; break;
                case 'E': direction = 'N'; break;
                case 'W': direction = 'S'; break;
                default: break;
            }
        }
        public void TurnLeft(int[,] room)
        {
            switch (Direction)
            {
                case 'N': direction = 'E'; break;
                case 'S': direction = 'W'; break;
                case 'E': direction = 'S'; break;
                case 'W': direction = 'N'; break;
                default: break;
            }
        }
        public void MoveForward(int[,] room, int n)
        {
            switch (direction)
            {
                case 'N':
                    for (int i = y; i <= y + n; i++)
                    {
                        if (pen == true)
                            room[x, i] = 1;
                        else
                            room[x, i] = 0;
                    }
                    y = y + n;
                    break;
                case 'S':
                    for (int i = y - n; i <= y; i++)
                    {
                        if (pen == true)
                            room[x, i] = 1;
                        else
                            room[x, i] = 0;
                    }
                    y = y - n;
                    break;
                case 'E':
                    for (int i = x; i <= x + n; i++)
                    {
                        if (pen == true)
                            room[i, y] = 1;
                        else
                            room[i, y] = 0;
                    }
                    x = x + n;
                    break;
                case 'W':
                    for (int i = x - n; i <= x; i++)
                    {
                        if (pen == true)
                            room[i, y] = 1;
                        else
                            room[i, y] = 0;
                    }
                    x = x - n;
                    break;
            }
            room[x, y] = 2;
        }
        public void DrawRoom(int[,] room)
        {
            for (int y = 0; y < room.GetLength(1); y++)
            {
                for (int x = 0; x < room.GetLength(0); x++)
                {
                    if (room[x, y] == 0) Console.Write(".");
                    else if (room[x, y] == 1) Console.Write("*");
                    else if (room[x, y] == 2) Console.Write("T");
                }
                Console.WriteLine();
            }
        }
        public static void menu(Turtle player, int[,] room, string[] mainArgs)
        {
            Console.WriteLine("1 Pen UP");
            Console.WriteLine("2 Pen DOWN");
            Console.WriteLine("3 Turn RIGHT");
            Console.WriteLine("4 Turn LEFT");
            Console.WriteLine("5,10 Move FORWARD 10 spaces (replace 10 for a different number of spaces)");
            Console.WriteLine($"6 Display the 20-by-{player.RoomWidth} array");
            Console.WriteLine("9 End of Data (sentinel)");
            Console.WriteLine($"Turtle is facing {player.direction} from {player.x}, {player.y} and pen is {(player.Pen ? "Down" : "Up")}");

            string[] commandList = mainArgs;

            bool ShowScreen = false;    // don't draw by default
            bool Continue = true;       // continue by default

            if (mainArgs == null)
            {
                Console.WriteLine("Choose a command (comma separated list of commands is accepted): ");
                commandList = Console.ReadLine().Split(new char[] { ',', ' ' },
                                                            StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                Continue = false;
            }


            for (int i = 0; i < commandList.Length; i++)
            {
                int command;
                if (int.TryParse(commandList[i], out command))
                {
                    switch (command)
                    {
                        case 1: player.PenUp(); break;
                        case 2: player.PenDown(); break;
                        case 3: player.TurnRight(room); break;
                        case 4: player.TurnLeft(room); break;
                        case 5: player.MoveForward(room, int.Parse(commandList[++i])); break;
                        case 6: ShowScreen = true; break;
                        case 9: Continue = false; break;
                        default: Console.WriteLine("Invalid command!"); break;
                    }
                }
            }
            Console.Clear();                               // clear screen
            if (ShowScreen) Program.RefreshScreen();       // show what you drew
            if (Continue) Program.NextCommand();           // refresh menu
        }
    }
}
