using Simulation.Tiling;
using UnityEngine;

namespace Assets.MapGen.TileManagement
{
    public class HexMember : MonoBehaviour, ITilemapMember
    {
        public AxialCoordinate localPosition;
        public HexTileMapManager managerSetForInspector;


        private ITilemapMember _parentMemeCache;
        private ITilemapMember parentMemberTransform
        {
            get {
                if(_parentMemeCache == null)
                {
                    _parentMemeCache = transform.parent.GetComponentInParent<ITilemapMember>();
                }
                return _parentMemeCache;
            }
        }

        void Awake()
        {
            if (managerSetForInspector != null)
            {
                MapManager = managerSetForInspector;
            }
        }

        // Start is called before the first frame update
        public void Start()
        {
            // this is not a no-op: will trigger the logic which ensures that this component is registered
            // in the grid, while making sure it is only registered once
            this.PositionInTileMap = this.PositionInTileMap;
            UpdatePositionInWorldSpace();
        }

        public void OnDestroy()
        {
            MapManager.DeRegisterInGrid(this);
        }


        public AxialCoordinate LocalPosition => localPosition;

        public AxialCoordinate PositionInTileMap
        {
            get => LocalPosition + (parentMemberTransform?.PositionInTileMap ?? new AxialCoordinate(0, 0));
            set
            {
                MapManager?.DeRegisterInGrid(this);
                localPosition = value - (parentMemberTransform?.PositionInTileMap ?? new AxialCoordinate(0, 0));
                MapManager?.RegisterInGrid(this);
                UpdatePositionInWorldSpace();
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
                    return cachedTileMapManager;
                }
                newManager = GetComponentInParent<HexTileMapManager>();
                if (newManager != null)
                {
                    cachedTileMapManager = newManager;
                    return cachedTileMapManager;
                }
                return null;
            }
            set => cachedTileMapManager = value;
        }

        public void UpdatePositionInWorldSpace()
        {
            transform.position = GetPositionInWorldSpace();
        }

        protected virtual Vector3 GetPositionInWorldSpace()
        {
            var placeSpace = PositionInTilePlane;
            var placementInsideTilemap = new Vector3(placeSpace.x, 0, placeSpace.y);
            return MapManager.transform.TransformPoint(placementInsideTilemap);
        }

        public T TryGetType<T>()
        {
            if (this == null)
            {
                Debug.LogError($"Error: attempting to access destroyed HexMember at {this.PositionInTileMap}");
            }
            return GetComponentInChildren<T>();
        }
    }
}