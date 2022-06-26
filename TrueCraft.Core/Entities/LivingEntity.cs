﻿using System;
using TrueCraft.Core.Server;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Entities
{
    public abstract class LivingEntity : Entity
    {
        protected LivingEntity(IDimension dimension, IEntityManager entityManager) : base(dimension, entityManager)
        {
            Health = MaxHealth;
        }

        protected short _Air;
        public short Air
        {
            get { return _Air; }
            set
            {
                _Air = value;
                OnPropertyChanged("Air");
            }
        }

        protected short _Health;
        public short Health
        {
            get { return _Health; }
            set
            {
                _Health = value;
                OnPropertyChanged("Health");
            }
        }

        protected float _HeadYaw;
        public float HeadYaw
        {
            get { return _HeadYaw; }
            set
            {
                _HeadYaw = value;
                OnPropertyChanged("HeadYaw");
            }
        }

        public override bool SendMetadataToClients
        {
            get
            {
                return true;
            }
        }

        public abstract short MaxHealth { get; }
    }
}
