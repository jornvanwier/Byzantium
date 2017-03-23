using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Century : UnitBase, IMultipleUnits<Contubernium>
    {
        public new const float DefaultSpeed = 1.5f;
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

        public override Quaternion Rotation
        {
            get { return base.Rotation; }
            set
            {
                base.Rotation = value;
                foreach (UnitBase child in this)
                {
                    child.Rotation = value;
                }
            }
        }

        public override int UnitCount => contubernia.Count;

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}