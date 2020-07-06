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
            var newColors = targetMesh.colors32;

            SetColorOnColorArray(newColors, duplicateIndex, color);

            targetMesh.colors32 = newColors;
        }

        public void SetColorsOnVertexesAtDuplicates(IEnumerable<(int, Color32)> duplicateColors)
        {
            var newColors = targetMesh.colors32;

            foreach(var duplicate in duplicateColors)
            {
                SetColorOnColorArray(newColors, duplicate.Item1, duplicate.Item2);
            }

            targetMesh.colors32 = newColors;
        }

        private void SetColorOnColorArray(Color32[] colorArray, int duplicateIndex, Color32 color)
        {
            var beginningColorIndex = duplicateIndex * sourceMeshVertexSize;
            var endingColorIndex = beginningColorIndex + sourceMeshVertexSize;
            for (var i = beginningColorIndex; i < endingColorIndex; i++)
            {
                colorArray[i] = color;
            }
        }
    }
}
