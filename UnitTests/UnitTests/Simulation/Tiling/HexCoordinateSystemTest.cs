using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulation.Tiling;
using System.Linq;
using UnityEngine;
using Extensions;

namespace UnitTests.Simulation.Tiling
{
    [TestClass]
    public class HexCoordinateSystemTest
    {
        [TestMethod]
        public void ShouldEvaluateOffsetColumns()
        {
            var coords = new[]
            {
                new OffsetCoordinate(0, 0),
                new OffsetCoordinate(1, 0),
                new OffsetCoordinate(0, 1),
                new OffsetCoordinate(4, 1),
                new OffsetCoordinate(-1, 1),
                new OffsetCoordinate(-3, 1),
                new OffsetCoordinate(-10, 1),
            };
            var expectedOffset = new[]
            {
                true,
                false,
                true,
                true,
                false,
                false,
                true
            };

            var coordSystem = new HexCoordinateSystem(1);

#pragma warning disable CS0618 // Type or member is obsolete
            var actualOffset = coords.Select(vector => vector.IsInOffsetColumn());
#pragma warning restore CS0618 // Type or member is obsolete

            foreach (var pair in expectedOffset.Zip(actualOffset, (a, b) => new { expected = a, actual = b }))
            {
                Assert.AreEqual(pair.expected, pair.actual);
            }
        }
        [TestMethod]
        public void ShouldTranslateCoordinatesToRelativePlane()
        {
            var coords = new[]
            {
                new OffsetCoordinate(0, 0),
                new OffsetCoordinate(1, 0),
                new OffsetCoordinate(0, 1),
                new OffsetCoordinate(1, 1),
            };
            var expectedCoords = new[]
            {
                new Vector2(0, 0),
                new Vector2(1.5f, -Mathf.Sqrt(3)/2),
                new Vector2(0, -Mathf.Sqrt(3)),
                new Vector2(1.5f, -Mathf.Sqrt(3) * 1.5f),
            };

            var coordSystem = new HexCoordinateSystem(1);

            var actualCoords = coords.Select(x => coordSystem.TileMapToRelative(x));

            foreach (var pair in expectedCoords.Zip(actualCoords, (a, b) => new { expected = a, actual = b }))
            {
                Assert.AreEqual(pair.expected, pair.actual);
            }
        }
        [TestMethod]
        public void ShouldTranslateCoordinatesToRelativeRealPlane()
        {
            var coords = new[]
            {
                new OffsetCoordinate(0, 0),
                new OffsetCoordinate(1, 0),
                new OffsetCoordinate(0, 1),
                new OffsetCoordinate(1, 1),
            };
            var expectedCoords = new[]
            {
                new Vector2(0, 0),
                new Vector2(1.5f, -Mathf.Sqrt(3)/2),
                new Vector2(0, -Mathf.Sqrt(3)),
                new Vector2(1.5f, -Mathf.Sqrt(3) * 1.5f),
            }.Select(x => x * 2f);

            var coordSystem = new HexCoordinateSystem(2);

            var actualCoords = coords.Select(x => coordSystem.TileMapToReal(x));

            foreach (var pair in expectedCoords.Zip(actualCoords, (a, b) => new { expected = a, actual = b }))
            {
                Assert.AreEqual(pair.expected, pair.actual);
            }
        }
        [TestMethod]
        public void ShouldTranslateCoordinatesFromRelativeToTiling()
        {
            var rt3 = Mathf.Sqrt(3);
            var realCoords = new[]
            {
                new Vector2(0, rt3/2),
                new Vector2(1.5f, 0),
                new Vector2(0, rt3 * 1.5f),
                new Vector2(1.5f, rt3),

                new Vector2(0f, rt3/2 + 0.7f),
                new Vector2(0f, rt3/2 + 1.1f),
                new Vector2(0.1f, rt3/2 + 0.1f),

                new Vector2(0.7f, rt3/2),
                new Vector2(0.6f, rt3/2 + 0.6f),
                new Vector2(1f, rt3/2 + 0.1f),

                new Vector2(0.8f, rt3/2 + 0.1f),
                new Vector2(0.6f, rt3/2 + 0.8f),

                new Vector2(1.1f, rt3/2 + 1.8f),
                new Vector2(2.1f, rt3/2 - 0.1f),
                new Vector2(0.3f, rt3/2 - 0.9f),
            }
                .Select(x => (x - new Vector2(0, rt3/2)))
                .ToArray();
            var expectedCoords = new[]
            {
                new OffsetCoordinate(0, 0),
                new OffsetCoordinate(1, 0),
                new OffsetCoordinate(0, 1),
                new OffsetCoordinate(1, 1),

                new OffsetCoordinate(0, 0),
                new OffsetCoordinate(0, 1),
                new OffsetCoordinate(0, 0),

                new OffsetCoordinate(0, 0),
                new OffsetCoordinate(0, 0),
                new OffsetCoordinate(1, 1),

                new OffsetCoordinate(0, 0),
                new OffsetCoordinate(1, 1),

                new OffsetCoordinate(1, 2),
                new OffsetCoordinate(2, 0),
                new OffsetCoordinate(0, -1),
            }
            .Select(x => new OffsetCoordinate(x.column, -x.row))
            .ToArray();

            var coordSystem = new HexCoordinateSystem(1);

            var actualCoords = realCoords
                .Select(x => coordSystem.RelativeToTileMap(x))
                .Select(x => x.ToOffset())
                .ToList();

            foreach (var pair in expectedCoords
                .Select((x, index) => new { x, index })
                .Zip(actualCoords, (a, b) => new { expected = a.x, actual = b, index = a.index }))
            {
                Assert.AreEqual(
                    pair.expected,
                    pair.actual, 
                    $"Expected {realCoords[pair.index]} to translate to {pair.expected}, but instead got {pair.actual}");
            }
        }

        [TestMethod]
        public void ShouldEvaluateDistanceBetweenPointsInJumps()
        {
            var coords = new[]
            {
                new OffsetCoordinate(0, 0),
                new OffsetCoordinate(1, 0),
                new OffsetCoordinate(0, 1),
                new OffsetCoordinate(1, 1),
                new OffsetCoordinate(3, 3),
                new OffsetCoordinate(2, 4),
                new OffsetCoordinate(1, 3),
                new OffsetCoordinate(5, 1),
            };
            var expectedJumps = new[]
            {
                1, 1, 1, 3, 1, 1, 4
            };

            var coordSystem = new HexCoordinateSystem(2);

            var actualJumps = coords
                .RollingWindow(2)
                .Select(pair => coordSystem.DistanceBetweenInJumps(pair[0], pair[1]));

            foreach (var pair in expectedJumps
                .Select((x, index) => new { x, index })
                .Zip(actualJumps, (a, b) => new { expected = a.x, actual = b, index = a.index }))
            {
                Assert.AreEqual(
                    pair.expected,
                    pair.actual,
                    $"Expected distance between {coords[pair.index]} and {coords[pair.index + 1]} to be {pair.expected} but was {pair.actual}");
            }
        }

        [TestMethod]
        public void ShouldGetAllPositionsWithinDistance()
        {
            var coord = new OffsetCoordinate(3, 3);
            var expectedCoordsInDistance = new[]
            {
                new OffsetCoordinate(3, 3),

                new OffsetCoordinate(3, 4),
                new OffsetCoordinate(2, 3),
                new OffsetCoordinate(4, 3),
                new OffsetCoordinate(2, 2),
                new OffsetCoordinate(3, 2),
                new OffsetCoordinate(4, 2),

                new OffsetCoordinate(3, 5),
                new OffsetCoordinate(1, 4),
                new OffsetCoordinate(2, 4),
                new OffsetCoordinate(4, 4),
                new OffsetCoordinate(5, 4),
                new OffsetCoordinate(1, 3),
                new OffsetCoordinate(5, 3),
                new OffsetCoordinate(1, 2),
                new OffsetCoordinate(5, 2),
                new OffsetCoordinate(2, 1),
                new OffsetCoordinate(3, 1),
                new OffsetCoordinate(4, 1),
            }
                .OrderBy(x => x.row)
                .ThenBy(x => x.column)
                .ToList();

            var coordSystem = new HexCoordinateSystem(2);

            var actualJumps = coordSystem.GetPositionsWithinJumpDistance(coord, 2)
                .OrderBy(x => x.row)
                .ThenBy(x => x.column)
                .ToList();

            Assert.AreEqual(expectedCoordsInDistance.Count, actualJumps.Count);

            foreach (var pair in expectedCoordsInDistance
                .Zip(actualJumps, (a, b) => new { expected = a, actual = b}))
            {
                Assert.AreEqual(
                    pair.expected,
                    pair.actual);
            }
        }

        [TestMethod]
        public void ShouldGetAllPositionsWithinDistanceWhenCrossDomain()
        {
            var offsetToCrossDomain = new Vector2Int(-4, -2);
            var coord = new Vector2Int(3, 3) + offsetToCrossDomain;
            var realOffsetCoord = new OffsetCoordinate(coord.x, coord.y);
            var expectedCoordsInDistance = new[]
            {
                new Vector2Int(3, 3),

                new Vector2Int(3, 4),
                new Vector2Int(2, 3),
                new Vector2Int(4, 3),
                new Vector2Int(2, 2),
                new Vector2Int(3, 2),
                new Vector2Int(4, 2),
                    
                new Vector2Int(3, 5),
                new Vector2Int(1, 4),
                new Vector2Int(2, 4),
                new Vector2Int(4, 4),
                new Vector2Int(5, 4),
                new Vector2Int(1, 3),
                new Vector2Int(5, 3),
                new Vector2Int(1, 2),
                new Vector2Int(5, 2),
                new Vector2Int(2, 1),
                new Vector2Int(3, 1),
                new Vector2Int(4, 1),
            }
                .Select(x => x + offsetToCrossDomain)
                .Select(x => new OffsetCoordinate(x.x, x.y))
                .OrderBy(x => x.row)
                .ThenBy(x => x.column)
                .ToList();

            var coordSystem = new HexCoordinateSystem(2);

            var actualJumps = coordSystem.GetPositionsWithinJumpDistance(realOffsetCoord, 2)
                .OrderBy(x => x.row)
                .ThenBy(x => x.column)
                .ToList();

            Assert.AreEqual(expectedCoordsInDistance.Count, actualJumps.Count);

            foreach (var pair in expectedCoordsInDistance
                .Zip(actualJumps, (a, b) => new { expected = a, actual = b }))
            {
                Assert.AreEqual(
                    pair.expected,
                    pair.actual);
            }
        }


        [TestMethod]
        public void ShouldGeneratePathBetweenPoints()
        {
            var origin = new OffsetCoordinate(-1, 4).ToAxial();
            var destination = new OffsetCoordinate(3, -1).ToAxial();
            var expectedPathLength = 7;

            var coordSystem = new HexCoordinateSystem(2);

            var actualPath = coordSystem.GetRouteGenerator(origin, destination)
                .ToList();

            Assert.AreEqual(expectedPathLength, actualPath.Count);

            foreach (var pair in actualPath.RollingWindow(2))
            {
                var distanceBetween = coordSystem.DistanceBetweenInJumps(pair[0], pair[1]);
                Assert.AreEqual(
                    1,
                    distanceBetween,
                    $"Jump between {pair[0]} and {pair[1]} should have been distance of 1 but was {distanceBetween}");
            }
        }
        [TestMethod]
        public void ShouldGeneratePathBetweenPoints2()
        {
            var origin = new AxialCoordinate(-2, 2);
            var destination = new AxialCoordinate(0, 3);
            var expectedPathLength = 3;

            var coordSystem = new HexCoordinateSystem(2);

            var actualPath = coordSystem.GetRouteGenerator(origin, destination)
                .ToList();

            Assert.AreEqual(expectedPathLength, actualPath.Count);

            foreach (var pair in actualPath.RollingWindow(2))
            {
                var distanceBetween = coordSystem.DistanceBetweenInJumps(pair[0], pair[1]);
                Assert.AreEqual(
                    1,
                    distanceBetween,
                    $"Jump between {pair[0]} and {pair[1]} should have been distance of 1 but was {distanceBetween}");
            }
        }
    }
}
