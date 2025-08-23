using System;

namespace TrueCraft.Core.World;

/// <summary>
/// Local Column Coordinates specify the location of a Column of Blocks
/// within a Chunk relative to the North-West Corner.
/// </summary>
public class LocalColumnCoordinates : IEquatable<LocalColumnCoordinates>
{
    public LocalColumnCoordinates(int x, int z)
    {
        X = x;
        Z = z;
    }

    public int X { get; }
    public int Z { get; }

    /// <summary>
    /// Calculates the Euclidean distance between two Coordinates.
    /// </summary>
    /// <param name="other"></param>
    /// <returns>The Euclidean distance to the other instance.</returns>
    public double DistanceTo(LocalColumnCoordinates other)
    {
        var dx = other.X - X;
        var dz = other.Z - Z;

        return Math.Sqrt((dx * dx) + (dz * dz));
    }

    #region IEquatable<> & related

    public bool Equals(LocalColumnCoordinates? other)
    {
        if (other is null)
        {
            return false;
        }

        return X == other.X && Z == other.Z;
    }

    public static bool operator ==(LocalColumnCoordinates? l, LocalColumnCoordinates? r)
    {
        if (l is not null)
        {
            if (r is not null)
            {
                return l.Equals(r);
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (r is not null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public static bool operator !=(LocalColumnCoordinates l, LocalColumnCoordinates r) => !(l == r);

    #endregion

    #region object overrides

    public override bool Equals(object? obj) => Equals(obj as LocalColumnCoordinates);

    public override int GetHashCode()
    {
        unchecked
        {
            var rv = X * 17;
            rv += Z * 409;

            return rv;
        }
    }

    public override string ToString() => $"<{X},{Z}>";

    #endregion
}