using UnityEngine;

using TMPro;

public class CountDownUI : MonoBehaviour
{
    private TMP_Text tmpText;
    public float interval = 1f;

    void Awake()
    {
        tmpText = GetComponentInChildren<TMP_Text>();
        if (tmpText == null)
        {
            Debug.LogWarning("CountDownUI: 未找到TMP_Text组件");
        }
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(CountDownCoroutine());
    }

    private System.Collections.IEnumerator CountDownCoroutine()
    {
        if (tmpText == null) yield break;
        string[] steps = { "3", "2", "1", "Go!" };
        for (int i = 0; i < steps.Length; i++)
        {
            tmpText.text = steps[i];
            yield return new WaitForSeconds(interval);
        }
        // 触发关闭Canvas事件（假设事件名为 EventName.CloseCountDownCanvas）
        this.TriggerEvent(EventName.HideCountDownCanvas);
        // 开始游戏
        SEvent.Instance.TriggerEvent(EventName.GameStart); // 开启玩家重力
    }
}
