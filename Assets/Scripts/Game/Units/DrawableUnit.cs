using Assets.Scripts.Game.Units.Unit_Enums;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public class MeshDrawableUnit : UnitBase
    {
        public static Material Material = new Material(Shader.Find("Standard"));

        public MeshDrawableUnit(Mesh unitMesh, Mesh weaponMesh, Mesh defenseMesh = null, Defense defense = Defense.Armor,
            Weapon weapon = Weapon.Sword,
            Movement movement = Movement.Foot)
        {
            UnitMesh = unitMesh;
            WeaponMesh = weaponMesh;
            DefenseMesh = defenseMesh;
            DefenseType = defense;
            WeaponType = weapon;
            MovementType = movement;
        }

        public Mesh UnitMesh { get; }
        public Mesh WeaponMesh { get; }
        public Mesh DefenseMesh { get; }
        public Defense DefenseType { get; }
        public Weapon WeaponType { get; }
        public Movement MovementType { get; }

        public override void Draw()
        {
            Graphics.DrawMesh(UnitMesh, Matrix4x4.TRS(Position, Rotation, new Vector3(0.1f, 0.1f, 0.1f)), Material, 0);


            Vector3 WeaponPosition = (Position + new Vector3(0.2f, 0, 0));
            WeaponPosition = Rotation * WeaponPosition;

            Graphics.DrawMesh(WeaponMesh,
                Matrix4x4.TRS(WeaponPosition, Rotation, new Vector3(0.1f, 0.1f, 0.1f)), Material, 0);


            if (DefenseMesh != null)
            {
                Vector3 shieldPosition = Position + new Vector3(0, 0.2f, 0);
                Graphics.DrawMesh(DefenseMesh,
                    Matrix4x4.TRS(shieldPosition, Rotation, new Vector3(0.1f, 0.1f, 0.1f)),
                    Material, 0);
            }
        }
    }
}