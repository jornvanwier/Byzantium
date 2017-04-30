using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Config;
using Assets.Scripts.Game.Units.Controllers;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Game.Units.Formation.ContuberniumFormation;
using Assets.Scripts.Util;
using UnityEngine;
using static Assets.Scripts.Game.Units.MeshDrawableUnit;

namespace Assets.Scripts.Game.Units.Groups
{
    public class Contubernium : UnitGroup<MeshDrawableUnit>
    {
        private float lastAttack;

        public Contubernium(Faction faction) : base(faction)
        {
        }

        public IEnumerator<Contubernium> ContuberniumEnumerator
        {
            get { yield return this; }
        }

        public override IEnumerable<Contubernium> Contubernia => ContuberniumEnumerator.Iterate();

        public override string UnitName => "Contubernium";

        public override Vector2 GroupSpacing => new Vector2(0.1f, 0.1f);

        public UnitConfig Config { get; private set; }

        public Contubernium CurrentEnemy { get; set; }

        public SoldierType Type { get; private set; }

        public static Contubernium CreateSpearCavalryUnit(Faction faction)
        {
            return CreateCustomUnit(faction, SoldierType.HorseSpear);
        }

        public static Contubernium CreateSwordCavalryUnit(Faction faction)
        {
            return CreateCustomUnit(faction, SoldierType.HorseSword);
        }

        public static Contubernium CreateBowCavalryUnit(Faction faction)
        {
            return CreateCustomUnit(faction, SoldierType.HorseBow);
        }

        public static Contubernium CreateSwordUnit(Faction faction)
        {
            return CreateCustomUnit(faction, SoldierType.Sword);
        }

        public static Contubernium CreateBowUnit(Faction faction)
        {
            return CreateCustomUnit(faction, SoldierType.Bow);
        }

        public static Contubernium CreateSpearUnit(Faction faction)
        {
            return CreateCustomUnit(faction, SoldierType.Spear);
        }

        public static Contubernium CreateCustomUnit(Faction faction, SoldierType unitType)
        {
            var contuberium = new Contubernium(faction) {Formation = new SkirmisherFormation()};

            for (int i = 0; i < 8; ++i)
            {
                var mdm = new MeshDrawableUnit(unitType);
                contuberium.AddUnit(mdm);
                contuberium.IsCavalry = mdm.IsCavalry;
            }

            contuberium.Type = unitType;

            switch (unitType)
            {
                case SoldierType.Sword:
                    contuberium.Config = UnitConfig.Sword;
                    break;
                case SoldierType.Spear:
                    contuberium.Config = UnitConfig.Spear;
                    break;
                case SoldierType.Bow:
                    contuberium.Config = UnitConfig.Bow;
                    break;
                case SoldierType.HorseSword:
                    contuberium.Config = UnitConfig.HorseSword;
                    break;
                case SoldierType.HorseSpear:
                    contuberium.Config = UnitConfig.HorseSpear;
                    break;
                case SoldierType.HorseBow:
                    contuberium.Config = UnitConfig.HorseBow;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            contuberium.Set = Prefetch(contuberium);
            return contuberium;
        }

        public Contubernium ClosestEnemy(UnitController enemyArmy)
        {
            float closest = float.PositiveInfinity;
            Contubernium closestEnemy = null;
            foreach (Contubernium enemy in enemyArmy.AttachedUnit.Contubernia)
            {
                if (enemy.IsDead) continue;

                float distance = Vector3.Distance(enemy.Position, Position);
                if (!(distance < closest)) continue;
                closest = distance;
                closestEnemy = enemy;
            }
            return closestEnemy;
        }

        public void Kill()
        {
            Health = 0;
            (Parent as UnitGroup<Contubernium>)?.RemoveUnit(this);
        }

        private HashSet<FormationStats> AllFormationStatses()
        {
            var result = new HashSet<FormationStats>();
            UnitBase currentParent = this;
            while (currentParent.Parent != null)
            {
                result.Add(currentParent.Formation.Stats);
                currentParent = currentParent.Parent;
            }
            return result;
        }

        private static float AttackDamageMultiplier(IEnumerable<FormationStats> statses)
        {
            return statses.Select(s => s.AttackDamageMultiplier).Aggregate(1f, (a, b) => a * b);
        }

        private static float DefenseMultiplier(IEnumerable<FormationStats> statses)
        {
            return statses.Select(s => s.DefenseMultiplier).Aggregate(1f, (a, b) => a * b);
        }

        public void Attack(Contubernium enemy)
        {
            if (IsDead || enemy == null) return;

            Vector3 towardsEnemy = Vector3.MoveTowards(Position, enemy.Position, Config.MovementSpeed);
            Rotation = Quaternion.LookRotation(towardsEnemy);

            float enemyDistance = Vector3.Distance(Position, enemy.Position);
            bool isInRange = enemyDistance < Config.Range;
            bool canAttack = Time.realtimeSinceStartup - lastAttack > 1 / Config.AttackSpeed;
            if (isInRange)
            {
                if (Config.Range - enemyDistance > Config.Range / 2)
                    Position = Vector3.MoveTowards(enemy.Position, Position, Config.MovementSpeed);

                if (canAttack)
                {
                    HashSet<FormationStats> statses = AllFormationStatses();
                    float formationDamageMultiplier = AttackDamageMultiplier(statses);
                    float formationDefenseMultiplier = DefenseMultiplier(statses);

                    float multiplierVsEnemy = Config.VersusMultipliers[enemy.Type] * formationDamageMultiplier;
                    float damageDone = Config.Damage * multiplierVsEnemy * enemy.Config.Defense *
                                       formationDefenseMultiplier;

                    enemy.Health -= (int) damageDone;
                    if (enemy.IsDead)
                    {
                        CurrentEnemy = null;
                        enemy.Kill();
                    }

                    lastAttack = Time.realtimeSinceStartup;
                }
            }
            else
            {
                //walk towards enemy;
                Position = towardsEnemy;
            }
        }

        protected override void Order(bool instant = false)
        {
            Formation.Order(this, instant);
        }
    }
}