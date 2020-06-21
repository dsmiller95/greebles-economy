using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.UI.PathPlotter
{
    public class MultiPathPlotter : MonoBehaviour
    {
        //public Vector3[] path;
        public bool loop;
        public GameObject singlePathPlotterPrefab;

        private IList<SinglePathPlotter> pathRenders;

        // Start is called before the first frame update
        void Awake()
        {
            pathRenders = new List<SinglePathPlotter>();
        }

        public void SetPath(IList<Vector3> path)
        {
            //paths = new List<SinglePathPlotter>();
            var totalPoints = path.Count + (loop ? 0 : -1);
            //var previousPoint = path[0];
            for (var point = 0; point < totalPoints; point++)
            {
                SinglePathPlotter singlePath;
                if (point >= pathRenders.Count)
                {
                    var newPlotObject = Instantiate(singlePathPlotterPrefab, transform);
                    singlePath = newPlotObject.GetComponent<SinglePathPlotter>();
                    pathRenders.Add(singlePath);
                }
                else
                {
                    singlePath = pathRenders[point];
                }
                singlePath.transform.position = path[point];
                singlePath.end = path[(point + 1) % path.Count];
            }
        }

        public void SetPath(IEnumerable<Vector3> path)
        {
            this.SetPath(path.ToList());
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}