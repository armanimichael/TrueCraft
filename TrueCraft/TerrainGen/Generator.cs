using System.Collections.Generic;
using TrueCraft.Core.World;
using TrueCraft.World;

namespace TrueCraft.TerrainGen;

public abstract class Generator : IChunkProvider
{
    protected readonly int _seed;

    private readonly List<IChunkDecorator> _decorators;

    protected Generator(int seed)
    {
        _seed = seed;
        _decorators = new List<IChunkDecorator>();
    }

    protected IList<IChunkDecorator> ChunkDecorators => _decorators;

    public abstract IChunk GenerateChunk(GlobalChunkCoordinates coordinates);

    public abstract GlobalVoxelCoordinates GetSpawn(IDimension dimension);
}