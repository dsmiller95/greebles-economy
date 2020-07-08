using Assets.UI.SelectionManager;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.UI.PathPlotter
{
    public class MultiPathPlotter : MonoBehaviour, ISelectionInput
    {
        //public Vector3[] path;
        public bool loop;
        public GameObject singlePathPlotterPrefab;

        private IList<SinglePathPlotter> pathRenders;

        public float dragoutHeight = 1f;
        public string cameraName;
        private Camera cam;
        // Start is called before the first frame update
        void Awake()
        {
            pathRenders = new List<SinglePathPlotter>();
            cam = GameObject.FindGameObjectsWithTag("MainCamera")
                .Where(gameObject => gameObject.name == cameraName).First()
                .GetComponent<Camera>();
        }

        public void Update()
        {
            if (currentMouseTrackingIndex == -1)
            {
                return;
            }
            if (!Input.GetMouseButton(0))
            {
                this.CancelTrackingMouse();
                return;
            }
            if (EventSystem.current.IsPointerOverGameObject()) // is the touch on the GUI
            {
                // GUI Action
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit, 1000))
            {
                return;
            }

            var pointOnPlane = hit.point;
            pointOnPlane.y = dragoutHeight;

            this.MouseTrackingPositionChanged(pointOnPlane);
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

        private void CancelTrackingMouse()
        {
            var removed = pathRenders[currentMouseTrackingIndex];
            pathRenders.RemoveAt(currentMouseTrackingIndex);

            pathRenders[(currentMouseTrackingIndex) % pathRenders.Count].start = removed.start;
            Destroy(removed.gameObject);
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

        public void BeginSelectionInput()
        {
        }

        public void CloseSelectionInput()
        {
        }

        public bool Supersceded(ISelectionInput other)
        {
            return false;
        }

        public bool IsValidSelection(GameObject o)
        {
            return o.GetComponent<SinglePathPlotter>();
        }

        public bool SelectedObject(GameObject o, RaycastHit rayHit)
        {
            var singlePathPlotter = o.GetComponent<SinglePathPlotter>();
            var index = pathRenders.IndexOf(singlePathPlotter);
            StartTrackingMouseAtPathIndex(index, rayHit.point);
            return false;
        }
    }
}