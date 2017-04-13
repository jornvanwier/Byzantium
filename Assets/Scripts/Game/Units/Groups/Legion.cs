using System.Collections;
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

        public override string UnitName => "Legion";

        public override float DefaultSpeed => 1.5f;

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

        protected override void Order(bool instant = false)
        {
            Formation.Order(this, instant);
        }
    }
}