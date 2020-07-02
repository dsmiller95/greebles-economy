using Assets.MapGen.TileManagement;
using UnityEngine;

namespace Assets.Scripts.MovementExtensions
{
    public class HexMember : MonoBehaviour, ITilemapMember
    {
        public Vector2Int localPosition;
        public HexTileMapManager managerSetForInspector;

        private ITilemapMember parentMemberTransform;

        void Awake()
        {
            if (managerSetForInspector != null)
            {
                MapManager = managerSetForInspector;
            }
            parentMemberTransform = transform.parent.GetComponentInParent<ITilemapMember>();
        }

        // Start is called before the first frame update
        void Start()
        {
            //Debug.Log($"Registered {gameObject.name} at {localPosition}: {tilemapManager}");
            var manager = MapManager;
            manager.PlaceNewMapMember(this);
        }


        public Vector2Int LocalPosition => localPosition;

        public Vector2Int PositionInTileMap
        {
            get => LocalPosition + (parentMemberTransform?.PositionInTileMap ?? new Vector2Int(0, 0));
            set
            {
                MapManager?.DeRegisterInGrid(this);
                localPosition = value - (parentMemberTransform?.PositionInTileMap ?? new Vector2Int(0, 0));
                MapManager?.RegisterInGrid(this);
            }
        }

        public Vector2 PositionInTilePlane => MapManager.TileMapPositionToPositionInPlane(PositionInTileMap);

        private HexTileMapManager cachedTileMapManager;
        public HexTileMapManager MapManager
        {
            get
            {
                if (cachedTileMapManager != null)
                {
                    return cachedTileMapManager;
                }
                var newManager = parentMemberTransform?.MapManager;
                if (newManager != null)
                {
                    cachedTileMapManager = newManager;
                }
                return cachedTileMapManager;
            }
            set => cachedTileMapManager = value;
        }

        public void UpdateWorldSpace()
        {
            var placeSpace = PositionInTilePlane;
            var placementInsideTilemap = new Vector3(placeSpace.x, 0, placeSpace.y);
            transform.position = MapManager.transform.TransformPoint(placementInsideTilemap);
        }

        public T TryGetType<T>()
        {
            return this.GetComponent<T>();
        }
    }
}