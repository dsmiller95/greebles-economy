using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Simulation.Tiling
{
    [Serializable]
    public struct OffsetCoordinate
    {
        public int row;
        public int column;

        public OffsetCoordinate(int column, int row)
        {
            this.column = column;
            this.row = row;
        }

        public bool IsZero()
        {
            return row == 0 && column == 0;
        }

        public static explicit operator OffsetCoordinate(Vector2Int d) => new OffsetCoordinate(d.x, d.y);

        public static OffsetCoordinate operator +(OffsetCoordinate a, OffsetCoordinate b)
        {
            return new OffsetCoordinate(a.column + b.column, a.row + b.row);
        }

        public static OffsetCoordinate operator -(OffsetCoordinate a, OffsetCoordinate b)
        {
            return new OffsetCoordinate(a.column - b.column, a.row - b.row);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is OffsetCoordinate)
            {
                OffsetCoordinate other = (OffsetCoordinate)obj;
                return other.column == column && other.row == row;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return $"{column},{row}".GetHashCode();
        }

        public override string ToString()
        {
            return $"Offset column: {column} row: {row}";
        }
    }
}
