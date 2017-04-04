using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Units;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using Assets.Scripts.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class WorldManager : MonoBehaviour
    {
        public static Material UnitMaterial;

        private readonly List<UnitController> allArmies = new List<UnitController>();
        private bool applicationHasFocus;

        private new Camera camera;
        private GameObject cameraObject;
        public float CameraRotateSpeed = 50;
        public Vector3 CameraStartPosition;

        public float CameraZoomLowerLimit = 1;

        public float CameraZoomUpperLimit = 1000;
        public GameObject Goal;

        private InfoPanel infoPanel;
        public float InitialCameraAngle = 35;
        public float InitialCameraMoveSpeed = 2;
        public float InitialZoomSpeed = 2;

        private Rect mapBounds;
        public GameObject MapRendererObject;
        protected MapRenderer MapRendererScript;

        public MeshHolder MeshHolder;

        private bool middleMouseDown;


        private Vector2 prevMousePos = Vector2.zero;
        private bool rightMouseDown;

        private UnitController selectedArmy;
        private Vector3 startIntersect;
        public Material TestMaterial;

        public Mesh TestMesh;

        private Canvas uiCanvas;
        public Material UMatter;

        private UnitBase unit;
        private UnitController unitController;

        public static MeshHolder Meshes { get; private set; }
        private float CameraHeight => cameraObject?.transform.position.y ?? CameraStartPosition.y;
        private float CameraMoveSpeed => InitialCameraMoveSpeed * CameraHeight;
        private float ZoomSpeed => InitialZoomSpeed * CameraHeight;

        public UnitController SelectedArmy
        {
            get { return selectedArmy; }
            set
            {
                DeselectAll();
                selectedArmy = value;
                Select(selectedArmy);
            }
        }

        [UsedImplicitly]
        private void OnApplicationFocus(bool hasFocus)
        {
            applicationHasFocus = hasFocus;
        }

        [UsedImplicitly]
        private void Start()
        {
            uiCanvas = GameObject.Find("uiCanvas").GetComponent<Canvas>();
            var loadingPanel = uiCanvas.GetComponent<LoadingPanel>();
            loadingPanel.Show();

            UnitMaterial = UMatter;
            // Ugly hack to allow static retrieval of the attached meshes
            MeshHolder.Initialize();
            Meshes = MeshHolder;

            var faction = new Faction();

            unit = Century.CreateMixedUnit(faction);
            unit.Position = new Vector3(5, 0, 5);


            MapRendererObject = Instantiate(MapRendererObject);
            MapRendererObject.name = "Map";
            cameraObject = new GameObject("MainCamera");
            cameraObject.AddComponent<Camera>();
            cameraObject.transform.position = new Vector3(CameraStartPosition.x, CameraStartPosition.y,
                CameraStartPosition.z);
            camera = cameraObject.GetComponent<Camera>();
            camera.farClipPlane = CameraZoomUpperLimit + 100;
            camera.nearClipPlane = 0.01f;

            MapRendererScript = MapRendererObject.GetComponent<MapRenderer>();

            var obj = new GameObject("Army");
            unitController = obj.AddComponent<UnitController>();
            unitController.AttachedUnit = unit;
            unitController.MapRenderer = MapRendererScript;
            unitController.Goal = MapRendererScript.WorldToCubicalCoordinate(Goal.transform.position);

            unitController.AttachCamera(camera);

            allArmies.Add(unitController);

            Vector3 objectRight = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.right;
            Rotate(objectRight, Space.Self, InitialCameraAngle);

            var miniMap = uiCanvas.GetComponent<MiniMap>();
            miniMap.AttachCamera(camera);
            miniMap.AttachMapObject(MapRendererObject);
            miniMap.AttachArmies(allArmies);

            miniMap.UpdateOverlayTexture();

            Vector3 pos = MapRendererObject.transform.position;
            int size = MapRendererObject.GetComponent<MapRenderer>().MapSize;
            var scale = new Vector3(size * 0.9296482412060302f, size, 1);
            pos = pos - scale / 2;
            mapBounds = new Rect(pos.x, pos.y, scale.x, scale.y);

            loadingPanel.Hide();
        }

        public void AttachInfoPanel(InfoPanel panel)
        {
            infoPanel = panel;
            infoPanel.Hide();
        }

        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
            unitController.Goal = MapRendererScript.WorldToCubicalCoordinate(Goal.transform.position);
            UpdateCamera();
        }

        private static Vector3 MultiplyVector(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        private static Vector3 NegateY(Vector3 vector)
        {
            Vector3 result = MultiplyVector(vector, new Vector3(1, 0, 1));
            return Vector3.Normalize(result);
        }

        private void UpdateCamera()
        {
            Vector3 worldRight = cameraObject.transform.right;
            Vector3 worldForward = cameraObject.transform.forward;
            Vector3 objectRight = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.right;

            //Keyboard movement
            if (Input.GetKey(KeyCode.Space))
                Ascend();
            if (Input.GetKey(KeyCode.LeftShift))
                Descend();

            if (Input.GetKey(KeyCode.W))
                Pan(worldForward);
            if (Input.GetKey(KeyCode.A))
                Pan(worldRight, -1f);
            if (Input.GetKey(KeyCode.S))
                Pan(worldForward, -1f);
            if (Input.GetKey(KeyCode.D))
                Pan(worldRight);

            if (Input.GetKey(KeyCode.UpArrow))
                Rotate(objectRight, Space.Self);
            if (Input.GetKey(KeyCode.DownArrow))
                Rotate(objectRight, Space.Self, -1f);
            if (Input.GetKey(KeyCode.RightArrow))
                Rotate(Vector3.up);
            if (Input.GetKey(KeyCode.LeftArrow))
                Rotate(Vector3.up, Space.World, -1f);

            //Mouse scroll zoom
            Vector3 prevPos = Clone(cameraObject.transform.position);

            float zoom = Input.GetAxis("Mouse ScrollWheel");
            Zoom(worldForward, zoom);
            if (Math.Abs(zoom) > 0.001f)
                CheckBounds(prevPos);

            //Middle mouse drag and right mouse rotate
            if (Input.GetMouseButtonDown(1))
            {
                prevPos = Clone(cameraObject.transform.position);

                Vector3 position = new Vector2(Screen.width / 2, Screen.height / 2);
                var plane = new Plane(Vector3.up, Vector3.zero);
                var camera = cameraObject.GetComponent<Camera>();
                Ray ray = camera.ScreenPointToRay(position);
                if (plane.Raycast(ray, out float rayDistance))
                    startIntersect = ray.GetPoint(rayDistance);
                rightMouseDown = true;

                CheckBounds(prevPos);
            }
            if (Input.GetMouseButtonUp(1))
            {
                rightMouseDown = false;
                prevMousePos = Vector2.zero;
            }
            if (Input.GetMouseButtonDown(2))
                middleMouseDown = true;
            if (Input.GetMouseButtonUp(2))
            {
                middleMouseDown = false;
                prevMousePos = Vector2.zero;
            }
            if (middleMouseDown || rightMouseDown)
            {
                Vector2 position = Input.mousePosition;
                var plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = camera.ScreenPointToRay(position);
                plane.Raycast(ray, out float rayDistance);
                if (prevMousePos != Vector2.zero)
                {
                    prevPos = Clone(cameraObject.transform.position);
                    Vector2 movement = prevMousePos - position;
                    if (middleMouseDown)
                    {
                        movement *= rayDistance / 10.5f;

                        cameraObject.transform.Translate(new Vector3(movement.x, 0, 0) * Time.deltaTime);
                        cameraObject.transform.Translate(NegateY(worldForward) * movement.y * Time.deltaTime,
                            Space.World);
                    }
                    if (rightMouseDown)
                        cameraObject.transform.RotateAround(startIntersect, Vector3.up, -movement.x);
                    CheckBounds(prevPos);
                }
                prevMousePos = position;
            }


            //Border mouse move
            if (!middleMouseDown && !rightMouseDown && applicationHasFocus)
            {
                const int margin = 10;
                if (Input.mousePosition.x < margin)
                    Pan(worldRight, -1f);
                if (Input.mousePosition.y < margin)
                    Pan(worldForward, -1f);
                if (Input.mousePosition.y > Screen.height - margin)
                    Pan(worldForward);
                if (Input.mousePosition.x > Screen.width - margin)
                    Pan(worldRight);
            }

            //Click units
            if (Input.GetMouseButtonUp(0))
            {
                DeselectAll();
                Vector2 position = Input.mousePosition;
                var plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = camera.ScreenPointToRay(position);
                plane.Raycast(ray, out float rayDistance);
                Vector3 intersectPoint = ray.GetPoint(rayDistance);
                var intersect = new Vector2(intersectPoint.x, intersectPoint.z);
                foreach (UnitController controller in allArmies)
                    if (controller.AttachedUnit.Hitbox.Contains(intersect))
                        SelectedArmy = controller;
            }

            foreach (UnitController controller in allArmies)
            {
                var meshDrawableUnits = controller.AttachedUnit as Contubernium;
                if (meshDrawableUnits == null) continue;
                foreach (MeshDrawableUnit meshDrawableUnit in meshDrawableUnits)
                    Graphics.DrawMesh(TestMesh,
                        Matrix4x4.TRS(meshDrawableUnit.Position, Quaternion.Euler(0, 0, 0),
                            new Vector3(0.01f, 0.01f, 0.01f)), TestMaterial, 0);
            }
        }

        private void Select(UnitController army)
        {
            army.HealthBar.Show();
            infoPanel.Title = army.AttachedUnit.Info;
            infoPanel.Show();
        }

        private void Deselect(UnitController army)
        {
            army.HealthBar.Hide();
            infoPanel.Hide();
        }

        private void DeselectAll()
        {
            foreach (UnitController controller in allArmies)
                Deselect(controller);
        }


        private void Ascend()
        {
            Vector3 prevPos = Clone(cameraObject.transform.position);

            Vector3 objectUp = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.up;
            cameraObject.transform.Translate(objectUp * CameraMoveSpeed * Time.deltaTime, Space.World);

            CheckBounds(prevPos);
        }

        private void Descend()
        {
            Vector3 prevPos = Clone(cameraObject.transform.position);

            Vector3 objectUp = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.up;
            cameraObject.transform.Translate(objectUp * -CameraMoveSpeed * Time.deltaTime, Space.World);

            CheckBounds(prevPos);
        }


        private void CheckBounds(Vector3 prevPos)
        {
            if (CameraHeight < CameraZoomLowerLimit)
                cameraObject.transform.position = prevPos;
            else if (CameraHeight > CameraZoomUpperLimit)
                cameraObject.transform.position = prevPos;
            var position = new Vector2(cameraObject.transform.position.x, cameraObject.transform.position.z);
            if (!mapBounds.Contains(position))
                cameraObject.transform.position = prevPos;
        }

        private void Pan(Vector3 direction, float multiplier = 1)
        {
            Vector3 prevPos = Clone(cameraObject.transform.position);

            cameraObject.transform.Translate(NegateY(direction) * CameraMoveSpeed * Time.deltaTime * multiplier,
                Space.World);

            CheckBounds(prevPos);
        }

        private void Rotate(Vector3 around, Space space = Space.World, float multiplier = 1)
        {
            Vector3 prevPos = Clone(cameraObject.transform.position);

            cameraObject.transform.Rotate(around, CameraRotateSpeed * multiplier * Time.deltaTime, space);

            CheckBounds(prevPos);
        }

        private void Zoom(Vector3 forward, float zoom)
        {
            cameraObject.transform.Translate(forward * ZoomSpeed * zoom * 10 * Time.deltaTime, Space.World);
        }

        public static Vector2 RotateVector(Vector2 v, float radians)
        {
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            float tx = v.x;
            float ty = v.y;
            v.x = cos * tx - sin * ty;
            v.y = sin * tx + cos * ty;
            return v;
        }

        public static Vector2 Clone(Vector2 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector3 Clone(Vector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
    }
}