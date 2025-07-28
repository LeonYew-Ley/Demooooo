using UnityEngine;

public class PlatformGenerate : MonoBehaviour
{
    [Header("格子预制体")]
    public GameObject hexCellPrefab; // 正六边形格子的预制体

    [Header("蜂窝参数")]
    public int edgeLength = 7; // 每条边的格子数
    public Vector3 startPosition = Vector3.zero; // 起始位置
    [HideInInspector]
    public float cellSize = 1f; // 自动获取格子边长（外接圆半径）

    [Header("生成父物体")]
    public Transform parentTransform; // 指定生成的父物体

    void Start()
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
        GenerateHexPlatform();
    }

    // 生成蜂窝地板
    void GenerateHexPlatform()
    {
        // 参考: https://www.redblobgames.com/grids/hexagons/
        int N = edgeLength;
        Transform parent = parentTransform != null ? parentTransform : transform;
        for (int q = -N + 1; q <= N - 1; q++)
        {
            for (int r = -N + 1; r <= N - 1; r++)
            {
                int s = -q - r;
                // 满足大六边形范围条件
                if (Mathf.Max(Mathf.Abs(q), Mathf.Abs(r), Mathf.Abs(s)) < N)
                {
                    Vector3 pos = HexToWorld(q, r, cellSize) + startPosition;
                    Instantiate(hexCellPrefab, pos, Quaternion.identity, parent);
                }
            }
        }
    }

    // 轴向坐标转世界坐标
    Vector3 HexToWorld(int q, int r, float size)
    {
        // 紧密排布：size为六边形外接圆半径
        // x轴间距: size * 1.5f
        // z轴间距: size * sqrt(3) * (q + r/2)
        float x = size * 1.5f * q;
        float z = size * Mathf.Sqrt(3f) * (r + q / 2f);
        return new Vector3(x, 0, z);
    }
}
