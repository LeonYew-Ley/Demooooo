using System.Collections.Generic;
using UnityEngine;

public class STimerManager : MonoBehaviour
{
    private static STimerManager _instance;
    private static List<STimer> timers = new List<STimer>();

    public static void Register(STimer timer)
    {
        if (_instance == null)
        {
            var go = new GameObject("STimerManager");
            _instance = go.AddComponent<STimerManager>();
            DontDestroyOnLoad(go);
        }
        if (!timers.Contains(timer))
            timers.Add(timer);
    }

    public static void Unregister(STimer timer)
    {
        timers.Remove(timer);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        for (int i = timers.Count - 1; i >= 0; i--)
        {
            timers[i]?.Tick(dt);
        }
    }
}
