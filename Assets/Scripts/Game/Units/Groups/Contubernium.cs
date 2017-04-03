﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Game.Units.Formation.ContuberniumFormations;
using Assets.Scripts.Game.Units.Unit_Enums;
using UnityEngine;

namespace Game.Units.Groups
{
    public class Contubernium : UnitBase, IMultipleUnits<MeshDrawableUnit>
    {
        private readonly List<MeshDrawableUnit> drawableUnits = new List<MeshDrawableUnit>();

        private Contubernium(Faction faction)
        {
            Commander = new Commander(this, faction);
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

        public static Contubernium CreateSwordUnit(Faction faction)
        {
            return CreateCustomUnit(faction, Defense.SmallShield, Weapon.Sword, Soldier.Armored);
        }

        public static Contubernium CreateLongbowUnit(Faction faction)
        {
            return CreateCustomUnit(faction, Defense.None, Weapon.Longbow, Soldier.Unarmored);
        }

        public static Contubernium CreatePikeUnit(Faction faction)
        {
            return CreateCustomUnit(faction, Defense.LargeShield, Weapon.Pike, Soldier.Armored);
        }

        public static Contubernium CreateCustomUnit(Faction faction, Defense defense, Weapon weapon, Soldier soldier)
        {
            var contuberium = new Contubernium(faction) {Formation = new SquareFormation()};

            for (int i = 0; i < 8; ++i)
                contuberium.AddUnit(new MeshDrawableUnit(
                    defense,
                    weapon,
                    soldier
                ));

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

        IEnumerator<MeshDrawableUnit> IEnumerable<MeshDrawableUnit>.GetEnumerator()
        {
            return drawableUnits.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<MeshDrawableUnit>)this).GetEnumerator();
        }
    }
}