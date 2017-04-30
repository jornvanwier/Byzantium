using System;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Game.Units.Formation.ContuberniumFormation;
using Assets.Scripts.Game.Units.Formation.LegionFormation;
using Assets.Scripts.Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Controllers
{
    public class AiController : UnitController
    {
        private const float TimeBetweenEnemySearches = 5;
        private const float LowHealthPercentage = .3f;

        public override bool IsAi { get; } = true;

        private void MoveToEnemy()
        {
            UnitController nearestEnemy = NearestEnemy();
            if (nearestEnemy == null)
                return;

            Goal = nearestEnemy.Position;
            Debug.Log("Tick " + Goal);
        }

        protected override void ControllerAttack()
        {
            if (AttachedUnit.GetType() == typeof(Legion) &&
                AttachedUnit.Formation.GetType() != typeof(StandardFormation))
            {
                // Unit is marching, change
                Debug.Log("henlo i switch");
                AttachedUnit.Formation = new StandardFormation();
            }
        }

        protected override void ConsiderFormation(Contubernium unit)
        {
            // Is null at beginning and after killing a enemy (for a single frame)
            Contubernium enemy = unit.CurrentEnemy;

            Type formationType = unit.Formation.GetType();
            if (IsLowHealth(unit))
            {
                if (formationType != typeof(OrbFormation))
                    unit.Formation = new OrbFormation();
            }
            else if (enemy != null && IsLowHealth(enemy))
            {
                if (formationType != typeof(SkirmisherFormation))
                    unit.Formation = new SkirmisherFormation();
            }
            else
            {
                if (formationType != typeof(SquareFormation))
                    unit.Formation = new SquareFormation();
            }
        }

        private static bool IsLowHealth(UnitBase unit)
        {
            return unit.Health < unit.MaxHealth * LowHealthPercentage;
        }

        protected override void ControllerTick()
        {
            if (Time.realtimeSinceStartup % TimeBetweenEnemySearches < Time.deltaTime)
                MoveToEnemy();
        }
    }
}