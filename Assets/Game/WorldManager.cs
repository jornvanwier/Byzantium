﻿using Assets.Map;
 using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Game
{
    public class WorldManager : MonoBehaviour
    {
        public GameObject mapRenderer;
        public GameObject StartPin;
        public GameObject GoalPin;

        private GameObject cameraObject;
        private Vector2 aimPoint;


        [UsedImplicitly]
        void Start()
        {
            mapRenderer = Instantiate(mapRenderer);
            mapRenderer.GetComponent<MapRenderer>().StartPin = StartPin;
            mapRenderer.GetComponent<MapRenderer>().GoalPin = GoalPin;
            cameraObject = new GameObject("CAMERA");
            cameraObject.AddComponent<Camera>();
            cameraObject.transform.position = new Vector3(0, 10, 0);
        }

        // Update is called once per frame
        [UsedImplicitly]
        void Update()
        {
            UpdateCamera();
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


            const float moveSpeed = 16.0f;
            const float rotateSpeed = 1.0f;

            if (Input.GetKey(KeyCode.Space))
                cameraObject.transform.Translate(objectUp * moveSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.LeftShift))
                cameraObject.transform.Translate(objectUp * moveSpeed * -Time.deltaTime);

            if (Input.GetKey(KeyCode.W))
                cameraObject.transform.Translate(objectForward * moveSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.A))
                cameraObject.transform.Translate(-objectRight * moveSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.S))
                cameraObject.transform.Translate(-objectForward * moveSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.D))
                cameraObject.transform.Translate(objectRight * moveSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.UpArrow))
                cameraObject.transform.Rotate(objectRight, rotateSpeed);
            if (Input.GetKey(KeyCode.DownArrow))
                cameraObject.transform.Rotate(objectRight, -rotateSpeed);
            if (Input.GetKey(KeyCode.RightArrow))
                cameraObject.transform.Rotate(Vector3.up, rotateSpeed, Space.World);
            if (Input.GetKey(KeyCode.LeftArrow))
                cameraObject.transform.Rotate(Vector3.up, -rotateSpeed, Space.World);
        }
    }
}