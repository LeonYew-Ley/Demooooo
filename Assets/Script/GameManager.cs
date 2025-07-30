using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        // 加载主场景
        // 检查场景是否已加载
        var scene = SceneManager.GetSceneByName("MainScene");
        if (!scene.isLoaded)
        {
            // 异步加载并在完成后触发事件
            var asyncOp = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive);
            asyncOp.completed += (op) =>
            {
                SEvent.Instance.TriggerEvent(EventName.HideHomeCanvas);
                SEvent.Instance.TriggerEvent(EventName.GenerateHexPlatform);
                SEvent.Instance.TriggerEvent(EventName.SpawnPlayer);
                SEvent.Instance.TriggerEvent(EventName.ShowCountDownCanvas);
            };
        }
        else
        {
            SEvent.Instance.TriggerEvent(EventName.HideHomeCanvas);
            SEvent.Instance.TriggerEvent(EventName.GenerateHexPlatform);
            SEvent.Instance.TriggerEvent(EventName.SpawnPlayer);
            SEvent.Instance.TriggerEvent(EventName.ShowCountDownCanvas);
        }
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
        // 销毁MainScene
        var scene = SceneManager.GetSceneByName("MainScene");
        if (scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync("MainScene");
        }
        SLog.Info("Game Restarted.");
    }

}