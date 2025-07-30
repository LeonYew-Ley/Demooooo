using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [Header("格子预制体")]
    public GameObject hexCellPrefab; // 正六边形格子的预制体

    [Header("蜂窝参数")]
    public int edgeLength = 7; // 每条边的格子数
    [Header("多层平台生成位置（按顺序）")]
    public Transform[] genPosList; // 多层平台生成位置
    [HideInInspector]
    public float cellSize = 1f; // 自动获取格子边长（外接圆半径）

    [Header("六边形朝向开关 (true=平顶, false=尖顶)")]
    public bool isFlatTopped = false; // 控制六边形朝向

    // [Header("生成父物体")]
    // public Transform parentTransform; // 已废弃，不再使用

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
    }

    void OnDisable()
    {
        // 移除监听
        SEvent.Instance.RemoveListener(EventName.GenerateHexPlatform, GenerateHexPlatform);
        SEvent.Instance.RemoveListener(EventName.DestroyPlatform, DestroyPlatform);
    }

    // 生成蜂窝地板
    void GenerateHexPlatform()
    {
        SLog.Hello();
        // 参考: https://www.redblobgames.com/grids/hexagons/
        int N = edgeLength;
        if (genPosList == null || genPosList.Length == 0)
        {
            Debug.LogWarning("PlatformGenerate: genPosList 为空，未生成平台");
            return;
        }
        foreach (var startPos in genPosList)
        {
            for (int q = -N + 1; q <= N - 1; q++)
            {
                for (int r = -N + 1; r <= N - 1; r++)
                {
                    int s = -q - r;
                    // 满足大六边形范围条件
                    if (Mathf.Max(Mathf.Abs(q), Mathf.Abs(r), Mathf.Abs(s)) < N)
                    {
                        Vector3 pos = HexToWorld(q, r, cellSize) + startPos.position;
                        Instantiate(hexCellPrefab, pos, Quaternion.identity, startPos);
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
        foreach (var startPos in genPosList)
        {
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
    }
}
