using AI_TowerDefense;
using System.Collections.Generic;

namespace GameFramework
{
    /*
     * The playerLane represents one playfield. Each game takes place with two lanes 
     * in parallel, where the same player attacks one lane and defends the other.
     */
    public class PlayerLane
    {
        /*
         * HEIGHT and WIDTH represent the size of the PlayerLane.
         */
        public static readonly int HEIGHT = 20;
        public static readonly int WIDTH = 7;

        public static readonly int HEIGHT_OF_SAFETY_ZONE = 3;

        protected Cell[,] cells = new Cell[WIDTH, HEIGHT];

        protected List<Tower> towers = new List<Tower>();
        protected List<Soldier> soldiers = new List<Soldier>();

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         * 
         * creates a new PlayerLane.
         * returns the number of Soldiers currently deployed and alive in this
         * PlayerLane
         * 
         */
        public PlayerLane()
        {
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    cells[x, y] = new Cell(this, x, y);
                }
            }
        }

        /*
         * returns the Cell at the given position. Returns null, if asked for a
         * Cell outside the range.
         */
        public Cell GetCellAt(int x, int y)
        {
            if (0 <= x && x < WIDTH && 0 <= y && y < HEIGHT)
            {
                return cells[x, y];
            }
            return null;
        }

        /*
         * returns the number of Towers currently deployed and alive in this
         * PlayerLane
         */
        public int TowerCount()
        {
            return towers.Count;
        }

        /*
         * returns the number of Soldiers currently deployed and alive in this
         * PlayerLane
         */
        public int SoldierCount()
        {
            return soldiers.Count;
        }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         * 
         * Removes a Unit from the appropriate list of units (either died or destination reached)
         */
        public void RemoveUnit(Unit unit)
        {
            if (unit is Soldier soldier)
            {
                soldiers.Remove(soldier);
            }
            else if (unit is Tower tower)
            {
                towers.Remove(tower);
            }
        }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         * 
         * Adds a unit to the appropriate list of units.
         */
        public void AddUnit(Unit unit)
        {
            if (unit is Soldier)
            {
                soldiers.Add((Soldier)unit);
            }
            else if (unit is Tower)
            {
                towers.Add((Tower)unit);
            }
        }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         */
        public void TowerAction()
        {
            foreach (Tower tower in towers.ToArray())
            {
                tower.AttackEnemyInRange();
            }
        }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         */
        public void SoldierAction()
        {
            List<Soldier> sortedSoldiers = TowerDefense.Instance.SortedSoldierArray(this, soldiers);
            foreach (Soldier soldier in sortedSoldiers.ToArray())
            {
                soldier.AttackEnemyInRange();
                soldier.Move();
                soldier.CheckDestination();
            }
        }
    }
}
