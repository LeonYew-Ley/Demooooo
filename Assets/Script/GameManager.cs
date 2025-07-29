using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void OnEnable()
    {
        SEvent.Instance.AddListener(EventName.PlayerDead, OnPlayerDead);
    }

    private void Start()
    {
        InitializeGame();
    }
    void OnDisable()
    {
        SEvent.Instance.RemoveListener(EventName.PlayerDead, OnPlayerDead);
    }
    private void InitializeGame()
    {
        SLog.Info("GameStarted");
    }

    private void OnPlayerDead()
    {
        SLog.Info($"Open Dead Scene");
        SEvent.Instance.TriggerEvent(EventName.OpenGameOverCanvas);
    }

}