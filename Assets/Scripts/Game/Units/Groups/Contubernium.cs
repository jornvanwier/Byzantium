using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Contubernium : UnitBase, IMultipleUnits<MeshDrawableUnit>
    {
        public override float DefaultSpeed => 1.5f;

        public static Contubernium CreateSwordUnit()
        {
            var contubernium = new Contubernium();

            for (int i = 0; i < 8; ++i)
            {
//                contubernium.AddUnit(new MeshDrawableUnit());
            }

            return null;
        }


        private readonly List<MeshDrawableUnit> drawableUnits = new List<MeshDrawableUnit>();

        public override Vector3 Position
        {
            set
            {
                base.Position = value;
                Formation.Order(this);
            }
        }

        public override int UnitCount => drawableUnits.Count;

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