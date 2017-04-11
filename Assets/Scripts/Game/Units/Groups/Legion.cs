using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Formation.LegionFormation;
using Assets.Scripts.Util;
using UnityEngine;
using static Assets.Scripts.Game.Units.MeshDrawableUnit;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Legion : UnitBase, IMultipleUnits<Cohort>
    {
        private readonly List<Cohort> cohorts = new List<Cohort>();
        private DrawingSet set;

        private Legion(Faction faction)
        {
            Commander = new Commander(this, faction);
        }

        public override string UnitName => "Legion";

        public override int Health
        {
            get { return cohorts[0].Health; }
            set
            {
                foreach (Cohort cohort in cohorts)
                    cohort.Health = value;
            }
        }

        public override float DefaultSpeed => 1.5f;

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

        public override void SetPositionInstant(Vector3 pos)
        {
            base.Position = pos;
            Formation.Order(this, true);
        }

        public override int UnitCount => cohorts.Count;

        public override Vector2 DrawSize => Vector2.Scale(cohorts[0].DrawSize, ChildrenDimensions);

        public IEnumerator<MeshDrawableUnit> DrawableUnitsEnumerator
        {
            get
            {
                foreach (Cohort cohort in cohorts)
                foreach (MeshDrawableUnit drawableUnit in cohort.AllUnits)
                    yield return drawableUnit;
            }
        }

        public override IEnumerable<MeshDrawableUnit> AllUnits => DrawableUnitsEnumerator.Iterate();

        public void AddUnit(Cohort unit)
        {
            cohorts.Add(unit);
            set = Prefetch(this);
        }

        public void RemoveUnit(Cohort unit)
        {
            cohorts.Remove(unit);
            set = Prefetch(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Cohort> GetEnumerator()
        {
            return cohorts.GetEnumerator();
        }

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


        public override void Draw()
        {
            if (set != null)
                DrawAll(set);
        }
    }
}