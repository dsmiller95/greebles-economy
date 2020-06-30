using Assets.MapGen.TileManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.MapGen
{

    public class HexCellGenerator: MonoBehaviour
    {
        public GameObject hexTile;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void CreateTileAtPosition(Vector2Int position, HexTileMapManager tilemapManager)
        {
            Debug.Log(position);
            var newTile = Instantiate(hexTile, transform, false);
            var tilemapMember = newTile.GetComponent<ITileMapMember>();
            tilemapManager.RegisterNewMapMember(tilemapMember, position);
        }

    }
}