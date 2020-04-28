using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Gatherer_Code;

public class TimeTracker : MonoBehaviour, ITimeTracker
{
    private Dictionary<ResourceType, float> timeDictionary;
    private ResourceType? currentTracked;

    public TimeTracker()
    {
    }

    // Use this for initialization
    void Start()
    {
        this.timeDictionary = new Dictionary<ResourceType, float>();
        currentTracked = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTracked is ResourceType tracked)
        {
            timeDictionary[tracked] += Time.deltaTime;
        }
    }

    public Dictionary<ResourceType, float> getResourceTimeSummary()
    {
        return timeDictionary;
    }

    public void clearTime()
    {
        timeDictionary.Clear();
    }

    public void startTrackingResource(ResourceType type)
    {
        this.currentTracked = type;
        if (!timeDictionary.ContainsKey(type))
        {
            timeDictionary.Add(type, 0);
        }
    }

    public void pauseTracking()
    {
        this.currentTracked = null;
    }
}

public interface ITimeTracker
{
    Dictionary<ResourceType, float> getResourceTimeSummary();

    void clearTime();

    void startTrackingResource(ResourceType type);
    void pauseTracking();
}
