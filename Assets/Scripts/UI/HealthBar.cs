using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HealthBar : RawImage
    {
        private const float HeightWidthRatio = 0.1f;

        private static readonly Color HealthyColor = Color.green;
        private static readonly Color DamageColor = Color.red;

        public static List<HealthBar> AllHealthBars = new List<HealthBar>();

        private readonly Vector2 textExtraSize = new Vector2(20, 40);

        private UnitController army;

        private Color[] pixels;

        private float posX, posY;

        private int size;
        private Texture2D texture2D;
        public Text HealthText { get; set; }
        public int MaxValue => army?.AttachedUnit.MaxHealth ?? -1;

        public int Value
        {
            set
            {
                if (pixels == null) return;

                if (HealthText != null)
                    HealthText.text = $"{army.Faction.Name}\n{value} hp";

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

        public Rect HitBox => new Rect(PosX - Size / 2 - textExtraSize.x / 2,
            PosY - Size * HeightWidthRatio / 2 - textExtraSize.y, size + textExtraSize.x,
            size * HeightWidthRatio + textExtraSize.y);

        private bool OverlapsAny(Vector2 pos)
        {
            float oldX = posX;
            float oldY = posY;
            posX = pos.x;
            posY = pos.y;

            bool result = AllHealthBars.Where(healthbar => this != healthbar)
                .Any(healthbar => HitBox.Overlaps(healthbar.HitBox));

            posX = oldX;
            posY = oldY;
            return result;
        }

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

        public void Destroy()
        {
            AllHealthBars.Remove(this);
            Destroy(gameObject);
        }

        protected override void Start()
        {
            AllHealthBars.Add(this);
            base.Start();
            Size = 100;
//            Hide();
            Show();
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
                if (OverlapsAny(new Vector2(value, PosY))) return;


                posX = value;
                transform.position = new Vector2(PosX, PosY);
            }
        }

        public float PosY
        {
            get { return posY; }
            set
            {
                if (OverlapsAny(new Vector2(PosX, value))) return;

                posY = value;
                transform.position = new Vector2(PosX, PosY);
            }
        }
        // ReSharper restore ArrangeAccessorOwnerBody
    }
}