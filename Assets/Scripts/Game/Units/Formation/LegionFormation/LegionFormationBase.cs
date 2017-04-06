using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Groups;

namespace Assets.Scripts.Game.Units.Formation.LegionFormation
{
    public abstract class LegionFormationBase : FormationBase
    {
        public override void Order(Contubernium unit)
        {
            throw new FormationIncompatibleException(unit);
        }

        public override void Order(Cohort unit)
        {
            throw new FormationIncompatibleException(unit);
        }

        public override void Order(Century unit)
        {
            throw new FormationIncompatibleException(unit);
        }

        protected Cohort[] SortCavalryFirst(Legion legion)
        {
            var cohorts = new List<Cohort>();
            var cavalries = new List<Cohort>();

            foreach (Cohort cohort in legion)
            {
                if (cohort.IsCavalry)
                {
                    cavalries.Add(cohort);
                }
                else
                {
                    cohorts.Add(cohort);
                }
            }

            return cavalries.Concat(cohorts).ToArray();
        }
    }
}