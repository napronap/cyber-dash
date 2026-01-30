using JetBrains.Annotations;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] private float movingSpeed = 5f;
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private float dashCooldownTime = 30.0f;
    [SerializeField] private float jumpForce = 20f;
    //[SerializeField] private float jumpTime = 0.2f;
    [SerializeField] private float groundLevel = -4f;
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float verticalDashMultiplier = 0.3f;

    private Rigidbody2D rb;
    private float _initialMovingSpeed;
    private bool isDashing;
    private bool isRunning;
    private bool isJumping;
    private bool isGrounded;
    private bool canDash;
    private bool dashStarted = false;
    private bool willDash = false;
    private bool isDashBackwards = false;
    private bool isDashUp = false;
    private bool isDashAnim = false;
    private bool jumpPressed = false;
    private bool canJump = true;
    private bool jumpCooldown = false;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        _initialMovingSpeed = movingSpeed;
        rb.gravityScale = gravityScale;
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerDash += GameInput_OnPlayerDash;
        GameInput.Instance.OnPlayerJump += GameInput_OnPlayerJump;
    }

    private void GameInput_OnPlayerDash(object sender, EventArgs e)
    {
        Dash();
    }

    private void GameInput_OnPlayerJump(object sender, EventArgs e)
    {
        if (canJump && !jumpCooldown)
        {
            jumpPressed = true;
        }
    }

    public void Jump()
    {
        if (isGrounded && canJump && !jumpCooldown)
        {
            isJumping = true;
            canJump = false;
            jumpCooldown = true;
            Vector2 inputVector = GameInput.Instance.GetMovementVector();

            float horizontalMultiplier = 0f;
            if (Mathf.Abs(inputVector.x) > 0.1f)
            {
                horizontalMultiplier = inputVector.x * 0.3f;
            }

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(new Vector2(horizontalMultiplier * jumpForce, jumpForce), ForceMode2D.Impulse);
            jumpPressed = false;

            //StartCoroutine(JumpCooldownRoutine());
            //canJump = true;
        }
    }

    private IEnumerator JumpCooldownRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        jumpCooldown = false;
    }

    private void Dash()
    {
        if (canDash)
        {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        willDash = true;
        isDashing = true;
        dashStarted = true;

        Vector2 inputVector = GameInput.Instance.GetMovementVector();

        float x = inputVector.x;
        float y = inputVector.y;

        bool hasHorizontalInput = Mathf.Abs(x) > 0.1f;
        bool isRight = x > 0.1f;
        bool isUp = y > 0.1f;

        bool longDash = isRight || (!hasHorizontalInput && isUp);

        if (!longDash)
        {
            isDashBackwards = true;
        }
        else if (isUp)
        {
            isDashUp = true;
        }
        else
        {
            isDashAnim = true;
        }

        movingSpeed *= longDash ? dashSpeed : dashSpeed / 2f;
        trailRenderer.emitting = true;

        float dashTimer = 0f;
        while (dashTimer < dashTime)
        {
            dashTimer += Time.deltaTime;
            yield return null;
        }

        willDash = false;

        trailRenderer.emitting = false;
        movingSpeed = _initialMovingSpeed;

        yield return new WaitForSeconds(dashCooldownTime);
        isDashing = false;
        isDashBackwards = false;
        isDashUp = false;
        isDashAnim = false;
    }

    void FixedUpdate()
    {
        Debug.Log(isGrounded);
        Debug.Log(rb.linearVelocityY);
        HandleMovement();

        isGrounded = CheckIsGrounded();
        CheckDash();

        if (isGrounded)
        {
            canJump = true;
            isJumping = false;
            jumpCooldown = false;
        }

        if (jumpPressed && isGrounded && canJump && !jumpCooldown)
        {
            Jump();
        }

        jumpPressed = false;

        Vector2 inputVector = GameInput.Instance.GetMovementVector();
        isRunning = Mathf.Abs(inputVector.x) > 0.1f || Mathf.Abs(inputVector.y) > 0.1f;
    }

    private bool CheckIsGrounded()
    {
        return transform.position.y <= groundLevel + 0.1f && Mathf.Abs(rb.linearVelocity.y) < 0.01f;
    }

    private void CheckDash()
    {
        if (dashStarted && !isDashing)
        {
            if (isGrounded)
            {
                canDash = true;
                dashStarted = false;
            }
        }
        else
        {
            canDash = !dashStarted;
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVector();

        if (willDash)
        {
            float dashX = inputVector.x;
            float dashY = inputVector.y * verticalDashMultiplier;

            Vector2 dashDirection = new Vector2(dashX, dashY).normalized;
            if (dashDirection.magnitude < 0.1f)
            {
                dashDirection = Vector2.right;
            }

            rb.linearVelocity = dashDirection * movingSpeed;
        }
        else
        {
            Vector2 movement = new Vector2(inputVector.x * movingSpeed, rb.linearVelocity.y);
            rb.linearVelocity = movement;
        }
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public bool IsJumping()
    {
        return isJumping;
    }

    public bool IsDashing()
    {
        return isDashAnim;
    }

    public bool IsDashBackwards()
    {
        return isDashBackwards;
    }

    public bool IsDashUp()
    {
        return isDashUp;
    }

    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }
}