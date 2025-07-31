using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;


// This class receives input from a PlayerInput component and disptaches it
// to the appropriate Cinemachine InputAxis.  The playerInput component should
// be on the same GameObject, or specified in the PlayerInput field.
class CustomInputHandler : InputAxisControllerBase<CustomInputHandler.Reader>
{
    [Header("Input Source Override")]
    private PlayerInput playerInput;

    void Awake()
    {
        // When the PlayerInput receives an input, send it to all the controllers
        Transform playerControllerObj = transform.parent; // PlayerController
        playerInput = playerControllerObj.GetComponent<PlayerInput>();
        if (playerInput == null)
            Debug.LogError("Cannot find PlayerInput component");
        else
        {
            playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            playerInput.onActionTriggered += (value) =>
            {
                for (var i = 0; i < Controllers.Count; i++)
                    Controllers[i].Input.ProcessInput(value.action);
            };
        }
    }

    // We process user input on the Update clock
    void Update()
    {
        if (Application.isPlaying)
            UpdateControllers();
    }

    // Controllers will be instances of this class.
    [Serializable]
    public class Reader : IInputAxisReader
    {
        public InputActionReference Input;
        public float Sensitivity = 1.0f; // 灵敏度调节变量，默认为1.0，可在Inspector中调整
        Vector2 m_Value; // the cached value of the input

        public void ProcessInput(InputAction action)
        {
            // If it's my action then cache the new value
            if (Input != null && Input.action.id == action.id)
            {
                if (action.expectedControlType == "Vector2")
                    m_Value = action.ReadValue<Vector2>();
                else
                    m_Value.x = m_Value.y = action.ReadValue<float>();
            }
        }

        // IInputAxisReader interface: Called by the framework to read the input value
        public float GetValue(UnityEngine.Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
        {
            // 乘以灵敏度
            return (hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? m_Value.y : m_Value.x) * Sensitivity;
        }
    }
}