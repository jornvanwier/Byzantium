using Boo.Lang;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public class Faction
    {
        private static List<Color> usedColors;

        public Faction(string name)
        {
            Name = name;
            do
            {
                Color = Random.ColorHSV();
            } while (usedColors.Contains(Color));
            usedColors.Add(Color);
        }

        public string Name { get; set; }
        public Color Color { get; set; }
    }
}