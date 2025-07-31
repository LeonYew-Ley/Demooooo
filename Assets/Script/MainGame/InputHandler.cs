using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour, AxisState.IInputAxisProvider
{
    [HideInInspector]
    public InputAction horizontal;
    [HideInInspector]
    public InputAction vertical;
    public float GetAxisValue(int axis)
    {
        switch (axis)
        {
            case 0: return horizontal.ReadValue<Vector2>().x; // 水平轴
            case 1: return vertical.ReadValue<Vector2>().y; // 垂直
            case 2: return horizontal.ReadValue<float>();
        }
        return 0f;
    }
}
