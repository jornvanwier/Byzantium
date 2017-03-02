using Assets.Map;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

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

            Vector3 worldRight = cameraObject.transform.right;
            Vector3 worldUp = cameraObject.transform.up;
            Vector3 worldForward = cameraObject.transform.forward;

            Vector3 objectForward = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.forward;
            Vector3 objectRight = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.right;
            Vector3 objectUp = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.up;


            if (Input.GetKey(KeyCode.Space))
                Pan(cameraObject, objectUp);
            if (Input.GetKey(KeyCode.LeftShift))
                Pan(cameraObject, -objectUp);

            if (Input.GetKey(KeyCode.W))
                Pan(cameraObject, worldForward);
            if (Input.GetKey(KeyCode.A))
                Pan(cameraObject, -worldRight);
            if (Input.GetKey(KeyCode.S))
                Pan(cameraObject, -worldForward);
            if (Input.GetKey(KeyCode.D))
                Pan(cameraObject, worldRight);

            if (Input.GetKey(KeyCode.UpArrow))
                Rotate(cameraObject, objectRight);
            if (Input.GetKey(KeyCode.DownArrow))
                Rotate(cameraObject, -objectRight);
            if (Input.GetKey(KeyCode.RightArrow))
                Rotate(cameraObject, Vector3.up);
            if (Input.GetKey(KeyCode.LeftArrow))
                Rotate(cameraObject, -Vector3.up);

            float zoom = Input.GetAxis("Mouse ScrollWheel");
            cameraObject.transform.Translate(worldForward * CameraMoveSpeed * zoom * 10 * Time.deltaTime, Space.World);
        }

        private void Pan(GameObject cameraObject, Vector3 direction)
        {
            cameraObject.transform.Translate(NegateY(direction) * CameraMoveSpeed * Time.deltaTime, Space.World);
        }

        private void Rotate(GameObject cameraObject, Vector3 around)
        {
            cameraObject.transform.Rotate(around, CameraRotateSpeed, Space.World);
        }

        private void Zoom(GameObject cameraObject)
        {
        }
    }
}