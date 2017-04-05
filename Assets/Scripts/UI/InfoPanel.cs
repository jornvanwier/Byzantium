using Assets.Scripts.Game;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class InfoPanel : MonoBehaviour
    {
        private readonly Color transparent = new Color(0, 0, 0, 0);
        private Image image;
        private Image miniMap;

        private GameObject panel;
        private float posX;
        private float posY;
        
        private float sizeX;

        private float sizeY;
        private Text titleText;

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

        public string Title
        {
            get { return titleText.text; }
            set { titleText.text = value; }
        }

        private void UpdatePositionAndSize()
        {
            PosX = SizeX / 2;
            PosY = SizeY / 2;

            SizeX = Screen.width - miniMap.rectTransform.sizeDelta.x;
            SizeY = miniMap.rectTransform.sizeDelta.y;
        }

        public string Commander
        {
            get { return commanderText.text; }
            set { commanderText.text = value; }
        }

        private RectTransform rectTransform;
        private Text commanderText;
        // Use this for initialization
        [UsedImplicitly]
        private void Start()
        {
            titleText = GameObject.Find("InfoText").GetComponent<Text>();
            commanderText = GameObject.Find("CommanderText").GetComponent<Text>();
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