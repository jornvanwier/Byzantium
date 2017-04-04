using System.Collections.Generic;
using Assets.Scripts.Game.Units;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class MiniMap : MonoBehaviour
    {
        private const float ZoomUpperLimit = 1000;
        private const float ZoomLowerLimit = 1;

        private readonly Color transparent = new Color(0, 0, 0, 0);

        private List<UnitController> armies;

        private Image border;

        [Range(0.1f, 100)] public float BorderSize = 10;

        private Rect boundaries;
        private new Camera camera;
        private Color[] colors;
        private RawImage image;
        [Range(ZoomLowerLimit, ZoomUpperLimit)] public float InitialZoom = 100;

        [Range(0.1f, 5)] public float InitialZoomSpeed = 2f;

        private Camera mainCamera;

        private GameObject mapObject;

        [Range(25, 35)] public float OffsetX = 28;
        [Range(25, 35)] public float OffsetY = 27;

        private float posX;
        private float posY;

        public bool ShowUnits;

        private int sizeX;

        private int sizeY;

        private Texture2D texture2D;
        private RawImage unitOverlay;

        private float ZoomSpeed
            => InitialZoomSpeed * (camera.transform.position.y - ZoomLowerLimit) / 100f;

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

        public int SizeX
        {
            get { return sizeX; }
            set
            {
                sizeX = value;
                image.rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
                unitOverlay.rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
                border.rectTransform.sizeDelta = new Vector2(SizeX + BorderSize * 2, SizeY + BorderSize * 2);

                boundaries = new Rect(Vector2.zero, new Vector2(SizeX, SizeY));
                texture2D = new Texture2D(SizeX, SizeY);
                colors = new Color[SizeX * SizeY];
            }
        }

        public int SizeY
        {
            get { return sizeY; }
            set
            {
                sizeY = value;
                image.rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
                unitOverlay.rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
                border.rectTransform.sizeDelta = new Vector2(SizeX + BorderSize * 2, SizeY + BorderSize * 2);

                boundaries = new Rect(Vector2.zero, new Vector2(SizeX, SizeY));
                texture2D = new Texture2D(SizeX, SizeY);
                colors = new Color[SizeX * SizeY];
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
            if (ShowUnits)
                UpdateOverlayTexture();
        }

        public void UpdateOverlayTexture()
        {
            UpdateTexture();
            unitOverlay.texture = texture2D;
        }

        public void AttachArmies(List<UnitController> armies)
        {
            this.armies = armies;
        }

        private void UpdateTexture()
        {
            for (int i = 0; i < SizeX * SizeY; i++)
                colors[i] = transparent;

            texture2D.SetPixels(colors);

            foreach (UnitController army in armies)
            foreach (MeshDrawableUnit drawableUnit in army.AttachedUnit.AllUnits)
            {
                Vector2 mappedUnitPosition = UnitToPosition(drawableUnit);
                if (boundaries.Contains(mappedUnitPosition))
                    texture2D.SetPixel((int) mappedUnitPosition.x, (int) mappedUnitPosition.y, army.Faction.Color);
            }
            texture2D.Apply();
        }

        private Vector2 UnitToPosition(UnitBase unit)
        {
            return camera.WorldToScreenPoint(unit.Position) * 0.8f;
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
            Vector3 position = new Vector2(Screen.width / 2, Screen.height / 2);
            var plane = new Plane(Vector3.up, Vector3.zero);
            var intersect = new Vector3();

            Ray ray = mainCamera.ScreenPointToRay(position);
            if (plane.Raycast(ray, out float rayDistance))
                intersect = ray.GetPoint(rayDistance);

            camera.transform.position = new Vector3(intersect.x, InitialZoom, intersect.z);
            Vector3 oldAngle = camera.transform.rotation.eulerAngles;
            var newAngle = new Vector3(oldAngle.x, mainCamera.transform.rotation.eulerAngles.y, oldAngle.z);
            camera.transform.eulerAngles = newAngle;

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