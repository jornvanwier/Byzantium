using System;
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

        protected UnitController NearestEnemy()
        {
            if (Enemies.Count == 0) return null;
            UnitController nearest = Enemies[0];
            float nearestDistance = Vector3.Distance(nearest.AttachedUnit.Position, AttachedUnit.Position);
            for (int i = 1; i < Enemies.Count; i++)
            {
                UnitController enemy = Enemies[i];
                float distance = Vector3.Distance(enemy.AttachedUnit.Position, AttachedUnit.Position);
                if (!(distance < nearestDistance)) continue;
                nearestDistance = distance;
                nearest = enemy;
            }
            return nearest;
        }

        private void MoveToEnemy()
        {
            UnitController nearestEnemy = NearestEnemy();
            if (nearestEnemy == null)
                return;

            Goal = nearestEnemy.Position;
            Debug.Log("Tick " + Goal);
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