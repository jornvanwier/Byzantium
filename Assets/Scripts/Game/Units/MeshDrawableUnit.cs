using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Unit_Enums;
using Assets.Scripts.Map;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public class MeshDrawableUnit : UnitBase
    {
        private const int StartHealth = 200;
        public static Material Material = WorldManager.UnitMaterial;

        private readonly Int2 dimensions = new Int2(1, 1);

        private Vector3 oldPosition = Vector3.zero;

        public MeshDrawableUnit(Defense defense = Defense.Armor,
            Weapon weapon = Weapon.Sword,
            Soldier soldier = Soldier.Armored)
        {
            if (defense == Defense.None && soldier == Soldier.Armored)
                throw new ArgumentException("Defense type of None cannot be used with a Soldier type of Armored!");

            MeshHolder m = WorldManager.Meshes;

            UnitMesh = m.SoldierEnum[soldier];
            WeaponMesh = m.WeaponEnum[weapon];
            DefenseMesh = m.DefenseEnum[defense];

            DefenseType = defense;
            WeaponType = weapon;
            SoldierType = soldier;
        }

        public override int Health { get; set; } = StartHealth;

        public override Int2 ChildrenDimensions
        {
            get { return dimensions; }
            set { throw new MemberAccessException("Cannot set dimensions of this object."); }
        }

        public override Vector2 DrawSize => new Vector2(0.11f, 0.05f);

        public override float DefaultSpeed => 1.5f;

        public Mesh UnitMesh { get; }
        public Mesh WeaponMesh { get; }
        public Mesh DefenseMesh { get; }
        public Defense DefenseType { get; }
        public Weapon WeaponType { get; }
        public Soldier SoldierType { get; }

        public override int UnitCount => 1;

        public IEnumerator<MeshDrawableUnit> DrawableUnitsEnumerator
        {
            get { yield return this; }
        }

        public override IEnumerable<MeshDrawableUnit> AllUnits => DrawableUnitsEnumerator.Iterate();

        public override void Draw()
        {
            Quaternion rotate = Rotation * Quaternion.Euler(0, 180, 0);
            Graphics.DrawMesh(UnitMesh, Matrix4x4.TRS(Position, rotate, new Vector3(0.1f, 0.1f, 0.1f)), Material, 0);

//            Vector3 weaponPosition = Position + (Rotation * new Vector3(0.2f, 0, 0));

//            Graphics.DrawMesh(WeaponMesh,
//                Matrix4x4.TRS(weaponPosition, Rotation, new Vector3(0.1f, 0.1f, 0.1f)), Material, 0);
//
//
//            if (DefenseMesh != null)
//            {
//                Vector3 shieldPosition = Position + (Rotation * new Vector3(0, 0.2f, 0));
//                Graphics.DrawMesh(DefenseMesh,
//                    Matrix4x4.TRS(shieldPosition, Rotation, new Vector3(0.1f, 0.1f, 0.1f)),
//                    Material, 0);
//            }
        }
    }
}