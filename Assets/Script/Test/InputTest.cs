using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // InputSystem 键盘按键
        if (Keyboard.current == null)
        {
            Debug.LogWarning("Keyboard.current is null");
        }
        else
        {
            foreach (var key in Keyboard.current.allKeys)
            {
                if (key == null) continue;
                if (key.wasPressedThisFrame)
                    SLog.Info($"键盘按下: {key.name}");
                if (key.wasReleasedThisFrame)
                    SLog.Info($"键盘松开: {key.name}");
            }
        }

        // InputSystem 鼠标按键
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                SLog.Info("鼠标左键按下");
            if (Mouse.current.leftButton.wasReleasedThisFrame)
                SLog.Info("鼠标左键松开");
            if (Mouse.current.rightButton.wasPressedThisFrame)
                SLog.Info("鼠标右键按下");
            if (Mouse.current.rightButton.wasReleasedThisFrame)
                SLog.Info("鼠标右键松开");
            if (Mouse.current.middleButton.wasPressedThisFrame)
                SLog.Info("鼠标中键按下");
            if (Mouse.current.middleButton.wasReleasedThisFrame)
                SLog.Info("鼠标中键松开");
        }

        // InputSystem 手柄按键
        if (Gamepad.current != null)
        {
            foreach (var control in Gamepad.current.allControls)
            {
                if (control is ButtonControl btn)
                {
                    if (btn.wasPressedThisFrame)
                        SLog.Info($"手柄按下: {btn.name}");
                    if (btn.wasReleasedThisFrame)
                        SLog.Info($"手柄松开: {btn.name}");
                }
            }
        }
    }
}
