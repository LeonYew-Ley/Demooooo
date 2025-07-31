using UnityEngine;

public class PlayerPawn : MonoBehaviour
{
    [Header("相机设置")]
    public Camera cameraRef;
    [Header("移动设置")]
    public float runSpeed = 5f;
    [Header("Dash设置")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    [Header("跳跃设置")]
    public float jumpForce = 15f;
    public float gravityScale = 5f;
    public LayerMask groundLayer = 1;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    [Header("旋转设置")]
    public float turnSmoothSpeed = 10f;

    private Rigidbody rb;
    private bool isGrounded;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;
    private bool jumpRequested = false;
    private Vector2 moveInput;
    void OnEnable()
    {
        SEvent.Instance.AddListener(EventName.PlayerDead, OnPlayerDead);
    }
    void OnDisable()
    {
        SEvent.Instance.RemoveListener(EventName.PlayerDead, OnPlayerDead);
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 1f;
        Physics.gravity = new Vector3(0, -9.81f * gravityScale, 0);
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }
    }

    void Update()
    {
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
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
            HandleMovement();
            HandleJump();
        }
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void RequestJump()
    {
        jumpRequested = true;
    }

    public void RequestDash(Vector3 direction)
    {
        if (!isDashing && dashCooldownTimer <= 0)
        {
            dashDirection = direction.normalized;
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void HandleMovement()
    {
        float currentSpeed = runSpeed;
        Camera cam = cameraRef;
        if (cam == null)
        {
            // 查找同级目录下的Camera对象
            Transform parent = transform.parent;
            if (parent != null)
            {
                foreach (Transform sibling in parent)
                {
                    if (sibling != transform)
                    {
                        Camera siblingCam = sibling.GetComponent<Camera>();
                        if (siblingCam != null)
                        {
                            cam = siblingCam;
                            break;
                        }
                    }
                }
            }
        }
        if (cam != null)
        {
            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();
            Vector3 moveDirection = camForward * moveInput.y + camRight * moveInput.x;
            float inputMagnitude = Mathf.Clamp01(moveInput.magnitude);
            Vector3 movement = moveDirection * currentSpeed * inputMagnitude * Time.deltaTime;
            rb.MovePosition(rb.position + movement);
            if (inputMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, turnSmoothSpeed * Time.deltaTime));
            }
        }
    }

    void HandleJump()
    {
        if (isGrounded && jumpRequested)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            jumpRequested = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    void OnPlayerDead()
    {
        SLog.Info("玩家死亡，2s后销毁玩家对象");
        Destroy(gameObject, 2f);
    }
}
