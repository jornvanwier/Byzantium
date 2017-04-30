namespace Assets.Scripts.Game.Units.Formation
{
    public class FormationStats
    {
        public float WalkSpeed { get; set; }
        public float AttackDamageMultiplier { get; set; }
        public float DefenseMultiplier { get; set; }

        public const float DefaultWalkSpeed = 2.5f;
    }
}