using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Unit_Enums;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public class MeshDrawableUnit : UnitBase
    {
        public static Material Material = new Material(Shader.Find("Standard"));

        public MeshDrawableUnit(Defense defense = Defense.Armor,
            Weapon weapon = Weapon.Sword,
            Soldier soldier = Soldier.Armored)
        {
            if (defense == Defense.None && soldier == Soldier.Armored)
            {
                throw new ArgumentException("Defense type of None cannot be used with a Soldier type of Armored!");
            }

            Debug.Log(soldier);

            MeshHolder m = WorldManager.Meshes;

            UnitMesh = m.SoldierEnum[soldier];
            WeaponMesh = m.WeaponEnum[weapon];
            DefenseMesh = m.DefenseEnum[defense];

            DefenseType = defense;
            WeaponType = weapon;
            SoldierType = soldier;
        }

        public Mesh UnitMesh { get; }
        public Mesh WeaponMesh { get; }
        public Mesh DefenseMesh { get; }
        public Defense DefenseType { get; }
        public Weapon WeaponType { get; }
        public Soldier SoldierType { get; }

        public override int UnitCount => 1;

        public override void Draw()
        {
            Graphics.DrawMesh(UnitMesh, Matrix4x4.TRS(Position, Rotation, new Vector3(0.1f, 0.1f, 0.1f)), Material, 0);

            Vector3 weaponPosition = Position + (Rotation * new Vector3(0.2f, 0, 0));
//            weaponPosition = Rotation * weaponPosition;

            Graphics.DrawMesh(WeaponMesh,
                Matrix4x4.TRS(weaponPosition, Rotation, new Vector3(0.1f, 0.1f, 0.1f)), Material, 0);


            if (DefenseMesh != null)
            {
                Vector3 shieldPosition = Position + (Rotation * new Vector3(0, 0.2f, 0));
                Graphics.DrawMesh(DefenseMesh,
                    Matrix4x4.TRS(shieldPosition, Rotation, new Vector3(0.1f, 0.1f, 0.1f)),
                    Material, 0);
            }
        }
    }
}