using System;
using System.Collections.Generic;
using GameFramework;

namespace AI_Strategy
{
    public class TowerPlacementTestStrategy : AbstractStrategy
    {
        private readonly Random _random = new Random();
        
        public TowerPlacementTestStrategy(Player player) : base(player)
        {
        }

        public override void DeployTowers()
        {
            while (player.Gold < 15000)
            {
                player.Earn();
            }

            var number_of_tries_since_placement = 0;

            while (number_of_tries_since_placement < 20)
            {
                var random_x = _random.Next(0, PlayerLane.WIDTH);
                var random_y = _random.Next(PlayerLane.HEIGHT_OF_SAFETY_ZONE, PlayerLane.HEIGHT);

                if (player.TryBuyTower<Tower>(random_x, random_y) != Player.TowerPlacementResult.Success)
                {
                    number_of_tries_since_placement++;
                }
                else
                {
                    number_of_tries_since_placement = 0;
                }
            }
        }

        public override void DeploySoldiers()
        {
            
        }

        public override List<Soldier> SortedSoldierArray(List<Soldier> unsortedList)
        {
            return unsortedList;
        }
    }
}