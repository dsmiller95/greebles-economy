using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class Plotter : MonoBehaviour
{
    public GameObject[] plots;
    private IList<IPlottableSeries> plottables;
    public RectTransform container;
    public Sprite circleSprite;
    public float xOffset = 1;
    public float updateTimeDelay = 0.4f;

    private IList<PlotContainer> plotContainers;

    private void Awake()
    {
        this.plottables = plots
            .Select(g => g.GetComponent<IPlottableSeries>())
            .Where(p => p != default)
            .ToList();
        plotContainers = this.plottables
            .Select(plottable => new PlotContainer(plottable, this))
            .ToList();
    }

    // Update is called once per frame
    void Start()
    {
        foreach (var container in plotContainers)
        {
            container.Init();
        }
    }
}
