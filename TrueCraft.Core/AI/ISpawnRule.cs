﻿using TrueCraft.Core.World;
using TrueCraft.Core.Server;

namespace TrueCraft.Core.AI;

public interface ISpawnRule
{
    /// <summary>
    /// One in every n chunks will spawn mobs (with randomness mixed in).
    /// </summary>
    int ChunkSpawnChance { get; }

    /// <summary>
    /// Spawns mobs on a given chunk immediately after the chunk has been generated.
    /// </summary>
    void GenerateMobs(IChunk chunk, IEntityManager entityManager);

    /// <summary>
    /// Spawns mobs as part of the ongoing spawn cycle.
    /// </summary>
    void SpawnMobs(IChunk chunk, IEntityManager entityManager);
}