using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Scripts.Game.Units.Unit_Enums;
using UnityEngine;

namespace Assets.Scripts.Game
{

    [Serializable]
    public class MeshHolder
    {
        [Serializable]
        public class WeaponMeshHolder
        {
            public Mesh Sword;
            public Mesh Pike;
            public Mesh Longbow;
            public Mesh Shortbow;
        }

        [SerializeField]
        public WeaponMeshHolder Weapons = new WeaponMeshHolder();

        public Dictionary<Weapon, Mesh> WeaponEnum;

        [Serializable]
        public class DefenseMeshHolder
        {
            public Mesh LargeShield;
            public Mesh SmallShield;
        }

        [SerializeField]
        public DefenseMeshHolder Defenses = new DefenseMeshHolder();

        public Dictionary<Defense, Mesh> DefenseEnum;

        [Serializable]
        public class SoldierMeshHolder
        {
            public Mesh Unarmored;
            public Mesh Armored;
            public Mesh Mounted;
        }

        [SerializeField]
        public SoldierMeshHolder Soldiers = new SoldierMeshHolder();

        public Dictionary<Soldier, Mesh> SoldierEnum;

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public MeshHolder()
        {

            WeaponEnum = new Dictionary<Weapon, Mesh>()
            {
                {Weapon.Sword, Weapons.Sword},
                {Weapon.Pike, Weapons.Pike},
                {Weapon.Shortbow, Weapons.Shortbow},
                {Weapon.Longbow, Weapons.Longbow},
            };

            DefenseEnum = new Dictionary<Defense, Mesh>()
            {
                {Defense.None, null},
                {Defense.Armor, null},
                {Defense.SmallShield, Defenses.SmallShield},
                {Defense.LargeShield, Defenses.LargeShield}
            };

            SoldierEnum = new Dictionary<Soldier, Mesh>()
            {
                {Soldier.Armored, Soldiers.Armored},
                {Soldier.Unarmored, Soldiers.Unarmored},
                {Soldier.Mounted, Soldiers.Mounted}
            };
        }
    }
}