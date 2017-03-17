using Assets.Game.Units.Unit_Enums;
using UnityEngine;

namespace Assets.Game.Units
{
    public abstract class DrawableUnit : UnitBase
    {
        private Mesh mesh;

        protected DrawableUnit(Mesh mesh, Defense defense = Defense.Armor, Weapon weapon = Weapon.Sword,
            Movement movement = Movement.Foot)
        {
            this.mesh = mesh;

            DefenseType = defense;
            WeaponType = weapon;
            MovementType = movement;
        }

        public Defense DefenseType { get; }
        public Weapon WeaponType { get; }
        public Movement MovementType { get; }
    }
}