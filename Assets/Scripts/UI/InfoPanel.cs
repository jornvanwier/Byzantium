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
                panelTransform.sizeDelta = new Vector2(SizeX, SizeY);
            }
        }

        public float SizeY
        {
            get { return sizeY; }
            set
            {
                sizeY = value;
                panelTransform.sizeDelta = new Vector2(SizeX, SizeY);
            }
        }

        private void UpdatePositionAndSize()
        {
            PosX = SizeX / 2;
            PosY = SizeY / 2;

            SizeX = Screen.width - miniMap.rectTransform.sizeDelta.x;
            SizeY = miniMap.rectTransform.sizeDelta.y;
        }

        private RectTransform panelTransform;
        // Use this for initialization
        [UsedImplicitly]
        private void Start()
        {
            panel = GameObject.Find("InfoPanel");
            panelTransform = panel.GetComponent<RectTransform>();
            miniMap = GameObject.Find("MiniMapBorder").GetComponent<Image>();

            UpdatePositionAndSize();
        }

        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
            UpdatePositionAndSize();
        }
    }
}