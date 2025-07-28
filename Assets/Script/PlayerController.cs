using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float runSpeed = 5f;      // 默认奔跑速度
    public float sprintSpeed = 8f;   // 冲刺速度    [Header("跳跃设置")]
    public float jumpForce = 10f;
    public float gravityScale = 2f;  // 重力倍数，越大下落越快
    public LayerMask groundLayer = 1;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    [Header("输入设置")]
    public InputActionAsset inputActions;

    // 输入动作引用
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    // 私有变量
    private Rigidbody rb;
    private bool isGrounded;
    private Vector2 moveInput;
    private bool isSprinting; private bool jumpPressed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 设置更重的重力
        rb.mass = 1f;  // 确保质量为1
        Physics.gravity = new Vector3(0, -9.81f * gravityScale, 0);

        // 如果没有设置groundCheck，自动创建一个
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }

        // 初始化输入系统
        SetupInputActions();
    }
    void OnEnable()
    {
        // 启用输入动作
        if (moveAction != null) moveAction.Enable();
        if (jumpAction != null) jumpAction.Enable();
        if (sprintAction != null) sprintAction.Enable();
    }
    void OnDisable()
    {
        // 禁用输入动作
        if (moveAction != null) moveAction.Disable();
        if (jumpAction != null) jumpAction.Disable();
        if (sprintAction != null) sprintAction.Disable();
    }

    void SetupInputActions()
    {
        if (inputActions != null)
        {
            // 获取输入动作
            moveAction = inputActions.FindAction("Move");
            jumpAction = inputActions.FindAction("Jump");
            sprintAction = inputActions.FindAction("Sprint");

            // 绑定跳跃事件
            if (jumpAction != null)
            {
                jumpAction.performed += OnJump;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 获取输入
        GetInput();

        // 检查是否在地面上
        CheckGrounded();

        // 处理移动
        HandleMovement();

        // 处理跳跃
        HandleJump();
    }
    void GetInput()
    {
        // 获取移动输入
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }

        // 检查是否按下冲刺键
        if (sprintAction != null)
        {
            isSprinting = sprintAction.IsPressed();
        }
    }
    void CheckGrounded()
    {
        // 使用球形检测来判断是否在地面上（3D版本）
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }
    void HandleMovement()
    {
        // 确定当前速度（默认奔跑或冲刺）
        float currentSpeed = isSprinting ? sprintSpeed : runSpeed;
        // 3D移动：保持Y轴速度不变，更新X和Z轴
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 newVelocity = new Vector3(moveDirection.x * currentSpeed, rb.linearVelocity.y, moveDirection.z * currentSpeed);
        rb.linearVelocity = newVelocity;

        // 翻转角色朝向（基于X轴移动）
        if (moveInput.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
    void HandleJump()
    {
        // 只有在地面上且按下跳跃键时才能跳跃
        if (isGrounded && jumpPressed)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            jumpPressed = false; // 重置跳跃标志
        }
    }

    // 跳跃输入回调
    private void OnJump(InputAction.CallbackContext context)
    {
        jumpPressed = true;
    }

    void OnDestroy()
    {
        // 清理输入事件绑定
        if (jumpAction != null)
        {
            jumpAction.performed -= OnJump;
        }
    }

    // 在Scene视图中显示地面检测的可视化
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
