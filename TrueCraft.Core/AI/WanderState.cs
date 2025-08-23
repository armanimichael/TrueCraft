﻿using TrueCraft.Core.Entities;
using TrueCraft.Core.Server;
using TrueCraft.Core.World;
using System.Threading.Tasks;

namespace TrueCraft.Core.AI;

public class WanderState : IMobState
{
    /// <summary>
    /// The maximum distance the mob will move in an iteration.
    /// </summary>
    /// <value>The distance.</value>
    public int Distance { get; set; }

    public AStarPathFinder PathFinder { get; set; }

    public WanderState()
    {
        Distance = 25;
        PathFinder = new AStarPathFinder();
    }

    public void Update(IMobEntity entity, IEntityManager manager)
    {
        var cast = entity as IEntity;

        if (entity.CurrentPath != null)
        {
            if (entity.AdvancePath(manager.TimeSinceLastUpdate))
            {
                entity.CurrentState = new IdleState(new WanderState());
            }
        }
        else
        {
            var target = new GlobalVoxelCoordinates(
                (int) (cast.Position.X + (MathHelper.Random.Next(Distance) - (Distance / 2))),
                0,
                (int) (cast.Position.Z + (MathHelper.Random.Next(Distance) - (Distance / 2)))
            );

            IChunk? chunk;
            var adjusted = entity.Dimension.FindBlockPosition(target, out chunk);

            if (chunk is null)
            {
                return;
            }

            target = new GlobalVoxelCoordinates(
                target.X,
                chunk.GetHeight((byte) adjusted.X, (byte) adjusted.Z),
                target.Z
            );

            Task.Factory.StartNew(
                () =>
                {
                    entity.CurrentPath = PathFinder.FindPath(
                        entity.Dimension,
                        entity.BoundingBox,
                        (GlobalVoxelCoordinates) cast.Position,
                        target
                    );
                }
            );
        }
    }
}