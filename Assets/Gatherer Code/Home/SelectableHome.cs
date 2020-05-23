using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableHome : MonoBehaviour, ISelectable
{
    public ResourceTimeSeriesAdapter ResourcePlotter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public InfoPaneConfiguration GetInfoPaneConfiguration()
    {
        return new InfoPaneConfiguration()
        {
            plottables = new List<GameObject>() { ResourcePlotter.gameObject }
        };
    }

    public void OnMeDeselected()
    {
        Debug.Log($"{gameObject.name} deselected");
    }

    public void OnMeSelected()
    {
        Debug.Log($"{gameObject.name} selected");
    }
}
