using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Simulation.Tiling
{
    [Serializable]
    public struct AxialCoordinate
    {
        public int q;
        public int r;

        public AxialCoordinate(int q, int r)
        {
            this.q = q;
            this.r = r;
        }

        public CubeCoordinate ToCube()
        {
            var x = q;
            var z = r;
            var y = -x - z;
            return new CubeCoordinate(x, y, z);
        }

        public OffsetCoordinate ToOffset()
        {
            return this.ToCube().ToOffset();
        }

        public int DistanceTo(AxialCoordinate other)
        {
            return ToCube().DistanceTo(other.ToCube());
        }

        public static AxialCoordinate operator +(AxialCoordinate a, AxialCoordinate b)
        {
            return new AxialCoordinate(a.q + b.q, a.r + b.r);
        }

        public static AxialCoordinate operator -(AxialCoordinate a, AxialCoordinate b)
        {
            return new AxialCoordinate(a.q - b.q, a.r - b.r);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is AxialCoordinate)
            {
                AxialCoordinate other = (AxialCoordinate)obj;
                return other.q == q && other.r == r;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return $"{q},{r}".GetHashCode();
        }

        public override string ToString()
        {
            return $"Axial Coordinate: q: {q} r: {r})";
        }
    }
}
