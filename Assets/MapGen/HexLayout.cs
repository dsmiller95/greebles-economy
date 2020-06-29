using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.MapGen
{
    [RequireComponent(typeof(HexCellGenerator))]
    public class HexLayout : MonoBehaviour
    {
        public float hexRadius;
        public float width;
        public float height;

        private readonly Vector2 displacementRatio = new Vector2(3f/2f, Mathf.Sqrt(3));

        /// <summary>
        /// distance between each cell and the row of vertical cells next to it
        /// </summary>
        private float horizontalDisplacement;
        /// <summary>
        /// distance between each cell and the cell directly beloy it
        /// </summary>
        private float verticalDisplacement;

        public void Awake()
        {
            horizontalDisplacement = hexRadius * displacementRatio.x;
            verticalDisplacement = hexRadius * displacementRatio.y;
        }

        // Start is called before the first frame update
        void Start()
        {
            this.GenerateLayout(this.GetComponent<HexCellGenerator>());
        }

        private void GenerateLayout(HexCellGenerator cellGenerator)
        {
            var totalCells = new Vector2(width / horizontalDisplacement, height / verticalDisplacement);
            for(var verticalIndex = 0; verticalIndex < totalCells.y; verticalIndex++)
            {
                for(var horizontalIndex = 0; horizontalIndex < totalCells.x; horizontalIndex++)
                {
                    var agnosticCoords = Vector2.Scale(displacementRatio, new Vector2(horizontalIndex, verticalIndex + ((horizontalIndex % 2) / 2f)) );
                    var scaledCoords = agnosticCoords * hexRadius;
                    var realCoords = new Vector3(scaledCoords.x, 0f, scaledCoords.y);
                    cellGenerator.CreateTileAtPosition(realCoords, agnosticCoords);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}