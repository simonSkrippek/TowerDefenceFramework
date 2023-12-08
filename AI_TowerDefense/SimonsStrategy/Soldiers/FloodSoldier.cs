using System;
using GameFramework;

namespace AI_TowerDefense.SimonsStrategy
{
    public class FloodSoldier : BaseSoldier
    {
        private bool _forceMove;
        public void StartForceMove()
        {
            DebugLogger.Log("soldier forced move");
            _forceMove = true;
        }

        public static event Action<FloodSoldier> Damaged;

        public static void OnDamaged(FloodSoldier soldier)
        {
            Damaged?.Invoke(soldier);
        }

        private int _lastHealth = int.MinValue;
        public override void Move()
        {
            if (_lastHealth > Health)
            {
                OnDamaged(this);
            }
            _lastHealth = Health;
            
            // movement to goal
            if (lane.GetCellAt(posX, posY + 1) is null)
            {
                MoveTo(posX, posY + 1);
            }
            
            // forced movement
            if(_forceMove)
            {
                MoveTo(posX, posY + 1);
                return;
            }

            // normal squad movement
            for (var y_offset = 1; posY - y_offset >= 0; y_offset++)
            {
                if (lane.GetCellAt(posX, posY - y_offset) is { Unit : not Soldier})
                {
                    return;
                }
            }

            // meaning there is just cell with a soldier behind us
            if (lane.GetCellAt(posX, posY + 1) is { Unit : null})
            {
                MoveTo(posX, posY + 1);
            }
        }
    }
}