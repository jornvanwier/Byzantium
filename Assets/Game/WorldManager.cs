using Assets.Map;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game
{
    public class WorldManager : MonoBehaviour
    {
        public GameObject StartPin;
        public GameObject GoalPin;
        public GameObject MapRenderer;

        private GameObject cameraObject;
        public float CameraRotateSpeed = 50;
        public float InitialCameraAngle = 35;
        public float InitialZoomSpeed = 2;
        public float InitialCameraMoveSpeed = 2;

        private float CameraHeight => cameraObject?.transform.position.y ?? 10;
        private float CameraMoveSpeed => InitialCameraMoveSpeed * CameraHeight;
        private float ZoomSpeed => InitialZoomSpeed * (CameraHeight - 1);

        private bool applicationHasFocus;

        [UsedImplicitly]
        private void OnApplicationFocus(bool hasFocus)
        {
            applicationHasFocus = hasFocus;
        }

        [UsedImplicitly]
        void Start()
        {
            MapRenderer = Instantiate(MapRenderer);
            MapRenderer.name = "Map";
            MapRenderer.GetComponent<MapRenderer>().StartPin = StartPin;
            MapRenderer.GetComponent<MapRenderer>().GoalPin = GoalPin;
            float cHeight = CameraHeight;
            cameraObject = new GameObject("MainCamera");
            cameraObject.AddComponent<Camera>();
            cameraObject.transform.position = new Vector3(0, cHeight, 0);

            Vector3 objectRight = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.right;
            Rotate(objectRight, Space.Self, InitialCameraAngle);
        }

        // Update is called once per frame
        [UsedImplicitly]
        void Update()
        {
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
            Vector3 worldPosition = cameraObject.transform.position;
            Vector3 localPosition = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.position;

            Vector3 worldRight = cameraObject.transform.right;
            Vector3 worldUp = cameraObject.transform.up;
            Vector3 worldForward = cameraObject.transform.forward;

            Vector3 objectForward = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.forward;
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
            float zoom = Input.GetAxis("Mouse ScrollWheel");
            Zoom(worldForward, zoom);

            //Middle mouse drag and right mouse rotate
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 position = Input.mousePosition;
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Camera camera = cameraObject.GetComponent<Camera>();
                Ray ray = camera.ScreenPointToRay(position);
                if (plane.Raycast(ray, out float rayDistance))
                    startIntersect = ray.GetPoint(rayDistance);
                rightMouseDown = true;
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
                Vector3 intersect = Vector3.zero;
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Camera camera = cameraObject.GetComponent<Camera>();
                Ray ray = camera.ScreenPointToRay(position);
                plane.Raycast(ray, out float rayDistance);
                if (prevMousePos != Vector2.zero)
                {
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
        }

        private bool middleMouseDown;
        private bool rightMouseDown;
        private Vector2 prevMousePos = Vector2.zero;
        private Vector3 startIntersect;

        private void Ascend()
        {
            Vector3 objectUp = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.up;
            cameraObject.transform.Translate(objectUp * CameraMoveSpeed * Time.deltaTime, Space.World);
        }

        private void Descend()
        {
            Vector3 objectUp = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.up;
            cameraObject.transform.Translate(objectUp * -CameraMoveSpeed * Time.deltaTime, Space.World);
        }

        private void Pan(Vector3 direction, float multiplier = 1)
        {
            cameraObject.transform.Translate(NegateY(direction) * CameraMoveSpeed * Time.deltaTime * multiplier,
                Space.World);
        }

        private void Rotate(Vector3 around, Space space = Space.World, float multiplier = 1)
        {
            cameraObject.transform.Rotate(around, CameraRotateSpeed * multiplier * Time.deltaTime, space);
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
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }
    }
}