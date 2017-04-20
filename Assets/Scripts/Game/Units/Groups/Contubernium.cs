using System;
using Assets.Scripts.Game.Units.Formation;
using UnityEngine;
using static Assets.Scripts.Game.Units.MeshDrawableUnit;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Contubernium : UnitGroup<MeshDrawableUnit>
    {
        public Contubernium(Faction faction) : base(faction)
        {
        }

        public override string UnitName => "Contubernium";

        public override float DefaultSpeed => 1.5f;

        public override Vector2 GroupSpacing => new Vector2(0.1f, 0.1f);

        public static Contubernium CreateSpearCavalryUnit(Faction faction)
        {
            return CreateCustomUnit(faction, SoldierType.HorseSpear);
        }

        public static Contubernium CreateSwordCavalryUnit(Faction faction)
        {
            return CreateCustomUnit(faction, SoldierType.HorseSword);
        }

        public static Contubernium CreateBowCavalryUnit(Faction faction)
        {
            return CreateCustomUnit(faction, SoldierType.HorseBow);
        }

        public static Contubernium CreateSwordUnit(Faction faction)
        {
            return CreateCustomUnit(faction, SoldierType.Sword);
        }

        public static Contubernium CreateBowUnit(Faction faction)
        {
            return CreateCustomUnit(faction, SoldierType.Bow);
        }

        public static Contubernium CreateSpearUnit(Faction faction)
        {
            return CreateCustomUnit(faction, SoldierType.Spear);
        }

        public static Contubernium CreateCustomUnit(Faction faction, SoldierType unitType)
        {
            var contuberium = new Contubernium(faction) {Formation = new SquareFormation()};

            for (int i = 0; i < 16; ++i)
            {
                var mdm = new MeshDrawableUnit(unitType);
                contuberium.AddUnit(mdm);
                contuberium.IsCavalry = mdm.IsCavalry;
            }

            contuberium.Set = Prefetch(contuberium);
            return contuberium;
        }

        protected override void Order(bool instant = false)
        {
            Formation.Order(this, instant);
        }
    }
}