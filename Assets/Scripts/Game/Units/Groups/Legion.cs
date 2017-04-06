using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Formation.LegionFormation;
using Assets.Scripts.Map;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Legion : UnitBase, IMultipleUnits<Cohort>
    {
        private readonly List<Cohort> cohorts = new List<Cohort>();

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
        }

        public void RemoveUnit(Cohort unit)
        {
            int index = cohorts.IndexOf(unit);
            cohorts.RemoveAt(index);
        }

        public static Legion CreateStandardLegion(Faction faction)
        {
            var legion = new Legion(faction)
            {
                Formation = new MarchingFormation()
            };

            for (int i = 0; i < 2; i++)
                legion.AddUnit(Cohort.CreateCavalryUnit(faction));

            for (int i = 0; i < 6; i++)
                legion.AddUnit(Cohort.CreateUniformMixedUnit(faction));

            legion.IsCavalry = false;

            return legion;
        }

        public override void Draw()
        {
            foreach (Cohort unit in this)
                unit.Draw();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Cohort> GetEnumerator()
        {
            return cohorts.GetEnumerator();
        }
    }
}