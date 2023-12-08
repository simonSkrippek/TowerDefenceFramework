using System;
using System.Linq;
using GameFramework;

namespace AI_TowerDefense.SimonsStrategy
{
    public class DelayedSoldierFloodStrategy : SoldierFloodStrategy
    {
        private static readonly FloodSoldier[] FloodSoldierBuffer = new FloodSoldier[1000];
        private static readonly Soldier[] SoldierBuffer = new Soldier[NumberOfCellsInLane];
        private static readonly int ExpectedNumberOfSoldiersOnFullLane = NumberOfCellsInLane - 2 * PlayerLane.WIDTH;

        private static readonly int CriticalSoldierCount = PlayerLane.WIDTH * 3;
        
        
        private int _enemyUnitAdvancement = 0;
        private int LowestSpotY => _enemyUnitAdvancement + 2;
        
        private readonly Func<Cell, bool> _preferredTowerSpotValidPredicate;

        private bool DonePlacingTowers =>
            _towerPlacementMode switch
            {
                TowerPlacementMode.EmergencyStop => true,
                TowerPlacementMode.PlaceSlowly => TowerCount >= 4,
                TowerPlacementMode.Spam => false,
                _ => true
            };
        private TowerPlacementMode _towerPlacementMode = TowerPlacementMode.PlaceSlowly;
        
        private enum TowerPlacementMode
        {
            PlaceSlowly,
            Spam,
            EmergencyStop,
        }

        public DelayedSoldierFloodStrategy(Player player) : base(player)
        {
            _preferredTowerSpotValidPredicate = spot => spot.PosY > LowestSpotY && spot.Unit is null;

            FloodSoldier.Damaged += HandleSoldierDamaged;
        }

        private void HandleSoldierDamaged(FloodSoldier obj)
        {
            var count = GetUnitsOnEnemyLane(FloodSoldierBuffer);
            // DebugLogger.Log($"soldier damaged, soldier count is {count}");
            if (count > CriticalSoldierCount)
            {
                // DebugLogger.Log("forcing march");
                for (var i = 0; i < count; i++)
                {
                    var soldier = FloodSoldierBuffer[i];
                    soldier?.StartForceMove();
                }
            }
        }

        public override void DeploySoldiers()
        {
            // if (!DonePlacingTowers && AreAnyTowerSpotsFree(out _))
            // {
            //     return;
            // }
            
            base.DeploySoldiers();
        }

        public override void DeployTowers()
        {
            CacheEnemyAdvancement();
            EvaluateTowerPlacementMode();
            switch (_towerPlacementMode)
            {
                case TowerPlacementMode.PlaceSlowly:
                    while (TowerCount < 4 && Money >= TowerCost && AreAnyTowerSpotsFree(out var cell))
                    {
                        var result = player.TryBuyTower<Tower>(cell.PosX, cell.PosY);
                    }
                    break;
                case TowerPlacementMode.Spam:
                    while (Money - RowOfSoldiersCost >= TowerCost && AreAnyTowerSpotsFree(out var cell))
                    {
                        var result = player.TryBuyTower<Tower>(cell.PosX, cell.PosY);
                    }
                    break;
                case TowerPlacementMode.EmergencyStop:
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void EvaluateTowerPlacementMode()
        {
            // if enemy lane is full with soldiers, place as many towers as we can
            if (GetUnitsOnEnemyLane(SoldierBuffer) >= ExpectedNumberOfSoldiersOnFullLane)
            {
                _towerPlacementMode = TowerPlacementMode.Spam;
            }
            else if (_enemyUnitAdvancement >= PlayerLane.HEIGHT - 1 || GetUnitsOnHomeLane<GameFramework.Tower>().Any(tower => _enemyUnitAdvancement >= tower.PosY))
            {
                _towerPlacementMode = TowerPlacementMode.EmergencyStop;
            }
            else
            {
                _towerPlacementMode = TowerPlacementMode.PlaceSlowly;
            }
            DebugLogger.Log(_towerPlacementMode.ToString());
        }


        private void CacheEnemyAdvancement()
        {
            var number_of_soldiers = GetUnitsOnHomeLane(SoldierBuffer);
            _enemyUnitAdvancement = 0;
            if (number_of_soldiers > 0)
            {
                for (var i = 0; i < number_of_soldiers; i++)
                {
                    var soldier = SoldierBuffer[i];
                    _enemyUnitAdvancement = Math.Max(soldier.PosY, _enemyUnitAdvancement);
                }
            }
        }

        protected override bool AreAnyTowerSpotsFree(out Cell cell)
        {
            if (_preferredTowerSpots.FirstOrDefault(_preferredTowerSpotValidPredicate) is not { } found_cell)
            {
                found_cell =
                    _preferredTowerSpots
                        .Find(CellEmptyPredicate)
                    ?? Array
                        .Find(_homeCells, _validTowerSpotPredicate);

                if (found_cell == null)
                {
                    cell = null;
                    return false;
                }
            }

            cell = found_cell;
            return true;
        }
        
        
    }
}