using UnityEngine;
using System;
using System.Collections;

[SelectionBase]
[RequireComponent(typeof(KnockBack))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public event EventHandler OnPlayerDeath;
    public event EventHandler OnFlashBlink;

    [SerializeField] private int _maxhealth =10;
    [SerializeField] private float _damageRecoveryTime = 0.5f;

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
    private bool _canTakeDamage;
    private bool _isAlive;

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

        _canTakeDamage = true;

        _isAlive = true;

        _currentHealth = _maxhealth;
    }

    public bool IsAlive() => _isAlive;


    public void TakeDamage(Transform damageSource, int damage)
    {
        if (_canTakeDamage && _isAlive)
        {
            _canTakeDamage = false;
            _currentHealth = Mathf.Max(0, _currentHealth -= damage);
            Debug.Log(_currentHealth);
            _knockBack.GetKnockedBack(damageSource);

            OnFlashBlink?.Invoke(this, EventArgs.Empty);

            StartCoroutine(DamageRecoveryRoutine());
        }
        DetectDeath();
    }

    private void DetectDeath()
    {
        if(_currentHealth == 0 && _isAlive )
        {

            _isAlive = false;
            _knockBack.StopKnockBackMovement();
            GameInput.Instance.DisableMovement();
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);

        }
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(_damageRecoveryTime);
        _canTakeDamage = true;

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

    private void FixedUpdate()
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