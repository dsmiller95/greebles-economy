using Assets.MapGen.TileManagement;
using Simulation.Tiling;
using UnityEngine;

namespace Assets.Scripts.MovementExtensions
{
    public class HexMember : MonoBehaviour, ITilemapMember
    {
        public OffsetCoordinate localPosition;
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
        public void Start()
        {
            var manager = MapManager;
            manager.PlaceNewMapMember(this);
            this.UpdatePositionInWorldSpace();
        }

        public void OnDestroy()
        {
            MapManager.DeRegisterInGrid(this);
        }


        public OffsetCoordinate LocalPosition => localPosition;

        public OffsetCoordinate PositionInTileMap
        {
            get => LocalPosition + (parentMemberTransform?.PositionInTileMap ?? new OffsetCoordinate(0, 0));
            set
            {
                MapManager?.DeRegisterInGrid(this);
                localPosition = value - (parentMemberTransform?.PositionInTileMap ?? new OffsetCoordinate(0, 0));
                MapManager?.RegisterInGrid(this);
                this.UpdatePositionInWorldSpace();
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

        public void UpdatePositionInWorldSpace()
        {
            transform.position = this.GetPositionInWorldSpace();
        }

        protected virtual Vector3 GetPositionInWorldSpace()
        {
            var placeSpace = PositionInTilePlane;
            var placementInsideTilemap = new Vector3(placeSpace.x, 0, placeSpace.y);
            return MapManager.transform.TransformPoint(placementInsideTilemap);
        }

        public T TryGetType<T>()
        {
            return this.GetComponent<T>();
        }
    }
}