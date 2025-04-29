using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    static public TimeManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void TimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = timeScale * 0.02f;
    }
}