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
        if(paneConfig.plottables.Count > 0)
        {
            var newPlottable = Instantiate(plottablePrefab, this.transform);
            var plotter = newPlottable.GetComponentInChildren<Plotter>();
            plotter.plots = paneConfig.plottables.ToArray();
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
