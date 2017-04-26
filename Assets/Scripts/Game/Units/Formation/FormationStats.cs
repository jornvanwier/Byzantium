namespace Assets.Scripts.Game.Units.Formation
{
    public class FormationStats
    {
        public float WalkSpeed { get; set; }
        public float AttackDamageMultiplier { get; set; }
        public float DefenseMultiplier { get; set; }

        public static float DefaultWalkSpeed { get; } = 1.5f;
    }
}