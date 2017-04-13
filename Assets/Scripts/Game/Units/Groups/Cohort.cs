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

        public override string UnitName => "Cohort";

        public override float DefaultSpeed => 1.5f;

        public override Quaternion Rotation
        {
            get => base.Rotation;
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

        public override int UnitCount => centuries.Count;

        public override Vector2 DrawSize => Vector2.Scale(centuries[0].DrawSize, ChildrenDimensions);

        public IEnumerator<MeshDrawableUnit> DrawableUnitsEnumerator
        {
            get
            {
                foreach (Century century in centuries)
                foreach (MeshDrawableUnit drawableUnit in century.AllUnits)
                    yield return drawableUnit;
            }
        }


        public override IEnumerable<MeshDrawableUnit> AllUnits => DrawableUnitsEnumerator.Iterate();


        public override int Health
        {
            get { return centuries[0].Health; }
            set
            {
                foreach (Century century in centuries)
                    century.Health = value;
            }
        }

        public void AddUnit(Century unit)
        {
            centuries.Add(unit);
            set = Prefetch(this);
        }

        public void AddUnit(Contubernium unit)
        {
            centuries.Where(c => c.IsCavalry == unit.IsCavalry).ToList().random().AddUnit(unit);
        }

        public void RemoveUnit(Century unit)
        {
            centuries.Remove(unit);
            set = Prefetch(this);
        }

        public void RemoveUnit(Contubernium unit)
        {
            foreach (Century century in centuries)
                century.RemoveUnit(unit);
        }

        IEnumerator<Century> IEnumerable<Century>.GetEnumerator()
        {
            return centuries.GetEnumerator();
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

        public override void Draw()
        {
            Formation.Order(this, instant);
        }
    }
}