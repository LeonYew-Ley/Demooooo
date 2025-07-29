using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject[] canvasList;

    void OnEnable()
    {
        SEvent.Instance.AddListener(EventName.OpenGameOverCanvas, ShowGameOverCanvas);
    }
    void OnDisable()
    {
        SEvent.Instance.RemoveListener(EventName.OpenGameOverCanvas, ShowGameOverCanvas);
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
        canvasList[(int)index].SetActive(false);
        SLog.Info($"Hiding canvas at index: {index}");
    }

    #region 各种Canvas显示关闭方法
    public void ShowGameOverCanvas() => ShowCanvas(CanvasEnums.GameOver);
    #endregion

}
