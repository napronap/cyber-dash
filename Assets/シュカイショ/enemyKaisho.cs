using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class enemyKaisho : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField, Tooltip("最大HP")]
    private int maxHealth = 1;
    public int CurrentHealth { get; private set; }

    [Header("Movement")]
    [SerializeField, Tooltip("移动速度")]
    private float moveSpeed = 2f;

    [Header("Ground Check")]
    [SerializeField, Tooltip("地面判定使用的 Transform（未设置时从对象中心下方检查）")]
    private Transform groundCheck;

    [SerializeField, Tooltip("地面判定半径"), Min(0.01f)]
    private float groundCheckRadius = 0.1f;

    [SerializeField, Tooltip("地面图层")]
    private LayerMask groundLayers = ~0;

    // 内部
    private Rigidbody2D rb;

    // 画面に一度でも入ったか
    private bool _hasEnteredView;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentHealth = maxHealth;
        _hasEnteredView = false;
    }

    void Update()
    {
        var cam = Camera.main;
        if (cam == null) return;

        Vector3 vp = cam.WorldToViewportPoint(transform.position);
        bool inView = vp.z > 0f && vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f;

        if (inView)
        {
            _hasEnteredView = true;
        }
        else if (_hasEnteredView)
        {
            // 入場後に画面外へ出たら自動破棄
            Destroy(gameObject);
        }
    }

    #region Movement API
   
    public void Move(float direction)
    {
        float clamped = Mathf.Clamp(direction, -1f, 1f);
        rb.linearVelocity = new Vector2(clamped * moveSpeed, rb.linearVelocity.y);
        // 不自动翻转，避免与美术默认朝向冲突
    }

    public void SetMoveSpeed(float speed) => moveSpeed = Mathf.Max(0f, speed);

    public void SetMaxHealth(int value)
    {
        maxHealth = Mathf.Max(1, value);
        CurrentHealth = Mathf.Min(CurrentHealth, maxHealth);
    }
    #endregion

    #region Utilities
    public bool IsGrounded()
    {
        Vector2 origin = (groundCheck != null)
            ? groundCheck.position
            : transform.position + Vector3.down * 0.1f;

        Collider2D hit = Physics2D.OverlapCircle(origin, groundCheckRadius, groundLayers);
        return hit != null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = (groundCheck != null) ? groundCheck.position : transform.position + Vector3.down * 0.1f;
        Gizmos.DrawWireSphere(origin, groundCheckRadius);
    }
    #endregion

    #region Health API
    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Destroy(gameObject);
        }
    }
    #endregion
}
