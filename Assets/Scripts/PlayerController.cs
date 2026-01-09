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
    [SerializeField] private int dashSpeed = 4;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private float dashCooldownTime = 30.0f;
    [SerializeField] private float jumpForce = 1000f;
    [SerializeField] private float jumpTime = 0.2f;
    [SerializeField] private float groundLevel = -4f;

    private Rigidbody2D rb;
    private float minMovingSpeed = 0.1f;
    private bool isRunning = false;
    private float _initialMovingSpeed;
    private bool isDashing;
    private bool isJumping;
    private bool isGrounded;
    private bool canDash;
    private bool dashStarted = false;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        _initialMovingSpeed = movingSpeed;
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerDash += GameInput_OnPlayerDash;

        GameInput.Instance.OnPlayerJump += GameInput_OnPlayerJump;

        isGrounded = transform.position.y < groundLevel;
    }

    private void GameInput_OnPlayerDash(object sender, System.EventArgs e)
    {
        Dash();
    }

    private void GameInput_OnPlayerJump(object sender, System.EventArgs e)
    {
        Jump();
    }

    private void Jump()
    {
        if (!isJumping)
            StartCoroutine(JumpRoutine());
    }
    private IEnumerator JumpRoutine()
    {
        if (isGrounded)
        {
            
            isJumping = true;

            
            Vector2 inputVector = GameInput.Instance.GetMovementVector();

            
            if (Mathf.Abs(inputVector.x) > 0.1f)
            {
                
                Vector2 jumpDirection = new Vector2(inputVector.x * 0.7f, 1f).normalized;
                rb.AddForce(jumpDirection * jumpForce * 5);
                Debug.Log("Jump Cross " + inputVector.x);
            }
            else
            {
                
                rb.AddForce(Vector2.up * jumpForce * 5);
                Debug.Log("Jump");
            }

            yield return new WaitForSeconds(jumpTime);
            isJumping = false;
        }


    }

    private void Dash()
    {
        if (canDash && !dashStarted)
            StartCoroutine(DashRoutine());

    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        dashStarted = true;
        movingSpeed *= dashSpeed;
        trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashTime);

        trailRenderer.emitting = false;
        movingSpeed = _initialMovingSpeed;

        yield return new WaitForSeconds(dashCooldownTime);
        isDashing = false;
    }

    void FixedUpdate()
    {
        HandleMovement();

        isGrounded = CheckIsGrounded();

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

    private bool CheckIsGrounded()
    {
        return transform.position.y < groundLevel;
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVector();
        inputVector = inputVector.normalized;


        //rb.MovePosition(rb.position + new Vector2(inputVector.x * movingSpeed * Time.fixedDeltaTime, 0f));


        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime));






    }
    public bool IsRunning()
    {
        return isRunning;

    }

    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }
}