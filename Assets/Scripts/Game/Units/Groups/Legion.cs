using Assets.Scripts.Game.Units.Formation.LegionFormation;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Legion : UnitGroup<Cohort>
    {
        public Legion(Faction faction) : base(faction)
        {
        }

        public override string UnitName => "Legion";

        public override float DefaultSpeed => 1.5f;

        public static Legion CreateStandardLegion(Faction faction)
        {
            var legion = new Legion(faction)
            {
                Formation = new MarchingFormation()
            };

            legion.AddUnit(Cohort.CreateCavalryUnit(faction));

            for (int i = 0; i < 3; i++)
                legion.AddUnit(Cohort.CreateUniformMixedUnit(faction));

            legion.IsCavalry = false;

            return legion;
        }

        public static Legion CreateCustomUnit(Faction faction, SoldierType type)
        {
            var legion = new Legion(faction) {Formation = new MarchingFormation()};

            for (int i = 0; i < 4; i++)
            {
                Cohort cohort = Cohort.CreateCustomUnit(faction, type);
                legion.AddUnit(cohort);
                legion.IsCavalry = cohort.IsCavalry;
            }

            return legion;
        }

        protected override void Order(bool instant = false)
        {
            Formation.Order(this, instant);
        }
    }
}