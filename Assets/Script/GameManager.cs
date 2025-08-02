using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("玩家控制器对象（控制加入）")]
    public PlayerInputManager playerInputMangaer; // 主界面Canvas
    [Header("UI相机")]
    public Camera uiCamera; // UI相机
    [Header("局内设置")]

    public GameObject AirWallObj; // 空气墙对象
    public float FlipInterval = 10f; // 平台翻转间隔时间
    private STimer flipTimer; // 平台翻转计时器
    void OnEnable()
    {
        SEvent.Instance.AddListener(EventName.AllPlayerDead, OnPlayerDead);
        SEvent.Instance.AddListener(EventName.GameEnter, EnterGame);
        SEvent.Instance.AddListener(EventName.GameCountDown, CountDownInGame);
        SEvent.Instance.AddListener(EventName.GameStart, StartGame);
        SEvent.Instance.AddListener(EventName.GameRestart, RestartGame);
        SEvent.Instance.AddListener(EventName.ToggleMainCamera, ToggleMainCamera);
    }

    private void Start()
    {
        InitializeApplication();
    }
    void OnDisable()
    {
        // 清理定时器
        if (flipTimer != null)
        {
            flipTimer.Dispose();
            flipTimer = null;
        }

        SEvent.Instance.RemoveListener(EventName.AllPlayerDead, OnPlayerDead);
        SEvent.Instance.RemoveListener(EventName.GameEnter, EnterGame);
        SEvent.Instance.RemoveListener(EventName.GameCountDown, CountDownInGame);
        SEvent.Instance.RemoveListener(EventName.GameStart, StartGame);
        SEvent.Instance.RemoveListener(EventName.GameRestart, RestartGame);
        SEvent.Instance.RemoveListener(EventName.ToggleMainCamera, ToggleMainCamera);
    }
    private void InitializeApplication()
    {
        // 暂时关闭玩家加入
        playerInputMangaer.DisableJoining();
        SLog.Info("Application started.");
    }

    private void OnPlayerDead()
    {
        SLog.Info($"Open Dead Canvas");
        SEvent.Instance.TriggerEvent(EventName.ShowGameOver);
    }

    public void EnterGame()
    {
        AirWallObj.SetActive(true); // 激活空气墙
        SLog.Info("Game Entered.");
        // 关闭主界面，打开ReadyToStartCanvas
        SEvent.Instance.TriggerEvent(EventName.HideHomeCanvas);
        SEvent.Instance.TriggerEvent(EventName.ShowReadyToStartCanvas);
        SEvent.Instance.TriggerEvent(EventName.GenerateHexPlatform);// 生成平台
        playerInputMangaer.EnableJoining(); // 启用玩家加入

    }

    private void CountDownInGame()
    {
        // 关闭ReadyToStartCanvas，开启倒计时
        SEvent.Instance.TriggerEvent(EventName.HideReadyToStartCanvas);
        SEvent.Instance.TriggerEvent(EventName.ShowCountDownCanvas);
        playerInputMangaer.DisableJoining(); // 禁用玩家加入
        SLog.Info("Game Starting.");
    }
    private void StartGame()
    {
        AirWallObj.SetActive(false);
        SLog.Info("Game Begin.");

        // 先清理旧的定时器（如果存在）
        if (flipTimer != null)
        {
            SLog.Info("Disposing existing flip timer.");
            flipTimer.Dispose();
            flipTimer = null;
        }

        // 开启场景翻转倒计时（循环）
        SLog.Info($"Creating flip timer with interval: {FlipInterval}s");
        flipTimer = new STimer(FlipInterval, OnPlatformFlipped, true);
        flipTimer.Start();
        SLog.Info("Flip timer started.");
    }

    private void OnPlatformFlipped()
    {
        SLog.Info("Platform Flipped.");
        SEvent.Instance.TriggerEvent(EventName.FlipPlatform);
    }

    private void RestartGame()
    {
        // 销毁场景
        this.TriggerEvent(EventName.DestroyPlatform);
        // 重置计时器
        flipTimer.Dispose();
        // TODO:销毁玩家&道具卡
        // 回到主界面
        this.TriggerEvent(EventName.ShowHomeCanvas);
        this.TriggerEvent(EventName.HideGameOver);

        SLog.Info("Game Restarted.");
    }

    private void ToggleMainCamera()
    {
        uiCamera.gameObject.SetActive(!uiCamera.gameObject.activeSelf);
    }

}