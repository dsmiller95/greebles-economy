﻿using Assets.UI.Plotter.Series;
using Extensions;
using System.Collections.Generic;
using System.Linq;
using TradeModeling;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Plotter
{
    class PlotContainer
    {
        private IPlottableSeries plottable;
        private PlottableConfig config;
        private GraphPlotter parent;

        private IList<GameObject> dots;
        private bool hasDots;
        private IList<GameObject> connections;
        private bool hasConnectors;

        public PlotContainer(IPlottableSeries plottable, GraphPlotter parent)
        {
            this.parent = parent;
            config = plottable.GetPlottableConfig();
            this.plottable = plottable;
            hasDots = config.dotColor != default;
            hasConnectors = config.lineColor != default;
            if (hasDots)
            {
                dots = new List<GameObject>();
            }
            if (hasConnectors)
            {
                connections = new List<GameObject>();
            }
        }


        public void Init()
        {
            plottable.SeriesUpdated += Plottable_SeriesUpdated;
        }

        public void OnDestroy()
        {
            plottable.SeriesUpdated -= Plottable_SeriesUpdated;
        }

        private void Plottable_SeriesUpdated(object sender)
        {
            UpdateGraph();
        }

        private void UpdateGraph()
        {
            var positions = GetPositionsInGraph().ToList();

            if (hasConnectors)
            {
                connections = MyUtilities.EnsureAllObjectsCreated(
                    positions.Count - 1,
                    connections,
                    () => CreateUnpositionedDotConnection(),
                    connection => GameObject.Destroy(connection));
                foreach (var connection in positions.RollingWindow(2).Zip(connections, (pair, line) => new { pair, line }))
                {
                    UpdateDotConnection(connection.pair[0], connection.pair[1], connection.line);
                }
            }

            if (hasDots)
            {
                dots = MyUtilities.EnsureAllObjectsCreated(
                    positions.Count,
                    dots,
                    () => CreateUnpositionedDot(),
                    dot => GameObject.Destroy(dot));
                foreach (var dot in positions.Zip(dots, (pos, dot) => new { pos, dot }))
                {
                    UpdateDot(dot.pos, dot.dot);
                }
            }
        }

        private void UpdateDot(Vector2 anchor, GameObject dot)
        {
            var transform = dot.GetComponent<RectTransform>();
            transform.anchoredPosition = anchor;
        }
        private void UpdateDotConnection(Vector2 start, Vector2 end, GameObject connection)
        {
            var transform = connection.GetComponent<RectTransform>();

            var difference = end - start;
            var dir = difference.normalized;
            var distance = difference.magnitude;

            transform.anchoredPosition = start + dir * distance * 0.5f;
            transform.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, dir));
            transform.sizeDelta = new Vector2(distance, 3);
        }

        private IEnumerable<Vector2> GetPositionsInGraph()
        {
            var points = plottable.GetSeries();
            var plotConfig = plottable.GetPlottableConfig();

            float graphHeight = parent.container.rect.height;
            float graphWidth = parent.container.rect.width;
            float xUIScale = graphWidth / plottable.GetPointRange();
            var uiScale = new Vector2(xUIScale, graphHeight / plotConfig.yScale);
            return points.Select(point => new Vector2(point.x * uiScale.x, Mathf.Clamp(point.y * uiScale.y, 0, graphHeight)));
        }
        private GameObject CreateUnpositionedDot()
        {
            var newDot = new GameObject("circle", typeof(Image));
            newDot.transform.SetParent(parent.container, false);

            var image = newDot.GetComponent<Image>();
            image.sprite = parent.circleSprite;
            image.color = config.dotColor;

            var transform = newDot.GetComponent<RectTransform>();
            transform.sizeDelta = new Vector2(11, 11);
            transform.anchorMin = new Vector2(0, 0);
            transform.anchorMax = new Vector2(0, 0);
            return newDot;
        }
        private GameObject CreateUnpositionedDotConnection()
        {
            var connection = new GameObject("connector", typeof(Image));
            connection.transform.SetParent(parent.container, false);
            connection.GetComponent<Image>().color = config.lineColor;
            var transform = connection.GetComponent<RectTransform>();
            transform.anchorMin = new Vector2(0, 0);
            transform.anchorMax = new Vector2(0, 0);
            return connection;
        }



        /// <summary>
        /// /////////////////////////////////////
        /// </summary>
        /// <param name="anchor"></param>
        /// <returns></returns>


        //private GameObject CreateDot(Vector2 anchor)
        //{
        //    var newDot = new GameObject("circle", typeof(Image));
        //    newDot.transform.SetParent(this.parent.container, false);

        //    var image = newDot.GetComponent<Image>();
        //    image.sprite = this.parent.circleSprite;
        //    image.color = config.dotColor;

        //    var transform = newDot.GetComponent<RectTransform>();
        //    transform.anchoredPosition = anchor;
        //    transform.sizeDelta = new Vector2(11, 11);
        //    transform.anchorMin = new Vector2(0, 0);
        //    transform.anchorMax = new Vector2(0, 0);
        //    return newDot;
        //}


        //private GameObject CreateDotConnection(Vector2 start, Vector2 end)
        //{
        //    var connection = new GameObject("connector", typeof(Image));
        //    connection.transform.SetParent(this.parent.container, false);
        //    connection.GetComponent<Image>().color = config.lineColor;
        //    var transform = connection.GetComponent<RectTransform>();
        //    transform.anchorMin = new Vector2(0, 0);
        //    transform.anchorMax = new Vector2(0, 0);

        //    var difference = end - start;
        //    var dir = difference.normalized;
        //    var distance = difference.magnitude;

        //    transform.anchoredPosition = start + dir * distance * 0.5f;
        //    transform.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, dir));
        //    transform.sizeDelta = new Vector2(distance, 3);
        //    return connection;
        //}

    }
}