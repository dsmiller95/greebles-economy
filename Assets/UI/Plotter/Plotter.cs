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

    private IList<PlotContainer> plotContainers;

    public void SetPlottablesPreStart(IEnumerable<IPlottableSeries> plottables)
    {
        this.plottables = plottables.ToList();
    }

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        if(this.plottables == default)
        {
            this.plottables = plots
                .SelectMany(gameObject => gameObject.GetComponents<IPlottableSeries>())
                .Where(plottable => plottable != default)
                .Concat(plots
                    .SelectMany(gameObject => gameObject.GetComponents<IMultiPlottableSeries>())
                    .Where(multiPlottable => multiPlottable != default)
                    .SelectMany(multiPlottable => multiPlottable.GetPlottableSeries()))
                .ToList();
        }
        Debug.Log($"found {plottables.Count} plots");
        plotContainers = this.plottables
            .Select(plottable => new PlotContainer(plottable, this))
            .ToList();
        foreach (var container in plotContainers)
        {
            container.Init();
        }
    }

    private void OnDestroy()
    {
        foreach (var container in plotContainers)
        {
            container.OnDestroy();
        }
    }
}
