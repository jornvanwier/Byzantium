using System.Collections;
using System.Collections.Generic;
using Assets.Map;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public GameObject mapRenderer;

    private GameObject cameraObject;
    private Vector2 _aimPoint;


    void Start()
    {
        mapRenderer = Instantiate(mapRenderer);
        cameraObject = new GameObject("CAMERA");
        cameraObject.AddComponent<Camera>();
        cameraObject.transform.position = new Vector3(0, 10, 0);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
    }

    void UpdateCamera()
    {
        Vector3 worldPosition = cameraObject.transform.position;
        Vector3 localPosition = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.position;

        Vector3 worldForward = cameraObject.transform.forward;
        Vector3 worldRight = cameraObject.transform.right;
        Vector3 worldUp = cameraObject.transform.up;

        Vector3 objectForward = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.forward;
        Vector3 objectRight = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.right;
        Vector3 objectUp = cameraObject.transform.worldToLocalMatrix * cameraObject.transform.up;


        float moveSpeed = 16.0f;
        float rotateSpeed = 1.0f;

        if (Input.GetKey(KeyCode.W))
        {
            cameraObject.transform.Translate(objectForward * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            cameraObject.transform.Translate(-objectForward * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            cameraObject.transform.Translate(-objectRight * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            cameraObject.transform.Translate(objectRight * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            cameraObject.transform.Rotate(worldUp, rotateSpeed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            cameraObject.transform.Rotate(worldUp, -rotateSpeed);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            cameraObject.transform.Rotate(worldRight, rotateSpeed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            cameraObject.transform.Rotate(worldRight, -rotateSpeed);
        }
    }
}