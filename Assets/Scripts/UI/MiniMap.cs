using System.Collections.Generic;
using Assets.Scripts.Game.Units;
using Game.Units;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class MiniMap : MonoBehaviour
    {
        private const float ZoomUpperLimit = 1000;
        private const float ZoomLowerLimit = 5;

        private List<UnitController> armies;

        private Image border;

        [Range(0.1f, 100)] public float BorderSize = 10;
        private Camera camera;
        private RawImage image;
        [Range(ZoomLowerLimit, ZoomUpperLimit)] public float InitialZoom = 100;

        [Range(0.1f, 5)] public float InitialZoomSpeed = 2f;

        private Camera mainCamera;

        private GameObject mapObject;

        private float posX;
        private float posY;

        private float sizeX;

        private float sizeY;
        private RawImage unitOverlay;

        private float ZoomSpeed
            => InitialZoomSpeed * (camera.transform.position.y - ZoomLowerLimit) / 100f;

        [SerializeField]
        public float PosX
        {
            get { return posX; }
            set
            {
                posX = value;
                image.transform.position = new Vector3(PosX - BorderSize, PosY + BorderSize);
                unitOverlay.transform.position = new Vector3(PosX - BorderSize, PosY + BorderSize);
                border.transform.position = new Vector3(PosX - BorderSize, PosY + BorderSize);
            }
        }

        public float PosY
        {
            get { return posY; }
            set
            {
                posY = value;
                image.transform.position = new Vector3(PosX - BorderSize, PosY + BorderSize);
                unitOverlay.transform.position = new Vector3(PosX - BorderSize, PosY + BorderSize);
                border.transform.position = new Vector3(PosX - BorderSize, PosY + BorderSize);
            }
        }

        public float SizeX
        {
            get { return sizeX; }
            set
            {
                sizeX = value;
                image.rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
                unitOverlay.rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
                border.rectTransform.sizeDelta = new Vector2(SizeX + BorderSize * 2, SizeY + BorderSize * 2);
            }
        }

        public float SizeY
        {
            get { return sizeY; }
            set
            {
                sizeY = value;
                image.rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
                unitOverlay.rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
                border.rectTransform.sizeDelta = new Vector2(SizeX + BorderSize * 2, SizeY + BorderSize * 2);
            }
        }

        // Use this for initialization
        [UsedImplicitly]
        private void Start()
        {
            camera = GameObject.Find("MiniMapCamera").GetComponent<Camera>();
            image = GameObject.Find("MiniMapImage").GetComponent<RawImage>();
            border = GameObject.Find("MiniMapBorder").GetComponent<Image>();
            unitOverlay = GameObject.Find("UnitOverlay").GetComponent<RawImage>();

            UpdatePositionAndSize();
        }

        private void UpdatePositionAndSize()
        {
            PosX = Screen.width - SizeX / 2;
            PosY = SizeY / 2;

            SizeX = 200;
            SizeY = 200;
        }

        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
            //Mini map set position takes ~200 nanoseconds
            UpdateCamera();
            UpdatePositionAndSize();
        }

        public void UpdateOverlayTexture()
        {
            Texture2D newTex = GetTexture();
            newTex.Apply();
            unitOverlay.texture = newTex;
        }

        public void AttachArmies(List<UnitController> armies)
        {
            this.armies = armies;
        }

        private Texture2D GetTexture()
        {
            var t = new Texture2D((int) SizeX, (int) SizeY);

            foreach (UnitController army in armies)
            foreach (MeshDrawableUnit drawableUnit in army.AttachedUnit.AllUnits)
                Debug.Log(drawableUnit);

            for (int x = 0; x < SizeX; x++)
            for (int y = 0; y < SizeY; y++)
                t.SetPixel(x, y, Random.Range(0, 2) == 1 ? Color.red : new Color(0, 0, 0, 0));

            return t;
        }

        public void AttachMapObject(GameObject mapRenderer)
        {
            mapObject = mapRenderer;
            mapObject.layer = LayerMask.NameToLayer("MapLayer");
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