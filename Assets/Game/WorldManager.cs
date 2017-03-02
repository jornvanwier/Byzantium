﻿using Assets.Map;
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
        private Vector2 aimPoint;
        private const float CameraRotateSpeed = 50;

        private float CameraHeight => cameraObject?.transform.position.y ?? 10;
        private float CameraMoveSpeed => 2 * CameraHeight;
        private float ZoomSpeed => 2 * (CameraHeight - 1);


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

            float zoom = Input.GetAxis("Mouse ScrollWheel");
            Zoom(worldForward, zoom);


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
    }
}