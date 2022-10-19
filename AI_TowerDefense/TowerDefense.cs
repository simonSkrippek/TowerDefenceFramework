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

        private readonly Player _playerA;
        private readonly Player _playerB;

        private readonly PlayerLane _lane1;
        private readonly PlayerLane _lane2;

        private readonly AbstractStrategy _playerAStrategy;
        private readonly AbstractStrategy _playerBStrategy;

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
            _lane1 = new PlayerLane();
            _lane2 = new PlayerLane();
            
            _playerA = new Player("A", _lane1, _lane2);
            _playerB = new Player("B", _lane2, _lane1);

            // here you replace the selected strategy with your strategy class(es).
            // Your strategy should at least be able to beat random!
            
            _playerAStrategy = new RandomStrategyLoggerDemo(_playerA);
            _playerBStrategy = new RandomStrategyLoggerDemo(_playerB);
        }

        public static TowerDefense Instance => instance ??= new TowerDefense();

        /*
         * prints the current game state.
         */
        private void PrintLanes()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("Turns: " + turns + "");

            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.SetCursorPosition(0, 2);

            string clearSpaces = "                    ";

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Player A");
            Console.ForegroundColor = defaultColor;

            Console.WriteLine("Score: " + _playerA.Score + clearSpaces);
            Console.WriteLine("Gold:  " + _playerA.Gold + clearSpaces);

            Console.SetCursorPosition(35, 2);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Player B");
            Console.ForegroundColor = defaultColor;

            Console.SetCursorPosition(35, 3);
            Console.WriteLine("Score: " + _playerB.Score + clearSpaces);

            Console.SetCursorPosition(35, 4);
            Console.WriteLine("Gold:  " + _playerB.Gold + clearSpaces);
            Console.WriteLine("");

            for (int y = 0; y < PlayerLane.HEIGHT; y++)
            {
                for (int x = 0; x < PlayerLane.WIDTH; x++)
                {
                    _lane1.GetCellAt(x, y).PrintCell();
                }
                Console.Write("| +++ ");
                for (int x = 0; x < PlayerLane.WIDTH; x++)
                {
                    _lane2.GetCellAt(x, PlayerLane.HEIGHT - y - 1).PrintCell();
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
                _playerAStrategy.DeployTowers();
                _playerBStrategy.DeployTowers();
                _playerAStrategy.DeploySoldiers();
                _playerBStrategy.DeploySoldiers();

                PrintLanes();
                Thread.Sleep(this.fastSpeedActivated ? FAST_SPEED_MS : NORMAL_SPEED_MS);

                _lane1.SoldierAction();
                _lane2.SoldierAction();
                _lane1.TowerAction();
                _lane2.TowerAction();

                _playerA.Earn();
                _playerB.Earn();

                PrintLanes();

                DebugLogger.RenderCache();
                Thread.Sleep(this.fastSpeedActivated ? FAST_SPEED_MS : NORMAL_SPEED_MS);
            }

            Console.ReadKey();
        }


        /*
         * called by the game play environment. Forbidden to use directly.
         */
        public List<Soldier> SortedSoldierArray(PlayerLane lane, List<Soldier> unsortedList)
        {
            if (_playerA.EnemyLane == lane)
            {
                return _playerAStrategy.SortedSoldierArray(unsortedList);
            }
            if (_playerB.EnemyLane == lane)
            {
                return _playerBStrategy.SortedSoldierArray(unsortedList);
            }
            return unsortedList;
        }
    }
}
