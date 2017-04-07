using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Formation;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Contubernium : UnitBase, IMultipleUnits<MeshDrawableUnit>
    {
        private readonly List<MeshDrawableUnit> drawableUnits = new List<MeshDrawableUnit>();

        private Contubernium(Faction faction)
        {
            Commander = new Commander(this, faction);
        }

        public override string UnitName => "Contubernium";

        public override int Health
        {
            get { return drawableUnits[0].Health; }
            set
            {
                foreach (MeshDrawableUnit meshDrawableUnit in drawableUnits)
                    meshDrawableUnit.Health = value;
            }
        }

        public override float DefaultSpeed => 1.5f;

        public override Vector3 Position
        {
            set
            {
                base.Position = value;
                Formation.Order(this);
            }
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

        public override int UnitCount => drawableUnits.Count;

        public override Vector2 DrawSize => Vector2.Scale(drawableUnits[0].DrawSize, ChildrenDimensions);

        public override IEnumerable<MeshDrawableUnit> AllUnits => drawableUnits;

        public void AddUnit(MeshDrawableUnit unit)
        {
            drawableUnits.Add(unit);
        }

        public void RemoveUnit(MeshDrawableUnit unit)
        {
            int index = drawableUnits.IndexOf(unit);
            drawableUnits.RemoveAt(index);
        }

        IEnumerator<MeshDrawableUnit> IEnumerable<MeshDrawableUnit>.GetEnumerator()
        {
            return drawableUnits.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<MeshDrawableUnit>) this).GetEnumerator();
        }

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

        public static Contubernium CreatePikeUnit(Faction faction)
        {
            return CreateCustomUnit(faction, SoldierType.Spear);
        }

        public static Contubernium CreateCustomUnit(Faction faction, SoldierType unitType)
        {
            var contuberium = new Contubernium(faction) {Formation = new SetColumnFormation(2)};

            for (int i = 0; i < 8; ++i)
                contuberium.AddUnit(new MeshDrawableUnit(
                    unitType
                ));

            contuberium.IsCavalry = contuberium.First().IsCavalry;

            return contuberium;
        }

        public int GetGroupSize()
        {
            return drawableUnits.Count;
        }

        public override void Draw()
        {
            foreach (MeshDrawableUnit unit in this)
                unit.Draw();
        }
    }
}