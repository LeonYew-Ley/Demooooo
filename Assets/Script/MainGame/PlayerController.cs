using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // [Header("输入设置")]
    private InputActionAsset inputAsset;

    [Header("Pawn设置")]
    public GameObject playerPawnObj; // 直接引用场景中的Pawn对象
    private PlayerPawn playerPawn;

    // 输入动作引用
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    void Awake()
    {
        // 获取PlayerInput实例化后的Actions
        inputAsset = GetComponent<PlayerInput>().actions;
    }
    void Start()
    {
        // 直接获取Pawn引用
        if (playerPawnObj != null)
        {
            playerPawn = playerPawnObj.GetComponent<PlayerPawn>();
        }
        else
        {
            Debug.LogError("PlayerPawnObj未设置");
        }
    }

    void OnEnable()
    {
        SetupInputActions();
        if (moveAction != null) moveAction.Enable();
        if (jumpAction != null) jumpAction.Enable();
        if (dashAction != null) dashAction.Enable();
        SEvent.Instance.AddListener(EventName.PlayerDead, OnPlayerDead);
    }
    void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (jumpAction != null) jumpAction.Disable();
        if (dashAction != null) dashAction.Disable();
        SEvent.Instance.RemoveListener(EventName.PlayerDead, OnPlayerDead);
    }

    void SetupInputActions()
    {
        if (inputAsset != null)
        {
            moveAction = inputAsset.FindAction("Move");
            jumpAction = inputAsset.FindAction("Jump");
            dashAction = inputAsset.FindAction("Dash");
            if (jumpAction != null)
            {
                jumpAction.performed += OnJump;
            }
            if (dashAction != null)
            {
                dashAction.performed += OnDash;
            }
        }
    }

    void Update()
    {
        if (playerPawn == null) return;
        // 获取移动输入并传递给Pawn
        if (moveAction != null)
        {
            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            playerPawn.SetMoveInput(moveInput);
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (playerPawn != null)
            playerPawn.RequestJump();
    }

    void OnDash(InputAction.CallbackContext context)
    {
        if (playerPawn != null)
        {
            // Dash方向为当前输入方向或角色前方
            Camera cam = Camera.main;
            Vector3 camForward = cam != null ? cam.transform.forward : playerPawn.transform.forward;
            Vector3 camRight = cam != null ? cam.transform.right : playerPawn.transform.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();
            Vector2 moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
            Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;
            if (moveDir.sqrMagnitude < 0.01f)
                moveDir = playerPawn.transform.forward;
            playerPawn.RequestDash(moveDir);
        }
    }

    void OnDestroy()
    {
        if (jumpAction != null)
        {
            jumpAction.performed -= OnJump;
        }
        if (dashAction != null)
        {
            dashAction.performed -= OnDash;
        }
    }

    void OnPlayerDead()
    {
        SLog.Info("玩家死亡，2s后销毁Controller");
        Destroy(gameObject, 2f);
    }
}
