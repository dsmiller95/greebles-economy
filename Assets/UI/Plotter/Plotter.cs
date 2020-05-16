using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlottableConfig
{
    public float start;
    public float end;
    public float resolution;
    public Color lineColor;
}

public class Plotter : MonoBehaviour
{
    public IPlottable[] plots;
    public PlottableConfig[] plotConfigs;
    public RectTransform container;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
