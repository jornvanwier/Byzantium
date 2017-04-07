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

        private new Camera camera;

        private PathfindingJobInfo currentPathInfo;

        private Vector3 movementDrawOffset;
        private int nextPathId = -1;
        private CubicalCoordinate previousPosition;

        public UnitBase AttachedUnit { get; set; }

        public MapRenderer MapRenderer { get; set; }

        public CubicalCoordinate Position { get; set; }
        public CubicalCoordinate Goal { get; set; }
        public HealthBar HealthBar { get; private set; }

        public Faction Faction => AttachedUnit.Commander.Faction;
        public GameObject SpawnObject { get; private set; }
        public Mesh SpawnMesh;
        public Material SpawnMeshMaterial;
        private MapRenderer mapRenderer;

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


        private void UpdateHealthBar()
        {
            if (camera == null) return;
            Vector3 healthBarPosition = transform.position + Vector3.up;
            Vector3 point = camera.WorldToScreenPoint(healthBarPosition);
            HealthBar.PosX = point.x;
            HealthBar.PosY = point.y;

            HealthBar.Value = AttachedUnit.Health;
        }

        public void CreateBuilding()
        {
            SpawnObject = new GameObject("SpawnHouse");

            CubicalCoordinate buildingCc = mapRenderer.HexBoard.RandomValidTile();
            Vector3 buildingPos = mapRenderer.CubicalCoordinateToWorld(buildingCc);
            SpawnObject.transform.position = buildingPos;

            var meshFilter = SpawnObject.AddComponent<MeshFilter>();
            var meshRenderer = SpawnObject.AddComponent<MeshRenderer>();
            SpawnObject.AddComponent<BoxCollider>();
            meshFilter.mesh = SpawnMesh;
            meshRenderer.material = SpawnMeshMaterial;

            SetUnitWorldPos(buildingPos);
        }

        public void Update()
        {
            if (SpawnObject == null && mapRenderer?.HexBoard != null)
            {
                CreateBuilding();
            }

            UpdateHealthBar();
            AttachedUnit.Draw();

            if (currentPathInfo?.Path != null)
                foreach (CubicalCoordinate c in currentPathInfo.Path)
                    MapRenderer.MarkTileSelectedForNextFrame(c);

            SetWorldPosition(CreateWorldPos());

            Position = MapRenderer.WorldToCubicalCoordinate(CreateWorldPos());

            if (Position == Goal)
                return;
            if (IsPathValid())
            {
                AdvanceOnPath();
            }
            else
            {
                if (nextPathId == -1)
                {
                    RequestNewPath();
                }
                else
                {
                    // Check on the state of the job
                    if (PathfindingJobManager.Instance.GetInfo(nextPathId).State == JobState.Failure)
                    {
                        // Pathing has failed for some reason, lets try again
                        RequestNewPath();
                    }
                    else if (PathfindingJobManager.Instance.IsFinished(nextPathId))
                    {
                        currentPathInfo = PathfindingJobManager.Instance.GetInfo(nextPathId);
                        PathfindingJobManager.Instance.ClearJob(nextPathId);

                        nextPathId = -1;
                        AdvanceOnPath();
                    }
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
            nextPathId = PathfindingJobManager.Instance.CreateJob(Position, Goal);
        }

        protected bool IsPathValid()
        {
            return currentPathInfo?.GoalPos == Goal;
        }
    }
}