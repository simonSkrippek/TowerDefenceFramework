using System.Collections.Generic;
using System.Linq;
using GameFramework;

namespace AI_TowerDefense.SimonsStrategy
{
    public class BaseSoldier : Soldier
    {
        protected override bool TryGetTarget(out Unit unit)
        {
            var soldiers = GetTargetsInRange();
            var soldiers_list = soldiers.ToList();
            if (!soldiers_list.Any())
            {
                unit = null;
                return false;
            }

            if(soldiers_list.Count > 1)
                soldiers_list.Sort(
                    (soldierA, soldierB)
                        => (soldierA?.Health ?? int.MaxValue).CompareTo(soldierB.Health) is var comparison && comparison != 0
                            ? comparison
                            : soldierA!.PosX.CompareTo(soldierB.PosY) * -1);
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

        protected IEnumerable<Unit> GetTargetsInRange()
        {
            for (var x = posX - range; x <= posX + range; x++)
            {
                for (var y = posY - range; y <= posY + range; y++)
                {
                    if (lane.GetCellAt(x, y)?.Unit is GameFramework.Tower { Health: > 0 } tower)
                    {
                        yield return tower;
                    }
                }
            }
        }
    }
}