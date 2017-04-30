using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Util;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Game.Units
{
    public class MeshDrawableUnit : UnitBase
    {
        private const int StartHealth = 200;
        public static readonly Vector2 horseSize = new Vector2(0.4f, 1.0f);
        public static readonly Vector2 manSize = new Vector2(0.4f, 0.5f);
        private readonly float gravity = -Physics.gravity.y / 1000;
        private readonly SoldierType soldierType;
        private float jumpSpeed;
        private float nextJumpHeight;

        public MeshDrawableUnit(SoldierType type)
        {
            soldierType = type;
            if (type == SoldierType.HorseBow || type == SoldierType.HorseSpear || type == SoldierType.HorseSword)
                IsCavalry = true;

            GameObject m;
            switch (soldierType)
            {
                case SoldierType.Sword:
                    m = UnitMeshes[0];
                    break;
                case SoldierType.Spear:
                    m = UnitMeshes[1];
                    break;
                case SoldierType.Bow:
                    m = UnitMeshes[2];
                    break;
                case SoldierType.HorseSword:
                    m = UnitMeshes[3];
                    break;
                case SoldierType.HorseSpear:
                    m = UnitMeshes[4];
                    break;
                case SoldierType.HorseBow:
                    m = UnitMeshes[5];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Mesh = m.GetComponent<MeshFilter>().sharedMesh;
            Material = m.GetComponent<MeshRenderer>().sharedMaterial;
            Transform = m.transform;
        }

        public override int MaxHealth => StartHealth;

        public static List<GameObject> UnitMeshes { get; set; } = null;

        public Mesh Mesh { get; set; }
        public Material Material { get; set; }
        public Transform Transform { get; set; }

        private int health = StartHealth;

        public override int Health
        {
            // ReSharper disable ArrangeAccessorOwnerBody
            get { return health; }
            // ReSharper restore ArrangeAccessorOwnerBody
            set
            {
                health = value;
                if (health <= 0)
                {
                    Rotation = IsCavalry ? Quaternion.Euler(-90, 0, 90) : Quaternion.Euler(-90, 0, 0);
                }
            }
        }

        public override Vector2 DrawSize => IsCavalry ? horseSize : manSize;

        public override int UnitCount => 1;

        public IEnumerator<MeshDrawableUnit> DrawableUnitsEnumerator
        {
            get { yield return this; }
        }

        public override IEnumerable<MeshDrawableUnit> AllUnits => DrawableUnitsEnumerator.Iterate();

        public override string UnitName => "Single Unit";

        public override IEnumerable<Contubernium> Contubernia
        {
            get { throw new Exception("This unit does not contain children of type contubernia"); }
        }

        public override void SetPositionInstant(Vector3 pos)
        {
            Position = pos;
        }

        public void Jump(float height = 1)
        {
            jumpSpeed = height;
        }

        public void Bounce(float height = 1)
        {
            nextJumpHeight = height;
        }

        public void JumpUpdate()
        {
            Position += new Vector3(0, jumpSpeed, 0);
            if (Position.y > 0)
                jumpSpeed -= Math.Min(gravity, Position.y);
            else if (nextJumpHeight > 0)
                Jump(nextJumpHeight);
        }

        public override void Draw()
        {
            Quaternion rotate = Rotation * Transform.rotation;
            Graphics.DrawMesh(Mesh, Matrix4x4.TRS(Position, rotate, Transform.localScale), Material, 0);
        }

        public class DrawingSet
        {
            public List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> BowMatricesSet =
                new List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>(128);

            public List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> HBowMatricesSet =
                new List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>(128);

            public List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> HSpearMatricesSet =
                new List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>(128);

            public List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> HSwordMatricesSet =
                new List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>(128);

            public List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> SpearMatricesSet =
                new List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>(128);

            public Mesh Sword, Spear, Bow, Hsword, Hspear, Hbow;

            public List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> SwordMatricesSet =
                new List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>(128); // ReSharper disable InconsistentNaming

            public Material msw, msp, msb, mhsw, mhsp, mhb;

            // ReSharper enable InconsistentNaming
        }

        public static DrawingSet Prefetch(UnitBase u)
        {
            var set = new DrawingSet();

            var swordMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000),
                new List<Matrix4x4>(1000));
            var spearMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000),
                new List<Matrix4x4>(1000));
            var bowMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000),
                new List<Matrix4x4>(1000));

            var hSwordMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000),
                new List<Matrix4x4>(1000));
            var hSpearMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000),
                new List<Matrix4x4>(1000));
            var hBowMatrices = new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000),
                new List<Matrix4x4>(1000));

            Mesh sword = null, spear = null, bow = null, hsword = null, hspear = null, hbow = null;
            Material msw = null, msp = null, msb = null, mhsw = null, mhsp = null, mhb = null;

            foreach (MeshDrawableUnit unit in u.AllUnits)
            {
                if (unit.IsDead) continue;

                Quaternion rotate = unit.Rotation * unit.Transform.rotation;
                Matrix4x4 m = Matrix4x4.TRS(unit.Position, rotate, unit.Transform.localScale);
                switch (unit.soldierType)
                {
                    case SoldierType.Sword:
                        sword = unit.Mesh;
                        msw = unit.Material;
                        swordMatrices.Item1.Add(unit);
                        swordMatrices.Item2.Add(m);
                        if (swordMatrices.Item1.Count >= 1000)
                        {
                            set.SwordMatricesSet.Add(swordMatrices);
                            swordMatrices =
                                new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000),
                                    new List<Matrix4x4>(1000));
                        }
                        break;
                    case SoldierType.Spear:
                        spear = unit.Mesh;
                        msp = unit.Material;
                        spearMatrices.Item1.Add(unit);
                        spearMatrices.Item2.Add(m);
                        if (spearMatrices.Item1.Count >= 1000)
                        {
                            set.SpearMatricesSet.Add(spearMatrices);
                            spearMatrices =
                                new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000),
                                    new List<Matrix4x4>(1000));
                        }
                        break;
                    case SoldierType.Bow:
                        bow = unit.Mesh;
                        msb = unit.Material;
                        bowMatrices.Item1.Add(unit);
                        bowMatrices.Item2.Add(m);
                        if (bowMatrices.Item1.Count >= 1000)
                        {
                            set.BowMatricesSet.Add(bowMatrices);
                            bowMatrices =
                                new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000),
                                    new List<Matrix4x4>(1000));
                        }
                        break;
                    case SoldierType.HorseSword:
                        hsword = unit.Mesh;
                        mhsw = unit.Material;
                        hSwordMatrices.Item1.Add(unit);
                        hSwordMatrices.Item2.Add(m);
                        if (hSwordMatrices.Item1.Count >= 1000)
                        {
                            set.HSwordMatricesSet.Add(hSwordMatrices);
                            hSwordMatrices =
                                new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000),
                                    new List<Matrix4x4>(1000));
                        }
                        break;
                    case SoldierType.HorseSpear:
                        hspear = unit.Mesh;
                        mhsp = unit.Material;
                        hSpearMatrices.Item1.Add(unit);
                        hSpearMatrices.Item2.Add(m);
                        if (hSpearMatrices.Item1.Count >= 1000)
                        {
                            set.HSpearMatricesSet.Add(hSpearMatrices);
                            hSpearMatrices =
                                new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000),
                                    new List<Matrix4x4>(1000));
                        }
                        break;
                    case SoldierType.HorseBow:
                        hbow = unit.Mesh;
                        mhb = unit.Material;
                        hBowMatrices.Item1.Add(unit);
                        hBowMatrices.Item2.Add(m);
                        if (hBowMatrices.Item1.Count >= 1000)
                        {
                            set.HBowMatricesSet.Add(hBowMatrices);
                            hBowMatrices =
                                new Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>(new List<MeshDrawableUnit>(1000),
                                    new List<Matrix4x4>(1000));
                        }
                        break;
                }
            }

            set.SwordMatricesSet.Add(swordMatrices);
            set.SpearMatricesSet.Add(spearMatrices);
            set.BowMatricesSet.Add(bowMatrices);

            set.HSwordMatricesSet.Add(hSwordMatrices);
            set.HSpearMatricesSet.Add(hSpearMatrices);
            set.HBowMatricesSet.Add(hBowMatrices);

            set.Hbow = hbow;
            set.Hsword = hsword;
            set.Hspear = hspear;

            set.Spear = spear;
            set.Sword = sword;
            set.Bow = bow;

            set.msw = msw;
            set.msp = msp;
            set.msb = msb;

            set.mhb = mhb;
            set.mhsp = mhsp;
            set.mhsw = mhsw;


            return set;
        }

        public static void DrawAll(List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> list, Mesh mesh, Material mat)
        {
            foreach (Tuple<List<MeshDrawableUnit>, List<Matrix4x4>> s in list)
            {
                List<MeshDrawableUnit> units = s.Item1;
                List<Matrix4x4> matrices = s.Item2;

                for (int i = 0; i < units.Count; ++i)
                {
                    Quaternion rotate = units[i].Rotation * units[i].Transform.rotation;
                    Matrix4x4 m = Matrix4x4.TRS(units[i].Position, rotate, units[i].Transform.localScale);
                    matrices[i] = m;
                }
                if (matrices.Count > 0)
                    Graphics.DrawMeshInstanced(mesh, 0, mat, matrices.ToArray(), matrices.Count, null,
                        ShadowCastingMode.Off, false);
            }
        }

        public static void DrawAll(DrawingSet set)
        {
            var list = new List<Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>>>
            {
                new Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>>(
                    set.BowMatricesSet, new Tuple<Mesh, Material>(set.Bow, set.msb)),
                new Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>>(
                    set.SpearMatricesSet, new Tuple<Mesh, Material>(set.Spear, set.msp)),
                new Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>>(
                    set.SwordMatricesSet, new Tuple<Mesh, Material>(set.Sword, set.msw)),
                new Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>>(
                    set.HBowMatricesSet, new Tuple<Mesh, Material>(set.Hbow, set.mhb)),
                new Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>>(
                    set.HSpearMatricesSet, new Tuple<Mesh, Material>(set.Hspear, set.mhsw)),
                new Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>>(
                    set.HSwordMatricesSet, new Tuple<Mesh, Material>(set.Hsword, set.mhsw))
            };

            foreach (Tuple<List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>>, Tuple<Mesh, Material>> lt in list)
            {
                List<Tuple<List<MeshDrawableUnit>, List<Matrix4x4>>> items = lt.Item1;
                Material mat = lt.Item2.Item2;
                Mesh mesh = lt.Item2.Item1;

                foreach (Tuple<List<MeshDrawableUnit>, List<Matrix4x4>> s in items)
                {
                    List<MeshDrawableUnit> units = s.Item1;
                    List<Matrix4x4> matrices = s.Item2;

                    for (int i = 0; i < units.Count; ++i)
                    {
                        Quaternion rotate = units[i].Rotation * units[i].Transform.rotation;
                        Matrix4x4 m = Matrix4x4.TRS(units[i].Position, rotate, units[i].Transform.localScale);
                        matrices[i] = m;
                    }

                    if (matrices.Count > 0)
                        Graphics.DrawMeshInstanced(mesh, 0, mat, matrices.ToArray(), matrices.Count, null,
                            ShadowCastingMode.Off, false);
                }
            }
        }
    }
}