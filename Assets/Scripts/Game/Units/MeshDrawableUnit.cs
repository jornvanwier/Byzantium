using System;
using System.Collections.Generic;
using Assets.Scripts.Map;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public enum SoldierType
    {
        Sword,
        Spear,
        Bow,
        HorseSword,
        HorseSpear,
        HorseBow
    }



    public class MeshDrawableUnit : UnitBase
    {
        private const int StartHealth = 200;
        public static List<GameObject> unitMeshes = null;

        private Mesh mesh;
        private Material material;
        private Transform transform;

        private readonly Int2 dimensions = new Int2(1, 1);
        private SoldierType soldierType;
        private Vector3 oldPosition = Vector3.zero;

        public MeshDrawableUnit(SoldierType type)
        {
            soldierType = type;
            if (type == SoldierType.HorseBow || type == SoldierType.HorseSpear || type == SoldierType.HorseSword)
            {
                IsCavalry = false;
            }

            GameObject m = null;
            switch (soldierType)
            {
                case SoldierType.Sword:
                    m = unitMeshes[0];
                    break;
                case SoldierType.Spear:
                    m = unitMeshes[1];
                    break;
                case SoldierType.Bow:
                    m = unitMeshes[2];
                    break;
                case SoldierType.HorseSword:
                    m = unitMeshes[3];
                    break;
                case SoldierType.HorseSpear:
                    m = unitMeshes[4];
                    break;
                case SoldierType.HorseBow:
                    m = unitMeshes[5];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            mesh = m.GetComponent<MeshFilter>().sharedMesh;
            material = m.GetComponent<MeshRenderer>().sharedMaterial;
            transform = m.transform;

        }

        public override int Health { get; set; } = StartHealth;

        public override Int2 ChildrenDimensions
        {
            get { return dimensions; }
            set { throw new MemberAccessException("Cannot set dimensions of this object."); }
        }

        private readonly Vector2 horseSize = new Vector2(0.22f, 0.2f);
        private readonly Vector2 manSize = new Vector2(0.22f, 0.16f);

        public override Vector2 DrawSize => IsCavalry ? manSize : manSize;

        public override float DefaultSpeed => 1.5f;

        public override int UnitCount => 1;

        public IEnumerator<MeshDrawableUnit> DrawableUnitsEnumerator
        {
            get { yield return this; }
        }

        public override IEnumerable<MeshDrawableUnit> AllUnits => DrawableUnitsEnumerator.Iterate();

        public override string UnitName => "Single Unit";

        public override void Draw()
        {
            Quaternion rotate = Rotation * transform.rotation;
            Graphics.DrawMesh(mesh, Matrix4x4.TRS(Position, rotate, transform.localScale), material, 0);
        }
    }
}