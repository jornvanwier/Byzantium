using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Formation.LegionFormation;
using Assets.Scripts.Util;
using UnityEngine;
using static Assets.Scripts.Game.Units.MeshDrawableUnit;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Legion : UnitGroup<Cohort>
    {
        public Legion(Faction faction) : base(faction)
        {
        }

        private IEnumerator<Contubernium> ContuberniumEnumerator
        {
            get
            {
                foreach (Cohort child in Storage)
                foreach (Contubernium contubernium in child.Contubernia)
                    yield return contubernium;
            }
        }

        public override IEnumerable<Contubernium> Contubernia => ContuberniumEnumerator.Iterate();

        public override string UnitName => "Legion";

        public override Vector2 GroupSpacing => new Vector2(1.0f, 1.0f);

        public void AddUnit(Century unit)
        {
            Storage.PickRandom().AddUnit(unit);
            Set = Prefetch(this);
        }

        public void AddUnit(Contubernium unit)
        {
            Storage.PickRandom().AddUnit(unit);
            Set = Prefetch(this);
        }

        public IEnumerable<Cohort> ChildrenAreCavalry(bool cavalryChoice)
        {
            return this.Where(c => c.IsCavalry == cavalryChoice).GetEnumerator().Iterate();
        }

        public override Vector2 DrawSize => Vector2.Scale(
                                                ChildrenAreCavalry(true)
                                                    .DefaultIfEmpty(null)
                                                    .FirstOrDefault()
                                                    ?.DrawSize ?? Storage[0].DrawSize, ChildrenDimensions) +
                                            GroupSpacing;

        public static Legion CreateStandardLegion(Faction faction)
        {
            var legion = new Legion(faction)
            {
                Formation = new StandardFormation()
            };

            for (int i = 0; i < 6; i++)
                legion.AddUnit(Cohort.CreateCavalryUnit(faction));

            for (int i = 0; i < 3; i++)
                legion.AddUnit(Cohort.CreateUniformMixedUnit(faction));

            legion.IsCavalry = false;

            return legion;
        }

        public static Legion CreateCustomUnit(Faction faction, SoldierType type)
        {
            var legion = new Legion(faction) {Formation = new StandardFormation()};

            for (int i = 0; i < 6; i++)
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