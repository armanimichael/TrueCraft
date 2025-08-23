using System;

namespace TrueCraft.Core.World;

public class Vector3i : IEquatable<Vector3i>
{
    public Vector3i(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public int X { get; }
    public int Y { get; }
    public int Z { get; }

    public static Vector3i operator +(Vector3i l, Vector3i r) => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z);

    /// <summary>
    /// Clamps the coordinates to within the specified value.
    /// </summary>
    /// <params>
    /// <param name="value">Value.</param>
    /// </params>
    public Vector3i Clamp(int value)
    {
        int x, y, z;

#if DEBUG
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException($"{nameof(value)} must be non - negative.");
        }
#endif

        if (Math.Abs(X) > value)
        {
            x = value * (X < 0
                ? -1
                : 1);
        }
        else
        {
            x = X;
        }

        if (Math.Abs(Y) > value)
        {
            y = value * (Y < 0
                ? -1
                : 1);
        }
        else
        {
            y = Y;
        }

        if (Math.Abs(Z) > value)
        {
            z = value * (Z < 0
                ? -1
                : 1);
        }
        else
        {
            z = Z;
        }

        return new Vector3i(x, y, z);
    }

    #region Object overrides

    public override bool Equals(object? obj) => Equals(obj as Vector3i);

    public override int GetHashCode()
    {
        unchecked
        {
            var rv = X * 409;
            rv *= 409 * Y;
            rv *= 397 * Z;

            return rv;
        }
    }

    public override string ToString() => $"<{X},{Y},{Z}>";

    #endregion

    #region IEquatable<> & related

    public bool Equals(Vector3i? other)
    {
        if (other is null)
        {
            return false;
        }

        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public static bool operator ==(Vector3i? l, Vector3i? r)
    {
        if (l is null)
        {
            if (r is null)
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
            if (r is null)
            {
                return false;
            }
            else
            {
                return l.Equals(r);
            }
        }
    }

    public static bool operator !=(Vector3i? l, Vector3i? r) => !(l == r);

    #endregion

    public static Vector3i operator -(Vector3i arg) => new(-arg.X, -arg.Y, -arg.Z);

    public static Vector3i operator *(Vector3i v, int a) => new(a * v.X, a * v.Y, a * v.Z);

    public static Vector3i operator *(int a, Vector3i v) => new(a * v.X, a * v.Y, a * v.Z);

    #region constant vectors

    public static readonly Vector3i Zero = new(0, 0, 0);
    public static readonly Vector3i One = new(1, 1, 1);
    public static readonly Vector3i Up = new(0, 1, 0);
    public static readonly Vector3i Down = new(0, -1, 0);
    public static readonly Vector3i North = new(0, 0, -1);
    public static readonly Vector3i East = new(1, 0, 0);
    public static readonly Vector3i South = new(0, 0, 1);
    public static readonly Vector3i West = new(-1, 0, 0);

    /// <summary>
    /// A set of Vector3i objects pointing to the 4-connected neighbors at the
    /// same Y-level.
    /// </summary>
    public static readonly Vector3i[] Neighbors4 = { North, East, South, West };

    /// <summary>
    /// A set of Vector3i objects pointing to the 6-connected neighbors.
    /// These are connected to each face of a Block.
    /// </summary>
    public static readonly Vector3i[] Neighbors6 = { North, East, South, West, Up, Down };

    #endregion
}