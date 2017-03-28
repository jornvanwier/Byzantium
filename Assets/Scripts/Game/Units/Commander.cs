using Game.Units;

namespace Assets.Scripts.Game.Units
{
    public class Commander
    {
        private UnitBase children;
        private string name;

        public Commander(UnitBase children, string name = null)
        {
            this.children = children;

            if (name == null)
                name = NameGenerator.Generate();

            this.name = name;
        }
    }
}