using Assets.Game.Units.Unit_Enums;
using UnityEngine;

namespace Assets.Game.Units
{
    public class MeshDrawableUnit : UnitBase
    {
        public Mesh mesh { get; }
        public Defense DefenseType { get; }
        public Weapon WeaponType { get; }
        public Movement MovementType { get; }
        public static Material material = new Material(Shader.Find("Standard"));

        public MeshDrawableUnit(Mesh mesh, Defense defense = Defense.Armor, Weapon weapon = Weapon.Sword,
            Movement movement = Movement.Foot)
        {
            this.mesh = mesh;
            DefenseType = defense;
            WeaponType = weapon;
            MovementType = movement;
        }

        public override void Draw()
        {
            Graphics.DrawMesh(mesh, Matrix4x4.TRS(Position, Rotation, new Vector3(0.1f,0.1f,0.1f)), material, 0);
        }


    }




}