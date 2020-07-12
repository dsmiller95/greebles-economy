using Assets.UI.SelectionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.UI.PathPlotter
{
    public class MultiPathPlotter : MonoBehaviour, IClickable
    {
        //public Vector3[] path;
        public bool loop;
        public GameObject singlePathPlotterPrefab;

        private IList<SinglePathPlotter> pathRenders;

        public float dragoutHeight = 1f;
        public string cameraName;
        private Camera cam;

        private Plane targetPlane;

        /// <summary>
        /// Called whenever one of the individual paths is being dragged out,
        ///     every time the mouse position needs to be updated. The vector of the hit
        ///     of a raycast from the camera on a flat plane is passed in, and the 
        ///     position on the plane that should be snapped to is returned. for
        ///     no snapping simply return the input vector
        /// </summary>
        public Func<Vector2, Vector2> GetPathPointOnPlaneFromPointHitOnDragoutPlane;
        /// <summary>
        /// Called whenver the drag has ended. the Vector2 passed in corresponds to the
        ///     same vector2 that was snapped to, the same vector that was returned from
        ///     <see cref="GetPathPointOnPlaneFromPointHitOnDragoutPlane"/>
        /// </summary>
        public Action<Vector2, int> PathDragEnd;

        // Start is called before the first frame update
        void Awake()
        {
            pathRenders = new List<SinglePathPlotter>();
            cam = GameObject.FindGameObjectsWithTag("MainCamera")
                .Where(gameObject => gameObject.name == cameraName).First()
                .GetComponent<Camera>();

            targetPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        }

        public void Update()
        {
            if (currentMouseTrackingIndex == -1)
            {
                return;
            }
            if (!Input.GetMouseButton(0))
            {
                EndTrackingMouse();
                return;
            }
            if (EventSystem.current.IsPointerOverGameObject()) // is the touch on the GUI
            {
                // GUI Action
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            float planeHit;
            if (!targetPlane.Raycast(ray, out planeHit))
            {
                Debug.Log("ray from camera did not hit the floating plane");
                return;
            }

            var pointHitOnPlane = ray.GetPoint(planeHit);
            var pointConverted = GetPathPointOnPlaneFromPointHitOnDragoutPlane(new Vector2(pointHitOnPlane.x, pointHitOnPlane.z));
            lastDragPosition = pointConverted;
            var newPointOnPlane = new Vector3(pointConverted.x, dragoutHeight, pointConverted.y);
            MouseTrackingPositionChanged(newPointOnPlane);
        }

        /// <summary>
        /// set up the endpoints of the path. Call this whenever the path changes
        /// </summary>
        /// <param name="path"></param>
        public void SetPath(IList<Vector3> path)
        {
            //paths = new List<SinglePathPlotter>();
            var totalPoints = path.Count + (loop ? 0 : -1);
            while (totalPoints > pathRenders.Count)
            {
                AddNewSinglePath();
            }
            while (totalPoints < pathRenders.Count)
            {
                Destroy(pathRenders[0].gameObject);
                pathRenders.RemoveAt(0);
            }
            for (var point = 0; point < totalPoints; point++)
            {
                SetPathPoint(point, path[point]);
            }
        }

        public void SetPathPoint(int index, Vector3 point)
        {
            var previousPlotter = pathRenders[index];
            var nextPlotter = pathRenders[(index + 1) % pathRenders.Count];
            previousPlotter.end = nextPlotter.start = point;
        }

        private SinglePathPlotter CreateNewSinglePath()
        {
            var newPlotObject = Instantiate(singlePathPlotterPrefab, transform);
            return newPlotObject.GetComponent<SinglePathPlotter>();
        }
        public SinglePathPlotter AddNewSinglePath()
        {
            var newPath = CreateNewSinglePath();
            pathRenders.Add(newPath);
            return newPath;
        }

        private int currentMouseTrackingIndex = -1;
        private Vector2 lastDragPosition;

        private void EndTrackingMouse()
        {
            var removed = pathRenders[currentMouseTrackingIndex];
            pathRenders.RemoveAt(currentMouseTrackingIndex);

            pathRenders[(currentMouseTrackingIndex) % pathRenders.Count].start = removed.start;
            Destroy(removed.gameObject);

            if (lastDragPosition != default)
            {
                PathDragEnd(lastDragPosition, currentMouseTrackingIndex);
                lastDragPosition = default;
            }


            currentMouseTrackingIndex = -1;
        }

        private void StartTrackingMouseAtPathIndex(int pathIndex, Vector3 startPoint)
        {
            var newPath = CreateNewSinglePath();
            newPath.start = pathRenders[pathIndex % pathRenders.Count].start;
            pathRenders.Insert(pathIndex, newPath);
            SetPathPoint(pathIndex, startPoint);
            currentMouseTrackingIndex = pathIndex;
        }

        private void MouseTrackingPositionChanged(Vector3 newPosition)
        {
            SetPathPoint(currentMouseTrackingIndex, newPosition);
        }

        private void SinglePathPlotterClicked(SinglePathPlotter plotter, RaycastHit hit)
        {
            var index = pathRenders.IndexOf(plotter);
            StartTrackingMouseAtPathIndex(index, hit.point);
        }

        public void MeClicked(RaycastHit hit)
        {
            Debug.Log("I've been clicked!!!!");
            var hitObject = hit.transform.gameObject;
            var hitPath = hitObject.GetComponent<SinglePathPlotter>();
            if (hitPath != null)
            {
                SinglePathPlotterClicked(hitPath, hit);
            }
        }
    }
}