using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HealthBar : RawImage
    {
        private const int MaxValue = 200;

        private const float HeightWidthRatio = 0.1f;

        private static readonly Color HealthyColor = Color.green;
        private static readonly Color DamageColor = Color.red;

        private readonly Color transparent = new Color(0, 0, 0, 0);

        private Color[] pixels;

        private float posX, posY;

        private int size;
        private Texture2D texture2D;

        public int Value
        {
            set
            {
                if (pixels == null) return;
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

        public int Size
        {
            get => size;
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
            get => posX;
            set
            {
                posX = value;
                transform.position = new Vector3(PosX, PosY);
            }
        }

        public float PosY
        {
            get => posY;
            set
            {
                posY = value;
                transform.position = new Vector3(PosX, PosY);
            }
        }

        public void Hide()
        {
            color = transparent;
        }

        public void Show()
        {
            color = Color.white;
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
            Hide();
        }
    }
}