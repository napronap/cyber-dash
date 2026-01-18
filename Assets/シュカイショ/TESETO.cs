using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class TESETO : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Tooltip("悈暯堏摦懍搙")]
    private float moveSpeed = 4f;
    [SerializeField, Tooltip("僕儍儞僾椡")]
    private float jumpForce = 6f;
    [SerializeField, Tooltip("嬻拞憖嶌偺岠偒嬶崌(0-1)")]
    private float airControl = 0.6f;

    [Header("Facing")]
    [SerializeField, Tooltip("斀揮偵巊偆 SpriteRenderer乮枹愝掕側傜帺摦庢摼乯")]
    private SpriteRenderer spriteRenderer;

    [Header("Ground Check")]
    [SerializeField, Tooltip("抧柺敾掕梡偺 Transform乮枹愝掕帪偼杮懱偺彮偟壓乯")]
    private Transform groundCheck;
    [SerializeField, Tooltip("抧柺敾掕敿宎"), Min(0.01f)]
    private float groundCheckRadius = 0.12f;
    [SerializeField, Tooltip("抧柺儗僀儎乕")]
    private LayerMask groundLayers = ~0;

    // 撪晹
    private Rigidbody2D rb;
    private float inputX;
    private bool jumpPressed;
    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 嶲徠庢摼
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 擖椡庢摼
        inputX = Input.GetAxisRaw("Horizontal"); // -1, 0, 1
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }

        // 岦偒斀揮乮尒偨栚偺傒乯
        if (spriteRenderer != null && Mathf.Abs(inputX) > 0.01f)
        {
            var ls = spriteRenderer.transform.localScale;
            ls.x = Mathf.Abs(ls.x) * (inputX >= 0 ? 1f : -1f);
            spriteRenderer.transform.localScale = ls;
        }
    }

    void FixedUpdate()
    {
        // 抧柺敾掕
        isGrounded = IsGrounded();

        // 墶堏摦乮抧忋偼僼儖丄嬻拞偼 airControl乯
        float targetVx = inputX * moveSpeed;
        float control = isGrounded ? 1f : Mathf.Clamp01(airControl);

        Vector2 v = rb.linearVelocity;
        float accel = moveSpeed * 5f; // 揔搙側壛懍
        v.x = Mathf.MoveTowards(v.x, targetVx, accel * control * Time.fixedDeltaTime);

        // 僕儍儞僾
        if (jumpPressed && isGrounded)
        {
            v.y = jumpForce;
        }

        rb.linearVelocity = v;
        jumpPressed = false;
    }

    private bool IsGrounded()
    {
        Vector2 origin = (groundCheck != null)
            ? groundCheck.position
            : transform.position + Vector3.down * 0.1f;

        Collider2D hit = Physics2D.OverlapCircle(origin, groundCheckRadius, groundLayers);
        return hit != null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 origin = (groundCheck != null) ? groundCheck.position : transform.position + Vector3.down * 0.1f;
        Gizmos.DrawWireSphere(origin, groundCheckRadius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ApplyEnemyDamage(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null || collision.collider == null) return;
        ApplyEnemyDamage(collision.collider);
    }

    // 仅当玩家主体碰撞体与 Tag=Enemy 接触时扣 1 点生命值；忽略 PlayerAttack 子碰撞体触发
    private void ApplyEnemyDamage(Collider2D col)
    {
        if (col == null) return;
        if (!col.CompareTag("Enemy")) return;

        // 取玩家主体碰撞体（挂在同一 GameObject 上的 Collider2D）
        var bodyCollider = GetComponent<Collider2D>();
        if (bodyCollider == null) return;

        // 只有主体碰撞体与敌人发生接触时才结算伤害
        if (!bodyCollider.IsTouching(col)) return;

        var hp = GetComponent<PlayerHealth>();
        if (hp != null)
        {
            hp.TakeDamage(1);
        }
    }
}