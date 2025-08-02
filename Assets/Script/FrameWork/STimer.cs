using UnityEngine;
using System;
public class STimer
{
    public float Duration { get; private set; }
    public float Elapsed { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsLooping { get; private set; }

    private Action onComplete;


    public STimer(float duration, Action onComplete = null, bool isLooping = false)
    {
        SLog.Info("A Timer has been created.");
        Duration = duration;
        this.onComplete = onComplete;
        IsLooping = isLooping;
        Elapsed = 0f;
        IsRunning = false;
        STimerManager.Register(this);
    }


    public void Start()
    {
        Elapsed = 0f;
        IsRunning = true;
    }


    public void Stop()
    {
        IsRunning = false;
    }


    public void Reset()
    {
        Elapsed = 0f;
        IsRunning = false;
    }


    public void Resume()
    {
        IsRunning = true;
    }
    public void Tick(float deltaTime)
    {
        if (!IsRunning)
            return;

        Elapsed += deltaTime;

        if (Elapsed >= Duration)
        {
            SLog.Info($"Timer completed! Duration: {Duration}s, Actual elapsed: {Elapsed}s");
            onComplete?.Invoke();
            if (IsLooping)
            {
                Elapsed = 0f; // 保留多余的时间（精度更高）
                SLog.Info($"Timer restarted for loop. Remaining time carried over: {Elapsed}s");
            }
            else
            {
                IsRunning = false;
            }
        }
    }

    public float GetRemainingTime()
    {
        return Mathf.Max(0, Duration - Elapsed);
    }

    public float GetProgress()
    {
        return Mathf.Clamp01(Elapsed / Duration);
    }

    public void Dispose()
    {
        STimerManager.Unregister(this);
        onComplete = null;
    }
}
