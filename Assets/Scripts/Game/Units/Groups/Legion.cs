using System.Collections;
using System.Collections.Generic;
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

        public override string UnitName
        {
            get { return "Legion"; }
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
        
        public override void SetPositionInstant(Vector3 pos)
        {
            base.Position = pos;
            Formation.Order(this, true);
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

        public static Legion CreateCustomUnit(Faction faction, SoldierType type)
        {
            var legion = new Legion(faction) {Formation = new MarchingFormation()};

            for (int i = 0; i < 4; i++)
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