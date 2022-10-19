using System;
using AI_Strategy;

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
        
        public PlayerLane HomeLane { get; }
        public PlayerLane EnemyLane { get; }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         * 
         * creates a new player.
         */
        public Player(string name, PlayerLane homeLane, PlayerLane targetLane)
        {
            this.name = name;
            HomeLane = homeLane;
            EnemyLane = targetLane;
        }

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         * 
         * pays the new unit, if budget is sufficient.
         */
        protected bool TryPayCost(int cost)
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
         * an non-empty field, false is returned.
         */
        public bool TryBuySoldier<TSoldier>(int x) where TSoldier : Soldier, new() => TryBuySoldier<TSoldier>( x, out _);
        public bool TryBuySoldier<TSoldier>(int x, out TSoldier soldier) where TSoldier : Soldier, new()
        {
            if (x < 0 || x > PlayerLane.WIDTH || EnemyLane.GetCellAt(x, 0).Unit != null)
            {
                soldier = null;
                return false;
            }

            soldier = Soldier.CreateSoldier<TSoldier>(this, EnemyLane, x);
            if (!TryPayCost(soldier.Cost))
            {
                return false;
            }

            EnemyLane.GetCellAt(x, 0).Unit = soldier;
            EnemyLane.AddUnit(soldier);
            return true;
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
        public bool TryBuyTower<TTower>(int x, int y) where TTower : Tower, new() => TryBuyTower<TTower>(x, y, out _);
        public bool TryBuyTower<TTower>(int x, int y, out TTower tower) where TTower : Tower, new()
        {
            if (
                y < PlayerLane.HEIGHT_OF_SAFETY_ZONE 
                || y >= PlayerLane.HEIGHT 
                || x < 0 
                || x >= PlayerLane.WIDTH 
                || HomeLane.GetCellAt(x, y).Unit != null 
                // 
                // || y % 2 == 0 
                // || x % 2 != 0
                )
            {
                tower = null;
                return false;
            }

            tower = Tower.CreateTower<TTower>(this, HomeLane, x, y);
            if (!TryPayCost(tower.Cost + HomeLane.TowerCount()))
            {
                return false;
            }

            HomeLane.GetCellAt(x, y).Unit = tower;
            HomeLane.AddUnit(tower);
            return true;

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
