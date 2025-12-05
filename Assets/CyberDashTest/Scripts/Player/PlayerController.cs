using CyberDash;
using UnityEngine;


namespace CyberDash
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
        [SerializeField] private GameObject jumpEffect;

        [Header("Dashing")]
        [SerializeField] private float dashSpeed = 20f;
        [SerializeField] private float dashDuration = 0.15f;
        [SerializeField] private float dashCooldown = 0.25f;
        [SerializeField] private float dashCollisionCheckDistance = 0.3f;
        [SerializeField] private GameObject dashEffect;

        [HideInInspector] public bool isGrounded;
        [HideInInspector] public bool isDashing;

        private PlayerInputActions input;
        private Vector2 moveInput;
        private bool jumpPressed;

        private Rigidbody2D rb;
        private float gravityDefault;

        private float dashTimer;
        private float dashCooldownTimer;
        private Vector2 dashDirection;
        private bool hasDashedInAir;
        private bool canDash;

        private bool jumpLocked;
        private float jumpLockTimer;

        // -------------------------------------------------------
        // EFFECT SPAWNER  ★ EFFECTS AUTO-CLEAN
        // -------------------------------------------------------
        private void SpawnEffect(GameObject fxPrefab, Vector3 pos, float life = 0.5f)
        {
            if (fxPrefab == null)
                return;

            GameObject fx = Instantiate(fxPrefab, pos, Quaternion.identity);
            Destroy(fx, life);   // Auto delete clone after X seconds
        }

        private void Awake()
        {
            input = new PlayerInputActions();

            input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            input.Player.Move.canceled += _ => moveInput = Vector2.zero;

            input.Player.Jump.performed += _ =>
            {
                if (!jumpLocked)
                    jumpPressed = true;
            };
        }

        private void OnEnable() => input.Enable();
        private void OnDisable() => input.Disable();

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            gravityDefault = rb.gravityScale;

            // Remove these if not needed for testing
            SpawnEffect(jumpEffect, groundCheck.position, 0.5f);
            SpawnEffect(dashEffect, transform.position, 0.5f);

            canDash = true;
        }

        private void FixedUpdate()
        {
            HandleGroundCheck();

            if (isDashing)
            {
                HandleDashMovement();
                return;
            }

            HandleMovement();
            HandleFastFall();


        }

        private void Update()
        {
            UpdateJumpLock();
            HandleDashInput();
            UpdateDashTimer();
            UpdateDashCooldown();
            HandleJumpInput();
        }

        // ---------------------------------------------
        // MOVEMENT
        // ---------------------------------------------
        private void HandleMovement()
        {
            rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
        }

        private void HandleFastFall()
        {
            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
        }

        // ---------------------------------------------
        // GROUND CHECK
        // ---------------------------------------------
        private void HandleGroundCheck()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

            if (isGrounded)
            {
                hasDashedInAir = false;
                jumpLocked = false;
            }
        }

        // ---------------------------------------------
        // JUMP
        // ---------------------------------------------
        private void HandleJumpInput()
        {
            if (!jumpPressed || jumpLocked || isDashing)
                return;

            jumpPressed = false;

            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                SpawnEffect(jumpEffect, groundCheck.position, 0.5f);  // AUTO CLEAN
            }
        }

        // ---------------------------------------------
        // DASH INPUT
        // ---------------------------------------------
        private void HandleDashInput()
        {
            if (!input.Player.Dash.triggered || !canDash || isDashing || jumpLocked)
                return;

            if (isGrounded || (!isGrounded && !hasDashedInAir))
            {
                Vector2 dir = moveInput == Vector2.zero
                    ? new Vector2(transform.localScale.x, 0).normalized
                    : moveInput.normalized;

                dashDirection = dir;
                StartDash();
            }
        }

        // ---------------------------------------------
        // DASH MOVEMENT
        // ---------------------------------------------
        private void HandleDashMovement()
        {
            var hit = Physics2D.Raycast(transform.position, dashDirection, dashCollisionCheckDistance, whatIsGround);
            if (hit.collider != null)
            {
                EndDash();
                return;
            }

            rb.linearVelocity = dashDirection * dashSpeed;
        }

        // ---------------------------------------------
        // DASH STATE
        // ---------------------------------------------
        private void StartDash()
        {
            isDashing = true;
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;

            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
            canDash = false;

            jumpLocked = true;
            jumpLockTimer = dashDuration + 0.2f;

            SpawnEffect(dashEffect, transform.position, 0.5f);  // AUTO CLEAN

            if (!isGrounded)
                hasDashedInAir = true;
        }

        private void EndDash()
        {
            isDashing = false;
            rb.gravityScale = gravityDefault;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            jumpLocked = true;
            jumpLockTimer = 0.3f;
        }

        private void UpdateDashTimer()
        {
            if (!isDashing)
                return;

            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
                EndDash();
        }

        private void UpdateDashCooldown()
        {
            dashCooldownTimer -= Time.deltaTime;

            if (dashCooldownTimer <= 0f && isGrounded && !isDashing)
                canDash = true;
        }

        // ---------------------------------------------
        // JUMP LOCK
        // ---------------------------------------------
        private void UpdateJumpLock()
        {
            if (!jumpLocked)
                return;

            jumpLockTimer -= Time.deltaTime;
            if (jumpLockTimer <= 0)
                jumpLocked = false;
        }

        // ---------------------------------------------
        // DASH REFRESH TRIGGER
        // ---------------------------------------------
        private void RefreshDash()
        {
            canDash = true;
            hasDashedInAir = false;
            dashCooldownTimer = 0f;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("EnemyTrigger"))
                RefreshDash();
        }

        // ---------------------------------------------
        // GIZMOS
        // ---------------------------------------------
        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

            if (Application.isPlaying && isDashing)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, dashDirection * dashCollisionCheckDistance);
            }
        }
    }
}