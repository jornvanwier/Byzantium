using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Util;
using UnityEngine;
using static Assets.Scripts.Game.Units.MeshDrawableUnit;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Century : UnitBase, IMultipleUnits<Contubernium>
    {
        private const float ChildSpacing = 1.3f;
        private readonly List<Contubernium> contubernia = new List<Contubernium>();
        private DrawingSet set;

        private Century(Faction faction)
        {
            Commander = new Commander(this, faction);
        }

        public override string UnitName => "Century";

        public override int Health
        {
            get { return contubernia[0].Health; }
            set
            {
                foreach (Contubernium contubernium in this)
                    contubernium.Health = value;
            }
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
            set = Prefetch(this);
        }

        public void RemoveUnit(Contubernium unit)
        {
            int index = contubernia.IndexOf(unit);
            contubernia.RemoveAt(index);
            set = Prefetch(this);
        }

        IEnumerator<Contubernium> IEnumerable<Contubernium>.GetEnumerator()
        {
            return contubernia.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<Contubernium>) this).GetEnumerator();
        }

        public static Century CreateMixedUnit(Faction faction)
        {
            var century = new Century(faction) {Formation = new SetColumnFormation(5)};

            // Frontline with swords
            for (int i = 0; i < 4; ++i)
                century.AddUnit(Contubernium.CreateSwordUnit(faction));

            // Mid with pikes
            for (int i = 0; i < 3; ++i)
                century.AddUnit(Contubernium.CreatePikeUnit(faction));

            // Backline with bows
            for (int i = 0; i < 3; ++i)
                century.AddUnit(Contubernium.CreateBowUnit(faction));

            century.IsCavalry = false;

            return century;
        }

        public static Century CreateCavalryUnit(Faction faction)
        {
            var century = new Century(faction) {Formation = new SquareFormation()};

            for (int i = 0; i < 16; i++)
            {
                century.AddUnit(Contubernium.CreateSwordCavalryUnit(faction));
            }

            century.IsCavalry = true;

            return century;
        }

        public override void Draw()
        {
            if (set != null)
                DrawAll(set);
        }
    }
}