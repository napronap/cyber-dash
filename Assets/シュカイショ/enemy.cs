using UnityEngine;

/// <summary>
/// 2D 用のモジュール化された敵コンポーネント
/// - Rigidbody2D と Collider2D を必須化
/// - インスペクタで調整可能なパラメータ（HP / 移動 / ジャンプ / 飛行 / パトロール）
/// - Move/Jump/TakeDamage/Heal 等の公開メソッドを提供
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField, Tooltip("最大HP")]
    private int maxHealth = 10;
    public int CurrentHealth { get; private set; }

    [Header("Movement")]
    [SerializeField, Tooltip("移動速度")]
    private float moveSpeed = 2f;

    [SerializeField, Tooltip("ジャンプ力（インパルス）")]
    private float jumpForce = 5f;

    [SerializeField, Tooltip("飛行可能か（true のとき重力を無効化）")]
    private bool canFly = false;

    [Header("AI / Patrol")]
    [SerializeField, Tooltip("自動パトロールを行うか")]
    private bool patrol = true;

    [SerializeField, Tooltip("パトロールの往復距離（横方向）")]
    private float patrolDistance = 3f;

    [Header("Ground Check")]
    [SerializeField, Tooltip("地面判定に使う Transform（未設定時はオブジェクト中心から下にチェック）")]
    private Transform groundCheck;

    [SerializeField, Tooltip("地面判定の半径"), Min(0.01f)]
    private float groundCheckRadius = 0.1f;

    [SerializeField, Tooltip("地面レイヤー")]
    private LayerMask groundLayers = ~0;

    // 内部
    private Rigidbody2D rb;
    private Vector2 patrolOrigin;
    private int patrolDirection = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentHealth = maxHealth;
        patrolOrigin = transform.position;
        rb.gravityScale = canFly ? 0f : 1f;
    }

    void Update()
    {
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
    /// 水平移動。direction は -1..1 を想定
    /// </summary>
    public void Move(float direction)
    {
        float clamped = Mathf.Clamp(direction, -1f, 1f);
        rb.linearVelocity = new Vector2(clamped * moveSpeed, rb.linearVelocity.y);
        // 向き変更（必要ならスプライトの反転などをここで行う）
        if (clamped != 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(clamped) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    /// <summary>
    /// ジャンプ。飛行可能なら垂直速度を直接設定して上昇させる（必要に応じて調整）
    /// </summary>
    public void Jump()
    {
        if (canFly)
        {
            // 飛行時は上方向に速度を与えて高さを調整できるようにする
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            return;
        }

        if (IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// 飛行モード切替
    /// </summary>
    public void SetCanFly(bool value)
    {
        canFly = value;
        rb.gravityScale = canFly ? 0f : 1f;
    }

    public void SetMoveSpeed(float speed) => moveSpeed = Mathf.Max(0f, speed);
    public void SetJumpForce(float value) => jumpForce = Mathf.Max(0f, value);
    public void SetMaxHealth(int value)
    {
        maxHealth = Mathf.Max(1, value);
        CurrentHealth = Mathf.Min(CurrentHealth, maxHealth);
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
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
    }

    private void Die()
    {
        // シンプルに破棄。プールやエフェクトがあればここを差し替えてください。
        Destroy(gameObject);
    }
    #endregion

    #region Utilities
    public bool IsGrounded()
    {
        Vector2 origin;
        if (groundCheck != null)
            origin = groundCheck.position;
        else
            origin = transform.position + Vector3.down * 0.1f;

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
}
