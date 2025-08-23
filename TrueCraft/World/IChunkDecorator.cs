﻿using TrueCraft.Core.Logic;
using TrueCraft.Core.World;

namespace TrueCraft.World
{
    // TODO: this interface should be server-side only.
    /// <summary>
    /// Used to decorate chunks with "decorations" such as trees, flowers, ores, etc.
    /// </summary>
    public interface IChunkDecorator
    {
        void Decorate(int seed, IChunk chunk, IBlockRepository blockRepository, IBiomeRepository biomes);
    }
}
