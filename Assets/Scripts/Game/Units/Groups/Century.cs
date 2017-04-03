using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Util;
using Game.Units;
using Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Century : UnitBase, IMultipleUnits<Contubernium>
    {
        private const float ChildSpacing = 1.3f;
        private readonly List<Contubernium> contubernia = new List<Contubernium>();

        private Century(Faction faction)
        {
            Commander = new Commander(this, faction);
        }

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

        public override Vector2 DrawSize => ChildSpacing * Vector2.Scale(contubernia[0].DrawSize, ChildrenDimensions);

        public IEnumerator<MeshDrawableUnit> DrawableUnitsEnumerator
        {
            get
            {
                foreach (Contubernium contubernium in contubernia)
                foreach (MeshDrawableUnit drawableUnit in contubernium.AllUnits)
                    yield return drawableUnit;
            }
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

        public static Century CreateMixedUnit(Faction faction)
        {
            var century = new Century(faction) {Formation = new SetColumnFormation()};

            // Frontline with swords
            for (int i = 0; i < 4; ++i)
                century.AddUnit(Contubernium.CreateSwordUnit(faction));

            // Mid with pikes
            for (int i = 0; i < 3; ++i)
                century.AddUnit(Contubernium.CreatePikeUnit(faction));

            // Backline with bows
            for (int i = 0; i < 3; ++i)
                century.AddUnit(Contubernium.CreateLongbowUnit(faction));

            return century;
        }

        public override void Draw()
        {
            foreach (Contubernium unit in this)
                unit.Draw();
        }

        IEnumerator<Contubernium> IEnumerable<Contubernium>.GetEnumerator()
        {
            return contubernia.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<Contubernium>)this).GetEnumerator();
        }
    }
}