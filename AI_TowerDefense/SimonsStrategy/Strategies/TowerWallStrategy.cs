using System.Collections.Generic;
using GameFramework;

namespace AI_TowerDefense.SimonsStrategy
{
    public class TowerWallStrategy : BaseStrategy
    {
        
        
        public TowerWallStrategy(Player player) : base(player, GenerateTowerSpots(player.HomeLane))
        {
            
        }

        private new static List<Cell> GenerateTowerSpots(PlayerLane homeLane)
        {
            var list = new List<Cell>();
            for (var y = PlayerLane.HEIGHT - 1; y >= PlayerLane.HEIGHT_OF_SAFETY_ZONE; y -= 2)
            {
                list.Add(homeLane.GetCellAt(2, y));
                list.Add(homeLane.GetCellAt(4, y));
                list.Add(homeLane.GetCellAt(0, y));
                list.Add(homeLane.GetCellAt(6, y));
            }

            return list;
        }

        public override void DeploySoldiers()
        {
            if(TowerCost <= 9 && AreAnyTowerSpotsFree(out _)) return;

            while (SOLDIER_COST <= Money && AreAnySoldierSpotsFree(out var cell))
            {
                var result = player.TryBuySoldier<RunnerSoldier>(cell.PosX);
                //DebugLogger.Log($"soldier buy result at {cell.PosX},{cell.PosY}: {result}");
            }
        }

        public override void DeployTowers()
        {
            if (TowerCost > 9)
            {
                return;
            }
            
            var number_of_tries = 0;
            while (TowerCost <= Money && AreAnyTowerSpotsFree(out var cell) && number_of_tries++ < 1000)
            {
                var result = player.TryBuyTower<Tower>(cell.PosX, cell.PosY);
                // if(result == Player.TowerPlacementResult.Success) DebugLogger.Log($"tower buy success at {cell.PosX},{cell.PosY}");
            }
        }


        
    }
}