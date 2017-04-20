using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Formation
{
    class RowFormation : FormationBase
    {
        public override void Order(Legion unit, bool instant = false)
        {
            OrderAny<Legion, Cohort>(unit, instant);
        }

        public override void Order(Contubernium unit, bool instant = false)
        {
            OrderAny<Contubernium, MeshDrawableUnit>(unit, instant);
        }

        public override void Order(Cohort unit, bool instant = false)
        {
            OrderAny<Cohort, Century>(unit, instant);
        }

        public override void Order(Century unit, bool instant = false)
        {
            OrderAny<Century, Contubernium>(unit, instant);
        }


        public static void OrderAny<T, TChild>(T unit, bool instant = false) where T : UnitGroup<TChild> where TChild : UnitBase
        {
            int unitCount = unit.UnitCount;

            if (unitCount <= 0)
                throw new Exception("Error!");

            Vector2 individualUnitSize = unit.First().DrawSize;

            Vector2 offsets;
            offsets = (unitCount / 2.0f) * individualUnitSize - (individualUnitSize / 2.0f);
            offsets *= -1;

            var localPositions = new List<Vector3>();

            for (int i = 0; i < unitCount; ++i)
            {
                float x, z;

                x = offsets.x + individualUnitSize.x * i;
                z = 0;
                localPositions.Add(new Vector3(x,0,z));
            }
            ProcessLocalOffsets<T, TChild>(localPositions, unit, instant);

            unit.ChildrenDimensions = new Map.Int2(unitCount, 1);
        }




    }
}
