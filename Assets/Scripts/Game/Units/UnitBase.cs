﻿using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public abstract class UnitBase
    {
        private Commander commander;

        private Rect hitbox;
        private float walkSpeed = 1.0f;

        public virtual Vector3 Position { get; set; } = Vector3.zero;

        public virtual Quaternion Rotation { get; set; } = Quaternion.identity;

        public virtual IFormation Formation { get; set; }

        public float WalkSpeed
        {
            // ReSharper disable ArrangeAccessorOwnerBody
            get { return walkSpeed * (IsCavalry ? CavalrySpeedMultiplier : 1); }
            set { walkSpeed = value; }
            // ReSharper restore ArrangeAccessorOwnerBody
        }

        public float DefaultWalkSpeed
        {
            get { return 1.5f * (IsCavalry ? CavalrySpeedMultiplier : 1); }
        }

        public abstract Vector2 DrawSize { get; }

        public bool IsCavalry { get; protected set; }

        public bool RespectFormation { get; set; } = true;

        public UnitBase Parent { get; set; }

        public Rect Hitbox
        {
            get
            {
                hitbox.x = Position.x - DrawSize.x / 2;
                hitbox.y = Position.y - DrawSize.y / 2;
                hitbox.width = DrawSize.x;
                hitbox.height = DrawSize.y;
                return hitbox;
            }
        }


        public abstract int UnitCount { get; }
        public abstract IEnumerable<MeshDrawableUnit> AllUnits { get; }

        public abstract int MaxHealth { get; }
        public abstract int Health { get; set; }

        public static float CavalrySpeedMultiplier { get; } = 1.6f;

        public Commander Commander
        {
            get { return commander; }
            set
            {
                if (this is MeshDrawableUnit)
                    throw new ArgumentException("Single unit cannot have a commander");
                commander = value;
            }
        }

        public abstract string UnitName { get; }

        public bool IsDead => Health <= 0;

        public string Info
        {
            get
            {
                int soldierCount = 0;
                int cavalryCount = 0;
                foreach (MeshDrawableUnit meshDrawableUnit in AllUnits)
                    if (meshDrawableUnit.IsCavalry)
                        cavalryCount++;
                    else
                        soldierCount++;
                return UnitName +
                       "\nHealth:\t\t" + (float) Health +
                       "hp\nSoldiers:\t\t" + soldierCount +
                       "\nCavalry:\t" + cavalryCount;
            }
        }

        public abstract IEnumerable<Contubernium> Contubernia { get; }


        public abstract void SetPositionInstant(Vector3 pos);

        public abstract void Draw();
    }
}