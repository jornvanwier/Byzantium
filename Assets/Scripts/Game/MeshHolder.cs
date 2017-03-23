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
        public Dictionary<Defense, Mesh> DefenseEnum;

        [SerializeField] public DefenseMeshHolder Defenses = new DefenseMeshHolder();

        public Dictionary<Soldier, Mesh> SoldierEnum;

        [SerializeField] public SoldierMeshHolder Soldiers = new SoldierMeshHolder();

        public Dictionary<Weapon, Mesh> WeaponEnum;

        [SerializeField] public WeaponMeshHolder Weapons = new WeaponMeshHolder();

        public void Initialize()
        {
            WeaponEnum = new Dictionary<Weapon, Mesh>
            {
                {Weapon.Sword, Weapons.Sword},
                {Weapon.Pike, Weapons.Pike},
                {Weapon.Shortbow, Weapons.Shortbow},
                {Weapon.Longbow, Weapons.Longbow}
            };

            DefenseEnum = new Dictionary<Defense, Mesh>
            {
                {Defense.None, null},
                {Defense.Armor, null},
                {Defense.SmallShield, Defenses.SmallShield},
                {Defense.LargeShield, Defenses.LargeShield}
            };

            SoldierEnum = new Dictionary<Soldier, Mesh>
            {
                {Soldier.Armored, Soldiers.Armored},
                {Soldier.Unarmored, Soldiers.Unarmored},
                {Soldier.Mounted, Soldiers.Mounted}
            };
        }

        [Serializable]
        public class WeaponMeshHolder
        {
            public Mesh Longbow;
            public Mesh Pike;
            public Mesh Shortbow;
            public Mesh Sword;
        }

        [Serializable]
        public class DefenseMeshHolder
        {
            public Mesh LargeShield;
            public Mesh SmallShield;
        }

        [Serializable]
        public class SoldierMeshHolder
        {
            public Mesh Armored;
            public Mesh Mounted;
            public Mesh Unarmored;
        }
    }
}