using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units;
using Game.Units.Formation;
using UnityEngine;

namespace Game.Units.Groups
{
    public class Century : UnitBase, IMultipleUnits<Contubernium>
    {
        public override float DefaultSpeed => 1.5f;
        private readonly List<Contubernium> contubernia = new List<Contubernium>();

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

        public override Vector3 Position
        {
            set
            {
                base.Position = value;
                Formation.Order(this);
            }
        }

        public override int UnitCount => contubernia.Count;

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

        public static Century CreateMixedUnit()
        {
            var century = new Century {Formation = new SquareFormation()};

            // Frontline with swords
            for (int i = 0; i < 4; ++i)
            {
                century.AddUnit(Contubernium.CreateSwordUnit());
            }

            // Mid with pikes
            for (int i = 0; i < 3; ++i)
            {
                century.AddUnit(Contubernium.CreatePikeUnit());
            }

            // Backline with bows
            for (int i = 0; i < 3; ++i)
            {
                century.AddUnit(Contubernium.CreateLongbowUnit());
            }

            return century;
        }

        public override void Draw()
        {
            foreach (Contubernium unit in this)
            {
                unit.Draw();
            }
        }
    }
}