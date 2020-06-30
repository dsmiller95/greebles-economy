using Assets.MapGen.TileManagement;
using UnityEngine;
using static Assets.MapGen.TileManagement.HexTileMapManager;

namespace Assets.Scripts.MovementExtensions
{
    public class HexMovementManager : MonoBehaviour, ITilemapMember, IObjectSeeker
    {
        public HexTileMapManager tilemapManager;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private TileMapItem hexGridItem;
        private ITilemapMember currentTargetMember;
        private GameObject currentTarget;

        public GameObject CurrentTarget
        {
            get => currentTarget;
            set
            {
                currentTarget = value;
                currentTargetMember = currentTarget.GetComponent<ITilemapMember>();
            }
        }


        #region IObjectSeeker
        public bool seekTargetToTouch()
        {
            throw new System.NotImplementedException();
        }

        private void moveTowardsMember(ITilemapMember member)
        {
            throw new System.NotImplementedException();
        }

        public void ClearCurrentTarget()
        {
            throw new System.NotImplementedException();
        }

        public bool isTouchingCurrentTarget()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region ITileMapMember
        public void SetMapItem(TileMapItem item)
        {
            hexGridItem = item;
        }

        public void UpdateWorldSpace()
        {
            var placeSpace = hexGridItem.PositionInTilePlane;
            var worldPlacement = new Vector3(placeSpace.x, 0, placeSpace.y);
            transform.localPosition = worldPlacement;
        }
        #endregion
    }
}