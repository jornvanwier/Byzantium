using Assets.Scripts.Game.Units.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HealthBar : RawImage
    {
        public Text HealthText { get; set; }

        private const float HeightWidthRatio = 0.1f;

        private static readonly Color HealthyColor = Color.green;
        private static readonly Color DamageColor = Color.red;

        private readonly Color transparent = new Color(0, 0, 0, 0);

        private UnitController army;

        private Color[] pixels;

        private float posX, posY;

        private int size;
        private Texture2D texture2D;
        public int MaxValue => army?.AttachedUnit.MaxHealth ?? -1;

        public int Value
        {
            set
            {
                if (pixels == null) return;

                if (HealthText != null)
                {
                    HealthText.text = $"{army.Faction.Name}\n{value} hp";
                }

                float filledPercentage = (float) value / MaxValue;
                int pixelsToFill = (int) (filledPercentage * Size);
                for (int i = 0; i < pixels.Length; i++)
                    if (i % texture2D.width <= pixelsToFill && pixelsToFill != 0)
                        pixels[i] = HealthyColor;
                    else
                        pixels[i] = DamageColor;
                UpdateTexture();
            }
        }

        // ReSharper disable ArrangeAccessorOwnerBody
        public int Size
        {
            get { return size; }
            set
            {
                size = value;
                rectTransform.sizeDelta = new Vector2(Size, Size * HeightWidthRatio);
                texture2D = new Texture2D(Size, (int) (Size * HeightWidthRatio));
                texture = texture2D;
                pixels = new Color[(int) (Size * Size * HeightWidthRatio)];
            }
        }

        public float PosX
        {
            get { return posX; }
            set
            {
                posX = value;
                transform.position = new Vector3(PosX, PosY);
            }
        }

        public float PosY
        {
            get { return posY; }
            set
            {
                posY = value;
                transform.position = new Vector3(PosX, PosY);
            }
        }
        // ReSharper restore ArrangeAccessorOwnerBody

        public void AttachArmy(UnitController army)
        {
            this.army = army;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void UpdateTexture()
        {
            texture2D.SetPixels(pixels);
            texture2D.Apply();
            texture = texture2D;
        }

        protected override void Start()
        {
            base.Start();
            Size = 100;
//            Hide();
        Show();
        }
    }
}