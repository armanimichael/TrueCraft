using System;

namespace TrueCraft.Core.World;

/// <summary>
/// Specifies the location of a Chunk within a Region
/// </summary>
/// <remarks>
/// <para>
/// These coordinates are relative to the North-West corner of the Region.
/// The units of these Coordinates are Chunks.
/// </para>
/// </remarks>
public class LocalChunkCoordinates : IEquatable<LocalChunkCoordinates>
{
    /// <summary>
    /// The X component of the coordinates.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// The Z component of the coordinates.
    /// </summary>
    public int Z { get; }

    /// <summary>
    /// Creates a new trio of coordinates from the specified values.
    /// </summary>
    /// <param name="x">The X component of the coordinates.</param>
    /// <param name="z">The Z component of the coordinates.</param>
    public LocalChunkCoordinates(int x, int z)
    {
#if DEBUG
        if (x < 0 || x >= WorldConstants.RegionWidth)
        {
            throw new ArgumentOutOfRangeException(
                $"{nameof(x)} is outside the valid range of [0,{WorldConstants.RegionWidth - 1}]"
            );
        }

        if (z < 0 || z >= WorldConstants.RegionDepth)
        {
            throw new ArgumentOutOfRangeException(
                $"{nameof(z)} is outside the valid range of [0,{WorldConstants.RegionDepth - 1}]"
            );
        }
#endif

        X = x;
        Z = z;
    }

    #region IEquatable<LocalChunkCoordinates> & related

    /// <summary>
    /// Determines whether this 3D coordinates and another are equal.
    /// </summary>
    /// <param name="other">The other coordinates.</param>
    /// <returns></returns>
    public bool Equals(LocalChunkCoordinates? other)
    {
        if (other is null)
        {
            return false;
        }

        return X == other.X && Z == other.Z;
    }

    public static bool operator !=(LocalChunkCoordinates? a, LocalChunkCoordinates? b) => !(a == b);

    public static bool operator ==(LocalChunkCoordinates? a, LocalChunkCoordinates? b)
    {
        if (ReferenceEquals(a, null))
        {
            if (ReferenceEquals(b, null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (ReferenceEquals(b, null))
            {
                return false;
            }
            else
            {
                return a.Equals(b);
            }
        }
    }

    #endregion // IEquatable<LocalChunkCoordinates>

    #region Object overrides

    /// <summary>
    /// Determines whether this and another object are equal.
    /// </summary>
    /// <param name="obj">The other object.</param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as LocalChunkCoordinates);

    /// <summary>
    /// Returns the hash code for this 3D coordinates.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        unchecked
        {
            var result = X.GetHashCode();
            result = (result * 397) ^ Z.GetHashCode();

            return result;
        }
    }

    /// <summary>
    /// Converts this LocalChunkCoordinates to a string in the format &lt;x, y, z&gt;.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"<{X},{Z}>";

    #endregion // object overrides

    #region Conversion operators

    public static explicit operator LocalChunkCoordinates(GlobalChunkCoordinates value)
    {
        int localX;
        int localZ;

        if (value.X >= 0)
        {
            localX = value.X / WorldConstants.RegionWidth;
        }
        else
        {
            localX = ((value.X + 1) / WorldConstants.RegionWidth) - 1;
        }

        if (value.Z >= 0)
        {
            localZ = value.Z / WorldConstants.RegionDepth;
        }
        else
        {
            localZ = ((value.Z + 1) / WorldConstants.RegionDepth) - 1;
        }

        return new LocalChunkCoordinates(
            value.X - (localX * WorldConstants.RegionWidth),
            value.Z - (localZ * WorldConstants.RegionDepth)
        );
    }

    public static explicit operator LocalChunkCoordinates(GlobalVoxelCoordinates value) => GlobalVoxelToLocalChunk(value.X, value.Z);

    private static LocalChunkCoordinates GlobalVoxelToLocalChunk(int x, int z)
    {
        int regionX;
        int regionZ;
        var regionWidth = WorldConstants.RegionWidth * WorldConstants.ChunkWidth;
        var regionDepth = WorldConstants.RegionDepth * WorldConstants.ChunkDepth;

        if (x >= 0)
        {
            regionX = x / regionWidth;
        }
        else
        {
            regionX = ((x + 1) / regionWidth) - 1;
        }

        if (z >= 0)
        {
            regionZ = z / regionDepth;
        }
        else
        {
            regionZ = ((z + 1) / regionDepth) - 1;
        }

        var localX = (x - (regionX * regionWidth)) / WorldConstants.ChunkWidth;
        var localZ = (z - (regionZ * regionDepth)) / WorldConstants.ChunkDepth;

        return new LocalChunkCoordinates(localX, localZ);
    }

    #endregion

    #region Constants

    public static readonly LocalChunkCoordinates Zero = new(0, 0);

    #endregion
}