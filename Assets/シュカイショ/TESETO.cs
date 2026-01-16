using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class TESETO : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Tooltip("水平移動速度")]
    private float moveSpeed = 4f;
    [SerializeField, Tooltip("ジャンプ力")]
    private float jumpForce = 6f;
    [SerializeField, Tooltip("空中操作の効き具合(0-1)")]
    private float airControl = 0.6f;

    [Header("Facing")]
    [SerializeField, Tooltip("反転に使う SpriteRenderer（未設定なら自動取得）")]
    private SpriteRenderer spriteRenderer;

    [Header("Ground Check")]
    [SerializeField, Tooltip("地面判定用の Transform（未設定時は本体の少し下）")]
    private Transform groundCheck;
    [SerializeField, Tooltip("地面判定半径"), Min(0.01f)]
    private float groundCheckRadius = 0.12f;
    [SerializeField, Tooltip("地面レイヤー")]
    private LayerMask groundLayers = ~0;

    // 内部
    private Rigidbody2D rb;
    private float inputX;
    private bool jumpPressed;
    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 参照取得
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 入力取得
        inputX = Input.GetAxisRaw("Horizontal"); // -1, 0, 1
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }

        // 向き反転（見た目のみ）
        if (spriteRenderer != null && Mathf.Abs(inputX) > 0.01f)
        {
            var ls = spriteRenderer.transform.localScale;
            ls.x = Mathf.Abs(ls.x) * (inputX >= 0 ? 1f : -1f);
            spriteRenderer.transform.localScale = ls;
        }
    }

    void FixedUpdate()
    {
        // 地面判定
        isGrounded = IsGrounded();

        // 横移動（地上はフル、空中は airControl）
        float targetVx = inputX * moveSpeed;
        float control = isGrounded ? 1f : Mathf.Clamp01(airControl);

        Vector2 v = rb.linearVelocity;
        float accel = moveSpeed * 5f; // 適度な加速
        v.x = Mathf.MoveTowards(v.x, targetVx, accel * control * Time.fixedDeltaTime);

        // ジャンプ
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
}