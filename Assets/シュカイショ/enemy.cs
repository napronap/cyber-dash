using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField, Tooltip("最大HP")]
    private int maxHealth = 10;
    public int CurrentHealth { get; private set; }

    [Header("Movement")]
    [SerializeField, Tooltip("移动速度")]
    private float moveSpeed = 2f;

    [Header("AI / Patrol")]
    [SerializeField, Tooltip("是否进行自动巡逻")]
    private bool patrol = true;

    [SerializeField, Tooltip("巡逻的往返距离（横向）")]
    private float patrolDistance = 3f;

    [Header("Ground Check")]
    [SerializeField, Tooltip("地面判定使用的 Transform（未设置时从对象中心下方检查）")]
    private Transform groundCheck;

    [SerializeField, Tooltip("地面判定半径"), Min(0.01f)]
    private float groundCheckRadius = 0.1f;

    [SerializeField, Tooltip("地面图层")]
    private LayerMask groundLayers = ~0;

    [Header("Movement Mode")]
    [SerializeField, Tooltip("开启后始终向左移动并禁用巡逻")]
    private bool alwaysMoveLeft = true;

    // 内部
    private Rigidbody2D rb;
    private Vector2 patrolOrigin;
    private int patrolDirection = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentHealth = maxHealth;
        patrolOrigin = transform.position;
    }

    void Update()
    {
        if (alwaysMoveLeft)
        {
            Move(-1f);
            return;
        }

        if (patrol)
        {
            float offset = transform.position.x - patrolOrigin.x;
            if (Mathf.Abs(offset) >= patrolDistance)
            {
                patrolDirection *= -1;
            }

            Move(patrolDirection);
        }
    }

    #region Movement API
    /// <summary>
    /// 水平移动。direction 取值范围为 -1..1
    /// </summary>
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

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
    }
    #endregion
}
