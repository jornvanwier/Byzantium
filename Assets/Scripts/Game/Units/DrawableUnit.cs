using Assets.Scripts.Game.Units.Unit_Enums;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public class MeshDrawableUnit : UnitBase
    {
        public static Material Material = new Material(Shader.Find("Standard"));

        public MeshDrawableUnit(Mesh mesh, Defense defense = Defense.Armor, Weapon weapon = Weapon.Sword,
            Movement movement = Movement.Foot)
        {
            Mesh = mesh;
            DefenseType = defense;
            WeaponType = weapon;
            MovementType = movement;
        }

        public Mesh Mesh { get; }
        public Defense DefenseType { get; }
        public Weapon WeaponType { get; }
        public Movement MovementType { get; }

        public override void Draw()
        {
            Graphics.DrawMesh(Mesh, Matrix4x4.TRS(Position, Rotation, new Vector3(0.1f, 0.1f, 0.1f)), Material, 0);
        }

        public override int UnitCount => 1;
    }
}