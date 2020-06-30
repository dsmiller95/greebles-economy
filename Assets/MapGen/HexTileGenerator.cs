using Assets.MapGen.TileManagement;
using UnityEngine;

namespace Assets.MapGen
{

    [RequireComponent(typeof(HexTileMapManager))]
    [RequireComponent(typeof(HexCellGenerator))]
    public class HexTileGenerator : MonoBehaviour
    {

        public void Awake()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            PlaceTiles(GetComponent<HexCellGenerator>(), GetComponent<HexTileMapManager>());
        }

        private void PlaceTiles(HexCellGenerator cellGenerator, HexTileMapManager tilemapManager)
        {
            var totalCells = new Vector2Int(tilemapManager.tileMapWidth, tilemapManager.tileMapHeight);
            for (var verticalIndex = 0; verticalIndex < totalCells.y; verticalIndex++)
            {
                for (var horizontalIndex = 0; horizontalIndex < totalCells.x; horizontalIndex++)
                {
                    var tileLocation = new Vector2Int(horizontalIndex, verticalIndex);
                    cellGenerator.CreateTileAtPosition(tileLocation, tilemapManager);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}