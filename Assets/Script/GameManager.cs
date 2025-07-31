using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void OnEnable()
    {
        SEvent.Instance.AddListener(EventName.AllPlayerDead, OnPlayerDead);
        SEvent.Instance.AddListener(EventName.GameStart, StartGame);
        SEvent.Instance.AddListener(EventName.GameRestart, RestartGame);
    }

    private void Start()
    {
        InitializeApplication();
    }
    void OnDisable()
    {
        SEvent.Instance.RemoveListener(EventName.AllPlayerDead, OnPlayerDead);
        SEvent.Instance.RemoveListener(EventName.GameStart, StartGame);
        SEvent.Instance.RemoveListener(EventName.GameRestart, RestartGame);
    }
    private void InitializeApplication()
    {
        SLog.Info("Application started.");
    }

    private void OnPlayerDead()
    {
        SLog.Info($"Open Dead Canvas");
        SEvent.Instance.TriggerEvent(EventName.ShowGameOver);
    }

    private void StartGame()
    {
        // 加载主场景
        SEvent.Instance.TriggerEvent(EventName.HideHomeCanvas);
        SEvent.Instance.TriggerEvent(EventName.GenerateHexPlatform);
        SEvent.Instance.TriggerEvent(EventName.SpawnPlayer);
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