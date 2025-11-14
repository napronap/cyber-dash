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
        private bool dashPressed;

        private Rigidbody2D rb;
        private float gravityDefault;
        private int extraJumps;
        private float dashTimer;
        private float dashCooldownTimer;
        private Vector2 dashDirection;
        private bool hasDashedInAir;

        private bool canDash;
        // can dash はなに？
        // 地面に着地するとき
        //　ダッシュが終わったとき/ダッシュしていない状況


        private void Awake()
        {
            input = new PlayerInputActions();

            input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

            input.Player.Jump.performed += _ => jumpPressed = true;
            input.Player.Dash.performed += _ => dashPressed = true;
            canDash = true;
            
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
        }

        private void FixedUpdate()
        {
            // ---- GROUND CHECK ----
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

            // ---- DASHING ----
            // effect
            if (isDashing)
            {
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
            if (rb.linearVelocity.y < 0)
                rb.linearVelocity += Vector2.up *
                                     Physics2D.gravity.y * (fallMultiplier - 1) *
                                     Time.fixedDeltaTime;
        }

        private void Update()
        {
            Debug.Log($"dashcooldowntimer: {dashCooldownTimer}");
            Debug.Log($"dashtimer: {dashTimer}");
            // ---- DASH INPUT ----
            //if (!isDashing && dashCooldownTimer <= 0)
            if (canDash)
            {
                //if (dashPressed)
                if (dashPressed)
                {
                    canDash = false;
                    dashPressed = false;

                    Vector2 inputDir = moveInput.normalized;

                    // Eğer hiçbir yön tuşu basılı değilse → baktığı yöne dash
                    if (inputDir == Vector2.zero)
                        inputDir = transform.localScale.x > 0
                            ? Vector2.right
                            : Vector2.left;

                    dashDirection = inputDir;

                    StartDash();
                }
            }

            if (isDashing)
            {
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0)
                {
                    EndDash();
                }
            }



            dashCooldownTimer -= Time.deltaTime;

            // ---- JUMP INPUT ----
            if (jumpPressed)
            {
                jumpPressed = false;

                if (!isGrounded && extraJumps > 0)
                {
                    extraJumps--;
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
                }
                else if (isGrounded)
                {
                    extraJumps = extraJumpCount;
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
                }
            }

            if (dashCooldownTimer <= 0f && !isDashing)
            {
                canDash = true;
            }
        }

        private void StartDash()
        {
            isDashing = true;
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;

            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;

            PoolManager.instance.ReuseObject(dashEffect, transform.position, Quaternion.identity);

            if (!isGrounded)
                hasDashedInAir = true;
        }

        private void EndDash()
        {
            isDashing = false;
            rb.gravityScale = gravityDefault;
            rb.linearVelocity = Vector2.zero;

            // Yerdeyse cooldown bittikten sonra tekrar dash yapabilir
            // Havadaysa bir sonraki yere düşüşüne kadar beklemeli
            if (isGrounded)
            {
                canDash = true;
                hasDashedInAir = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
