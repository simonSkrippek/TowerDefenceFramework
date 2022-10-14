using System;

namespace GameFramework
{
    /*
     * represents one player. A player can buy Soldiers or Towers, if sufficient budget is available.
     */
    public class Player
    {
        private int gold = 20;
        private int score = 0;

        private string name = "";

        /*
         * returns the player name.
         */
        public string Name
        {
            get
            {
                return name;
            }
        }

        /*
         * represents the resources the Player currently has to buy towers or soldiers.
         * The strategy can request this to take decisions.
         * 
         */
        public int Gold
        {
            get
            {
                return gold;
            }
        }

        /*
         * represents the current score of the Player.
         * The strategy can request this to take decisions.
         * 
         */
        public int Score
        {
            get
            {
                return score;
            }
        }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         * 
         * creates a new player.
         */
        public Player(string name)
        {
            this.name = name;
        }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         * 
         * pays the new unit, if budget is sufficient.
         */
        protected Boolean BuyUnit(int cost)
        {
            if (gold >= cost)
            {
                gold -= cost;
                return true;
            }
            return false;
        }

        /*
         * Soldiers will always be placed on the soldier deploy lane, which is
         * at y=0. The buyer can select the x position.
         * 
         * The buyer has to make sure, that the place, where to deploy the new
         * soldier is empty.
         * 
         * If the soldier is placed outside the soldier deploy lane, or on
         * an non-empty field, null is returned.
         */
        public Soldier BuySoldier(PlayerLane lane, int x)
        {
            if (x < 0 || x > PlayerLane.WIDTH || lane.GetCellAt(x, 0).Unit != null)
            {
                return null;
            }

            Soldier soldier = AI_TowerDefense.TowerDefense.CreateSoldier(this, lane, x);
            if (BuyUnit(soldier.Cost))
            {
                lane.GetCellAt(x, 0).Unit = soldier;
                lane.AddUnit(soldier);
                return soldier;
            }
            return null;
        }

        /*
         * Towers can be placed anywhere in the field, except the opponent's
         * soldier deploy lane, which is at y = 0.
         * 
         * The buyer has to make sure, that the place, where to deploy the new
         * tower is empty.
         * 
         * If the tower is placed outside the lane, inside the safety zone, or on
         * an non-empty field, null is returned.
         */
        public Tower BuyTower(PlayerLane lane, int x, int y)
        {
            if
            (
                y >= PlayerLane.HEIGHT_OF_SAFETY_ZONE && y < PlayerLane.HEIGHT &&
                x >= 0 && x < PlayerLane.WIDTH &&
                lane.GetCellAt(x, y).Unit == null &&
                (y % 2) != 0 && (x % 2) == 0 // Towers allowed on od colls (counted, not index wise)
                //(y % 2) != 0 && (x % 2) != 0 // Towsers allowed on even colls (counted, not index wise)
            )
            {
                Tower tower = new Tower(this, lane, x, y);
                if (BuyUnit(tower.Cost + lane.TowerCount()))
                {
                    lane.GetCellAt(x, y).Unit = tower;
                    lane.AddUnit(tower);
                    return tower;
                }
                return null;
            }

            else
                return null;
        }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         * 
         * Lets the Player earn the cost of a killed unit.
         */
        public void Earn(Unit unit)
        {
            gold += unit.Cost;
        }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         * 
         * Lets the Player earn one gold per round.
         */
        public void Earn()
        {
            gold++;
        }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         * 
         * Lets the Player score for a Soldier that reached the destination row.
         */
        public void IncScore()
        {
            score++;
        }
    }
}
