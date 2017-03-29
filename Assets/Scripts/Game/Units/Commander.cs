namespace Assets.Scripts.Game.Units
{
    public class Commander
    {
        private UnitBase children;
        private string name;

        public Commander(UnitBase children, Faction faction, string name = null)
        {
            this.children = children;
            Faction = faction;

            if (name == null)
                name = NameGenerator.Generate();

            this.name = name;
        }

        public Faction Faction { get; set; }
    }
}