using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Cavalry : UnitBase, IMultipleUnits<DrawableUnit>
    {
        private List<DrawableUnit> units;



        public void AddUnit(DrawableUnit unit)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<DrawableUnit> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void RemoveUnit(DrawableUnit unit)
        {
            throw new NotImplementedException();
        }
    }
}