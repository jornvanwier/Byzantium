using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Util;
using UnityEngine;
using static Assets.Scripts.Game.Units.MeshDrawableUnit;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Century : UnitGroup<Contubernium>
    {
        public Century(Faction faction) : base(faction)
        {
        }

        public override string UnitName => "Century";

        public override float DefaultSpeed => 1.5f;

        public static Century CreateMixedUnit(Faction faction)
        {
            var century = new Century(faction) {Formation = new SetColumnFormation(2)};

            // Frontline with swords
            for (int i = 0; i < 2; ++i)
                century.AddUnit(Contubernium.CreateSwordUnit(faction));

            // Mid with pikes
            century.AddUnit(Contubernium.CreatePikeUnit(faction));

            // Backline with bows
            century.AddUnit(Contubernium.CreateBowUnit(faction));

            century.IsCavalry = false;

            return century;
        }

        public static Century CreateCavalryUnit(Faction faction)
        {
            var century = new Century(faction) {Formation = new SetColumnFormation(2)};

            for (int i = 0; i < 4; i++)
                century.AddUnit(Contubernium.CreateSwordCavalryUnit(faction));

            century.IsCavalry = true;

            return century;
        }

        protected override void Order(bool instant = false)
        {
            Formation.Order(this, instant);
        }
    }
}