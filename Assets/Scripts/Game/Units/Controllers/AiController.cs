using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Game.Units.Formation.ContuberniumFormation;
using Assets.Scripts.Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Controllers
{
    public class AiController : UnitController
    {
        private const float TimeBetweenEnemySearches = 5;
        private const float LowHealthPercentage = .3f;

        public override bool IsAi { get; } = true;

        private void MoveToEnemey()
        {
            UnitController nearestEnemy = NearestEnemy();
            if (nearestEnemy == null)
            {
                Debug.LogWarning("Nearest enemy is null, everyone is dead");
                return;
            }

            Goal = nearestEnemy.Position;
            Debug.Log("Tick " + Goal);
        }

        protected override void ConsiderFormation(Contubernium unit)
        {
            // Is null at beginning and after killing a enemy (for a single frame)
            Contubernium enemy = unit.CurrentEnemy;

            if (IsLowHealth(unit))
            {
                if (!(unit.Formation is OrbFormation))
                {
                    unit.Formation = new OrbFormation();
                    Debug.Log("Changed to formatino defense");
                }
            }
            else if (enemy != null && IsLowHealth(enemy))
            {
                if (!(unit.Formation is SkirmisherFormation))
                {
                    unit.Formation = new SkirmisherFormation();
                    Debug.Log("Changed to formatino atack");
                }
            }
            else
            {
                if (!(unit.Formation is SquareFormation))
                {
                    unit.Formation = new SquareFormation();
                    Debug.Log("Changed to formatino normo");
                }
            }
        }

        private static bool IsLowHealth(UnitBase unit)
        {
            return unit.Health < unit.MaxHealth * LowHealthPercentage;
        }

        protected override void ControllerTick()
        {
            if (Time.realtimeSinceStartup % TimeBetweenEnemySearches < Time.deltaTime)
                MoveToEnemey();
        }
    }
}