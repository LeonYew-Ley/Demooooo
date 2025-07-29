using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 便于触发事件的扩展类
/// </summary>
public static class SEventTriggerExt
{
    /// <summary>
    /// 触发事件（无参数）
    /// </summary>
    /// <param name="sender">触发源</param>
    /// <param name="eventName">事件名</param>
    public static void TriggerEvent(this object sender, string eventName)
    {
        SEvent.Instance.TriggerEvent(eventName);
    }
    /// <summary>
    /// 触发事件（1个参数）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sender">触发源</param>
    /// <param name="eventName">事件名</param>
    /// <param name="info">参数</param>
    public static void TriggerEvent<T>(this object sender, string eventName, T info)
    {
        SEvent.Instance.TriggerEvent(eventName, info);
    }
    /// <summary>
    /// 触发事件（2个参数）
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="sender">触发源</param>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">参数1</param>
    /// <param name="info2">参数2</param>
    public static void TriggerEvent<T1, T2>(this object sender, string eventName, T1 info1, T2 info2)
    {
        SEvent.Instance.TriggerEvent(eventName, info1, info2);
    }
    /// <summary>
    /// 触发事件（3个参数）
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="sender">触发源</param>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">参数1</param>
    /// <param name="info2">参数2</param>
    /// <param name="info3">参数3</param>
    public static void TriggerEvent<T1, T2, T3>(this object sender, string eventName, T1 info1, T2 info2, T3 info3)
    {
        SEvent.Instance.TriggerEvent(eventName, info1, info2, info3);
    }
    /// <summary>
    /// 触发事件（4个参数）
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <param name="sender">触发源</param>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">参数1</param>
    /// <param name="info2">参数2</param>
    /// <param name="info3">参数3</param>
    /// <param name="info4">参数4</param>
    public static void TriggerEvent<T1, T2, T3, T4>(this object sender, string eventName, T1 info1, T2 info2, T3 info3, T4 info4)
    {
        SEvent.Instance.TriggerEvent(eventName, info1, info2, info3, info4);
    }
}

