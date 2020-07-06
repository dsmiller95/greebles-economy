using Assets.MapGen.TileManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.MapGen
{

    [RequireComponent(typeof(HexTileMapManager))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class HexTileGenerator : MonoBehaviour
    {
        public Mesh hexTile;

        /// <summary>
        /// list of floats from 0 to 1 in ascending order. must terminate with a 1
        /// </summary>
        public float[] extraTerrainRegions;

        private Vector2 perlinOffset;

        void Awake()
        {
            this.perlinOffset = new Vector2(
                UnityEngine.Random.Range(-1e5f, 1e5f),
                UnityEngine.Random.Range(-1e5f, 1e5f)
                );
            // Get instantiated mesh
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

        private int GetSubmeshForTerrainAtPoint(Vector3 point)
        {
            var sample = Mathf.PerlinNoise(point.x, point.z);
            for(var i = 0; i < extraTerrainRegions.Length; i++)
            {
                if(extraTerrainRegions[i] < sample)
                {
                    return i;
                }
            }
            throw new System.Exception("full range of extra terrains not properly defined");
        }


        private void SetupMesh(Mesh target, Mesh source, HexTileMapManager mapManager)
        {
            var offsets = GetTileOffsets(mapManager);

            var totalNumberOfCells = mapManager.hexWidth * mapManager.hexHeight;
            var copier = new MeshCopier(
                source, 2,
                target, 2,
                offsets, totalNumberOfCells);
            copier.CopyIntoTarget();

            //CopyMeshIntoOtherMeshAtOffsets(target, source, offsets, totalNumberOfCells);
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