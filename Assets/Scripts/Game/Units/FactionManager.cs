using Assets.Scripts.Game.Units;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Game
{
    public static class FactionManager
    {
        public static int Amount;

        public static Faction[] Factions { get; private set; }

        public static void Init(int numFactions)
        {
            Amount = numFactions;
            Factions = new Faction[Amount];

            for (int i = 0; i < Amount; i++)
            {
                Factions[i] = new Faction();
            }
        }

        public static IEnumerable<Faction> GetOpponents(Faction faction)
        {
            return Factions.Where(f => f != faction).ToList();
        } 
    }
}