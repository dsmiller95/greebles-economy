using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Timezone
{
    Day,
    Evening,
    Night
}
[Serializable]
public struct TimezoneConfig
{
    public Timezone zone;
    public float startTime;
}

public class TimeController : MonoBehaviour
{

    public static TimeController instance;

    public float dayLength = 10f;
    public TimezoneConfig[] timezones;
    private IList<TimezoneConfig> timezonesInteral;
    /// <summary>
    /// the current time in the day; represented as a value between 0 inclusive and 1 exclusive
    /// </summary>
    public float currentTime;
    public Animator sunAnimator;

    private void Awake()
    {
        instance = this;
        this.timezonesInteral = timezones
            .OrderByDescending(x => x.startTime)
            .ToList();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.currentTime += Time.deltaTime / dayLength;
        this.currentTime %= 1;
        sunAnimator.SetFloat("Motion", this.currentTime);
    }

    public Timezone GetTimezone()
    {
        foreach (var timezone in timezonesInteral)
        {
            if (this.currentTime >= timezone.startTime)
            {
                return timezone.zone;
            }
        }
        throw new Exception("incorrectly formatted time zone indexes");
    }
}
