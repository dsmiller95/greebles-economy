using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.UI.PathPlotter
{
    public class MultiPathPlotter : MonoBehaviour
    {
        public Vector3[] path;
        public bool loop;
        public GameObject singlePathPlotterPrefab;

        private IList<SinglePathPlotter> paths;

        // Start is called before the first frame update
        void Start()
        {
            paths = new List<SinglePathPlotter>();
            var previousPoint = loop ? path[path.Length - 1] : path[0];
            for(var nextPoint = loop ? 0 : 1; nextPoint < path.Length; nextPoint++)
            {
                var newPlotObject = Instantiate(singlePathPlotterPrefab, this.transform);
                newPlotObject.transform.position = previousPoint;
                var pathObj = newPlotObject.GetComponent<SinglePathPlotter>();
                pathObj.end = path[nextPoint];
                paths.Add(pathObj);

                previousPoint = path[nextPoint];
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}