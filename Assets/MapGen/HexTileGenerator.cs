using Assets.MapGen.TileManagement;
using Simulation.Tiling.HexCoords;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.MapGen
{
    [Serializable]
    public struct MaterialRegion
    {
        public float regionPoint;
        public int sourceSubmesh;
        public int destinationSubmesh;
    }

    [Serializable]
    public struct Octave
    {
        public Vector2 frequency;
        public float relativeWeight;

        public float SamplePerlin(Vector2 point)
        {
            var sampleVector = Vector2.Scale(point, frequency);
            return Mathf.PerlinNoise(sampleVector.x, sampleVector.y) * relativeWeight;
        }

        public Octave ScaleFrequencies(Vector2 scale)
        {
            return new Octave { frequency = Vector2.Scale(frequency, scale), relativeWeight = relativeWeight };
        }
    }


    [RequireComponent(typeof(HexTileMapManager))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class HexTileGenerator : MonoBehaviour
    {
        public Mesh hexTile;

        /// <summary>
        /// list of floats from 0 to 1 in ascending order. must terminate with a 2, but all other values should be between 0 and 1
        /// </summary>
        public MaterialRegion[] extraTerrainRegions;

        public Gradient colorRamp;

        public int[] defaultSubmeshCopies;

        public Vector2 perlinScale = new Vector2(1, 1);

        public Octave[] octaves = new[] { new Octave() { frequency = new Vector2(1, 1), relativeWeight = 1 } };

        private Octave[] scaledOctaves;
        private Vector2 perlinOffset;


        private HexTileMapManager mapManager;
        void Awake()
        {
            perlinOffset = new Vector2(
                UnityEngine.Random.Range(0, 5000),
                UnityEngine.Random.Range(0, 5000)
                );

            scaledOctaves = octaves
                .Select(x => x.ScaleFrequencies(perlinScale))
                .ToArray();

        }

        // Start is called before the first frame update
        void Start()
        {
            Mesh mesh = new Mesh();

            mapManager = GetComponent<HexTileMapManager>();

            // this mesh can get pretty big, so we need the extra size
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            SetupMesh(mesh, hexTile, mapManager);
            GetComponent<MeshFilter>().mesh = mesh;

        }

        // Update is called once per frame
        void Update()
        {
        }

        private float SampleNoise(Vector2 point)
        {
            return SampleNoiseAtOctaves(scaledOctaves, point);
        }

        private float SampleNoiseAtOctaves(Octave[] octaves, Vector2 point)
        {
            var perlinVector = point + perlinOffset;

            var sample = 0f;
            foreach (var octave in octaves)
            {
                sample += octave.SamplePerlin(perlinVector);
            }

            return sample / octaves.Sum(octave => octave.relativeWeight);
        }

        private Color32 GetColorForTerrainAtPoint(OffsetCoordinate point)
        {
            // map noise over a larger hex grid size to get larger-scale
            //  hexagons of terrain. looks kinda neato
            var largeHexGridRadius = 1;
            var largeHexGridRelativeScale = largeHexGridRadius * 2 + 1;
            var scaled = point.ToCube().GetCoordInLargerHexGrid(largeHexGridRadius);
            var sample = SampleNoise(new Vector2(scaled.x, scaled.y) * largeHexGridRelativeScale);
            return colorRamp.Evaluate(sample);
        }

        private MaterialRegion GetSubmeshForTerrainAtPoint(Vector3 point)
        {
            var sample = SampleNoise(new Vector2(point.x, point.z));
            for (var i = 0; i < extraTerrainRegions.Length; i++)
            {
                if (extraTerrainRegions[i].regionPoint > sample)
                {
                    return extraTerrainRegions[i];
                }
            }
            throw new Exception("full range of extra terrains not properly defined");
        }

        private CopiedMeshEditor meshEditor;

        private void SetupMesh(Mesh target, Mesh source, HexTileMapManager mapManager)
        {
            var offsets = GetTilesInRectangle(mapManager);
            var targetSubmeshes = defaultSubmeshCopies.Length + extraTerrainRegions.Length;

            //offsets = new[]
            //{
            //    new Vector3(2, 0, 2),
            //    new Vector3(0, 0, 2),
            //    new Vector3(2, 0, 0),
            //    new Vector3(0, 0, 0),
            //};

            var copier = new MeshCopier(
                source, 2,
                target, targetSubmeshes);

            foreach (var offset in offsets)
            {
                var planeLocation = mapManager.TileMapPositionToPositionInPlane(offset.ToAxial());
                var realOffset = new Vector3(planeLocation.x, 0, planeLocation.y);
                var vertexColor = GetColorForTerrainAtPoint(offset);
                copier.NextCopy(realOffset, vertexColor);

                foreach (var submesh in defaultSubmeshCopies)
                {
                    copier.CopySubmeshTrianglesToOffsetIndex(submesh, submesh);
                }

                if (extraTerrainRegions.Length > 0)
                {
                    var extraSubmeshMapping = GetSubmeshForTerrainAtPoint(realOffset);
                    copier.CopySubmeshTrianglesToOffsetIndex(extraSubmeshMapping.sourceSubmesh, extraSubmeshMapping.destinationSubmesh);
                }
            }

            meshEditor = copier.FinalizeCopy();
        }

        private IEnumerable<OffsetCoordinate> GetTilesInRectangle(HexTileMapManager tilemapManager)
        {
            var totalCells = new Vector2Int(tilemapManager.hexWidth, tilemapManager.hexHeight);//new Vector2Int(10, 10);
            var min = tilemapManager.tileMapMin.ToAxial();

            for (var verticalIndex = 0; verticalIndex < totalCells.y; verticalIndex++)
            {
                for (var horizontalIndex = 0; horizontalIndex < totalCells.x; horizontalIndex++)
                {
                    var newPos = new OffsetCoordinate(horizontalIndex, verticalIndex);
                    yield return (newPos.ToAxial() + min).ToOffset();
                }
            }
        }

        public struct HexTileColorChangeRecord
        {
            public IEnumerable<OffsetCoordinate> changedIndexes;
        }

        public HexTileColorChangeRecord SetHexTileColors(IEnumerable<(OffsetCoordinate, Color32)> colorsAtPositions)
        {
            var mapManager = GetComponent<HexTileMapManager>();
            var colors = colorsAtPositions.ToList();
            var colorChange = colors
                .Select(x => (LocationToIndex(x.Item1), x.Item2));
            meshEditor.SetColorsOnVertexesAtDuplicates(colorChange);
            return new HexTileColorChangeRecord
            {
                changedIndexes = colors.Select(x => x.Item1)
            };
        }

        public void ResetHexTileColors(HexTileColorChangeRecord changeRecord)
        {
            if (changeRecord.changedIndexes == default)
            {
                return;
            }
            var colorChange = changeRecord.changedIndexes.Select(location =>
            {
                var color = GetColorForTerrainAtPoint(location);
                return (LocationToIndex(location), color);
            });
            meshEditor.SetColorsOnVertexesAtDuplicates(colorChange);
        }

        public void SetHexTileColor(OffsetCoordinate locationInTileMap, Color vertexColor)
        {
            var copyIndex = LocationToIndex(locationInTileMap);
            meshEditor.SetColorOnVertexesAtDuplicate(copyIndex, vertexColor);
        }

        public void ResetHexTilecolor(OffsetCoordinate locationInTileMap)
        {
            SetHexTileColor(locationInTileMap, GetColorForTerrainAtPoint(locationInTileMap));
        }

        private int LocationToIndex(OffsetCoordinate locationInTileMap)
        {
            var offsetInArray = (locationInTileMap.ToAxial() - mapManager.tileMapMin.ToAxial()).ToOffset();
            var copyIndex = offsetInArray.row * mapManager.hexWidth + offsetInArray.column;
            return copyIndex;
        }
    }

}