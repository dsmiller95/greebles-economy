using System;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.MovementExtensions
{
    public class FreeFormObjectSeeker : MonoBehaviour, IObjectSeeker
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public float speed = 1;
        public float touchDistance = 1f;
        internal GameObject currentTarget;

        public GameObject seekTargetToTouch()
        {
            if (!currentTarget)
            {
                return null;
            }
            moveTowardsPosition(currentTarget.transform.position);
            if (isTouchingCurrentTarget())
            {
                var previousTarget = currentTarget;
                this.ClearCurrentTarget();
                return previousTarget;
            }
            return null;
        }

        public void ClearCurrentTarget()
        {
            currentTarget = null;
        }

        private void moveTowardsPosition(Vector3 targetPostion)
        {
            var difference = targetPostion - transform.position;
            var direction = new Vector3(difference.x, 0, difference.z).normalized;
            transform.position += direction * Time.deltaTime * speed;
        }

        public bool isTouchingCurrentTarget()
        {
            return distanceToCurrentTarget() <= touchDistance;
        }
        private float distanceToCurrentTarget()
        {
            if (!currentTarget)
            {
                return float.MaxValue;
            }
            var difference = transform.position - currentTarget.transform.position;
            return new Vector3(difference.x, difference.y, difference.z).magnitude;
        }

        public IEnumerable<(GameObject, float)> GetObjectsWithinDistanceFromFilter(float distance, Func<GameObject, bool> filter)
        {
            Collider[] itemsInRadius = Physics.OverlapSphere(transform.position, distance);
            foreach (Collider collider in itemsInRadius)
            {
                var gameObject = collider.gameObject;
                if (filter(gameObject))
                {
                    float individualDistance = (transform.position - collider.transform.position).magnitude;
                    yield return (collider.gameObject, individualDistance);
                }
            }
        }

        public void BeginApproachingNewTarget(GameObject target)
        {
            this.CurrentTarget = target;
        }

        public GameObject CurrentTarget
        {
            get => currentTarget;
            set => currentTarget = value;
        }
    }
}