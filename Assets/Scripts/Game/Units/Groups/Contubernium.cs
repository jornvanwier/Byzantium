using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Unit_Enums;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Contubernium : UnitBase, IMultipleUnits<MeshDrawableUnit>
    {
        private readonly List<MeshDrawableUnit> drawableUnits = new List<MeshDrawableUnit>();
        public override float DefaultSpeed => 1.5f;

        public override Vector3 Position
        {
            set
            {
                base.Position = value;
                Formation.Order(this);
            }
        }

        public override int UnitCount => drawableUnits.Count;

        public override Quaternion Rotation
        {
            get { return base.Rotation; }
            set
            {
                base.Rotation = value;
                foreach (UnitBase child in this)
                    child.Rotation = value;
            }
        }

        public void AddUnit(MeshDrawableUnit unit)
        {
            drawableUnits.Add(unit);
        }

        public void RemoveUnit(MeshDrawableUnit unit)
        {
            int index = drawableUnits.IndexOf(unit);
            drawableUnits.RemoveAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            return drawableUnits.GetEnumerator();
        }

        public static Contubernium CreateSwordUnit()
        {
            var contubernium = new Contubernium();

            for (var i = 0; i < 8; ++i)
                contubernium.AddUnit(new MeshDrawableUnit(
                    Defense.SmallShield,
                    Weapon.Sword,
                    Soldier.Armored
                ));

            return null;
        }

        public int GetGroupSize()
        {
            return drawableUnits.Count;
        }

        public override void Draw()
        {
            foreach (MeshDrawableUnit u in drawableUnits)
                u.Draw();
        }
    }
}