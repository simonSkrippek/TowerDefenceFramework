﻿using System;

namespace AI_TowerDefense
{
    class Program
    {
        /*
         * main method. Creates a new TowerDefense game and starts the game loop.
         */
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            TowerDefense towerDefense = TowerDefense.Instance;
            towerDefense.RunGame();
        }
    }
}
