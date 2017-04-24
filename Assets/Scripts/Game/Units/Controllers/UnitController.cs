using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using Assets.Scripts.Map.Pathfinding;
using Assets.Scripts.UI;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Controllers
{
    public abstract class UnitController : MonoBehaviour
    {
        protected WorldManager Manager;
        private const float RotationSpeed = 3.5f;

        protected Camera Camera;

        private PathfindingJobInfo currentPathInfo;

        protected List<UnitController> Enemies;
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

        public HashSet<CubicalCoordinate> WalkedAfterRequest { get; set; }

        public void AddUnit(Cohort unit)
        {
            if (AttachedUnit is Legion legion)
                legion.AddUnit(unit);
        }

        public void AttachWorldManager(WorldManager manager)
        {
            Manager = manager;
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
            Enemies = armies.Where(controller => controller != this).ToList();
        }

        public void CreateBuilding(Vector3 position)
        {
            SpawnObject.transform.position = position;
            SpawnObject.name = "Spawn " + Faction.Name;
        }

        public void AttachMapRenderer(MapRenderer renderer)
        {
            mapRenderer = renderer;
        }

        private new BoxCollider collider;

        public void Start()
        {
            Position = MapRenderer.WorldToCubicalCoordinate(transform.position);
            previousPosition = Position;

            var obj = new GameObject("ArmyHealth");
            obj.transform.SetParent(GameObject.Find("uiCanvas").transform);
            HealthBar = obj.AddComponent<HealthBar>();
            HealthBar.AttachArmy(this);
            collider = GetComponent<BoxCollider>();
            collider.size = new Vector3(4.84f, 1, 29.39f);
            collider.center = new Vector3(-1.69f, 0.44f, -4.84f);
            collider.center = new Vector3(0, 0.5f, 0);
        }

        public void OnDrawGizmos()
        {
            Color c = Color.cyan;
            c.a = 0.4f;
            Gizmos.color = c;
            Gizmos.DrawCube(AttachedUnit.Position, new Vector3(AttachedUnit.DrawSize.x, 0, AttachedUnit.DrawSize.y));
        }

        public void AttachCamera(Camera camera)
        {
            Camera = camera;
        }

        public static void Teleport(Vector3 loc, UnitBase unit)
        {
            unit.SetPositionInstant(loc);
        }

        public void Teleport(Vector3 loc)
        {
            transform.position = loc;
            Position = MapRenderer.WorldToCubicalCoordinate(loc);
            previousPosition = Position;
            movementDrawOffset = new Vector3(0, 0, 0);
            Teleport(loc, AttachedUnit);
        }

        private void UpdateHealthBar()
        {
            if (Camera == null) return;
            Vector3 healthBarPosition = transform.position + Vector3.up;
            Vector3 point = Camera.WorldToScreenPoint(healthBarPosition);
            HealthBar.PosX = point.x;
            HealthBar.PosY = point.y;

            HealthBar.Value = AttachedUnit.Health;
        }

        private const float AttackRange = 8;

        private void Attack(UnitController enemy)
        {
            foreach (MeshDrawableUnit unit in AttachedUnit.AllUnits)
            {
                float distance = Vector3.Distance(enemy.AttachedUnit.Position, unit.Position);
                if (distance < AttackRange)
                    unit.Attack(enemy.AttachedUnit);
            }
        }

        public abstract bool IsAi { get; }
        protected abstract void ControllerTick();

        public void Update()
        {
            ControllerTick();

            collider.size = new Vector3(AttachedUnit.DrawSize.x, 1, AttachedUnit.DrawSize.y);

            if (Enemies != null && AttachedUnit.Position != Vector3.zero)
                foreach (UnitController enemy in Enemies)
                {
                    float distance = Vector3.Distance(enemy.AttachedUnit.Position, AttachedUnit.Position);
                    if (distance < AttackRange)
                        Attack(enemy);
                }

            if (mapRenderer.HexBoard != null && AttachedUnit != null && spawnPosition == Vector3.zero)
            {
                spawnPosition = GetSpawnPosition();
                CreateBuilding(spawnPosition + new Vector3(7, 0, 0));
                Teleport(spawnPosition);
                if (!IsAi)
                {
                    Camera.transform.position = spawnPosition + new Vector3(-30, 15, 0);
                    Camera.transform.LookAt(spawnPosition);
                }
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
                if (PathfindingJobManager.GetInfo(nextPathId)?.State == JobState.Failure)
                {
                    // Pathing has failed for some reason, lets try again
                    RequestNewPath();
                }
                else if (PathfindingJobManager.IsFinished(nextPathId))
                {
                    currentPathInfo = PathfindingJobManager.GetInfo(nextPathId);
                    PathfindingJobManager.ClearJob(nextPathId);

                    RemoveWalkedPath();

                    nextPathId = -1;
                }
            }
        }

        protected void AdvanceOnPath()
        {
            Vector3 currentPos = CreateWorldPos();

            if (currentPathInfo.Path[0] == Position)
            {
                WalkedAfterRequest?.Add(Position);

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

        protected void RemoveWalkedPath()
        {
            while (WalkedAfterRequest.Contains(currentPathInfo.Path[0]))
            {
                currentPathInfo.Path.RemoveAt(0);
            }

            WalkedAfterRequest = null;
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
            WalkedAfterRequest = new HashSet<CubicalCoordinate> {Position};
        }

        protected bool IsPathValid()
        {
            return currentPathInfo?.GoalPos == Goal;
        }
    }
}