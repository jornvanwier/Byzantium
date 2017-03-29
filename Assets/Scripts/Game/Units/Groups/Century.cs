using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Map;
using Assets.Scripts.Util;
using Game.Units;
using Game.Units.Formation;
using Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Century : UnitBase, IMultipleUnits<Contubernium>
    {
        private readonly List<Contubernium> contubernia = new List<Contubernium>();
        public override float DefaultSpeed => 1.5f;

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

        public override Vector2 DrawSize => ChildSpacing * Vector2.Scale(contubernia[0].DrawSize, ChildrenDimensions + new Int2(0,2));
        protected override float ChildSpacing => 1f;

        public IEnumerator<MeshDrawableUnit> DrawableUnitsEnumerator
        {
            get { return contubernia.SelectMany(unit => unit.AllUnits).GetEnumerator(); }
        }

        public override IEnumerable<MeshDrawableUnit> AllUnits => DrawableUnitsEnumerator.Iterate();

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
            var century = new Century {Formation = new SetColumnFormation()};

            // Frontline with swords
            for (int i = 0; i < 4; ++i)
                century.AddUnit(Contubernium.CreateSwordUnit());

            // Mid with pikes
            for (int i = 0; i < 3; ++i)
                century.AddUnit(Contubernium.CreatePikeUnit());

            // Backline with bows
            for (int i = 0; i < 3; ++i)
                century.AddUnit(Contubernium.CreateLongbowUnit());

            return century;
        }

        public override void Draw()
        {
            foreach (Contubernium unit in this)
                unit.Draw();
        }
    }
}