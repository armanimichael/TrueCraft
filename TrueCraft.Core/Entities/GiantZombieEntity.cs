using TrueCraft.Core.Server;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Entities;

public class GiantZombieEntity : ZombieEntity
{
    public GiantZombieEntity(IDimension dimension, IEntityManager entityManager)
        :
        base(dimension, entityManager) { }

    public override Size Size => new Size(0.6, 1.8, 0.6) * 6;

    public override short MaxHealth => 100;

    public override sbyte MobType => 53;

    public override bool Friendly => false;
}