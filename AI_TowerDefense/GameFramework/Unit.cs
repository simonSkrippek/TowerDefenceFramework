using System;

namespace GameFramework
{
    /*
     * Base class for Soldiers and Towers. Provides functionality to perform unit actions 
     * such as (move, attack).
     * 
     * Units will not be instantiated directly. Instead, Soldiers and Towers are 
     * subclasses, which can be instantiated by buying them via the Player functions
     * buyTower and buySoldier.
     */
    public class Unit
    {
        protected int posX = 0;
        protected int posY = 0;

        protected int range = 0;
        protected int damageCaused = 0;
        protected int health = 0;
        protected int speed = 0;

        protected Player player;
        protected string type = "";
        protected int cost = 0;

        public int Cost
        {
            get
            {
                return cost;
            }
        }

        protected PlayerLane lane = null;
        private Cell cell = null;

        /*
         * returns the the x coordinate of this unit.
         */
        public int PosX
        {
            get
            {
                return posX;
            }
        }

        /*
         * returns the the y coordinate of this unit.
         */
        public int PosY
        {
            get
            {
                return posY;
            }
        }

        /*
         * returns the the health of this unit.
         */
        public int Health
        {
            get
            {
                return health;
            }
        }

        /*
         * returns the the type of this unit.
         */
        public string Type
        {
            get
            {
                return type;
            }
        }


        protected Unit(Player player, PlayerLane lane)
        {
            this.player = player;
            this.lane = lane;
        }

        protected Cell Cell { get => cell; set => cell = value; }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         */
        public void AttackEnemyInRange()
        {
            for (int x = posX - range; x <= posX + range; x++)
            {
                for (int y = posY - range; y <= posY + range; y++)
                {
                    Cell cell = lane.GetCellAt(x, y);
                    if (cell != null)
                    {
                        Unit unit = cell.Unit;
                        if (unit != null && unit.player != this.player && unit.health > 0)
                        {
                            unit.health -= damageCaused;
                            if (unit.health <= 0)
                            {
                                unit.health = 0;
                                unit.posX = -1;
                                unit.posY = -1;
                                cell.Unit = null;
                                lane.RemoveUnit(unit);
                                player.Earn(unit);
                            }
                            return; // only one shot per round.
                        }
                    }
                }
            }
        }

        /*
         * Automatically called by the game play. Forbidden to change as part of
         * the assignment.
         * 
         * Checks, if a move is possible and performs it.
         */
        protected Boolean MoveTo(int x, int y)
        {
            if (Math.Abs(posX-x) > speed || Math.Abs(posY-y) > speed)
            {
                return false; // illegal move!
            }
            Cell oldCell = lane.GetCellAt(posX, posY);
            Cell newCell = lane.GetCellAt(x, y);
            if (oldCell != null && newCell != null && newCell.Unit == null)
            {
                oldCell.Unit = null;
                newCell.Unit = this;
                this.cell = newCell;
                posX = x;
                posY = y;
                return true;
            }
            return false;
        }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         *
         * This method should be overridden in your version of the soldier, to have a
         * better motion strategy. 
         */
        public virtual void Move()
        {
            if (speed > 0 && posY < PlayerLane.HEIGHT - 1)
            {
                int x = posX;
                int y = posY;
                for (int i=speed; i>0; i--)
                {
                    if (MoveTo(x, y + i)) return;
                    if (MoveTo(x + i, y + i)) return;
                    if (MoveTo(x - i, y + i)) return;
                    if (MoveTo(x + i, y)) return;
                    if (MoveTo(x - i, y)) return;
                    if (MoveTo(x, y - i)) return;
                    if (MoveTo(x - i, y - i)) return;
                    if (MoveTo(x + i, y - i)) return;
                }
            }
        }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         */
        public void CheckDestination()
        {
            if (speed > 0 && posY >= PlayerLane.HEIGHT - 1)
            {
                player.Earn(this);
                player.IncScore();
                this.posX = -1;
                this.posY = -1;
                this.Cell.Unit = null;
                lane.RemoveUnit(this);
            }
        }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         */
        public void Print()
        {
            ConsoleColor defaultColor = Console.ForegroundColor;

            ConsoleColor unitColor = defaultColor;
            switch (health) {
                case 9: unitColor = ConsoleColor.DarkGreen; break;
                case 8: unitColor = ConsoleColor.Green; break;
                case 7: unitColor = ConsoleColor.DarkMagenta; break;
                case 6: unitColor = ConsoleColor.Magenta; break;
                case 5: unitColor = ConsoleColor.DarkCyan; break;
                case 4: unitColor = ConsoleColor.DarkBlue; break;
                case 3: unitColor = ConsoleColor.Blue; break;
                case 2: unitColor = ConsoleColor.DarkRed; break;
                case 1: unitColor = ConsoleColor.Red; break;
                case 0: unitColor = ConsoleColor.Gray; break;
                default: unitColor = ConsoleColor.Black; break;
            }
            Console.ForegroundColor = unitColor;
            Console.Write("" + player.Name + "" + type + "" + health);
            Console.ForegroundColor = defaultColor;
        }
    }
}