using Assets.MapGen.TileManagement;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MovementExtensions
{
    public class HexMovementManager : HexMember, IObjectSeeker
    {

        #region IObjectSeeker
        /// <summary>
        /// Seconds it takes to move to the next tile in the hex grid
        /// </summary>
        public float speed = 1;

        private ITilemapMember currentTargetMember;
        private GameObject currentTarget;

        public GameObject CurrentTarget
        {
            get => currentTarget;
            set
            {
                currentTarget = value;
                currentTargetMember = currentTarget?.GetComponentInParent<ITilemapMember>();
            }
        }

        private TileRoute currentRoute;
        private float lastTimeMoved;
        public bool seekTargetToTouch()
        {
            if (lastTimeMoved + speed > Time.time || CurrentTarget == null)
            {
                return false;
            }
            lastTimeMoved = Time.time;

            if(currentRoute == null)
            {
                currentRoute = MapManager.GetRouteBetweenMembers(this, this.currentTargetMember);
            }

            // if we're one away, we good
            if (currentRoute.Count() == 1)
            {
                currentRoute = null;
                return true;
            }
            var nextPosition = currentRoute.PopNextWaypoint();

            this.PositionInTileMap = nextPosition;
            //tileGridItem.SetPositionInTileMap(nextPosition);
            return false;
        }

        public void ClearCurrentTarget()
        {
            CurrentTarget = null;
            currentRoute = null;
        }

        public bool isTouchingCurrentTarget()
        {
            return MapManager.IsWithinDistance(this, currentTargetMember, 1);
        }
        #endregion

    }
}