using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SelectionTracker))]
public class InfoPaneBuilder : MonoBehaviour
{
    public GameObject plottablePrefab;

    private SelectionTracker selectionTracker;

    private void Awake()
    {
        this.selectionTracker = this.GetComponent<SelectionTracker>();
        this.selectionTracker.SelectionChanged += OnSelectionChanged;
    }

    private void OnSelectionChanged(object sender, ISelectable e)
    {
        this.ClearUI();
        var paneConfig = e.GetInfoPaneConfiguration();
        var plotVerticalOffset = 0f;
        foreach(var plottableConfig in paneConfig.plottables)
        {
            var newPlottable = Instantiate(plottablePrefab, this.transform);

            var plotter = newPlottable.GetComponentInChildren<Plotter>();
            plotter.SetPlottablesPreStart(plottableConfig.plot.GetPlottableSeries());

            var positioning = newPlottable.GetComponentInChildren<RectTransform>();
            positioning.position -= new Vector3(0, plotVerticalOffset);
            plotVerticalOffset += positioning.sizeDelta.y;
        }
    }

    private void ClearUI()
    {
        // transform does not implement a generic IEnumerable,
        //   but it will return all transform children when iterated
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
