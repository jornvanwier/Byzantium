﻿using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Util;
using static Assets.Scripts.Game.Units.MeshDrawableUnit;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Cohort : UnitGroup<Century>
    {
        public Cohort(Faction faction) : base(faction)
        {
        }

        public override string UnitName => "Cohort";

        public override float DefaultSpeed => 1.5f;

        public void AddUnit(Contubernium unit)
        {
            storage.PickRandom().AddUnit(unit);
            Set = Prefetch(this);
        }

        public static Cohort CreateUniformMixedUnit(Faction faction)
        {
            var cohort = new Cohort(faction) {Formation = new SquareFormation()};

            for (int i = 0; i < 6; ++i)
                cohort.AddUnit(Century.CreateMixedUnit(faction));

            cohort.IsCavalry = false;
            return cohort;
        }

        public static Cohort CreateCavalryUnit(Faction faction)
        {
            var cohort = new Cohort(faction) {Formation = new SquareFormation()};

            for (int i = 0; i < 6; i++)
                cohort.AddUnit(Century.CreateSwordCavalryUnit(faction));

            cohort.IsCavalry = true;
            cohort.Set = Prefetch(cohort);
            return cohort;
        }

        public static Cohort CreateCustomUnit(Faction faction, SoldierType type)
        {
            var cohort = new Cohort(faction) {Formation = new SquareFormation()};

            for (int i = 0; i < 4; i++)
            {
                Century century = Century.CreateCustomUnit(faction, type);
                cohort.AddUnit(century);
                cohort.IsCavalry = century.IsCavalry;
            }

            return cohort;
        }

        protected override void Order(bool instant = false)
        {
            Formation.Order(this, instant);
        }
    }
}