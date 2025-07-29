using UnityEngine;

public class DrawGizmosSphere : MonoBehaviour
{
    // 设置小球的颜色和半径
    public Color gizmoColor = Color.red; // Gizmos 的颜色
    public float gizmoRadius = 0.5f;     // Gizmos 小球的半径

    // Unity 的 Gizmos 绘制方法
    private void OnDrawGizmos()
    {
        // 设置 Gizmos 的颜色
        Gizmos.color = gizmoColor;

        // 在脚本对象的 Transform 位置绘制一个小球
        Gizmos.DrawSphere(transform.position, gizmoRadius);
    }
}