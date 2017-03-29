using Boo.Lang;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public class Faction
    {
        private static readonly List<Color> UsedColors = new List<Color>();

        public Faction(string name = null)
        {
            if (name == null)
                name = FactionNameGenerator.Generate();

            Name = name;
            do
            {
                Color = Random.ColorHSV();
            } while (UsedColors.Contains(Color));
            UsedColors.Add(Color);
        }

        public string Name { get; set; }
        public Color Color { get; set; }
    }
}