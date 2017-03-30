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
        private new Camera camera;
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
            if (ShowUnits)
                UpdateOverlayTexture();
        }

        public bool ShowUnits;

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
            var texture = new Texture2D((int) SizeX, (int) SizeY);
            var colors = new Color[(int) (SizeX * SizeY)];
            var transparent = new Color(0, 0, 0, 0);

            for (int i = 0; i < SizeX * SizeY; i++)
                colors[i] = transparent;

            texture.SetPixels(colors);

            foreach (UnitController army in armies)
            foreach (MeshDrawableUnit drawableUnit in army.AttachedUnit.AllUnits)
            {
                Vector2 mappedUnitPosition = UnitToPosition(drawableUnit);
                if (mappedUnitPosition.x >= 0 && mappedUnitPosition.y >= 0 && mappedUnitPosition.x < SizeX &&
                    mappedUnitPosition.y < SizeY)
                    texture.SetPixel((int) mappedUnitPosition.x, (int) mappedUnitPosition.y, army.Faction.Color);
            }

            return texture;
        }

        [Range(25, 35)] public float OffsetX = 28;
        [Range(25, 35)] public float OffsetY = 27;

        private Vector2 UnitToPosition(UnitBase unit)
        {
            return camera.WorldToScreenPoint(unit.Position) - new Vector3(OffsetX, OffsetY, 0);
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
            Vector3 newAngle = new Vector3(oldAngle.x, mainCamera.transform.rotation.eulerAngles.y, oldAngle.z);
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