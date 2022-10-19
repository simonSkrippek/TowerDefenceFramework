using System;
using GameFramework;
using System.Collections.Generic;

namespace AI_Strategy
{
    /*
     * very simple example strategy based on random placement of units.
     */
    public class RandomStrategyLoggerDemo : AbstractStrategy
    {
        private int messageCounter = 1;
        private static Random random = new Random();

        public RandomStrategyLoggerDemo(Player player) : base(player)
        {
        }

        /*
         * example strategy for deploying Towers based on random placement and budget.
         * Your one should be better!
         */
        public override void DeployTowers()
        {
            if (player.Gold > 8)
            {
                bool positioned = false;
                int count = 0;
                while (!positioned && count < 20)
                {
                    count++;
                    int x = random.Next(PlayerLane.WIDTH);
                    int y = random.Next(PlayerLane.HEIGHT - PlayerLane.HEIGHT_OF_SAFETY_ZONE) + PlayerLane.HEIGHT_OF_SAFETY_ZONE;
                    if (player.HomeLane.GetCellAt(x, y).Unit == null)
                    {
                        positioned = true;
                        player.TryBuyTower<Tower>(x, y);
                    }
                }
            }
        }

        /*
         * example strategy for deploying Soldiers based on random placement and budget.
         * Yours should be better!
         */
        public override void DeploySoldiers()
        {

          //DebugLoger.Log(Tower.GetNextTowerCosts(defendLane));
            DebugLogger.Log("#" + messageCounter + " Deployed Soldier!");
            messageCounter++;

            while (messageCounter is > 5 and <= 15)
            {
                DebugLogger.Log("#" + messageCounter + " " + random.Next(1000), true);
                //DebugLoger.Log("#" + messageCounter + ": " + random.Next(1000));
                messageCounter++;

                System.Threading.Thread.Sleep(50);
            }

            int round = 0;
            while (player.Gold > 5 && round < 5)
            {
                round++;
                bool positioned = false;
                int count = 0;
                while (!positioned && count < 10)
                {
                    count++;
                    int x = random.Next(PlayerLane.WIDTH);
                    int y = 0;
                    if (player.EnemyLane.GetCellAt(x, y).Unit == null)
                    {
                        positioned = true;
                        player.TryBuySoldier<MySoldier>(x, out var soldier);
                    }
                }
            }
        }

        /*
         * called by the game play environment. The order in which the array is returned here is
         * the order in which soldiers will plan and perform their movement.
         *
         * The default implementation does not change the order. Do better!
         */
        public override List<Soldier> SortedSoldierArray(List<Soldier> unsortedList)
        {
            return unsortedList;
        }
    }
}
