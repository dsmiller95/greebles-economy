using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.MapGen
{
    public class MeshCopier
    {
        private Mesh sourceMesh, targetMesh;
        private int totalDuplicates;

        private int sourceSubmeshes;
        private int targetSubmeshes;

        public MeshCopier(
            Mesh sourceMesh, int sourceSubmeshCount,
            Mesh targetMesh, int targetSubmeshCount,
            IEnumerable<Vector3> duplicateOffsets, int totalDuplicates)
        {
            this.sourceMesh = sourceMesh;
            this.sourceSubmeshes = sourceSubmeshCount;
            this.targetMesh = targetMesh;
            this.targetSubmeshes = targetSubmeshCount;


            this.totalDuplicates = totalDuplicates;
            targetMesh.Clear();
            this.CopyVertexesAndUVs(duplicateOffsets);
        }

        private int sourceVertexCount;

        private void CopyVertexesAndUVs(IEnumerable<Vector3> offsets)
        {
            var sourceVertexes = sourceMesh.vertices;
            this.sourceVertexCount = sourceVertexes.Length;
            var newVertexLength = sourceVertexes.Length * totalDuplicates;
            var newVertexes = new Vector3[newVertexLength];

            List<Vector2> sourceUVs = new List<Vector2>();
            sourceMesh.GetUVs(0, sourceUVs);
            var newUVs = new Vector2[sourceUVs.Count * totalDuplicates];
            var offsetIndex = 0;
            foreach (var displacement in offsets)
            {
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
                offsetIndex++;
            }

            targetMesh.SetVertices(newVertexes);
            targetMesh.SetUVs(0, newUVs);
        }


        public void CopyIntoTarget()
        {
            targetMesh.subMeshCount = targetSubmeshes;

            CopyDuplicatesOfTrianglesFromSubmeshToSubmesh(0, 0);
            CopyDuplicatesOfTrianglesFromSubmeshToSubmesh(1, 1);

            targetMesh.RecalculateNormals();
        }

        private void CopyDuplicatesOfTrianglesFromSubmeshToSubmesh(
            int targetSubmesh,
            int sourceSubmesh)
        {
            var sourceTriangles = sourceMesh.GetTriangles(sourceSubmesh);
            var newTriangles = new int[sourceTriangles.Length * totalDuplicates];

            for (var offsetIndex = 0; offsetIndex < totalDuplicates; offsetIndex++)
            {
                var vertexIndexOffset = offsetIndex * sourceVertexCount;

                var triangleOffset = offsetIndex * sourceTriangles.Length;
                for (var tri = 0; tri < sourceTriangles.Length; tri++)
                {
                    newTriangles[tri + triangleOffset] = sourceTriangles[tri] + vertexIndexOffset;
                }
            }

            var completeNewTriangles = new List<int>(newTriangles);
            completeNewTriangles.AddRange(targetMesh.GetTriangles(targetSubmesh));

            targetMesh.SetTriangles(completeNewTriangles, targetSubmesh);
        }

        private static void CopyTrianglesFromSubmeshToSubmeshAtOffset(
            Mesh targetMesh, int targetSubmesh,
            Mesh sourceMesh, int sourceSubmesh, int sourceVertexCount,
            int duplicates)
        {

        }
    }
}
