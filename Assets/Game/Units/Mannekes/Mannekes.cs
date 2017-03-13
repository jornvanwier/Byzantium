namespace Assets.Game.Units
{
    public class Mannekes
    {
        public Mannekes[] Units { get; set; }

        public Mannekes(Commander commander, Mannekes[] children)
        {
            commander.Units = this;
            Units = children;
        }
    }
}