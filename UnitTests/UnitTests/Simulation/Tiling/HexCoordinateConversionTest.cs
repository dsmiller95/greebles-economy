using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simulation.Tiling;

namespace UnitTests.Simulation.Tiling
{
    [TestClass]
    public class HexCoordinateConversionTest
    {
        [TestMethod]
        public void ShouldTranslateFromOffsetToCubeAndBack()
        {
            var assertions = new[]
            {
                (new OffsetCoordinate( 0, 0), new CubeCoordinate( 0, 0, 0)),
                (new OffsetCoordinate( 1, 0), new CubeCoordinate( 1,-1, 0)),
                (new OffsetCoordinate( 2, 0), new CubeCoordinate( 2,-1,-1)),
                (new OffsetCoordinate( 0, 1), new CubeCoordinate( 0,-1, 1)),
                (new OffsetCoordinate( 0, 2), new CubeCoordinate( 0,-2, 2)),
                (new OffsetCoordinate( 1, 1), new CubeCoordinate( 1,-2, 1)),

                (new OffsetCoordinate(-1,-1), new CubeCoordinate(-1, 1, 0)),
                (new OffsetCoordinate(-2, 0), new CubeCoordinate(-2, 1, 1)),
                (new OffsetCoordinate(-2,-2), new CubeCoordinate(-2, 3,-1)),
            };

            foreach(var assert in assertions)
            {
                var actualCube = assert.Item1.ToCube();
                Assert.AreEqual(assert.Item2, actualCube);

                var actualOffset = assert.Item2.ToOffset();
                Assert.AreEqual(assert.Item1, actualOffset);
            }
        }


        [TestMethod]
        public void ShouldTranslateFromAxialToCubeAndBack()
        {
            var assertions = new[]
            {
                (new AxialCoordinate( 0, 0), new CubeCoordinate( 0, 0, 0)),
                (new AxialCoordinate( 1, 0), new CubeCoordinate( 1,-1, 0)),
                (new AxialCoordinate( 2, 0), new CubeCoordinate( 2,-2, 0)),
                (new AxialCoordinate( 0, 1), new CubeCoordinate( 0,-1, 1)),
                (new AxialCoordinate( 0, 2), new CubeCoordinate( 0,-2, 2)),
                (new AxialCoordinate( 1, 1), new CubeCoordinate( 1,-2, 1)),

                (new AxialCoordinate(-1,-1), new CubeCoordinate(-1, 2,-1)),
                (new AxialCoordinate(-2, 0), new CubeCoordinate(-2, 2, 0)),
                (new AxialCoordinate(-2,-2), new CubeCoordinate(-2, 4,-2)),
            };

            foreach (var assert in assertions)
            {
                var actualCube = assert.Item1.ToCube();
                Assert.AreEqual(assert.Item2, actualCube);

                var actualAxial = assert.Item2.ToAxial();
                Assert.AreEqual(assert.Item1, actualAxial);
            }
        }

        
        [TestMethod]
        public void ShouldScaleCubeCoordinateOntoLargerHexCellTiles()
        {
            var assertions = new[]
            {
                (new AxialCoordinate( 0, 0), new AxialCoordinate(0, 0)),
                (new AxialCoordinate( 1, 0), new AxialCoordinate(0, 0)),
                (new AxialCoordinate(-1, 0), new AxialCoordinate(0, 0)),
                (new AxialCoordinate( 0,-1), new AxialCoordinate(0, 0)),
                (new AxialCoordinate( 0, 1), new AxialCoordinate(0, 0)),
                (new AxialCoordinate( 1,-1), new AxialCoordinate(0, 0)),
                (new AxialCoordinate(-1, 1), new AxialCoordinate(0, 0)),

                (new AxialCoordinate( 2,-2), new AxialCoordinate(1,-1)),
                (new AxialCoordinate( 2,-1), new AxialCoordinate(1,-1)),
                (new AxialCoordinate( 3,-3), new AxialCoordinate(1,-1)),
                (new AxialCoordinate( 3,-2), new AxialCoordinate(1,-1)),
                (new AxialCoordinate( 3,-1), new AxialCoordinate(1,-1)),
                (new AxialCoordinate( 4,-3), new AxialCoordinate(1,-1)),
                (new AxialCoordinate( 4,-2), new AxialCoordinate(1,-1)),


                (new AxialCoordinate( 1, 1), new AxialCoordinate(1, 0)),
                (new AxialCoordinate( 1, 2), new AxialCoordinate(1, 0)),
                (new AxialCoordinate( 2, 0), new AxialCoordinate(1, 0)),
                (new AxialCoordinate( 2, 1), new AxialCoordinate(1, 0)),
                (new AxialCoordinate( 2, 2), new AxialCoordinate(1, 0)),
                (new AxialCoordinate( 3, 0), new AxialCoordinate(1, 0)),
                (new AxialCoordinate( 3, 1), new AxialCoordinate(1, 0)),

                (new AxialCoordinate( 0,-2), new AxialCoordinate(0,-1)),
                (new AxialCoordinate(-1,-1), new AxialCoordinate(-1,0)),
                (new AxialCoordinate(-2, 1), new AxialCoordinate(-1,1)),
                (new AxialCoordinate(-1, 2), new AxialCoordinate(0, 1)),

            };

            foreach (var assert in assertions)
            {
                var inputCube = assert.Item1.ToCube();
                var scaledCube = inputCube.GetCoordInLargerHexGrid(1);
                var actualAxial = scaledCube.ToAxial();

                Assert.AreEqual(assert.Item2, actualAxial, $"Expected {assert.Item1} to scale to {assert.Item2} but instead got {actualAxial}");
            }
        }

    }
}
