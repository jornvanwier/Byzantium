using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Util;
using UnityEngine;
using static Assets.Scripts.Game.Units.MeshDrawableUnit;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Century : UnitBase, IMultipleUnits<Contubernium>
    {
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

        public override Vector2 DrawSize => Vector2.Scale(contubernia[0].DrawSize, ChildrenDimensions);

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
            contubernia.Remove(unit);
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

        public override void SetPositionInstant(Vector3 pos)
        {
            base.Position = pos;
            Formation.Order(this, true);
        }

        public static Century CreateMixedUnit(Faction faction)
        {
            var century = new Century(faction) {Formation = new SetColumnFormation(2)};

            // Frontline with swords
            for (int i = 0; i < 2; ++i)
                century.AddUnit(Contubernium.CreateSwordUnit(faction));

            // Mid with pikes
            century.AddUnit(Contubernium.CreateSpearUnit(faction));

            // Backline with bows
            century.AddUnit(Contubernium.CreateBowUnit(faction));

            century.IsCavalry = false;

            return century;
        }

        public static Century CreateSwordCavalryUnit(Faction faction)
        {
            var century = new Century(faction) {Formation = new SetColumnFormation(2)};

            for (int i = 0; i < 4; i++)
                century.AddUnit(Contubernium.CreateSwordCavalryUnit(faction));

            century.IsCavalry = true;

            return century;
        }

        public static Century CreateCustomUnit(Faction faction, SoldierType type)
        {
            var century = new Century(faction) { Formation = new SetColumnFormation(2) };

            for (int i = 0; i < 4; i++)
            {
                Contubernium contubernium = Contubernium.CreateCustomUnit(faction, type);
                century.AddUnit(contubernium);
                century.IsCavalry = contubernium.IsCavalry;
            }
            
            return century;
        }

        public override void Draw()
        {
            if (set != null)
                DrawAll(set);
        }
    }
}