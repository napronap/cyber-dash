using UnityEngine;

namespace SupanthaPaul
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float speed = 8f;

        [Header("Jumping")]
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float fallMultiplier = 2f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private int extraJumpCount = 1;
        [SerializeField] private GameObject jumpEffect;

        [Header("Dashing")]
        [SerializeField] private float dashSpeed = 20f;
        [SerializeField] private float dashDuration = 0.15f;
        [SerializeField] private float dashCooldown = 0.25f;
        [SerializeField] private float dashCollisionCheckDistance = 0.3f;
        [SerializeField] private GameObject dashEffect;

        [HideInInspector] public bool isGrounded;
        [HideInInspector] public bool isDashing;

        // ---- NEW INPUT SYSTEM ----
        private PlayerInputActions input;
        private Vector2 moveInput;
        private bool jumpPressed;

        private Rigidbody2D rb;
        private float gravityDefault;
        private int extraJumps;
        private float dashTimer;
        private float dashCooldownTimer;
        private Vector2 dashDirection;
        private bool hasDashedInAir;
        private bool canDash;

        // Jump lock system to prevent double jumping after dash
        private bool jumpLocked;
        private float jumpLockTimer;

        private void Awake()
        {
            input = new PlayerInputActions();

            input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            input.Player.Move.canceled += ctx => moveInput = Vector2.zero;
            input.Player.Jump.performed += _ =>
            {
                if (!jumpLocked) // Check jump lock
                    jumpPressed = true;
            };
        }

        private void OnEnable() => input.Enable();
        private void OnDisable() => input.Disable();

        private void Start()
        {
            PoolManager.instance.CreatePool(jumpEffect, 3);
            PoolManager.instance.CreatePool(dashEffect, 3);

            rb = GetComponent<Rigidbody2D>();
            gravityDefault = rb.gravityScale;
            extraJumps = extraJumpCount;
            canDash = true;
        }

        private void FixedUpdate()
        {
            // ---- GROUND CHECK ----
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

            // Reset dash ability when landing
            if (isGrounded)
            {
                hasDashedInAir = false;
                jumpLocked = false; // Remove jump lock when grounded
            }

            // ---- DASHING ----
            if (isDashing)
            {
                // Collision check during dash
                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position,
                    dashDirection,
                    dashCollisionCheckDistance,
                    whatIsGround
                );

                if (hit.collider != null)
                {
                    EndDash();
                    return;
                }

                rb.linearVelocity = dashDirection * dashSpeed;
                return;
            }

            // ---- NORMAL MOVEMENT ----
            rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);

            // ---- FAST FALL ----
            if (rb.linearVelocity.y < 0 && !isDashing)
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
        }

        private void Update()
        {
            // Debug info
            Debug.Log($"canDash: {canDash}, hasDashedInAir: {hasDashedInAir}, jumpLocked: {jumpLocked}");

            // Jump lock timer countdown
            if (jumpLocked)
            {
                jumpLockTimer -= Time.deltaTime;
                if (jumpLockTimer <= 0f)
                {
                    jumpLocked = false;
                }
            }

            // ---- DASH INPUT ----
            if (input.Player.Dash.triggered && canDash && !isDashing && !jumpLocked)
            {
                // Dash conditions
                if (isGrounded || (!isGrounded && !hasDashedInAir))
                {
                    Vector2 inputDir = moveInput.normalized;

                    // If no input direction, dash in facing direction
                    if (inputDir == Vector2.zero)
                    {
                        inputDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
                    }

                    dashDirection = inputDir;
                    StartDash();
                }
            }

            // ---- DASH TIMER ----
            if (isDashing)
            {
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0)
                {
                    EndDash();
                }
            }

            // ---- DASH COOLDOWN ----
            dashCooldownTimer -= Time.deltaTime;

            // Can dash again when cooldown is over and grounded
            if (dashCooldownTimer <= 0f && isGrounded && !isDashing)
            {
                canDash = true;
            }

            // ---- JUMP INPUT ----
            if (jumpPressed && !jumpLocked && !isDashing) // All checks here
            {
                jumpPressed = false;

                if (!isGrounded && extraJumps > 0)
                {
                    // Extra jump in air
                    extraJumps--;
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
                }
                else if (isGrounded)
                {
                    // Normal ground jump
                    extraJumps = extraJumpCount;
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
                }
            }
        }

        private void StartDash()
        {
            isDashing = true;
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;

            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
            canDash = false;

            // Lock jumping when dash starts
            jumpLocked = true;
            jumpLockTimer = dashDuration + 0.2f; // Dash duration + extra safety

            PoolManager.instance.ReuseObject(dashEffect, transform.position, Quaternion.identity);

            // Mark if dashed in air
            if (!isGrounded)
            {
                hasDashedInAir = true;
            }
        }

        private void EndDash()
        {
            isDashing = false;
            rb.gravityScale = gravityDefault;

            // Smooth transition - keep horizontal velocity, reset vertical
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            // Keep jump locked for a short time after dash ends
            jumpLocked = true;
            jumpLockTimer = 0.3f; // Cannot jump for 0.3 more seconds
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

            // Debug line for dash direction
            if (Application.isPlaying && isDashing)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, dashDirection * dashCollisionCheckDistance);
            }
        }
    }
}