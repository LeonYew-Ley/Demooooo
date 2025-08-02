using UnityEngine;
using System.Collections;

public class PlatformManager : MonoBehaviour
{
    [Header("平台对象")]
    public Transform platformObj; // 平台对象
    public Transform RotateCenterObj; // 平台旋转中心对象（可选）
    [Header("格子预制体")]
    public GameObject hexCellPrefab; // 正六边形格子的预制体

    [Header("蜂窝参数")]
    public int edgeLength = 7; // 每条边的格子数
    [System.Serializable]
    public class PlatformGenInfo
    {
        public Transform position; // 平台生成位置
        public int edgeLength = 7; // 该层边长
    }
    [Header("多层平台生成参数（按顺序）")]
    public PlatformGenInfo[] genPosList; // 多层平台生成参数
    [Header("层高设置")]
    public float layerHeight = 13f; // 每一层之间的高度间距
    [HideInInspector]
    public float cellSize = 1f; // 自动获取格子边长（外接圆半径）    [Header("六边形朝向开关 (true=平顶, false=尖顶)")]
    public bool isFlatTopped = false; // 控制六边形朝向

    [Header("平台翻转设置")]
    public float flipDuration = 1f; // 旋转时长（秒）

    void OnEnable()
    {
        // 自动获取cellSize（正六边形边长=模型宽度/2）
        if (hexCellPrefab != null)
        {
            MeshRenderer mr = hexCellPrefab.GetComponentInChildren<MeshRenderer>();
            if (mr != null)
            {
                float width = mr.bounds.size.x;
                cellSize = width / 2f;
            }
        }

        // 监听生成平台事件
        SEvent.Instance.AddListener(EventName.GenerateHexPlatform, GenerateHexPlatform);
        SEvent.Instance.AddListener(EventName.DestroyPlatform, DestroyPlatform);
        SEvent.Instance.AddListener(EventName.FlipPlatform, FlipPlatform);
    }

    void OnDisable()
    {
        // 移除监听
        SEvent.Instance.RemoveListener(EventName.GenerateHexPlatform, GenerateHexPlatform);
        SEvent.Instance.RemoveListener(EventName.DestroyPlatform, DestroyPlatform);
        SEvent.Instance.RemoveListener(EventName.FlipPlatform, FlipPlatform);
    }    // 生成蜂窝地板
    void GenerateHexPlatform()
    {
        SLog.Hello();
        // 参考: https://www.redblobgames.com/grids/hexagons/
        if (genPosList == null || genPosList.Length == 0)
        {
            Debug.LogWarning("PlatformGenerate: genPosList 为空，未生成平台");
            return;
        }

        for (int layerIndex = 0; layerIndex < genPosList.Length; layerIndex++)
        {
            var info = genPosList[layerIndex];
            if (info == null || info.position == null) continue;

            // 计算当前层的高度 (从y=0开始，每层递增layerHeight)
            float currentLayerY = layerIndex * layerHeight;

            int N = info.edgeLength;
            for (int q = -N + 1; q <= N - 1; q++)
            {
                for (int r = -N + 1; r <= N - 1; r++)
                {
                    int s = -q - r;
                    // 满足大六边形范围条件
                    if (Mathf.Max(Mathf.Abs(q), Mathf.Abs(r), Mathf.Abs(s)) < N)
                    {
                        Vector3 hexPos = HexToWorld(q, r, cellSize);
                        Vector3 basePos = info.position.position;
                        // 设置Y坐标为计算的层高度
                        Vector3 pos = new Vector3(hexPos.x + basePos.x, currentLayerY, hexPos.z + basePos.z);
                        Instantiate(hexCellPrefab, pos, Quaternion.identity, info.position);
                    }
                }
            }
        }
        SLog.Info("Hexagonal platform generated successfully.");
    }
    // 清理每一层的格子
    public void DestroyPlatform()
    {
        if (genPosList == null || genPosList.Length == 0)
            return;
        foreach (var info in genPosList)
        {
            if (info == null || info.position == null) continue;
            var startPos = info.position;
            for (int i = startPos.childCount - 1; i >= 0; i--)
            {
                GameObject child = startPos.GetChild(i).gameObject;
                GameObject.DestroyImmediate(child);
            }
        }
    }
    // 轴向坐标转世界坐标
    Vector3 HexToWorld(int q, int r, float size)
    {
        // 平顶六边形
        if (isFlatTopped)
        {
            // x轴间距: size * sqrt(3) * (q + r/2)
            // z轴间距: size * 1.5f
            float x = size * Mathf.Sqrt(3f) * (q + r / 2f);
            float z = size * 1.5f * r;
            return new Vector3(x, 0, z);
        }
        else // 尖顶六边形
        {
            // x轴间距: size * 1.5f
            // z轴间距: size * sqrt(3) * (r + q/2)
            float x = size * 1.5f * q;
            float z = size * Mathf.Sqrt(3f) * (r + q / 2f);
            return new Vector3(x, 0, z);
        }
    }    // 倒转平台
    public void FlipPlatform()
    {
        if (platformObj == null)
        {
            SLog.Error("PlatformManager: platformObj is not assigned.");
            return;
        }
        this.TriggerEvent(EventName.OnRotation); // 触发开始旋转事件
        StartCoroutine(FlipPlatformCoroutine());
    }
    private IEnumerator FlipPlatformCoroutine()
    {
        if (platformObj.childCount == 0)
        {
            SLog.Warn("PlatformManager: No child objects found under platformObj.");
            yield break;
        }

        // 计算平台整体的中心点
        Vector3 center = Vector3.zero;
        int childCount = 0;
        foreach (Transform child in platformObj)
        {
            center += child.position;
            childCount++;
        }
        center /= childCount;

        // 创建临时的旋转中心点
        GameObject tempPivot = new GameObject("TempRotationPivot");
        tempPivot.transform.position = center;

        // 将所有子物体临时设为这个旋转中心的子物体
        Transform[] originalChildren = new Transform[platformObj.childCount];
        for (int i = 0; i < platformObj.childCount; i++)
        {
            originalChildren[i] = platformObj.GetChild(i);
        }

        foreach (Transform child in originalChildren)
        {
            child.SetParent(tempPivot.transform);
        }

        // 执行整体旋转动画
        float elapsedTime = 0f;
        Quaternion initialRotation = tempPivot.transform.rotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(180f, 0, 0);

        while (elapsedTime < flipDuration)
        {
            float t = elapsedTime / flipDuration;
            // 使用平滑插值
            t = Mathf.SmoothStep(0f, 1f, t);

            tempPivot.transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终旋转精确
        tempPivot.transform.rotation = targetRotation;

        // 将子物体重新设回原来的父物体
        foreach (Transform child in originalChildren)
        {
            if (child != null)
            {
                child.SetParent(platformObj);
            }
        }

        // 销毁临时对象
        DestroyImmediate(tempPivot);
        this.TriggerEvent(EventName.EndRotation); // 触发结束旋转事件

        SLog.Info("Platform vertically flipped around collective center of mass.");
    }
}
