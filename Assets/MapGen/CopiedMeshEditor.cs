using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.MapGen
{
    public class CopiedMeshEditor
    {
        private Mesh targetMesh;
        private int sourceMeshVertexSize;

        public CopiedMeshEditor(
            int sourceMeshVertexSize,
            Mesh targetMesh)
        {
            this.targetMesh = targetMesh;
            this.sourceMeshVertexSize = sourceMeshVertexSize;
        }

        /// <summary>
        /// find the vertexes the were created as <paramref name="duplicateIndex"/> and set them to <paramref name="color"/>
        /// </summary>
        /// <param name="duplicateIndex"></param>
        /// <param name="color"></param>
        public void SetColorOnVertexesAtDuplicate(int duplicateIndex, Color32 color)
        {
            var sourceColorSize = sourceMeshVertexSize;
            var beginningColorIndex = duplicateIndex * sourceColorSize;
            var endingColorIndex = beginningColorIndex + sourceColorSize;

            var newColors = targetMesh.colors32;
            for (var i = beginningColorIndex; i < endingColorIndex; i++)
            {
                newColors[i] = color;
            }
            targetMesh.colors32 = newColors;
        }
    }
}
