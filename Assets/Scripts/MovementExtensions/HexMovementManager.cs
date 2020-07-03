using Assets.MapGen.TileManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MovementExtensions
{
    public class HexMovementManager : HexMember, IObjectSeeker
    {
        /// <summary>
        /// Seconds it takes to move to the next tile in the hex grid
        /// </summary>
        public float speed = 1;
        public Animator animtator;

        private ITilemapMember currentTargetMember;
        private GameObject currentTarget;

        public new void Start()
        {
            base.Start();
            animtator.SetFloat("Hop_Amount", 0);
            animtator.SetTrigger("Hop_Cancel");
        }

        public GameObject CurrentTarget
        {
            get => currentTarget;
            private set => currentTarget = value;
        }

        private TileRoute currentRoute;
        private float timeOfLastAction;
        public GameObject seekTargetToTouch()
        {
            if (CurrentTarget != null)
            {
                UpdateAnimation();
            }
            else
            {
                return null;
            }

            if (timeOfLastAction + speed > Time.time)
            {
                // not going to transition to a new spot this frame
                return null;
            }
            timeOfLastAction = Time.time;


            var nextPosition = currentRoute.PopNextWaypoint();
            PositionInTileMap = nextPosition;

            // if we're one or less away, we good
            if (currentRoute.waypoints.Count <= 1)
            {
                var previousTarget = this.CurrentTarget;
                this.TargetReached();
                return previousTarget;
            }
            StartNewAnimation(currentRoute.PeekNext());
            //tileGridItem.SetPositionInTileMap(nextPosition);
            return null;
        }

        private void UpdateAnimation()
        {
            var offsetFromLastMove = (Time.time - timeOfLastAction) / speed;
            animtator.SetFloat("Hop_Amount", offsetFromLastMove);
        }

        private void StartNewAnimation(Vector2Int nextWaypoint)
        {
            var nextWaypointReal = MapManager.TileMapToReal(nextWaypoint);
            var currentReal = PositionInTilePlane;
            var diff = nextWaypointReal - currentReal;
            var newRotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.y), Vector3.up);
            transform.localRotation = newRotation;

            animtator.SetTrigger("Hop_Start");
            animtator.ResetTrigger("Hop_Cancel");
        }

        private void CancelAnimation()
        {
            animtator.SetTrigger("Hop_Cancel");
            animtator.ResetTrigger("Hop_Start");
        }

        private void TargetReached()
        {
            this.ClearCurrentTarget();
            this.CancelAnimation();
        }

        public void ClearCurrentTarget()
        {
            CurrentTarget = null;
            currentRoute = null;
        }

        public void BeginApproachingNewTarget(GameObject target)
        {
            CurrentTarget = target;
            if (CurrentTarget != null)
            {
                currentTargetMember = currentTarget.GetComponentInParent<ITilemapMember>();
                currentRoute = MapManager.GetRouteBetweenMembers(this, currentTargetMember);
                StartNewAnimation(currentRoute.PeekNext());
                timeOfLastAction = Time.time;
            }
        }

        public bool isTouchingCurrentTarget()
        {
            return MapManager.IsWithinDistance(this, currentTargetMember, 1);
        }

        public IEnumerable<(GameObject, float)> GetObjectsWithinDistanceFromFilter(float maxDistance, Func<GameObject, bool> filter)
        {
            var myPosition = PositionInTileMap;
            return MapManager.GetPositionsWithinJumpDistance(myPosition, (int)maxDistance)
                .SelectMany(position =>
                {
                    var distance = MapManager.DistanceBetweenInJumps(myPosition, position);
                    return MapManager
                        .GetMembersAtLocation<HexMember>(position, member => filter(member.gameObject))
                        ?.Select(hexMember => (hexMember.gameObject, (float)distance)) ?? new (GameObject, float)[0];

                });
        }

    }
}