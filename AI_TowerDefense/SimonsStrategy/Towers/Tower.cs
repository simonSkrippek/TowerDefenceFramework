using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework;

namespace AI_TowerDefense.SimonsStrategy
{
    public class Tower : GameFramework.Tower
    {
        private static readonly Comparison<Unit> PreferredTargetComparison =
            (soldierA, soldierB)
                => (soldierA?.Health ?? int.MaxValue).CompareTo(soldierB.Health) is var comparison && comparison != 0
                    ? comparison
                    : soldierA!.PosX.CompareTo(soldierB.PosY) * -1;

        private List<Cell> _cellsInRange;
        private List<Cell> CellsInRange => _cellsInRange ??= GetCellsInRange();

        protected override bool TryGetTarget(out Unit unit)
        {
            var soldiers_list = GetTargetsInRange().ToList();
            if (!soldiers_list.Any())
            {
                unit = null;
                return false;
            }

            if(soldiers_list.Count > 1)
            {
                soldiers_list.Sort(PreferredTargetComparison);
            }

            unit = soldiers_list[0];
            // DebugLogger.Log($"shooting unit with {unit.Health} health");
            return true;

            // var soldiers_in_kill_range = soldiers_list.Where(soldier => soldier.Health < damageCaused).ToList();
            // if(soldiers_in_kill_range.Count != 0)
            // {
            //     soldiers_in_kill_range.Sort((soldierA, soldierB) => soldierA.PosX.CompareTo(soldierB.PosY));
            //     unit = soldiers_in_kill_range[0];
            //     return true;
            // }
        }

        protected override void Initialize(Player newPlayer, PlayerLane newLane)
        {
            base.Initialize(newPlayer, newLane);
        }

        private List<Cell> GetCellsInRange()
        {
            var cells_in_range = new List<Cell>();
            for (var x = posX - range; x <= posX + range; x++)
            {
                for (var y = posY - range; y <= posY + range; y++)
                {
                    if (lane.GetCellAt(x, y) is {} cell)
                    {
                        cells_in_range.Add(cell);
                    }
                }
            }
            // DebugLogger.Log($"found {cells_in_range.Count} cells in range");
            return cells_in_range;
        }

        private IEnumerable<Unit> GetTargetsInRange()
        {
            foreach (var cell in CellsInRange)
            {
                if (cell.Unit is Soldier { Health: > 0 } soldier)
                {
                    yield return soldier;
                }
            }
        }
    }
}