using System;
using Assets.Game.Units;
using Assets.Game.Units.Groups;

namespace Assets.Game
{
    public class Commander
    {
        private UnitGroup children;
        private string name;

        public Commander(UnitGroup children, string name = null)
        {
            this.children = children;

            if (name == null)
                name = NameGenerator.Generate();

            this.name = name;
        }
    }
}