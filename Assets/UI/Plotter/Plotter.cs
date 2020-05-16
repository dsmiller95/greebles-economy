using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct PlottableConfig
{
    public float start;
    public float end;
    public float yScale;
    public int steps;
    public Color lineColor;
    public Color dotColor;
}

public class Plotter : MonoBehaviour
{
    public GameObject[] plots;
    private IList<IPlottable> plottables;
    public PlottableConfig[] plotConfigs;
    public RectTransform container;
    public Sprite circleSprite;
    public float xOffset = 1;
    public float updateTimeDelay = 0.4f;

    private IList<PlotContainer> plotContainers;

    private void Awake()
    {
        this.plottables = plots
            .Select(g => g.GetComponent<IPlottable>())
            .Where(p => p != default)
            .ToList();
        plotContainers = this.plottables
            .Select((x, y) => new PlotContainer(x, plotConfigs[y], this))
            .ToList();
    }

    // Update is called once per frame
    void Start()
    {
        foreach (var container in plotContainers)
        {
            container.InitGraphObjects();
        }
    }

    private float lastUpdate = 0;
    private void Update()
    {
        if(lastUpdate + updateTimeDelay < Time.time)
        {
            foreach (var container in plotContainers)
            {
                container.Update();
            }
        }
    }
}
