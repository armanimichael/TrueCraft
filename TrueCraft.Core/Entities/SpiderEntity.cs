using TrueCraft.Core.Server;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Entities;

public class SpiderEntity : MobEntity
{
    public SpiderEntity(IDimension dimension, IEntityManager entityManager)
        :
        base(dimension, entityManager, 16, new Size(1.4, 0.9, 1.4)) { }

    public override sbyte MobType => 52;

    public override bool Friendly => false;
}