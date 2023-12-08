using System;
using System.Collections.Generic;
using System.Linq;
using AI_Strategy;
using GameFramework;

namespace AI_TowerDefense.SimonsStrategy
{
    public abstract class BaseStrategy : AbstractStrategy
    {
        protected static readonly int RowOfSoldiersCost = PlayerLane.WIDTH * SOLDIER_COST;
        protected static readonly int NumberOfCellsInLane = PlayerLane.WIDTH * PlayerLane.HEIGHT;
        
        // ===================== member variables =======================
        
        protected readonly Cell[] _homeCells;
        protected readonly Cell[] _enemyCells;
        protected readonly List<Cell> _preferredTowerSpots;
        
        
        protected const int SOLDIER_COST = 2;
        
        
        // ===================== accessors =========================
        protected int TowerCost => GameFramework.Tower.GetNextTowerCosts(HomeLane);
        
        protected int TowerCount => HomeLane.TowerCount();
        
        protected PlayerLane HomeLane => player.HomeLane;
        protected PlayerLane EnemyLane => player.EnemyLane;
        protected int Money => player.Gold;

        
        // ================== cached predicates =====================
        
        protected static readonly Func<Cell, bool> CellEmptyFunc = spot => spot.Unit is null;
        protected static readonly Predicate<Cell> CellEmptyPredicate = new(CellEmptyFunc);
        protected readonly Func<Cell, bool> _validTowerSpotFunc;
        protected readonly Predicate<Cell> _validTowerSpotPredicate;
        
        // ==========================================================

        protected BaseStrategy(Player player, List<Cell> preferredTowerSpots) : base(player)
        {
            _validTowerSpotFunc = spot
                => spot.PosY >= PlayerLane.HEIGHT_OF_SAFETY_ZONE
                   && spot.PosY < PlayerLane.HEIGHT
                   && spot.PosX >= 0
                   && spot.PosX < PlayerLane.WIDTH
                   && CheckAdjacentCellsAll(HomeLane, spot.PosX, spot.PosY, cell => cell.Unit is not GameFramework.Tower)
                   && spot.Unit is null;
            _validTowerSpotPredicate = new Predicate<Cell>(_validTowerSpotFunc);
            
            _homeCells = IterateCellsToArray(new Cell[NumberOfCellsInLane], HomeLane);
            _enemyCells = IterateCellsToArray(new Cell[NumberOfCellsInLane], EnemyLane);
            
            _preferredTowerSpots = preferredTowerSpots;
            
        }

        protected static List<Cell> GenerateTowerSpots(PlayerLane homeLane)
        {
            var list = new List<Cell>();
            for (var y = PlayerLane.HEIGHT_OF_SAFETY_ZONE + 1; y < PlayerLane.HEIGHT; y += 2)
            {
                list.Add(homeLane.GetCellAt(2, y));
                list.Add(homeLane.GetCellAt(4, y));
                list.Add(homeLane.GetCellAt(0, y));
                list.Add(homeLane.GetCellAt(6, y));
            }

            return list;
        }

        protected bool AreAnySoldierSpotsFree(out Cell cell)
        {
            cell = null;
            for (var x = 0; x < PlayerLane.WIDTH; x++)
            {
                cell = EnemyLane.GetCellAt(x, 0);
                if (cell.Unit is null)
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual bool AreAnyTowerSpotsFree(out Cell cell)
        {
            cell = null;
            if (_preferredTowerSpots.FirstOrDefault(CellEmptyFunc) is not { } found_cell)
            {
                return false;
            }

            cell = found_cell;
            return true;
        }

        public override List<Soldier> SortedSoldierArray(List<Soldier> unsortedList)
        {
            var sorted_list = new List<Soldier>(unsortedList);
            sorted_list
                .Sort((soldier, soldier1)
                    => -1 * (soldier.PosY.CompareTo(soldier1.PosY) is var height_comparison && height_comparison != 0
                        ? height_comparison
                        : DistanceToClosestEnemyTower(soldier.PosX, soldier.PosY).CompareTo(DistanceToClosestEnemyTower(soldier1.PosX, soldier1.PosY))));
            return sorted_list;
        }

        private static readonly GameFramework.Tower[] TowerBuffer = new  GameFramework.Tower[NumberOfCellsInLane / 3];

        // ================================ grid calculations ==========================================
        
        protected int DistanceToClosestEnemyTower(int x, int y)
        {
            var shortest_distance = int.MaxValue;
            for (var i = GetUnitsOnEnemyLane(TowerBuffer) - 1; i >= 0; i--)
            {
                var tower = TowerBuffer[i];
                var distance = Distance(x, y, tower.PosX, tower.PosY);
                shortest_distance = Math.Min(shortest_distance, distance);
            }
            return shortest_distance;
        }
        protected int DistanceToClosestEnemyTower(int x, int y, out GameFramework.Tower closestTower)
        {
            var shortest_distance = int.MaxValue;
            closestTower = null;
            for (var i = GetUnitsOnEnemyLane(TowerBuffer) - 1; i >= 0; i--)
            {
                var tower = TowerBuffer[i];
                var distance = Distance(x, y, tower.PosX, tower.PosY);
                if (distance >= shortest_distance)
                {
                    continue;
                }

                shortest_distance = distance;
                closestTower = tower;
            }
            return shortest_distance;
        }

        protected static int Distance(int x1, int y1, int x2, int y2) => Math.Max(Math.Abs(x2 - x1), Math.Abs(y2 - y1));
        
        
        public static bool CheckAdjacentCellsAny(PlayerLane lane, int x, int y, Func<Cell, bool> predicate)
        {
            return
                (lane.GetCellAt(x + 1, y) is {} cell_right && predicate(cell_right))
                || (lane.GetCellAt(x, y - 1) is {} cell_down && predicate(cell_down)) 
                || (lane.GetCellAt(x - 1, y) is {} cell_left && predicate(cell_left)) 
                || (lane.GetCellAt(x, y + 1) is {} cell_up && predicate(cell_up))
                ;
        }
        public static bool CheckAdjacentCellsAll(PlayerLane lane, int x, int y, Func<Cell, bool> predicate)
        {
            return
                (lane.GetCellAt(x + 1, y) is not {} cell_right || predicate(cell_right))
                && (lane.GetCellAt(x, y - 1) is not {} cell_down || predicate(cell_down)) 
                && (lane.GetCellAt(x - 1, y) is not {} cell_left || predicate(cell_left)) 
                && (lane.GetCellAt(x, y + 1) is not {} cell_up || predicate(cell_up))
                ;
        }

        // ================================ iterate units ==========================================
        
        protected int GetUnitsOnHomeLane<T>(T[] unitBuffer) where T : Unit => GetUnits(unitBuffer, _homeCells);
        protected int GetUnitsOnEnemyLane<T>(T[] unitBuffer) where T : Unit => GetUnits(unitBuffer, _enemyCells);
        protected static int GetUnits<T>(T[] unitBuffer, IEnumerable<Cell> cells) where T : Unit
        {
            var number_of_found_units = 0;
            var unit_buffer_length = unitBuffer.Length;
            
            foreach (var cell in cells)
            {
                if (number_of_found_units >= unit_buffer_length)
                {
                    break;
                }

                if (cell.Unit is not T unit)
                {
                    continue;
                }
                
                unitBuffer[number_of_found_units++] = unit;
            }

            return number_of_found_units;
        }
        
        
        protected IEnumerable<T> GetUnitsOnHomeLane<T>() where T : Unit => GetUnits<T>(_homeCells);
        protected IEnumerable<T> GetUnitsOnEnemyLane<T>() where T : Unit => GetUnits<T>(_enemyCells);
        protected static IEnumerable<T> GetUnits<T>(IEnumerable<Cell> cells) where T : Unit
        {
            foreach (var cell in cells)
            {
                if (cell.Unit is T unit)
                {
                    yield return unit;
                }
            }
        }
        
        // ================================= iterate cells ==========================================

        protected IEnumerable<Cell> IterateEnemyCells() => IterateCells(player.EnemyLane);
        protected IEnumerable<Cell> IterateHomeCells() => IterateCells(player.HomeLane);
        protected static IEnumerable<Cell> IterateCells(PlayerLane lane)
        {
            for (var y = 0; y < PlayerLane.HEIGHT; y++)
            {
                for (var x = 0; x < PlayerLane.WIDTH; x++)
                {
                    yield return lane.GetCellAt(x, y);
                }
            }
        }
        
        
        protected IEnumerable<Cell> IterateEnemyCellsReverse() => IterateCellsReverse(player.EnemyLane);
        protected IEnumerable<Cell> IterateHomeCellsReverse() => IterateCellsReverse(player.HomeLane);
        protected static IEnumerable<Cell> IterateCellsReverse(PlayerLane lane)
        {
            for (var y = PlayerLane.HEIGHT - 1; y >= 0; y--)
            {
                for (var x = 0; x < PlayerLane.WIDTH; x++)
                {
                    yield return lane.GetCellAt(x, y);
                }
            }
        }
        
        
        // =============================== iterate cells to array ===================================
        protected static Cell[] IterateCellsToArray(Cell[] cells, PlayerLane lane)
        {
            for (var y = 0; y < PlayerLane.HEIGHT; y++)
            {
                for (var x = 0; x < PlayerLane.WIDTH; x++)
                {
                    cells[y * PlayerLane.WIDTH + x] = lane.GetCellAt(x, y);
                }
            }

            return cells;
        }
    }
}