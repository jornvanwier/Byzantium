namespace Assets.Scripts.Game.Units
{
    public class UnitConfig
    {
        public float Range {get; private set; }
        public float Damage { get; private set; }
        public float AttackSpeed { get; private set; }
        public float Defense { get; private set; }
        public float MovementSpeed { get; private set; }

        public static UnitConfig Sword = new UnitConfig
        {
            Range = 1,
            Damage = 20,
            AttackSpeed = 3,
            Defense = 0.9f,
            MovementSpeed = 1.2f
        };

        public static UnitConfig Spear = new UnitConfig
        {
            Range = 2,
            Damage = 30,
            AttackSpeed = 6,
            Defense = 0.6f,
            MovementSpeed = 1.5f
        };

        public static UnitConfig Bow = new UnitConfig
        {
            Range = 5,
            Damage = 15,
            AttackSpeed = 4,
            Defense = 1,
            MovementSpeed = 1.5f
        };

        public static UnitConfig HorseSword = new UnitConfig
        {
            Range = 1.5f,
            Damage = 20,
            AttackSpeed = 3,
            Defense = 0.93f,
            MovementSpeed = 2f
        };

        public static UnitConfig HorseSpear = new UnitConfig
        {
            Range = 2.5f,
            Damage = 30,
            AttackSpeed = 6,
            Defense = 0.7f,
            MovementSpeed = 2f
        };

        public static UnitConfig HorseBow = new UnitConfig
        {
            Range = 5,
            Damage = 15,
            AttackSpeed = 5,
            Defense = 1,
            MovementSpeed = 2f
        };
    }
}