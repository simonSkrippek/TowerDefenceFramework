using System;

namespace GameFramework 
{
    /*
     * represents one Cell of a Lane. One Cell can contain one Unit 
     * (either Tower or Soldier).
     */
    public class Cell
    {
        private PlayerLane lane = null;
        private readonly int posX;
        private readonly int posY;

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         * 
         * creates a new Cell.
         */
        public Cell(PlayerLane lane, int posX, int posY)
        {
            this.lane = lane;
            this.posX = posX;
            this.posY = posY;
        }

        /*
         * returns the unit positioned in this Cell.
         */
        public Unit Unit { get; set; } = null;

        /*
         * returns the the x coordinate of this cell.
         */
        public int PosX => posX;

        /*
         * returns the the y coordinate of this cell.
         */
        public int PosY => posY;

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         * 
         * outputs the cell as part of the game state output in the game loop.
         */
        public void PrintCell()
        {
            ConsoleColor defaultColor = Console.ForegroundColor;

            Console.Write("|");
            if (Unit == null)
            {
                if (this.posY < PlayerLane.HEIGHT_OF_SAFETY_ZONE)
                    Console.ForegroundColor = ConsoleColor.Green;

                Console.Write("---");

                Console.ForegroundColor = defaultColor;
            }
            else
            {
                Unit.Print();
            }
        }
    }
}

