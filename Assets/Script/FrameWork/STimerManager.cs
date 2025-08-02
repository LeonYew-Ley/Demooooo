using System.Collections.Generic;
using UnityEngine;

public class STimerManager : MonoBehaviour
{
    private static STimerManager _instance;
    private static List<STimer> timers = new List<STimer>();

    public static STimerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("STimerManager");
                _instance = go.AddComponent<STimerManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return; // 直接返回，不再执行后续代码
        }
    }

    public static void Register(STimer timer)
    {
        // Ensure instance exists
        var instance = Instance;
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
