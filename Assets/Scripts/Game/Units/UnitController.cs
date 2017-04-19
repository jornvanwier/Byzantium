using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using Assets.Scripts.Map.Pathfinding;
using Assets.Scripts.UI;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public class UnitController : MonoBehaviour
    {
        private const float RotationSpeed = 3.5f;

        private const float AttackRange = 3;

        private const float TimeBetweenEnemySearches = 5;

        private new Camera camera;

        private PathfindingJobInfo currentPathInfo;

        private List<UnitController> enemies;
        private MapRenderer mapRenderer;

        private Vector3 movementDrawOffset;
        private int nextPathId = -1;
        private CubicalCoordinate previousPosition;
        private Vector3 spawnPosition;
        public UnitBase AttachedUnit { get; private set; }

        public MapRenderer MapRenderer { get; set; }

        public CubicalCoordinate Position { get; set; }


        public CubicalCoordinate Goal { get; set; }

        public HealthBar HealthBar { get; private set; }

        public Faction Faction => AttachedUnit.Commander.Faction;

        public GameObject SpawnObject { get; set; }


        public void AddUnit(Cohort unit)
        {
            if (AttachedUnit is Legion legion)
                legion.AddUnit(unit);
        }

        public void AddUnit(Century unit)
        {
            if (AttachedUnit is Cohort cohort)
                cohort.AddUnit(unit);
            if (AttachedUnit is Legion legion)
                legion.AddUnit(unit);
        }

        public void AddUnit(Contubernium unit)
        {
            if (AttachedUnit is Century century)
                century.AddUnit(unit);
            if (AttachedUnit is Cohort cohort)
                cohort.AddUnit(unit);
            if (AttachedUnit is Legion legion)
                legion.AddUnit(unit);
        }

        public void AddUnit(UnitBase unit)
        {
            if (unit is Contubernium contubernium)
                AddUnit(contubernium);
            else if (unit is Century century)
                AddUnit(century);
            else if (unit is Cohort cohort)
                AddUnit(cohort);
            //else if (unit is Legion legion)
            //    AddUnit(legion);
        }

        public void AttachUnit(UnitBase unit)
        {
            AttachedUnit = unit;
        }

        private Vector3 GetSpawnPosition()
        {
            CubicalCoordinate buildingCc = mapRenderer.HexBoard.RandomValidTile();
            return mapRenderer.CubicalCoordinateToWorld(buildingCc);
        }

        public void AttachEnemies(List<UnitController> armies)
        {
            enemies = armies.Where(controller => controller != this).ToList();
        }

        public void CreateBuilding(Vector3 position)
        {
            SpawnObject.AddComponent<BoxCollider>();
            SpawnObject.transform.position = position;
            SpawnObject.name = "Spawn " + Faction.Name;
        }

        public void AttachMapRenderer(MapRenderer renderer)
        {
            mapRenderer = renderer;
        }

        public void Start()
        {
            Position = MapRenderer.WorldToCubicalCoordinate(transform.position);
            previousPosition = Position;

            var obj = new GameObject("ArmyHealth");
            obj.transform.SetParent(GameObject.Find("uiCanvas").transform);
            HealthBar = obj.AddComponent<HealthBar>();
        }


        public void AttachCamera(Camera camera)
        {
            this.camera = camera;
        }

        public void Teleport(Vector3 loc)
        {
            transform.position = loc;
            Position = MapRenderer.WorldToCubicalCoordinate(loc);
            previousPosition = Position;
            movementDrawOffset = new Vector3(0, 0, 0);
            AttachedUnit.SetPositionInstant(loc);
        }

        private void UpdateHealthBar()
        {
            if (camera == null) return;
            Vector3 healthBarPosition = transform.position + Vector3.up;
            Vector3 point = camera.WorldToScreenPoint(healthBarPosition);
            HealthBar.PosX = point.x;
            HealthBar.PosY = point.y;

            HealthBar.Value = AttachedUnit.Health;
        }


        private void CombatTick()
        {
            // Check range
        }


        private UnitController NearestEnemy()
        {
            if (enemies.Count == 0) return null;
            UnitController nearest = enemies[0];
            float nearestDistance = Vector3.Distance(nearest.AttachedUnit.Position, AttachedUnit.Position);
            for (int i = 1; i < enemies.Count; i++)
            {
                UnitController enemy = enemies[i];
                float distance = Vector3.Distance(enemy.AttachedUnit.Position, AttachedUnit.Position);
                if (!(distance < nearestDistance)) continue;
                nearestDistance = distance;
                nearest = enemy;
            }
            return nearest;
        }

        private void Battle()
        {
            //UnitController nearestEnemy = NearestEnemy();
            //if (nearestEnemy == null)
            //{
            //    Debug.LogError("Nearest enemy is null");
            //    return;
            //}

            //Goal = nearestEnemy.Position;
            //Debug.Log("Tick " + Goal);
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(AttachedUnit.Position, new Vector3(AttachedUnit.DrawSize.y, 0, AttachedUnit.DrawSize.x));
            Gizmos.color = Color.red;
            Gizmos.DrawCube(AttachedUnit.Position, new Vector3(MeshDrawableUnit.manSize.y, 0, MeshDrawableUnit.manSize.x));
        }

        public void Update()
        {
            


            if (Time.realtimeSinceStartup % TimeBetweenEnemySearches < Time.deltaTime)
                Battle();

            if (enemies != null)
                foreach (UnitController enemy in enemies)
                {
                    float distance = Vector3.Distance(enemy.AttachedUnit.Position, AttachedUnit.Position);
                    if (distance < AttackRange)
                        Debug.Log("Attack!");
                }

            if (mapRenderer.HexBoard != null && AttachedUnit != null && spawnPosition == Vector3.zero)
            {
                spawnPosition = GetSpawnPosition();
                CreateBuilding(spawnPosition);
                Teleport(spawnPosition);
                camera.transform.position = spawnPosition + new Vector3(5, 10, 0);
                camera.transform.LookAt(spawnPosition);
            }

            UpdateHealthBar();
            AttachedUnit.Draw();

            if (currentPathInfo?.Path != null)
                foreach (CubicalCoordinate c in currentPathInfo.Path)
                    MapRenderer.MarkTileSelectedForNextFrame(c);


            Vector3 worldPos = CreateWorldPos();
            SetWorldPosition(worldPos);

            Position = MapRenderer.WorldToCubicalCoordinate(worldPos);

            if (MapRenderer.HexBoard[Goal] == (byte) TileType.WaterDeep || Position == Goal)
                return;


            if (currentPathInfo != null)
                AdvanceOnPath();

            // If the path is valid there is no need to calculate a new one
            if (IsPathValid()) return;

            if (nextPathId == -1)
            {
                RequestNewPath();
            }
            else
            {
                // Check on the state of the job
                if (PathfindingJobManager.GetInfo(nextPathId).State == JobState.Failure)
                {
                    // Pathing has failed for some reason, lets try again
                    RequestNewPath();
                }
                else if (PathfindingJobManager.IsFinished(nextPathId))
                {
                    currentPathInfo = PathfindingJobManager.GetInfo(nextPathId);
                    PathfindingJobManager.ClearJob(nextPathId);

                    nextPathId = -1;
                }
            }
        }

        protected void AdvanceOnPath()
        {
            Vector3 currentPos = CreateWorldPos();

            if (currentPathInfo.Path[0] == Position)
            {
                currentPathInfo.Path.RemoveAt(0);
                previousPosition = Position;
                movementDrawOffset = currentPos - MapRenderer.CubicalCoordinateToWorld(previousPosition);
            }

            Vector3 nextPos = Vector3.MoveTowards(currentPos,
                MapRenderer.CubicalCoordinateToWorld(currentPathInfo.Path[0]),
                AttachedUnit.WalkSpeed * Time.deltaTime);
            movementDrawOffset = nextPos - MapRenderer.CubicalCoordinateToWorld(previousPosition);

            SetWorldRotation(Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(nextPos - MapRenderer.CubicalCoordinateToWorld(currentPathInfo.Path[0])),
                Time.deltaTime * RotationSpeed)
            );
        }

        protected void SetUnitWorldPos(Vector3 position)
        {
            AttachedUnit.Position = position;
        }

        protected void SetUnitWorldRotation(Quaternion rotation)
        {
            AttachedUnit.Rotation = rotation;
        }

        protected void SetWorldPosition(Vector3 position)
        {
            SetUnitWorldPos(position);
            transform.position = position;
        }

        protected void SetWorldRotation(Quaternion rotation)
        {
            SetUnitWorldRotation(rotation);
            transform.rotation = rotation;
        }

        protected Vector3 CreateWorldPos()
        {
            return MapRenderer.CubicalCoordinateToWorld(previousPosition) + movementDrawOffset;
        }

        protected void RequestNewPath()
        {
            nextPathId = PathfindingJobManager.CreateJob(Position, Goal);
            Debug.Log($"{Faction.Name} requested new path with id {nextPathId}.");
        }

        protected bool IsPathValid()
        {
            return currentPathInfo?.GoalPos == Goal;
        }
    }
}