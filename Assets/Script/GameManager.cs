using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void OnEnable()
    {
        SEvent.Instance.AddListener(EventName.PlayerDead, OnPlayerDead);
        SEvent.Instance.AddListener(EventName.GameStart, StartGame);
        SEvent.Instance.AddListener(EventName.GameRestart, RestartGame);
    }

    private void Start()
    {
        InitializeApplication();
    }
    void OnDisable()
    {
        SEvent.Instance.RemoveListener(EventName.PlayerDead, OnPlayerDead);
        SEvent.Instance.RemoveListener(EventName.GameStart, StartGame);
        SEvent.Instance.AddListener(EventName.GameRestart, RestartGame);
    }
    private void InitializeApplication()
    {
        SLog.Info("Application started.");
    }

    private void OnPlayerDead()
    {
        SLog.Info($"Open Dead Scene");
        SEvent.Instance.TriggerEvent(EventName.ShowGameOver);
    }

    private void StartGame()
    {
        // 隐藏主界面
        SEvent.Instance.TriggerEvent(EventName.HideHomeCanvas);
        // 地形生成
        SEvent.Instance.TriggerEvent(EventName.GenerateHexPlatform);
        // 触发玩家生成事件
        SEvent.Instance.TriggerEvent(EventName.SpawnPlayer);

        SLog.Info("Game started.");
    }

    private void RestartGame()
    {
        SLog.Info("Game Restarted.");
    }

}