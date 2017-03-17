using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Century : UnitBase, IMultipleUnits<Contubernium>
    {
        public void AddUnit(Contubernium unit)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Contubernium> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void RemoveUnit(Contubernium unit)
        {
            throw new NotImplementedException();
        }
    }
}