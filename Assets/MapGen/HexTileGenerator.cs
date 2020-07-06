using Assets.MapGen.TileManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
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

        public int[] defaultSubmeshCopies;

        public Vector2 perlinScale = new Vector2(1, 1);
        private Vector2 perlinOffset;

        void Awake()
        {
            this.perlinOffset = new Vector2(
                UnityEngine.Random.Range(0, 5000),
                UnityEngine.Random.Range(0, 5000)
                );

        }

        // Start is called before the first frame update
        void Start()
        {
            Mesh mesh = new Mesh();
            // this mesh can get pretty big, so we need the extra size
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            SetupMesh(mesh, hexTile, GetComponent<HexTileMapManager>());
            GetComponent<MeshFilter>().mesh = mesh;
        }

        // Update is called once per frame
        void Update()
        {
        }

        private float SampleNoise(Vector2 point)
        {
            var samplePoint = Vector2.Scale(point, perlinScale) + perlinOffset;
            var sample = Mathf.PerlinNoise(samplePoint.x, samplePoint.y);
            //Debug.Log($"sampling {point} as {samplePoint}: {sample}");
            return sample;
        }

        private MaterialRegion GetSubmeshForTerrainAtPoint(Vector3 point)
        {
            var sample = SampleNoise(new Vector2(point.x, point.z));
            for(var i = 0; i < extraTerrainRegions.Length; i++)
            {
                if(extraTerrainRegions[i].regionPoint > sample)
                {
                    return extraTerrainRegions[i];
                }
            }
            throw new System.Exception("full range of extra terrains not properly defined");
        }


        private void SetupMesh(Mesh target, Mesh source, HexTileMapManager mapManager)
        {
            var offsets = GetTileOffsets(mapManager);
            var targetSubmeshes = defaultSubmeshCopies.Length + extraTerrainRegions.Length;

            //offsets = new[]
            //{
            //    new Vector3(2, 0, 2),
            //    new Vector3(0, 0, 2),
            //    new Vector3(2, 0, 0),
            //    new Vector3(0, 0, 0),
            //};

            using (var copier = new MeshCopier(
                source, 2,
                target, targetSubmeshes))
            {
                foreach (var offset in offsets)
                {
                    copier.NextCopy(offset);
                    foreach (var submesh in defaultSubmeshCopies)
                    {
                        copier.CopySubmeshTrianglesToOffsetIndex(submesh, submesh);
                    }
                    var extraSubmeshMapping = GetSubmeshForTerrainAtPoint(offset);
                    copier.CopySubmeshTrianglesToOffsetIndex(extraSubmeshMapping.sourceSubmesh, extraSubmeshMapping.destinationSubmesh);
                }
            }
        }

        private IEnumerable<Vector3> GetTileOffsets(HexTileMapManager tilemapManager)
        {
            var totalCells = new Vector2Int(tilemapManager.hexWidth, tilemapManager.hexHeight);
            for (var verticalIndex = 0; verticalIndex < totalCells.y; verticalIndex++)
            {
                for (var horizontalIndex = 0; horizontalIndex < totalCells.x; horizontalIndex++)
                {
                    var locationInTileMap = new Vector2Int(horizontalIndex, verticalIndex) + tilemapManager.tileMapMin;
                    var planeLocation = tilemapManager.TileMapPositionToPositionInPlane(locationInTileMap);
                    yield return new Vector3(planeLocation.x, 0, planeLocation.y);
                }
            }
        }
    }

}