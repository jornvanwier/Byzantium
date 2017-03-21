using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Century : UnitBase, IMultipleUnits<Contubernium>
    {
        protected new const float DefaultSpeed = 1.5f;
        private readonly List<Contubernium> contubernia = new List<Contubernium>();

        public void AddUnit(Contubernium unit)
        {
            contubernia.Add(unit);
        }

        public void RemoveUnit(Contubernium unit)
        {
            int index = contubernia.IndexOf(unit);
            contubernia.RemoveAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            return contubernia.GetEnumerator();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}