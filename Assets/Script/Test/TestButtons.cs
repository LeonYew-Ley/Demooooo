using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TestBtnName
{
    ReversePlatform,
}
public class TestButtons : MonoBehaviour
{
    public List<Button> buttons; // 按钮列表

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 添加按钮点击事件监听
        buttons[(int)TestBtnName.ReversePlatform].onClick.AddListener(OnReversePlatformBtnClick);
    }

    public void OnReversePlatformBtnClick()
    {
        SLog.Info("Reverse Platform Button Clicked.");
        // 触发事件，反转平台
        SEvent.Instance.TriggerEvent(EventName.FlipPlatform);
    }
}
