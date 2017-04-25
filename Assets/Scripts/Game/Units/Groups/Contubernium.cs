using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Units.Config;
using Assets.Scripts.Game.Units.Controllers;
using Assets.Scripts.Game.Units.Formation;
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

        public override float DefaultSpeed => 1.5f;

        public override Vector2 GroupSpacing => new Vector2(0.1f, 0.1f);

        public UnitConfig Config { get; private set; }

        public Contubernium CurrentEnemy { get; set; }

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

        public SoldierType Type { get; private set; }

        public static Contubernium CreateCustomUnit(Faction faction, SoldierType unitType)
        {
            var contuberium = new Contubernium(faction) {Formation = new SquareFormation()};

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

        public void Attack(Contubernium enemy)
        {
            Vector3 towardsEnemy = Vector3.MoveTowards(Position, enemy.Position, Config.MovementSpeed);
            Rotation = Quaternion.LookRotation(towardsEnemy);

            float enemyDistance = Vector3.Distance(Position, enemy.Position);
            bool isInRange = enemyDistance < Config.Range;
            bool canAttack = Time.realtimeSinceStartup - lastAttack > Config.AttackSpeed;
            if (isInRange)
            {
                if (Config.Range - enemyDistance > Config.Range / 2)
                    Position = Vector3.MoveTowards(enemy.Position, Position, Config.MovementSpeed);

                if (canAttack)
                {
                    Debug.Log("Unit ATTACK!");
                    float multiplierVsEnemy = Config.VersusMultipliers[enemy.Type];
                    float damageDone = Config.Damage * multiplierVsEnemy * enemy.Config.Defense;

                    enemy.Health -= (int) damageDone;
                    if (enemy.IsDead)
                        CurrentEnemy = null;

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