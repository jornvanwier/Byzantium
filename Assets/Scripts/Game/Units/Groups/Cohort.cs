using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Util;
using UnityEngine;
using static Assets.Scripts.Game.Units.MeshDrawableUnit;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Cohort : UnitGroup<Century>
    {
        public Cohort(Faction faction) : base(faction)
        {
        }

        public override string UnitName
        {
            get { return "Cohort"; }
        }

        public override float DefaultSpeed
        {
            get { return 1.5f; }
        }

        public override Quaternion Rotation
        {
            get { return base.Rotation; }
            set
            {
                base.Rotation = value;
                foreach (UnitBase child in this)
                    child.Rotation = value;
            }
        }

        public override Vector3 Position
        {
            set
            {
                base.Position = value;
                Formation.Order(this);
            }
        }


        public override IEnumerable<MeshDrawableUnit> AllUnits
        {
            get { return DrawableUnitsEnumerator.Iterate(); }
        }
        
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<Century>) this).GetEnumerator();
        }

        public override void SetPositionInstant(Vector3 pos)
        {
            base.Position = pos;
            Formation.Order(this, true);
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