using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Cohort : UnitBase, IMultipleUnits<Century>
    {
        public override string UnitName => "Cohort";
        private const float ChildSpacingX = 1.7f;
        private const float ChildSpacingY = 1.15f;

        private readonly List<Century> centuries = new List<Century>();

        private Cohort(Faction faction)
        {
            Commander = new Commander(this, faction);
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

        public override int UnitCount => centuries.Count;

        public override Vector2 DrawSize
            =>
                Vector2.Scale(new Vector2(ChildSpacingX, ChildSpacingY),
                    Vector2.Scale(centuries[0].DrawSize, ChildrenDimensions));

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
        }

        public void RemoveUnit(Century unit)
        {
            int index = centuries.IndexOf(unit);
            centuries.RemoveAt(index);
        }

        public static Cohort CreateUniformMixedUnit(Faction faction)
        {
            var cohort = new Cohort(faction) {Formation = new SquareFormation()};

            for (int i = 0; i < 6; ++i)
                cohort.AddUnit(Century.CreateMixedUnit(faction));

            return cohort;
        }

        public override void Draw()
        {
            foreach (Century unit in this)
                unit.Draw();
        }

        IEnumerator<Century> IEnumerable<Century>.GetEnumerator()
        {
            return centuries.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<Century>)this).GetEnumerator();
        }
    }
}