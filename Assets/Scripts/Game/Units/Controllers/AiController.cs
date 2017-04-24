using UnityEngine;

namespace Assets.Scripts.Game.Units.Controllers
{
    public class AiController : UnitController
    {
        private const float TimeBetweenEnemySearches = 5;

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

        private void MoveToEnemey()
        {
            UnitController nearestEnemy = NearestEnemy();
            if (nearestEnemy == null)
            {
                Debug.LogError("Nearest enemy is null");
                return;
            }

            Goal = nearestEnemy.Position;
            Debug.Log("Tick " + Goal);
        }

        protected override void ControllerTick()
        {
            if (Time.realtimeSinceStartup % TimeBetweenEnemySearches < Time.deltaTime)
                MoveToEnemey();
        }
    }
}