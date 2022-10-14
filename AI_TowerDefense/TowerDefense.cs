using AI_Strategy;
using GameFramework;
using System;
using System.Collections.Generic;
using System.Threading;
using Threading;

namespace AI_TowerDefense
{
    /*
     * Automatically called by the game play. Forbidden to use as part of
     * the assignment.
     * 
     * represents the TowerDefense game, maintains the game loop and outputs
     * the game state.
     * 
     */
    public class TowerDefense
    {
        public static bool DISPLAY_LOG_MESSAGES = true;
        public static int NORMAL_SPEED_MS = 500;
        public static int FAST_SPEED_MS = 200;

        private bool fastSpeedActivated = false;

        private Player playerA = new Player("A");
        private Player playerB = new Player("B");

        private PlayerLane lane1;
        private PlayerLane lane2;

        private AbstractStrategy playerAstrategy;
        private AbstractStrategy playerBstrategy;

        private int turns = 0;

        private static TowerDefense instance = null;

        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         * 
         * creates a new TowerDefense.
         */
        private TowerDefense()
        {
            lane1 = new PlayerLane(playerA, playerB);
            lane2 = new PlayerLane(playerB, playerA);

            // here you replace the selected strategy with your strategy class(es).
            // Your strategy should at least be able to beat random!
            
            playerAstrategy = new RandomStrategyLoggerDemo(lane1, lane2, playerA);
            playerBstrategy = new RandomStrategyLoggerDemo(lane2, lane1, playerB);
        }

        public static TowerDefense Instance()
        {
            if (instance == null)
            {
                instance = new TowerDefense();
            }

            return instance;
        }

        /*
         * prints the current game state.
         */
        protected void PrintLanes()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("Turns: " + turns + "");

            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.SetCursorPosition(0, 2);

            string clearSpaces = "                    ";

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Player A");
            Console.ForegroundColor = defaultColor;

            Console.WriteLine("Score: " + playerA.Score + clearSpaces);
            Console.WriteLine("Gold:  " + playerA.Gold + clearSpaces);

            Console.SetCursorPosition(35, 2);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Player B");
            Console.ForegroundColor = defaultColor;

            Console.SetCursorPosition(35, 3);
            Console.WriteLine("Score: " + playerB.Score + clearSpaces);

            Console.SetCursorPosition(35, 4);
            Console.WriteLine("Gold:  " + playerB.Gold + clearSpaces);
            Console.WriteLine("");

            for (int y = 0; y < PlayerLane.HEIGHT; y++)
            {
                for (int x = 0; x < PlayerLane.WIDTH; x++)
                {
                    lane1.GetCellAt(x, y).PrintCell();
                }
                Console.Write("| +++ ");
                for (int x = 0; x < PlayerLane.WIDTH; x++)
                {
                    lane2.GetCellAt(x, PlayerLane.HEIGHT - y - 1).PrintCell();
                }
                Console.WriteLine("|");
            }
        }


        /*
         * Automatically called by the game play. Forbidden to use as part of
         * the assignment.
         */
        public void RunGame()
        {
            while (turns < 1000)
            {
                if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    this.fastSpeedActivated = !this.fastSpeedActivated;
                }

                turns++;
                playerAstrategy.DeployTowers();
                playerBstrategy.DeployTowers();
                playerAstrategy.DeploySoldiers();
                playerBstrategy.DeploySoldiers();

                PrintLanes();
                Thread.Sleep(this.fastSpeedActivated ? FAST_SPEED_MS : NORMAL_SPEED_MS);

                lane1.SoldierAction();
                lane2.SoldierAction();
                lane1.TowerAction();
                lane2.TowerAction();

                playerA.Earn();
                playerB.Earn();

                PrintLanes();

                DebugLoger.RenderCache();
                Thread.Sleep(this.fastSpeedActivated ? FAST_SPEED_MS : NORMAL_SPEED_MS);
            }

            Console.ReadKey();
        }

        public static Soldier CreateSoldier(Player player, PlayerLane lane, int x)
        {
            if ("A".Equals(player.Name))
            {
                return CreatePlayerASoldier(player, lane, x);
            }
            if ("B".Equals(player.Name))
            {
                return CreatePlayerBSoldier(player, lane, x);
            }
            return null;
        }


        /*
         * creates a new Soldier for player A (player A uses Soldier), adapt class used for your test runs.
         */
        protected static Soldier CreatePlayerASoldier(Player player, PlayerLane lane, int x)
        {
            Soldier soldier = new MySoldier(player, lane, x);
            return soldier;
        }

        /*
         * creates a new Soldier for player B (player B uses MySoldier), adapt class used for your test runs.
         */
        protected static Soldier CreatePlayerBSoldier(Player player, PlayerLane lane, int x)
        {
            Soldier soldier = new MySoldier(player, lane, x);
            return soldier;
        }


        /*
         * called by the game play environment. Forbidden to use directly.
         */
        public static List<Soldier> SortedSoldierArray(Player player, List<Soldier> unsortedList)
        {
            if ("A".Equals(player.Name))
            {
                return instance.playerAstrategy.SortedSoldierArray(unsortedList);
            }
            if ("B".Equals(player.Name))
            {
                return instance.playerBstrategy.SortedSoldierArray(unsortedList);
            }
            return unsortedList;
        }
    }
}
