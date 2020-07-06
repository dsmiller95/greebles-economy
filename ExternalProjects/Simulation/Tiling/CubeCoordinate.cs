using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Simulation.Tiling
{
    [Serializable]
    public struct CubeCoordinate
    {
        public int x;
        public int y;
        public int z;

        public CubeCoordinate(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public OffsetCoordinate ToOffset()
        {
            var col = x;
            var row = z + (x - (x & 1)) / 2;
            return new OffsetCoordinate(col, row);
        }
        public AxialCoordinate ToAxial()
        {
            var q = x;
            var r = z;
            return new AxialCoordinate(q, r);
        }

        public int DistanceTo(CubeCoordinate other)
        {
            return Mathf.Max(Mathf.Abs(x - other.x), Mathf.Abs(y - other.y), Mathf.Abs(z - other.z));
        }

        public static CubeCoordinate operator +(CubeCoordinate a, CubeCoordinate b)
        {
            return new CubeCoordinate(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static CubeCoordinate operator -(CubeCoordinate a, CubeCoordinate b)
        {
            return new CubeCoordinate(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is CubeCoordinate)
            {
                CubeCoordinate other = (CubeCoordinate)obj;
                return other.x == x && other.y == y && other.z == z;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return $"{x},{y},{z}".GetHashCode();
        }

        public override string ToString()
        {
            return $"Cube Coordinate: ({x}, {y}, {z})";
        }
    }
}
