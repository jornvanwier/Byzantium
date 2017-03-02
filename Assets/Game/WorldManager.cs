using Assets.Map;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Game
{
    public class WorldManager : MonoBehaviour
    {
        public GameObject MapRenderer;

        private GameObject cameraObject;
        private Vector2 aimPoint;
        private const float CameraHeight = 10;
        private const float CameraMoveSpeed = 16;
        private const float CameraRotateSpeed = 1;


        [UsedImplicitly]
        void Start()
        {
            MapRenderer = Instantiate(MapRenderer);
            cameraObject = new GameObject("CAMERA");
            cameraObject.AddComponent<Camera>();
            cameraObject.transform.position = new Vector3(0, CameraHeight, 0);
        }

        // Update is called once per frame
        [UsedImplicitly]
        void Update()
        {
            UpdateCamera();
        }

        Vector3 MultiplyVector(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        Vector3 NegateY(Vector3 vector)
        {
            return MultiplyVector(vector, new Vector3(1, 0, 1));
        }

        private void UpdateCamera()
        {
            Vector3 worldPosition = cameraObject.transform.position;
            Vector3 localPosition = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.position;

            Vector3 worldForward = cameraObject.transform.forward;
            Vector3 worldRight = cameraObject.transform.right;
            Vector3 worldUp = cameraObject.transform.up;

            Vector3 objectForward = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.forward;
            Vector3 objectRight = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.right;
            Vector3 objectUp = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.up;


            if (Input.GetKey(KeyCode.Space))
                cameraObject.transform.Translate(objectUp * CameraMoveSpeed * Time.deltaTime, Space.World);
            if (Input.GetKey(KeyCode.LeftShift))
                cameraObject.transform.Translate(objectUp * CameraMoveSpeed * -Time.deltaTime, Space.World);

            if (Input.GetKey(KeyCode.W))
                cameraObject.transform.Translate(NegateY(worldForward) * CameraMoveSpeed * Time.deltaTime, Space.World);
            if (Input.GetKey(KeyCode.A))
                cameraObject.transform.Translate(NegateY(-worldRight) * CameraMoveSpeed * Time.deltaTime, Space.World);
            if (Input.GetKey(KeyCode.S))
                cameraObject.transform.Translate(NegateY(-worldForward) * CameraMoveSpeed * Time.deltaTime, Space.World);
            if (Input.GetKey(KeyCode.D))
                cameraObject.transform.Translate(NegateY(worldRight) * CameraMoveSpeed * Time.deltaTime, Space.World);

            if (Input.GetKey(KeyCode.UpArrow))
                cameraObject.transform.Rotate(objectRight, CameraRotateSpeed);
            if (Input.GetKey(KeyCode.DownArrow))
                cameraObject.transform.Rotate(objectRight, -CameraRotateSpeed);
            if (Input.GetKey(KeyCode.RightArrow))
                cameraObject.transform.Rotate(Vector3.up, CameraRotateSpeed, Space.World);
            if (Input.GetKey(KeyCode.LeftArrow))
                cameraObject.transform.Rotate(Vector3.up, -CameraRotateSpeed, Space.World);

            if (Input.GetMouseButton(1))
                Debug.Log(1);
            if (Input.GetMouseButton(2))
                Debug.Log(2);
            if (Input.GetMouseButton(3))
                Debug.Log(3);
        }
    }
}