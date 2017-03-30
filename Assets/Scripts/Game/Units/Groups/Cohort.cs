using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Util;
using Game.Units.Formation;
using UnityEngine;

namespace Game.Units.Groups
{
    public class Cohort : UnitBase, IMultipleUnits<Century>
    {
        private readonly List<Century> centuries = new List<Century>();
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

        public override int UnitCount => centuries.Count;

        public override Vector2 DrawSize => Vector2.Scale(new Vector2(ChildSpacingX, ChildSpacingY), Vector2.Scale(centuries[0].DrawSize, ChildrenDimensions));

        private const float ChildSpacingX = 1.7f;
        private const float ChildSpacingY = 1.15f;
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

        public void AddUnit(Century unit)
        {
            centuries.Add(unit);
        }

        public void RemoveUnit(Century unit)
        {
            int index = centuries.IndexOf(unit);
            centuries.RemoveAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            return centuries.GetEnumerator();
        }

        public static Cohort CreateUniformMixedUnit()
        {
            var cohort = new Cohort {Formation = new SetRowFormation()};

            for (int i = 0; i < 6; ++i)
                cohort.AddUnit(Century.CreateMixedUnit());
            
            var faction = new Faction();
            cohort.Commander = new Commander(cohort, faction);

            return cohort;
        }

        public override void Draw()
        {
            foreach (Century unit in this)
                unit.Draw();
        }
    }
}