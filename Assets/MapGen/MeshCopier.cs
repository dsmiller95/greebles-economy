using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.MapGen
{
    /// <summary>
    /// A utility class used to copy duplicates of one mesh into the target mesh
    ///     currently set up to copy all the vertextes to a set of offsets
    ///     and then selectively copy the triangles
    /// </summary>
    public class MeshCopier : IDisposable
    {
        private Mesh sourceMesh, targetMesh;

        private int sourceSubmeshes;
        private int targetSubmeshes;
        //private Vector3[] offsets;

        public MeshCopier(
            Mesh sourceMesh, int sourceSubmeshCount,
            Mesh targetMesh, int targetSubmeshCount)
        {
            this.sourceMesh = sourceMesh;
            this.sourceSubmeshes = sourceSubmeshCount;
            this.targetMesh = targetMesh;
            this.targetSubmeshes = targetSubmeshCount;


            this.targetTrianglesBySubmesh = new List<int>[targetSubmeshCount];
            for (var i = 0; i < targetSubmeshCount; i++)
            {
                targetTrianglesBySubmesh[i] = new List<int>();
            }
            this.targetVertexes = new List<Vector3>();
            this.targetUVs = new List<Vector2>();

            var sourceVertexes = sourceMesh.vertices;
            this.sourceVertexCount = sourceVertexes.Length;


            targetMesh.Clear();
            targetMesh.subMeshCount = targetSubmeshes;
        }

        private int sourceVertexCount;
        private List<Vector3> targetVertexes;
        private List<Vector2> targetUVs;
        private List<int>[] targetTrianglesBySubmesh;

        private void FinalizeCopy()
        {
            targetMesh.SetVertices(targetVertexes);
            targetMesh.SetUVs(0, targetUVs);

            for (var submesh = 0; submesh < targetSubmeshes; submesh++)
            {
                var submeshTriangles = targetTrianglesBySubmesh[submesh];
                targetMesh.SetTriangles(submeshTriangles, submesh);
            }
            targetMesh.RecalculateNormals();
        }

        private int currentDuplicateIndex = -1;
        public void NextCopy(Vector3 offset)
        {
            this.CopyVertexesToOffset(offset);
            this.CopyUVsToOffsetIndex();

            currentDuplicateIndex++;
        }

        public void CopySubmeshTrianglesToOffsetIndex(int sourceSubmesh, int targetSubmesh)
        {
            var sourceTriangles = sourceMesh.GetTriangles(sourceSubmesh);
            var vertexIndexOffset = currentDuplicateIndex * sourceVertexCount;

            var targetSubmeshTrianges = targetTrianglesBySubmesh[targetSubmesh];

            for (var tri = 0; tri < sourceTriangles.Length; tri++)
            {
                targetSubmeshTrianges.Add(sourceTriangles[tri] + vertexIndexOffset);
            }
        }

        private void CopyVertexesToOffset(Vector3 offset)
        {
            var sourceVertexes = sourceMesh.vertices;
            for (var vert = 0; vert < sourceVertexes.Length; vert++)
            {
                targetVertexes.Add(sourceVertexes[vert] + offset);
            }
        }

        private void CopyUVsToOffsetIndex()
        {
            var sourceUVs = new List<Vector2>();
            sourceMesh.GetUVs(0, sourceUVs);
            targetUVs.AddRange(sourceUVs);
        }


        public void Dispose()
        {
            this.FinalizeCopy();
        }
    }
}
