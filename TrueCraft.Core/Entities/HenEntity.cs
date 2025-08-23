﻿using TrueCraft.Core.Server;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Entities;

public class HenEntity : MobEntity
{
    public HenEntity(IDimension dimension, IEntityManager entityManager)
        :
        base(dimension, entityManager, 4, new Size(0.4, 0.3, 0.4)) { }

    public override sbyte MobType => 93;
}