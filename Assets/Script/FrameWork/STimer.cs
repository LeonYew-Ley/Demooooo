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


    public void StartTimer()
    {
        Elapsed = 0f;
        IsRunning = true;
    }


    public void StopTimer()
    {
        IsRunning = false;
    }


    public void ResetTimer()
    {
        Elapsed = 0f;
        IsRunning = false;
    }


    public void ResumeTimer()
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
            onComplete?.Invoke();
            if (IsLooping)
            {
                Elapsed -= Duration; // 保留多余的时间（精度更高）
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
