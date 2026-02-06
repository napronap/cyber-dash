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
    [SerializeField] private float jumpForce = 1000f;
    [SerializeField] private float jumpTime = 0.2f;
    [SerializeField] private float groundLevel = -4f;
    [SerializeField] private float attackRotationBase = 90f;

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
    private bool canJump = true;

    private CapsuleCollider2D attackCollider;
    private Transform attackColliderParent;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        _initialMovingSpeed = movingSpeed;

        attackColliderParent = transform.Find("PlayerAttack");
        attackCollider = attackColliderParent.GetComponent<CapsuleCollider2D>();
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
        Jump();
    }

    public void Jump()
    {
        if (isGrounded && canJump)
        {
            canJump = false;
            isJumping = true;
            Vector2 inputVector = GameInput.Instance.GetMovementVector();

            if (Mathf.Abs(inputVector.x) > 0.1f)
            {
                Vector2 jumpDirection = new Vector2(inputVector.x * 0.7f, 1f).normalized;
                rb.AddForce(jumpDirection * jumpForce * 7);
            }
            else
            {
                rb.AddForce(Vector2.up * jumpForce * 7);
            }
        }
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
        yield return new WaitForSeconds(dashTime);
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
        HandleMovement();

        isGrounded = CheckIsGrounded();
        if (isGrounded)
        {
            canJump = true;
        }
        CheckDash();

        Vector2 inputVector = GameInput.Instance.GetMovementVector();
        if (inputVector.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg;
            float snapped = Mathf.Round(angle / 45f) * 45f;
            float z = attackRotationBase + snapped;
            attackColliderParent.localRotation = Quaternion.AngleAxis(z, Vector3.forward);
        }
        else
        {
            attackColliderParent.localRotation = Quaternion.AngleAxis(attackRotationBase, Vector3.forward);
        }




        if (isJumping && rb.linearVelocity.y <= 0 && isGrounded)
        {
            isJumping = false;
        }


        isRunning = Mathf.Abs(inputVector.x) > 0.1f || Mathf.Abs(inputVector.y) > 0.1f;
    }

    private bool CheckIsGrounded()
    {
        return transform.position.y < groundLevel;
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

        float vy = willDash ? inputVector.y : 0f;
        Vector2 cleanedVector = new Vector2(inputVector.x, vy).normalized;

        var speed = cleanedVector * movingSpeed;
        rb.linearVelocity = speed;
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public bool IsJumping()
    {
        return isJumping;
    }

    // アニメーションのための変数だけです
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
