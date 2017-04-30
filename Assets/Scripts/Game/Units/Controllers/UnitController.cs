using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using Assets.Scripts.Map.Pathfinding;
using Assets.Scripts.UI;
using Assets.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Units.Controllers
{
    public abstract class UnitController : MonoBehaviour
    {
        private const float RotationSpeed = 3.5f;

        private static readonly float AttackRange = 8 * WorldManager.UnitScale;
        private ParticleSystem battleSmokeSystem;

        protected Camera Camera;

        private new BoxCollider collider;

        private PathfindingJobInfo currentPathInfo;

        protected List<UnitController> Enemies;
        protected WorldManager Manager;
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

        public abstract bool IsAi { get; }

        public int EmissionRateHigh = 200;
        public int EmissionRateLow = 0;

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
            SpawnObject.transform.localScale = new Vector3(0.2f, 0.4f, 0.2f);
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

            var healthObj = new GameObject("Health " + Faction.Name);
            var textObj = new GameObject("HealthText");
            healthObj.transform.SetParent(GameObject.Find("uiCanvas").transform);
            textObj.transform.SetParent(healthObj.transform);

            HealthBar = healthObj.AddComponent<HealthBar>();
            var text = textObj.AddComponent<Text>();
            HealthBar.HealthText = text;

            text.font = Font.CreateDynamicFontFromOSFont("AUGUSTUS", 14);
            text.alignment = TextAnchor.UpperCenter;
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 80);

            var outline = textObj.AddComponent<Outline>();
            outline.effectColor = Color.black;

            HealthBar.AttachArmy(this);

            collider = GetComponent<BoxCollider>();
            collider.size = new Vector3(4.84f, 1, 29.39f);
            collider.center = new Vector3(0, 0.5f, 0);
        }

        public void InitParticleSystem(Material smokeMaterial)
        {
            battleSmokeSystem = gameObject.AddComponent<ParticleSystem>();
            SetSmokeEmission(false);

            ParticleSystem.MainModule mainModule = battleSmokeSystem.main;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.Local;
            mainModule.startColor = new Color(.6f, .6f, .6f, .3f);
            mainModule.startSize = 13;

            ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetimeModule =
                battleSmokeSystem.limitVelocityOverLifetime;
            limitVelocityOverLifetimeModule.enabled = true;
            limitVelocityOverLifetimeModule.separateAxes = true;
            limitVelocityOverLifetimeModule.limitX = float.MaxValue;
            limitVelocityOverLifetimeModule.limitY = 0;
            limitVelocityOverLifetimeModule.limitZ = float.MaxValue;

            ParticleSystem.ShapeModule shapeModule = battleSmokeSystem.shape;
            shapeModule.shapeType = ParticleSystemShapeType.Sphere;
            shapeModule.radius = Mathf.Max(AttachedUnit.DrawSize.x, AttachedUnit.DrawSize.y) * 2;

            var pRenderer = GetComponent<ParticleSystemRenderer>();
            pRenderer.renderMode = ParticleSystemRenderMode.Billboard;
            pRenderer.material = smokeMaterial;
        }

        private void SetSmokeEmission(float rate)
        {
            ParticleSystem.EmissionModule emissionModule = battleSmokeSystem.emission;
            emissionModule.rateOverTime = rate;
        }

        private void SetSmokeEmission(bool on)
        {
            SetSmokeEmission(on ? EmissionRateHigh : EmissionRateLow);
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

        private void Attack(UnitController enemy)
        {
            if (enemy.AttachedUnit.Health <= 0)
            {
                AttachedUnit.RespectFormation = true;
                SetSmokeEmission(false);
                Debug.Log("Battle is over, " + Faction.Name + " won!");
                if (NearestEnemy() == null)
                {
                    Win();
                }
            }
            else if (AttachedUnit.Health <= 0)
            {
                SetSmokeEmission(false);
                Debug.Log("Battle is over, " + Faction.Name + " lost!");
            }
            else
            {
                AttachedUnit.RespectFormation = false;
                SetSmokeEmission(true);
                foreach (Contubernium unit in AttachedUnit.Contubernia)
                {
                    if (unit.CurrentEnemy == null)
                        unit.CurrentEnemy = unit.ClosestEnemy(enemy);
                    unit.Attack(unit.CurrentEnemy);
                    ConsiderFormation(unit);
                }
            }
        }

        private bool hasWon;

        public void Win()
        {
            if (hasWon) return;
            hasWon = true;

            foreach (MeshDrawableUnit unit in AttachedUnit.AllUnits)
            {
                unit.Bounce(Random.Range(50, 150) / 1000f);
            }
        }

        protected abstract void ConsiderFormation(Contubernium unit);

        protected abstract void ControllerTick();

        public void DestroyArmy()
        {
            Destroy(gameObject);
            HealthBar.Destroy();
        }

        public void Update()
        {
            if (hasWon)
            {
                foreach (MeshDrawableUnit unit in AttachedUnit.AllUnits)
                {
                    unit.JumpUpdate();
                }
            }

            if (AttachedUnit.IsDead)
                DestroyArmy();
            //if (AttachedUnit.IsDead) return;

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
            if (currentPathInfo.Path.Count < 1)
                return;

            Vector3 currentPos = CreateWorldPos();

            if (currentPathInfo.Path[0] == Position)
            {
                WalkedAfterRequest?.Add(Position);

                currentPathInfo.Path.RemoveAt(0);
                previousPosition = Position;
                movementDrawOffset = currentPos - MapRenderer.CubicalCoordinateToWorld(previousPosition);

                if (currentPathInfo.Path.Count < 1)
                    return;
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
                currentPathInfo.Path.RemoveAt(0);

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