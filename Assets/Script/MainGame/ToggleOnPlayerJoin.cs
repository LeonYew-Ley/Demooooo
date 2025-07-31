using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleOnPlayerJoin : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += ToggleThis;
    }
    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= ToggleThis;
    }
    private void ToggleThis(PlayerInput playerInput)
    {
        gameObject.SetActive(false);
    }
}
