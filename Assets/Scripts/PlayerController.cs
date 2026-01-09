using UnityEngine;
using System;
using System.Collections;

[SelectionBase]
[RequireComponent(typeof(KnockBack))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    [SerializeField] private int _maxhealth =10;

    Vector2 inputVector;

    [SerializeField] private float _movingSpeed = 5f;
    [Header("Dash Settings")]
    [SerializeField] private int _dashSpeed = 4;
    [SerializeField] private float _dashTime = 0.2f;
    [SerializeField] private  TrailRenderer _trailRenderer;
    [SerializeField] private float _dashCooldownTime = 0.25f;
    [SerializeField] private float _jumpForce = 1000f;
    [SerializeField] private float _jumpTime = 0.2f;

    private Rigidbody2D _rb;
    private KnockBack _knockBack;

    private float minMovingSpeed = 0.1f;
    private bool isRunning = false;
    private float _initialMovingSpeed;
    private bool isDashing;
    private bool isJumping;
    private int _currentHealth;

    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
        _initialMovingSpeed = _movingSpeed;
        _knockBack = GetComponent<KnockBack>();
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;

        GameInput.Instance.OnPlayerDash += GameInput_OnPlayerDash;

        GameInput.Instance.OnPlayerJump += GameInput_OnPlayerJump;

        _currentHealth = _maxhealth;
    }

    public void TakeDamage(Transform damageSource, int damage)
    {
        _currentHealth = Mathf.Max(0, _currentHealth -= damage);
        Debug.Log(_currentHealth);
        _knockBack.GetKnockedBack(damageSource);

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
        _rb.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
        
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
        _movingSpeed *= _dashSpeed;
        _trailRenderer.emitting = true;
        yield return new WaitForSeconds(_dashTime);

        _trailRenderer.emitting = false;
        _movingSpeed = _initialMovingSpeed;

        yield return new WaitForSeconds(_dashCooldownTime);
        isDashing = false;
    }

    void FixedUpdate()
    {
        if(_knockBack.IsGettingKnockedBack)
            return;
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVector();
        inputVector = inputVector.normalized;
        _rb.MovePosition(_rb.position + inputVector * (_movingSpeed * Time.fixedDeltaTime));

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