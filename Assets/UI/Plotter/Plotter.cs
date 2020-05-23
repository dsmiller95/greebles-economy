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
    }

    // Update is called once per frame
    void Start()
    {
        this.plottables = plots
            .Select(gameObject => gameObject.GetComponent<IPlottableSeries>())
            .Where(plottable => plottable != default)
            .Concat(plots
                .Select(gameObject => gameObject.GetComponent<IMultiPlottableSeries>())
                .Where(multiPlottable => multiPlottable != default)
                .Select(multiPlottable => multiPlottable.GetPlottableSeries())
                .SelectMany(plottable => plottable))
            .ToList();
        Debug.Log($"found {plottables.Count} plots");
        plotContainers = this.plottables
            .Select(plottable => new PlotContainer(plottable, this))
            .ToList();
        foreach (var container in plotContainers)
        {
            container.Init();
        }
    }
}
