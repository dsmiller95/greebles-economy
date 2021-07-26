using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulation.Tiling.HexCoords;
using System.Linq;

namespace UnitTests.Simulation.Tiling
{
    [TestClass]
    public class VoroniTest
    {
        [TestMethod]
        public void ShouldGenerateFullMapWithSinglePoint()
        {
            var points = new[] { new AxialCoordinate(0, 0) };

            var voroni = VoroniTilingMapper.SetupVoroniMap(points, new OffsetCoordinate(-1, -1), new OffsetCoordinate(1, 1));

            Assert.AreEqual(2, voroni.Length);
            Assert.AreEqual(2, voroni[0].Length);

            foreach (var row in voroni)
            {
                foreach (var cell in row)
                {
                    Assert.AreEqual(0, cell);
                }
            }
        }

        [TestMethod]
        public void ShouldGenerate1DVoroniwithTwoPoints()
        {
            var points = new[] {
                new OffsetCoordinate(-2, 0),
                new OffsetCoordinate(3, 0)
            }
            .Select(x => x.ToAxial()).ToList();

            var voroni = VoroniTilingMapper.SetupVoroniMap(
                points,
                new OffsetCoordinate(-2, 0),
                new OffsetCoordinate(4, 1));

            var expectedResult = new int[][]
            {
                new int[] {0, 0, 0, 1, 1, 1 }
            };

            Assert.AreEqual(expectedResult.Length, voroni.Length);
            Assert.AreEqual(expectedResult[0].Length, voroni[0].Length);


            for (var row = 0; row < voroni.Length; row++)
            {
                for (var col = 0; col < voroni[row].Length; col++)
                {
                    Assert.AreEqual(
                        expectedResult[row][col],
                        voroni[row][col],
                        $"Expected result mismatch at row: {row} col: {col}");
                }
            }
        }

        [TestMethod]
        public void ShouldGenerate2DVoroniWithTwoPoints()
        {
            var points = new[] {
                new OffsetCoordinate(0, 0),
                new OffsetCoordinate(6, 6)
            }
            .Select(x => x.ToAxial()).ToList();

            var voroni = VoroniTilingMapper.SetupVoroniMap(
                points,
                new OffsetCoordinate(0, 0),
                new OffsetCoordinate(7, 7));

            var expectedResult = new int[][]
            {
                new int[] {0, 0, 0, 0, 0, 0, 0 },
                new int[] {0, 0, 0, 0, 0, 0, 1 },
                new int[] {0, 0, 0, 0, 0, 1, 1 },
                new int[] {0, 0, 0, 1, 1, 1, 1 },
                new int[] {0, 0, 1, 1, 1, 1, 1 },
                new int[] {0, 1, 1, 1, 1, 1, 1 },
                new int[] {0, 1, 1, 1, 1, 1, 1 }
            };

            Assert.AreEqual(expectedResult.Length, voroni.Length);
            Assert.AreEqual(expectedResult[0].Length, voroni[0].Length);


            for (var row = 0; row < voroni.Length; row++)
            {
                for (var col = 0; col < voroni[row].Length; col++)
                {
                    Assert.AreEqual(
                        expectedResult[row][col],
                        voroni[row][col],
                        $"Expected result mismatch at row: {row} col: {col}");
                }
            }
        }

        [TestMethod]
        public void ShouldGenerate2DVoroniWithThreePoints()
        {
            var points = new[] {
                new OffsetCoordinate(1, 1),
                new OffsetCoordinate(6, 4),
                new OffsetCoordinate(2, 8)
            }
            .Select(x => x.ToAxial()).ToList();

            var voroni = VoroniTilingMapper.SetupVoroniMap(
                points,
                new OffsetCoordinate(0, 0),
                new OffsetCoordinate(10, 10));

            var expectedResult = new int[][]
            {
                new int[] {0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
                new int[] {0, 0, 0, 0, 0, 1, 1, 1, 1, 1 },
                new int[] {0, 0, 0, 0, 0, 1, 1, 1, 1, 1 },
                new int[] {0, 0, 0, 0, 1, 1, 1, 1, 1, 1 },
                new int[] {0, 0, 0, 1, 1, 1, 1, 1, 1, 1 },
                new int[] {0, 2, 2, 1, 1, 1, 1, 1, 1, 1 },
                new int[] {2, 2, 2, 2, 1, 1, 1, 1, 1, 1 },
                new int[] {2, 2, 2, 2, 2, 2, 1, 1, 1, 1 },
                new int[] {2, 2, 2, 2, 2, 2, 1, 1, 1, 1 },
                new int[] {2, 2, 2, 2, 2, 2, 2, 2, 1, 1 },
            };

            Assert.AreEqual(expectedResult.Length, voroni.Length);
            Assert.AreEqual(expectedResult[0].Length, voroni[0].Length);


            for (var row = 0; row < voroni.Length; row++)
            {
                for (var col = 0; col < voroni[row].Length; col++)
                {
                    Assert.AreEqual(
                        expectedResult[row][col],
                        voroni[row][col],
                        $"Expected result mismatch at row: {row} col: {col}");
                }
            }
        }
    }
}
