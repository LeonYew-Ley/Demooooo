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
        // 启用玩家操控事件
        SEvent.Instance.TriggerEvent(EventName.SpawnPlayer);
        // 显示倒计时界面
        SEvent.Instance.TriggerEvent(EventName.ShowCountDownCanvas);

        SLog.Info("Game started.");
    }

    private void RestartGame()
    {
        // 销毁场景
        this.TriggerEvent(EventName.DestroyPlatform);
        // TODO:销毁玩家&道具卡
        // 回到主界面
        this.TriggerEvent(EventName.ShowHomeCanvas);
        this.TriggerEvent(EventName.HideGameOver);
        SLog.Info("Game Restarted.");
    }

}