using System.Collections.Generic;

namespace Assets.Game.Units.Groups
{
    public class Century : UnitBase, IMultipleUnits<Contubernium>
    {
        public List<Contubernium> Contubernia { get; } = new List<Contubernium>();

        public void AddUnit(Contubernium unit)
        {
            Contubernia.Add(unit);
        }

        public void RemoveUnit(Contubernium unit)
        {
            int index = Contubernia.IndexOf(unit);
            Contubernia.RemoveAt(index);
        }
    }
}