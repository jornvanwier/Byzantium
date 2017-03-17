using Assets.Game.Units.Groups.Formations;

namespace Assets.Game.Units.Groups
{
    public class Contubernium : Unit
    {
        private IContuberniumFormation Formation { get; set; }

        public override void Order()
        {
            Formation.Order(this);
        }
    }
}