﻿using TrueCraft.Core.Server;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Entities;

public class SkeletonEntity : MobEntity
{
    public SkeletonEntity(IDimension dimension, IEntityManager entityManager)
        :
        base(dimension, entityManager, 20, new Size(0.6, 1.8, 0.6)) { }

    public override sbyte MobType => 51;

    public override bool Friendly => false;
}