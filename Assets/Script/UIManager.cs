using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
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
    }
    void OnDisable()
    {
        SEvent.Instance.RemoveListener(EventName.ShowHomeCanvas, ShowHomeCanvas);
        SEvent.Instance.RemoveListener(EventName.HideHomeCanvas, HideHomeCanvas);
        SEvent.Instance.RemoveListener(EventName.ShowGameOver, ShowGameOverCanvas);
        SEvent.Instance.RemoveListener(EventName.HideGameOver, HideGameOverCanvas);
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
        SEvent.Instance.TriggerEvent(EventName.GameStart);
    }
    public void OnExitBtnClick()
    {
        SLog.Info("Exit button clicked");
        Application.Quit();
    }
    public void OnRestartBtnClick()
    {
        SLog.Info("Restart button clicked");
    }
    #endregion

    #region 各种Canvas显示关闭方法
    public void ShowGameOverCanvas() => ShowCanvas(CanvasEnums.GameOver);
    public void HideGameOverCanvas() => HideCanvas(CanvasEnums.GameOver);
    public void ShowHomeCanvas() => ShowCanvas(CanvasEnums.GameOver);
    public void HideHomeCanvas() => HideCanvas(CanvasEnums.Home);
    #endregion

}
