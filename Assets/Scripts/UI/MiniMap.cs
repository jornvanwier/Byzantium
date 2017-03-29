using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class MiniMap : MonoBehaviour
    {
        private const float ZoomUpperLimit = 1000;
        private const float ZoomLowerLimit = 5;
        private Camera camera;
        private RawImage image;
        [Range(ZoomLowerLimit, ZoomUpperLimit)] public float InitialZoom = 100;

        [Range(0.1f, 5)] public float InitialZoomSpeed = 2f;

        private Camera mainCamera;

        private float ZoomSpeed
            => InitialZoomSpeed * (camera.transform.position.y - ZoomLowerLimit) / 100f;

        // Use this for initialization
        [UsedImplicitly]
        private void Start()
        {
            camera = GameObject.Find("MiniMapCamera").GetComponent<Camera>();
            image = GameObject.Find("MiniMapImage").GetComponent<RawImage>();

            SizeX = 200;
            SizeY = 200;
        }

        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
            //Mini map set position takes ~200 nanoseconds
            UpdateCamera();

            PosX = Screen.width - SizeX / 2;
            PosY = SizeY / 2;
        }

        private float posX;
        private float posY;

        public float PosX
        {
            get { return posX; }
            set
            {
                posX = value;
                image.transform.position = new Vector3(PosX, PosY);
            }
        }

        public float PosY
        {
            get { return posY; }
            set
            {
                posY = value;
                image.transform.position = new Vector3(PosX, PosY);
            }
        }

        private float sizeX;

        public float SizeX
        {
            get { return sizeX; }
            set
            {
                sizeX = value;
                image.rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
            }
        }

        private float sizeY;

        public float SizeY
        {
            get { return sizeY; }
            set
            {
                sizeY = value;
                image.rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
            }
        }

        public void AttachCamera(Camera mainCamera)
        {
            this.mainCamera = mainCamera;
        }

        private void UpdateCamera()
        {
            Vector3 camPos = mainCamera.transform.position;
            camera.transform.position = new Vector3(camPos.x, InitialZoom, camPos.z);

            if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus))
                InitialZoom -= ZoomSpeed;
            if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
            {
                InitialZoom += ZoomSpeed;
                if (InitialZoom > ZoomUpperLimit)
                    InitialZoom = ZoomUpperLimit;
            }
        }
    }
}