using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    void Start()
    {
        InitUI();
    }
    /// <summary>
    /// 初始化UI，只显示HomeCanvas，其他Canvas全部关闭
    /// </summary>
    public void InitUI()
    {
        for (int i = 0; i < canvasList.Length; i++)
        {
            if (i == (int)CanvasEnums.Home)
            {
                canvasList[i].SetActive(true);
            }
            else
            {
                canvasList[i].SetActive(false);
            }
        }
        SLog.Info("UI Initialized: Only HomeCanvas is active.");
    }
    public Button[] buttons;
    public GameObject[] canvasList;

    void OnEnable()
    {
        // 添加按钮点击事件监听
        buttons[(int)ButtonEnums.Start].onClick.AddListener(OnStartBtnClick);
        buttons[(int)ButtonEnums.Exit].onClick.AddListener(OnExitBtnClick);
        buttons[(int)ButtonEnums.Restart].onClick.AddListener(OnRestartBtnClick);
        // Canvas开闭事件
        SEvent.Instance.AddListener(EventName.ShowHomeCanvas, ShowHomeCanvas);
        SEvent.Instance.AddListener(EventName.HideHomeCanvas, HideHomeCanvas);
        SEvent.Instance.AddListener(EventName.ShowGameOver, ShowGameOverCanvas);
        SEvent.Instance.AddListener(EventName.HideGameOver, HideGameOverCanvas);
        SEvent.Instance.AddListener(EventName.ShowCountDownCanvas, ShowCountDownCanvas);
        SEvent.Instance.AddListener(EventName.HideCountDownCanvas, HideCountDownCanvas);
        SEvent.Instance.AddListener(EventName.ShowReadyToStartCanvas, ShowReadyToStartCanvas);
        SEvent.Instance.AddListener(EventName.HideReadyToStartCanvas, HideReadyToStartCanvas);
        SEvent.Instance.AddListener<int>(EventName.UpdateReadyToStartCanvas, UpdateReadyToStartCanvas);
    }
    void OnDisable()
    {
        SEvent.Instance.RemoveListener(EventName.ShowHomeCanvas, ShowHomeCanvas);
        SEvent.Instance.RemoveListener(EventName.HideHomeCanvas, HideHomeCanvas);
        SEvent.Instance.RemoveListener(EventName.ShowGameOver, ShowGameOverCanvas);
        SEvent.Instance.RemoveListener(EventName.HideGameOver, HideGameOverCanvas);
        SEvent.Instance.RemoveListener(EventName.ShowCountDownCanvas, ShowCountDownCanvas);
        SEvent.Instance.RemoveListener(EventName.HideCountDownCanvas, HideCountDownCanvas);
        SEvent.Instance.RemoveListener(EventName.ShowReadyToStartCanvas, ShowReadyToStartCanvas);
        SEvent.Instance.RemoveListener(EventName.HideReadyToStartCanvas, HideReadyToStartCanvas);
        SEvent.Instance.RemoveListener<int>(EventName.UpdateReadyToStartCanvas, UpdateReadyToStartCanvas);
    }
    public void ShowCanvas(CanvasEnums index)
    {
        if (index < 0 || (int)index >= canvasList.Length)
        {
            SLog.Error($"Invalid canvas index: {index}");
            return;
        }
        foreach (var canvas in canvasList)
        {
            canvas.SetActive(false);
        }
        canvasList[(int)index].SetActive(true);
        SLog.Info($"Showing canvas at index: {index}");
    }
    public void HideCanvas(CanvasEnums index)
    {
        if (index < 0 || (int)index >= canvasList.Length)
        {
            SLog.Error($"Invalid canvas index: {index}");
            return;
        }
        canvasList[(int)index].GetComponent<GradientShowCanvas>()?.HideAndDisable();
        SLog.Info($"Hiding canvas at index: {index}");
    }


    #region 按钮点击事件
    public void OnStartBtnClick()
    {
        SEvent.Instance.TriggerEvent(EventName.GameEnter);
    }
    public void OnExitBtnClick()
    {
        SLog.Info("Exit button clicked");
        Application.Quit();
    }
    public void OnRestartBtnClick()
    {
        SLog.Info("Restart button clicked");
        SEvent.Instance.TriggerEvent(EventName.GameRestart);
    }
    #endregion

    #region 各种Canvas显示关闭方法
    public void ShowGameOverCanvas()
    {
        ShowCanvas(CanvasEnums.GameOver);
        Invoke(nameof(TriggerToggleMainCamera), 1f);
    }
    private void TriggerToggleMainCamera()
    {
        SEvent.Instance.TriggerEvent(EventName.ToggleMainCamera);
    }

    public void HideGameOverCanvas() => HideCanvas(CanvasEnums.GameOver);
    public void ShowHomeCanvas() => ShowCanvas(CanvasEnums.Home);
    public void HideHomeCanvas() => HideCanvas(CanvasEnums.Home);
    public void ShowCountDownCanvas() => ShowCanvas(CanvasEnums.CountDown);
    public void HideCountDownCanvas() => HideCanvas(CanvasEnums.CountDown);
    public void ShowReadyToStartCanvas() => ShowCanvas(CanvasEnums.ReadyToStart);
    public void HideReadyToStartCanvas() => HideCanvas(CanvasEnums.ReadyToStart);
    public void UpdateReadyToStartCanvas(int playerNum)
    {
        var readyToStartCanvas = canvasList[(int)CanvasEnums.ReadyToStart];
        var titleTextObj = readyToStartCanvas.transform.Find("bg/VerticalLayout/TitleText");
        var textComponent = titleTextObj != null ? titleTextObj.GetComponent<TMPro.TextMeshProUGUI>() : null;
        if (textComponent != null && playerNum == -1) // 玩家已满，显示准备开始文字
        {
            textComponent.text = $"即将开始游戏..";
            SLog.Info($"Updated ReadyToStartCanvas text: {textComponent.text}");
            return;
        }
        else if (textComponent != null && playerNum >= 0) // 更新玩家数量
        {
            textComponent.text = $"等待玩家加入.. ({playerNum}/2)";
            SLog.Info($"Updated ReadyToStartCanvas text: {textComponent.text}");
        }
        else
        {
            SLog.Error("Text component not found in ReadyToStartCanvas");
        }
    }
    #endregion

}
