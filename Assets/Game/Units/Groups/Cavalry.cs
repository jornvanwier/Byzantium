using Assets.Game.Units.Groups.Formations;

namespace Assets.Game.Units.Groups
{
    public class Cavalry : Unit
    {
        private ICavalryFormation Formation { get; set; }

        public override bool ShouldPathfind => true;

        public override void Order()
        {
            Formation.Order(this);
        }
    }
}