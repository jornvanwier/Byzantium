using Assets.Scripts.Game;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class InfoPanel : MonoBehaviour
    {
        private Image miniMap;

        private GameObject panel;
        private float posX;
        private float posY;
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

        public string Title
        {
            get { return titleText.text; }
            set { titleText.text = value; }
        }

        private RectTransform rectTransform;
        private Image image;
        private Text titleText;
        // Use this for initialization
        [UsedImplicitly]
        private void Start()
        {
            titleText = GameObject.Find("Text").GetComponent<Text>();
            panel = GameObject.Find("InfoPanel");
            rectTransform = panel.GetComponent<RectTransform>();
            image = panel.GetComponent<Image>();
            miniMap = GameObject.Find("MiniMapBorder").GetComponent<Image>();

            GameObject.Find("WorldManager").GetComponent<WorldManager>().AttachInfoPanel(this);

            UpdatePositionAndSize();
        }

        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
            UpdatePositionAndSize();
        }

        private readonly Color transparent = new Color(0, 0, 0, 0);

        public void Hide()
        {
            panel.SetActive(false);
            //image.color = transparent;
        }

        public void Show()
        {
            panel.SetActive(true);
            //image.color = Color.white;
        }
    }
}