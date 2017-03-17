using Assets.Game.Units.Unit_Enums;
using UnityEngine;

namespace Assets.Game.Units
{
    public abstract class DrawableUnit : UnitBase
    {
        public Mesh Mesh { get; }
        public Defense DefenseType { get; }
        public Weapon WeaponType { get; }
        public Movement MovementType { get; }

        protected DrawableUnit(Mesh mesh, Defense defense = Defense.Armor, Weapon weapon = Weapon.Sword,
            Movement movement = Movement.Foot)
        {
            Mesh = mesh;
            DefenseType = defense;
            WeaponType = weapon;
            MovementType = movement;
        }
    }
}