using Assets.MapGen.TileManagement;
using Simulation.Tiling;
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
        public Animator[] animtators;

        private ITilemapMember currentTargetMember;
        private GameObject currentTarget;

        public new void Start()
        {
            base.Start();
            foreach (var animator in animtators)
            {
                animator.SetFloat("Motion", 0);
                animator.SetTrigger("Cancel");
            }
        }

        public GameObject CurrentTarget
        {
            get => currentTarget;
            private set => currentTarget = value;
        }

        private TileRoute currentRoute;
        private float timeOfLastAction;
        public GameObject seekTargetToTouch(bool intersect = false)
        {
            if (CurrentTarget == null)
            {
                return null;
            }
            if (currentRoute.TotalWaypoints() <= (intersect ? 0 : 1))
            {
                //the movement action started with no motion required. we can return without doing anything
                return TargetReached();
            }
            UpdateAnimation();

            if (timeOfLastAction + speed > Time.time)
            {
                // not going to transition to a new spot this frame
                return null;
            }
            timeOfLastAction = Time.time;


            var nextPosition = currentRoute.PopNextWaypoint();
            PositionInTileMap = nextPosition;

            // if we're one or less away, we good
            if (currentRoute.TotalWaypoints() <= (intersect ? 0 : 1))
            {
                return TargetReached();
            }
            StartNewAnimation(currentRoute.PeekNext());
            //tileGridItem.SetPositionInTileMap(nextPosition);
            return null;
        }

        private void UpdateAnimation()
        {
            var offsetFromLastMove = (Time.time - timeOfLastAction) / speed;
            foreach (var animator in animtators)
            {
                animator.SetFloat("Motion", offsetFromLastMove);
            }
        }

        private void StartNewAnimation(AxialCoordinate nextWaypoint)
        {
            var nextWaypointReal = MapManager.TileMapToReal(nextWaypoint);
            var currentReal = PositionInTilePlane;
            var diff = nextWaypointReal - currentReal;
            var newRotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.y), Vector3.up);
            transform.localRotation = newRotation;

            foreach (var animator in animtators)
            {
                animator.SetTrigger("Start");
                animator.ResetTrigger("Cancel");
            }
        }

        private void CancelAnimation()
        {
            foreach (var animator in animtators)
            {
                animator.SetTrigger("Cancel");
                animator.ResetTrigger("Start");
            }
        }

        private GameObject TargetReached()
        {
            var targetCached = CurrentTarget;
            ClearCurrentTarget();
            CancelAnimation();
            return targetCached;
        }

        public void ClearCurrentTarget()
        {
            CurrentTarget = null;
            currentRoute = null;
        }

        public void BeginApproachingNewTarget(GameObject target)
        {
            if (target != null)
            {
                currentTargetMember = target.GetComponentInParent<ITilemapMember>();
                currentRoute = MapManager.GetRouteBetweenMembers(this, currentTargetMember);
                if (currentRoute.TotalWaypoints() > 0)
                {
                    StartNewAnimation(currentRoute.PeekNext());
                }
                timeOfLastAction = Time.time;
                CurrentTarget = target;
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