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

        public Mesh mesh;
        public Material material;
        public Transform transform;

        private readonly Int2 dimensions = new Int2(1, 1);
        public SoldierType soldierType;
        private Vector3 oldPosition = Vector3.zero;

        public MeshDrawableUnit(SoldierType type)
        {
            soldierType = type;
            if (type == SoldierType.HorseBow || type == SoldierType.HorseSpear || type == SoldierType.HorseSword)
            {
                IsCavalry = true;
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

        public override Vector2 DrawSize => new Vector2(0.22f, 0.16f);

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

        public class DrawingSet
        {
            public List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> swordMatricesSet = new List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>(128);
            public List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> spearMatricesSet = new List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>(128);
            public List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> bowMatricesSet = new List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>(128);
            public List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> hSwordMatricesSet = new List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>(128);
            public List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> hSpearMatricesSet = new List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>(128);
            public List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> hBowMatricesSet = new List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>(128);

            public Mesh sword = null, spear = null, bow = null, hsword = null, hspear = null, hbow = null;
            public Material msw = null, msp = null, msb = null, mhsw = null, mhsp = null, mhb = null;



        }

        public static DrawingSet Prefetch(UnitBase u)
        {
            DrawingSet set = new DrawingSet();

            Tuple<List<MeshDrawableUnit>, List<Matrix4x4>> swordMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000), new List<Matrix4x4>(1000));
            Tuple<List<MeshDrawableUnit>, List<Matrix4x4>> spearMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000), new List<Matrix4x4>(1000));
            Tuple<List<MeshDrawableUnit>, List<Matrix4x4>> bowMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000), new List<Matrix4x4>(1000));

            Tuple<List<MeshDrawableUnit>, List<Matrix4x4>> hSwordMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000), new List<Matrix4x4>(1000));
            Tuple<List<MeshDrawableUnit>, List<Matrix4x4>> hSpearMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000), new List<Matrix4x4>(1000));
            Tuple<List<MeshDrawableUnit>, List<Matrix4x4>> hBowMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000), new List<Matrix4x4>(1000));

            Mesh sword = null, spear = null, bow = null, hsword = null, hspear = null, hbow = null;
            Material msw = null, msp = null, msb = null, mhsw = null, mhsp = null, mhb = null;

            foreach (MeshDrawableUnit unit in u.AllUnits)
            {
                Quaternion rotate = unit.Rotation * unit.transform.rotation;
                Matrix4x4 m = Matrix4x4.TRS(unit.Position, rotate, unit.transform.localScale);
                switch (unit.soldierType)
                {
                    case SoldierType.Sword:
                        sword = unit.mesh;
                        msw = unit.material;
                        swordMatrices.Item1.Add(unit);
                        swordMatrices.Item2.Add(m);
                        if (swordMatrices.Item1.Count >= 1000)
                        {
                            set.swordMatricesSet.Add(swordMatrices);
                            swordMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000), new List<Matrix4x4>(1000));
                        }
                        break;
                    case SoldierType.Spear:
                        spear = unit.mesh;
                        msp = unit.material;
                        spearMatrices.Item1.Add(unit);
                        spearMatrices.Item2.Add(m);
                        if (spearMatrices.Item1.Count >= 1000)
                        {
                            set.spearMatricesSet.Add(spearMatrices);
                            spearMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000), new List<Matrix4x4>(1000));
                        }
                        break;
                    case SoldierType.Bow:
                        bow = unit.mesh;
                        msb = unit.material;
                        bowMatrices.Item1.Add(unit);
                        bowMatrices.Item2.Add(m);
                        if (bowMatrices.Item1.Count >= 1000)
                        {
                            set.bowMatricesSet.Add(bowMatrices);
                            bowMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000), new List<Matrix4x4>(1000));
                        }
                        break;
                    case SoldierType.HorseSword:
                        hsword = unit.mesh;
                        mhsw = unit.material;
                        hSwordMatrices.Item1.Add(unit);
                        hSwordMatrices.Item2.Add(m);
                        if (hSwordMatrices.Item1.Count >= 1000)
                        {
                            set.hSwordMatricesSet.Add(hSwordMatrices);
                            hSwordMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000), new List<Matrix4x4>(1000));
                        }
                        break;
                    case SoldierType.HorseSpear:
                        hspear = unit.mesh;
                        mhsp = unit.material;
                        hSpearMatrices.Item1.Add(unit);
                        hSpearMatrices.Item2.Add(m);
                        if (hSpearMatrices.Item1.Count >= 1000)
                        {
                            set.hSpearMatricesSet.Add(hSpearMatrices);
                            hSpearMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000), new List<Matrix4x4>(1000));
                        }
                        break;
                    case SoldierType.HorseBow:
                        hbow = unit.mesh;
                        mhb = unit.material;
                        hBowMatrices.Item1.Add(unit);
                        hBowMatrices.Item2.Add(m);
                        if (hBowMatrices.Item1.Count >= 1000)
                        {
                            set.hBowMatricesSet.Add(hBowMatrices);
                            hBowMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000), new List<Matrix4x4>(1000));
                        }
                        break;
                }
            }

            set.swordMatricesSet.Add(swordMatrices);
            set.spearMatricesSet.Add(spearMatrices);
            set.bowMatricesSet.Add(bowMatrices);

            set.hSwordMatricesSet.Add(hSwordMatrices);
            set.hSpearMatricesSet.Add(hSpearMatrices);
            set.hBowMatricesSet.Add(hBowMatrices);

            set.hbow = hbow;
            set.hsword = hsword;
            set.hspear = hspear;

            set.spear = spear;
            set.sword = sword;
            set.bow = bow;

            set.msw = msw;
            set.msp = msp;
            set.msb = msb;

            set.mhb = mhb;
            set.mhsp = mhsp;
            set.mhsw = mhsw;



            return set;
        }

        public static void DrawAll(List<Tuple<List<MeshDrawableUnit>,List<Matrix4x4>>> list, Mesh mesh, Material mat)
        {
            foreach (var s in list)
            {
                List<MeshDrawableUnit> units = s.Item1;
                List<Matrix4x4> matrices = s.Item2;

                for (int i = 0; i < units.Count; ++i)
                {
                    Quaternion rotate = units[i].Rotation * units[i].transform.rotation;
                    Matrix4x4 m = Matrix4x4.TRS(units[i].Position, rotate, units[i].transform.localScale);
                    matrices[i] = m;
                }

                Graphics.DrawMeshInstanced(mesh, 0, mat, matrices);

            }               
        }

        public static void DrawAll(DrawingSet set)
        {

            List<Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh,Material>>> list = new List<Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>>>
            {
                new Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>>(set.bowMatricesSet, new Tuple<Mesh, Material>(set.bow, set.msb)),
                 new Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>>(set.spearMatricesSet, new Tuple<Mesh, Material>(set.spear, set.msp)),
                  new Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>>(set.swordMatricesSet, new Tuple<Mesh, Material>(set.sword, set.msw)),
                   new Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>>(set.hBowMatricesSet, new Tuple<Mesh, Material>(set.hbow, set.mhb)),
                    new Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>>(set.hSpearMatricesSet, new Tuple<Mesh, Material>(set.hspear, set.mhsw)),
                     new Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>>(set.hSwordMatricesSet, new Tuple<Mesh, Material>(set.hsword, set.mhsw)),
            };

            foreach (var lt in list)
            {
                List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> items = lt.Item1;
                Material mat = lt.Item2.Item2;
                Mesh mesh = lt.Item2.Item1;

                foreach (var s in items)
                {
                    List<MeshDrawableUnit> units = s.Item1;
                    List<Matrix4x4> matrices = s.Item2;

                    for (int i = 0; i < units.Count; ++i)
                    {
                        Quaternion rotate = units[i].Rotation * units[i].transform.rotation;
                        Matrix4x4 m = Matrix4x4.TRS(units[i].Position, rotate, units[i].transform.localScale);
                        matrices[i] = m;
                    }

                    if(matrices.Count > 0)
                        Graphics.DrawMeshInstanced(mesh, 0, mat, matrices);

                }
            }
        }
    }
}