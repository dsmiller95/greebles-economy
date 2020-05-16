using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

class PlotContainer
{
    private IPlottable plottable;
    private PlottableConfig config;
    private Plotter parent;

    private IList<GameObject> dots;
    private IList<GameObject> lines;

    public PlotContainer(IPlottable plottable, PlottableConfig config, Plotter parent)
    {
        this.parent = parent;
        this.config = config;
        this.plottable = plottable;
    }


    public void InitGraphObjects()
    {
        var positions = GraphPositioning(plottable, config).ToList();
        this.lines = positions
            .RollingWindow(2)
            .Select(pair => CreateDotConnection(pair[0], pair[1]))
            .ToList();
        this.dots = positions
            .Select(pos => CreateDot(pos))
            .ToList();
    }

    public void Update()
    {
        var positions = GraphPositioning(plottable, config).ToList();
        foreach (var connection in positions.RollingWindow(2).Zip(lines, (pair, line) => new { pair, line }))
        {
            UpdateDotConnection(connection.pair[0], connection.pair[1], connection.line);
        }
        foreach (var dot in positions.Zip(dots, (pos, dot) => new { pos, dot }))
        {
            UpdateDot(dot.pos, dot.dot);
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
    private IEnumerable<Vector2> GraphPositioning(IPlottable plot, PlottableConfig config)
    {
        float graphHeight = this.parent.container.sizeDelta.y;
        float xUIStep = this.parent.container.sizeDelta.x / config.steps;
        float xValueStep = (config.end - config.start) / config.steps;
        for (int i = 0; i < config.steps; i++)
        {
            var value = plot.PlotAt(i * xValueStep);
            float xPos = i * xUIStep + this.parent.xOffset;
            float yPos = Math.Min(value * graphHeight / config.yScale, graphHeight);
            var dotPos = new Vector2(xPos, yPos);
            yield return dotPos;
        }
    }


    private GameObject CreateDot(Vector2 anchor)
    {
        var newDot = new GameObject("circle", typeof(Image));
        newDot.transform.SetParent(this.parent.container, false);

        var image = newDot.GetComponent<Image>();
        image.sprite = this.parent.circleSprite;
        image.color = config.dotColor;

        var transform = newDot.GetComponent<RectTransform>();
        transform.anchoredPosition = anchor;
        transform.sizeDelta = new Vector2(11, 11);
        transform.anchorMin = new Vector2(0, 0);
        transform.anchorMax = new Vector2(0, 0);
        return newDot;
    }


    private GameObject CreateDotConnection(Vector2 start, Vector2 end)
    {
        var connection = new GameObject("connector", typeof(Image));
        connection.transform.SetParent(this.parent.container, false);
        connection.GetComponent<Image>().color = config.lineColor;
        var transform = connection.GetComponent<RectTransform>();
        transform.anchorMin = new Vector2(0, 0);
        transform.anchorMax = new Vector2(0, 0);

        var difference = end - start;
        var dir = difference.normalized;
        var distance = difference.magnitude;

        transform.anchoredPosition = start + dir * distance * 0.5f;
        transform.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, dir));
        transform.sizeDelta = new Vector2(distance, 3);
        return connection;
    }

}

