using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableGatherer : MonoBehaviour, ISelectable
{
    public ResourceTimeSeriesAdapter ResourcePlotter;
    public MeshRenderer meshRenderer;

    public Material baseMaterial;
    public Material selectedMaterial;

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
        this.meshRenderer.material = this.baseMaterial;
        Debug.Log($"{gameObject.name} deselected");
    }

    public void OnMeSelected()
    {
        this.meshRenderer.material = this.selectedMaterial;
        Debug.Log($"{gameObject.name} selected");
    }
}
