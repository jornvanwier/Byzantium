namespace Assets.Scripts.Game.Units
{
    public class Commander
    {
        private UnitBase children;

        public Commander(UnitBase children, Faction faction, string name = null)
        {
            this.children = children;
            Faction = faction;

            if (name == null)
                name = NameGenerator.Generate();

            Name = name;
        }

        public Faction Faction { get; set; }
        public string Name { get; set; }
    }
}