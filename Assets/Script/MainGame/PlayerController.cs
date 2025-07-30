using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float runSpeed = 5f;      // 默认奔跑速度
    [Header("Dash设置")]
    public float dashForce = 20f;    // Dash冲刺力度
    public float dashDuration = 0.2f; // Dash持续时间
    public float dashCooldown = 1f;   // Dash冷却时间
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;
    [Header("跳跃设置")]
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
    private InputAction dashAction;

    // 私有变量
    private Rigidbody rb;
    private bool isGrounded;
    private Vector2 moveInput;
    private bool jumpPressed;

    [Header("旋转设置")]
    public float turnSmoothSpeed = 10f; // 丝滑旋转速度

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
    }
    void OnEnable()
    {
        // 初始化输入系统（每次启用都重新绑定，防止实例化后失效）
        SetupInputActions();
        if (moveAction != null) moveAction.Enable();
        if (jumpAction != null) jumpAction.Enable();
        if (dashAction != null) dashAction.Enable();
        // 监听玩家死亡事件
        SEvent.Instance.AddListener(EventName.PlayerDead, OnPlayerDead);
    }
    void OnDisable()
    {
        // 禁用输入动作
        if (moveAction != null) moveAction.Disable();
        if (jumpAction != null) jumpAction.Disable();
        if (dashAction != null) dashAction.Disable();
        // 移除玩家死亡事件监听
        SEvent.Instance.RemoveListener(EventName.PlayerDead, OnPlayerDead);
    }

    void SetupInputActions()
    {
        if (inputActions != null)
        {
            // 获取输入动作
            moveAction = inputActions.FindAction("Move");
            jumpAction = inputActions.FindAction("Jump");
            dashAction = inputActions.FindAction("Dash");

            // 绑定跳跃事件
            if (jumpAction != null)
            {
                jumpAction.performed += OnJump;
            }
            // 绑定Dash事件
            if (dashAction != null)
            {
                dashAction.performed += OnDash;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 获取输入
        GetInput();

        // Dash冷却计时
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        // Dash持续计时
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); // 停止Dash的水平速度
            }
        }

        // 检查是否在地面上
        CheckGrounded();
    }
    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashForce;
        }
        else
        {
            // 处理移动
            HandleMovement();
            // 处理跳跃
            HandleJump();
        }
    }
    void GetInput()
    {
        // 获取移动输入
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }
    }
    void CheckGrounded()
    {
        // 使用球形检测来判断是否在地面上（3D版本）
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

    }
    void HandleMovement()
    {
        // 基于摄像机前方和右方进行移动，支持手柄摇杆幅度
        float currentSpeed = runSpeed;
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;
            // 忽略Y轴分量，保持在水平面
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();
            // 不归一化moveDirection，保留摇杆幅度
            Vector3 moveDirection = camForward * moveInput.y + camRight * moveInput.x;
            float inputMagnitude = Mathf.Clamp01(moveInput.magnitude); // 防止溢出
            Vector3 movement = moveDirection * currentSpeed * inputMagnitude * Time.deltaTime;
            rb.MovePosition(rb.position + movement);

            // 丝滑旋转角色朝向移动方向（使用Rigidbody避免抖动）
            // 只有输入幅度大于死区时才旋转，防止松开摇杆后角色还在转
            if (inputMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, turnSmoothSpeed * Time.deltaTime));
            }
        }
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
    // Dash冲刺输入回调
    void OnDash(InputAction.CallbackContext context)
    {
        if (!isDashing && dashCooldownTimer <= 0)
        {
            // Dash方向为当前移动方向或角色前方
            Camera cam = Camera.main;
            Vector3 camForward = cam != null ? cam.transform.forward : transform.forward;
            Vector3 camRight = cam != null ? cam.transform.right : transform.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();
            Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;
            if (moveDir.sqrMagnitude < 0.01f)
                moveDir = transform.forward; // 没有输入时向前Dash
            dashDirection = moveDir.normalized;
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
        }
    }

    // 跳跃输入回调
    void OnJump(InputAction.CallbackContext context)
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
        if (dashAction != null)
        {
            dashAction.performed -= OnDash;
        }
    }

    // 玩家死亡事件回调
    void OnPlayerDead()
    {
        // 这里可以禁用输入、播放死亡动画等
        SLog.Info("玩家死亡，2s后销毁玩家对象");
        // 两秒后销毁实例
        Destroy(gameObject, 2f);
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
