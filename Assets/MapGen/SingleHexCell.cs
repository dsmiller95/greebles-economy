using Assets.MapGen.TileManagement;
using UnityEngine;
using static Assets.MapGen.TileManagement.HexTileMapManager;

namespace Assets.MapGen
{
    public class SingleHexCell : MonoBehaviour, ITileMapMember
    {
        public MeshRenderer renderer;
        public Material[] materialOptions;

        // Start is called before the first frame update
        void Start()
        {
            renderer.material = materialOptions[Random.Range(0, materialOptions.Length)];
        }

        // Update is called once per frame
        void Update()
        {

        }
        private TileMapItem myItem;
        public void SetMapItem(TileMapItem item)
        {
            myItem = item;
        }

        public void UpdateWorldSpace()
        {
            var placeSpace = myItem.PositionInTilePlane;
            var worldPlacement = new Vector3(placeSpace.x, 0, placeSpace.y);
            this.transform.localPosition = worldPlacement;
        }
    }
}