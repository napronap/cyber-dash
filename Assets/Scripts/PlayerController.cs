using JetBrains.Annotations;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using System;
using System.Collections;

[SelectionBase]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] private float movingSpeed = 5f;
    [Header("Dash Settings")]
    [SerializeField] private int dashSpeed = 4;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private  TrailRenderer trailRenderer;
    [SerializeField] private float dashCooldownTime = 0.25f;
    [SerializeField] private float jumpForce = 1000f;
    [SerializeField] private float jumpTime = 0.2f;

    private Rigidbody2D rb;
    private float minMovingSpeed = 0.1f;
    private bool isRunning = false;
    private float _initialMovingSpeed;
    private bool isDashing;
    private bool isJumping;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        _initialMovingSpeed = movingSpeed;
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;

        GameInput.Instance.OnPlayerDash += GameInput_OnPlayerDash;

        GameInput.Instance.OnPlayerJump += GameInput_OnPlayerJump;
    }

    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e)
    {   
       ActiveWeapon.Instance.getActiveWeapon().Attack();
    } 

    private void GameInput_OnPlayerDash(object sender, System.EventArgs e)
    {
        Dash();
        ActiveWeapon.Instance.getActiveWeapon().Attack();
    }

    private void GameInput_OnPlayerJump(object sender, System.EventArgs e)
    {
        Jump();
        Debug.Log("Jumped");
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        
    }
    //private IEnumerator JumpRoutine()
    //{
    //    isJumping = true;
    //    rb.AddForce(Vector2.up * jumpForce*100);
    //    Debug.Log("Jumped");
    //    yield return new WaitForSeconds(jumpTime);
    //    isJumping = false;

    //}

    private void Dash()
    {
        if(!isDashing)
            StartCoroutine(DashRoutine());


    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
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

    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVector();
        inputVector = inputVector.normalized;
        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime));

        if ((Mathf.Abs(inputVector.x) > minMovingSpeed || Mathf.Abs(inputVector.y) > minMovingSpeed))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;

        }


    }
    public bool IsRunning()
    {
        return isRunning;

    }

    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 playerScreenPosition= Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }
}