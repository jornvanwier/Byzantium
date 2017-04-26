using System.Collections.Generic;

namespace Assets.Scripts.Game.Units.Config
{
    public class UnitConfig
    {
        public static UnitConfig Sword = new UnitConfig
        {
            Range = 1,
            Damage = 160,
            AttackSpeed = 3,
            Defense = 0.9f,
            MovementSpeed = 1.2f,
            VersusMultipliers = new Dictionary<SoldierType, float>
            {
                {SoldierType.Sword, 1},
                {SoldierType.Bow, 1.5f},
                {SoldierType.Spear, 1.5f},
                {SoldierType.HorseSword, 0.5f},
                {SoldierType.HorseBow, 0.5f},
                {SoldierType.HorseSpear, 0.5f}
            }
        };

        public static UnitConfig Spear = new UnitConfig
        {
            Range = 1.5f,
            Damage = 240,
            AttackSpeed = 2,
            Defense = 0.6f,
            MovementSpeed = 1.5f,
            VersusMultipliers = new Dictionary<SoldierType, float>
            {
                {SoldierType.Sword, 1},
                {SoldierType.Bow, 1.5f},
                {SoldierType.Spear, 1f},
                {SoldierType.HorseSword, 2f},
                {SoldierType.HorseBow, 2f},
                {SoldierType.HorseSpear, 2f}
            }
        };

        public static UnitConfig Bow = new UnitConfig
        {
            Range = 5,
            Damage = 120,
            AttackSpeed = 0.75f,
            Defense = 1,
            MovementSpeed = 1.5f,
            VersusMultipliers = new Dictionary<SoldierType, float>
            {
                {SoldierType.Sword, 1},
                {SoldierType.Bow, 1},
                {SoldierType.Spear, 1},
                {SoldierType.HorseSword, 0.5f},
                {SoldierType.HorseBow, 0.5f},
                {SoldierType.HorseSpear, 0.5f}
            }
        };

        public static UnitConfig HorseSword = new UnitConfig
        {
            Range = 1f,
            Damage = 160,
            AttackSpeed = 3,
            Defense = 0.93f,
            MovementSpeed = 2f,
            VersusMultipliers = new Dictionary<SoldierType, float>
            {
                {SoldierType.Sword, 2},
                {SoldierType.Bow, 2},
                {SoldierType.Spear, 1},
                {SoldierType.HorseSword, 1},
                {SoldierType.HorseBow, 1},
                {SoldierType.HorseSpear, 1}
            }
        };

        public static UnitConfig HorseSpear = new UnitConfig
        {
            Range = 1.5f,
            Damage = 240,
            AttackSpeed = 2,
            Defense = 0.7f,
            MovementSpeed = 2f,
            VersusMultipliers = new Dictionary<SoldierType, float>
            {
                {SoldierType.Sword, 1.5f},
                {SoldierType.Bow, 1.5f},
                {SoldierType.Spear, 1},
                {SoldierType.HorseSword, 1.5f},
                {SoldierType.HorseBow, 1.5f},
                {SoldierType.HorseSpear, 1.5f}
            }
        };

        public static UnitConfig HorseBow = new UnitConfig
        {
            Range = 4,
            Damage = 120,
            AttackSpeed = 0.75f,
            Defense = 1,
            MovementSpeed = 2f,
            VersusMultipliers = new Dictionary<SoldierType, float>
            {
                {SoldierType.Sword, 1},
                {SoldierType.Bow, 1},
                {SoldierType.Spear, 1},
                {SoldierType.HorseSword, 0.5f},
                {SoldierType.HorseBow, 0.5f},
                {SoldierType.HorseSpear, 0.5f}
            }
        };

        private const float RangeModifier = 0.5f;

        private float range;

        public float Range
        {
            get { return range * RangeModifier; }
            private set { range = value; }
        }

        public int Damage { get; private set; }
        public float AttackSpeed { get; private set; }
        public float Defense { get; private set; }
        public float MovementSpeed { get; private set; }
        public Dictionary<SoldierType, float> VersusMultipliers { get; private set; }
    }
}