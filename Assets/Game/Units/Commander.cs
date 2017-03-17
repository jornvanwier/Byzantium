using System;
using Assets.Game.Units;

namespace Assets.Game
{
    public class Commander
    {
        private IMultipleUnits children;
        private string name;

        public Commander(IMultipleUnits children, string name = null)
        {
            this.children = children;

            if (name == null)
                name = NameGenerator.Generate();

            this.name = name;
        }
    }
}