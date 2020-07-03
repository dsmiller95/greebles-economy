using Assets.MapGen.TileManagement;
using Assets.Scripts.MovementExtensions;
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

        void Awake()
        {
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

        private void SetupMesh(Mesh target, Mesh source, HexTileMapManager mapManager)
        {
            IEnumerable<Vector3> offsets = new[] {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(2, 0, 0),
                new Vector3(0, 0, 2),
                new Vector3(2, 0, 2),
            };// GetTileOffsets(mapManager);
            offsets = GetTileOffsets(mapManager);

            CopyMeshIntoOtherMeshAtOffsets(target, source, offsets.ToArray());
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


        static void CopyMeshIntoOtherMeshAtOffsets(Mesh targetMesh, Mesh sourceMesh, Vector3[] offsets)
        {
            targetMesh.Clear();

            var duplicates = offsets.Length;
            var sourceVertexes = sourceMesh.vertices;
            var newVertexLength = sourceVertexes.Length * duplicates;
            var newVertexes = new Vector3[newVertexLength];

            List<Vector2> sourceUVs = new List<Vector2>();
            sourceMesh.GetUVs(0, sourceUVs);
            var newUVs = new Vector2[sourceUVs.Count * duplicates];
            for (var offsetIndex = 0; offsetIndex < offsets.Length; offsetIndex++)
            {
                var displacement = offsets[offsetIndex];

                var vertexIndexOffset = offsetIndex * sourceVertexes.Length;
                for (var vert = 0; vert < sourceVertexes.Length; vert++)
                {
                    newVertexes[vert + vertexIndexOffset] = sourceVertexes[vert] + displacement;
                }
                var uvOffset = offsetIndex * sourceUVs.Count;
                for (var uv = 0; uv < sourceUVs.Count; uv++)
                {
                    newUVs[uv + uvOffset] = sourceUVs[uv];
                }
            }

            targetMesh.SetVertices(newVertexes);
            targetMesh.SetUVs(0, newUVs);

            var targetSubMeshCount = sourceMesh.subMeshCount;
            targetMesh.subMeshCount = targetSubMeshCount;

            for (int submesh = 0; submesh < targetMesh.subMeshCount; submesh++)
            {
                var sourceTriangles = sourceMesh.GetTriangles(submesh);
                var newTriangles = new int[sourceTriangles.Length * duplicates];

                for (var offsetIndex = 0; offsetIndex < duplicates; offsetIndex++)
                {
                    var vertexIndexOffset = offsetIndex * sourceVertexes.Length;

                    var triangleOffset = offsetIndex * sourceTriangles.Length;
                    for (var tri = 0; tri < sourceTriangles.Length; tri++)
                    {
                        newTriangles[tri + triangleOffset] = sourceTriangles[tri] + vertexIndexOffset;
                    }
                }
                targetMesh.SetTriangles(newTriangles, submesh);
            }


            //targetMesh.vertices = newVertexes;
            //targetMesh.uv = newUVs;
            //targetMesh.triangles = newTriangles;

            targetMesh.RecalculateNormals();
        }
    }

}