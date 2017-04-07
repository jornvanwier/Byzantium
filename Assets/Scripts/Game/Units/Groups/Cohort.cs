﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Util;
using UnityEngine;
using static Assets.Scripts.Game.Units.MeshDrawableUnit;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Cohort : UnitBase, IMultipleUnits<Century>
    {
        private readonly List<Century> centuries = new List<Century>();
        private DrawingSet set;

        private Cohort(Faction faction)
        {
            Commander = new Commander(this, faction);
        }

        public override string UnitName => "Cohort";

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

        public void RemoveUnit(Century unit)
        {
            int index = centuries.IndexOf(unit);
            centuries.RemoveAt(index);
            set = Prefetch(this);
        }

        IEnumerator<Century> IEnumerable<Century>.GetEnumerator()
        {
            return centuries.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<Century>) this).GetEnumerator();
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
                cohort.AddUnit(Century.CreateCavalryUnit(faction));

            cohort.IsCavalry = true;
            cohort.set = Prefetch(cohort);
            return cohort;
        }

        public override void Draw()
        {
            if (set != null)
                DrawAll(set);
        }
    }
}