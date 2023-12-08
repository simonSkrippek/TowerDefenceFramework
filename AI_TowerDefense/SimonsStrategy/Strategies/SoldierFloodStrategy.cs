using System.Collections.Generic;
using GameFramework;

namespace AI_TowerDefense.SimonsStrategy
{
    public class SoldierFloodStrategy : BaseStrategy
    {

        public SoldierFloodStrategy(Player player) : base(player, GenerateTowerSpots(player.HomeLane))
        {
            
        }

        public override void DeployTowers()
        {
            while (Money >= RowOfSoldiersCost + TowerCost 
                   && AreAnyTowerSpotsFree(out var cell))
            {
                player.TryBuyTower<Tower>(cell.PosX, cell.PosY);
            }
        }

        public override void DeploySoldiers()
        {
            if (Money > RowOfSoldiersCost)
            {
                for (var x = 0; x < PlayerLane.WIDTH; x++)
                {
                    player.TryBuySoldier<FloodSoldier>(x);
                }
            }
        }

        public override List<Soldier> SortedSoldierArray(List<Soldier> unsortedList)
        {
            var sorted_list = new List<Soldier>(unsortedList);
            sorted_list.Sort((soldier, soldier1) => -1 * soldier.PosY.CompareTo(soldier1.PosY));
            return sorted_list;
        }
    }
}