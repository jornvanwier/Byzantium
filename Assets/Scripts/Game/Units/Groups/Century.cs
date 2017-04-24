using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Formation;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Century : UnitGroup<Contubernium>
    {
        public Century(Faction faction) : base(faction)
        {
        }

        public override string UnitName => "Century";

        public override float DefaultSpeed => 1.5f;

        public override Vector2 GroupSpacing => new Vector2(0.4f, 0.4f);

        public override IEnumerable<Contubernium> Contubernia => Storage.AsEnumerable();

        public static Century CreateMixedUnit(Faction faction)
        {
            var century = new Century(faction) {Formation = new SquareFormation()};

            // Frontline with swords
            for (int i = 0; i < 2; ++i)
                century.AddUnit(Contubernium.CreateSwordUnit(faction));

            // Mid with pikes
            century.AddUnit(Contubernium.CreateSpearUnit(faction));

            // Backline with bows
            century.AddUnit(Contubernium.CreateBowUnit(faction));

            century.IsCavalry = false;

            return century;
        }

        public static Century CreateSwordCavalryUnit(Faction faction)
        {
            var century = new Century(faction) {Formation = new SquareFormation()};

            for (int i = 0; i < 4; i++)
                century.AddUnit(Contubernium.CreateSwordCavalryUnit(faction));

            century.IsCavalry = true;

            return century;
        }

        public static Century CreateCustomUnit(Faction faction, SoldierType type)
        {
            var century = new Century(faction) {Formation = new SquareFormation()};

            for (int i = 0; i < 4; i++)
            {
                Contubernium contubernium = Contubernium.CreateCustomUnit(faction, type);
                century.AddUnit(contubernium);
                century.IsCavalry = contubernium.IsCavalry;
            }

            return century;
        }

        protected override void Order(bool instant = false)
        {
            Formation.Order(this, instant);
        }
    }
}