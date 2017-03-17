using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Contubernium : UnitBase, IMultipleUnits<DrawableUnit>
    {
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}