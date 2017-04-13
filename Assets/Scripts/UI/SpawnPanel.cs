using Assets.Scripts.Game;
using Assets.Scripts.Game.Units;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class SpawnPanel : MonoBehaviour
    {
        private Image miniMap;

        private GameObject panel;
        private float posX;
        private float posY;

        private RectTransform rectTransform;

        private UnitController selectedArmy;

        private float sizeX;

        private float sizeY;

        public float PosX
        {
            get { return posX; }
            set
            {
                posX = value;
                panel.transform.position = new Vector3(PosX, PosY);
            }
        }

        public float PosY
        {
            get { return posY; }
            set
            {
                posY = value;
                panel.transform.position = new Vector3(PosX, PosY);
            }
        }

        public float SizeX
        {
            get { return sizeX; }
            set
            {
                sizeX = value;
                rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
            }
        }

        public float SizeY
        {
            get { return sizeY; }
            set
            {
                sizeY = value;
                rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
            }
        }

        private void UpdatePositionAndSize()
        {
            PosX = SizeX / 2;
            PosY = SizeY / 2;

            SizeX = Screen.width - miniMap.rectTransform.sizeDelta.x;
            SizeY = miniMap.rectTransform.sizeDelta.y;
        }

        // Use this for initialization
        [UsedImplicitly]
        private void Start()
        {
            panel = GameObject.Find("SpawnPanel");
            rectTransform = panel.GetComponent<RectTransform>();
            panel.GetComponent<Image>();
            miniMap = GameObject.Find("MiniMapBorder").GetComponent<Image>();

            GameObject.Find("WorldManager").GetComponent<WorldManager>().AttachSpawnPanel(this);

            UpdatePositionAndSize();
        }

        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
            UpdatePositionAndSize();
        }

        public void Hide()
        {
            panel.SetActive(false);
        }

        public void Show(UnitController army)
        {
            panel.SetActive(true);
            selectedArmy = army;
        }
    }
}