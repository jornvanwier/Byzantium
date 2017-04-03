using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Formation.LegionFormation;
using Assets.Scripts.Map;
using Assets.Scripts.Util;
using Game.Units;
using Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Legion : UnitBase, IMultipleUnits<Cohort>, IMultipleUnits<Cavalry>
    {
        private readonly List<Cavalry> cavalries = new List<Cavalry>();

        private readonly List<Cohort> cohorts = new List<Cohort>();

        private Legion(Faction faction)
        {
            Commander = new Commander(this, faction);
        }

        public override float DefaultSpeed => 1.5f;
        public IEnumerable<Cavalry> Cavalries => cavalries;
        public IEnumerable<Cohort> Cohorts => cohorts;

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

        public override int UnitCount => cavalries.Count + cohorts.Count;
        public int CavalryCount => cavalries.Count;
        public int CohortCount => cohorts.Count;

        public override Vector2 DrawSize => Vector2.Scale(cohorts[0].DrawSize, ChildrenDimensions);

        public Int2 ChildrenDimensionsCohort { get; set; }
        public Int2 ChildrenDimensionsCavalry { get; set; }

        public static Legion CreateStandardLegion(Faction faction)
        {
            var legion = new Legion(faction)
            {
                Formation = new MarchingFormation()
            };

            for (int i = 0; i < 2; i++)
            {
                legion.AddUnit(Cavalry.CreatePikeUnit(faction));
            }

            for (int i = 0; i < 6; i++)
            {
                legion.AddUnit(Cohort.CreateUniformMixedUnit(faction));
            }

            return legion;
        }

        public IEnumerator<MeshDrawableUnit> DrawableUnitsEnumerator
        {
            get
            {
                foreach (Cavalry cavalry in cavalries)
                foreach (MeshDrawableUnit drawableUnit in cavalry.AllUnits)
                    yield return drawableUnit;
                foreach (Cohort cohort in cohorts)
                foreach (MeshDrawableUnit drawableUnit in cohort.AllUnits)
                    yield return drawableUnit;
            }
        }

        public override IEnumerable<MeshDrawableUnit> AllUnits => DrawableUnitsEnumerator.Iterate();

        IEnumerator<Cavalry> IEnumerable<Cavalry>.GetEnumerator()
        {
            return Cavalries.GetEnumerator();
        }

        IEnumerator<Cohort> IEnumerable<Cohort>.GetEnumerator()
        {
            return Cohorts.GetEnumerator();
        }

        public void AddUnit(Cavalry unit)
        {
            cavalries.Add(unit);
        }

        public void RemoveUnit(Cavalry unit)
        {
            int index = cavalries.IndexOf(unit);
            cavalries.RemoveAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            int position = 0;
            while (position < cavalries.Count + cohorts.Count)
            {
                yield return
                    position < cavalries.Count ? (UnitBase) cavalries[position] : cohorts[position - cavalries.Count];
                position++;
            }
        }

        public void AddUnit(Cohort unit)
        {
            cohorts.Add(unit);
        }

        public void RemoveUnit(Cohort unit)
        {
            int index = cohorts.IndexOf(unit);
            cohorts.RemoveAt(index);
        }

        public override void Draw()
        {
            foreach (UnitBase unit in this)
                unit.Draw();
        }
    }
}