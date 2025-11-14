using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class toutekisakana : enemy
{
    [Header("上下移動")]
    [SerializeField, Tooltip("上下移動の速度（ワールド単位/秒）")]
    private float verticalSpeed = 2f;

    [SerializeField, Tooltip("画面端からの余白（ワールド単位）")]
    private float verticalPadding = 0.1f;

    [SerializeField, Tooltip("開始方向: true=上, false=下")]
    private bool startUp = false;

    [SerializeField, Tooltip("開始方向をランダムにする")]
    private bool randomStartDirection = false;

    private Rigidbody2D rb;
    private float topY;
    private float bottomY;
    private float halfHeight = 0.5f;
    private int verticalDirection = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError($"{nameof(toutekisakana)} requires Rigidbody2D.");
            enabled = false;
            return;
        }

        // 横移動の影響を切る（背景で横方向の見た目移動をする想定）
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        var rend = GetComponentInChildren<Renderer>();
        if (rend != null) halfHeight = rend.bounds.extents.y;

        verticalDirection = randomStartDirection ? (Random.value > 0.5f ? 1 : -1) : (startUp ? 1 : -1);
    }

    void Start()
    {
        UpdateScreenBounds();
        // 初期位置を画面内にクランプ
        float y = Mathf.Clamp(transform.position.y, bottomY, topY);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
        if (transform.position.y >= topY) verticalDirection = -1;
        if (transform.position.y <= bottomY) verticalDirection = 1;
    }

    void FixedUpdate()
    {
        UpdateScreenBounds();

        // 水平方向をゼロに固定し、垂直速度のみ与える
        rb.linearVelocity = new Vector2(0f, verticalDirection * Mathf.Abs(verticalSpeed));

        // 境界到達時に位置補正して反転
        if (transform.position.y <= bottomY)
        {
            transform.position = new Vector3(transform.position.x, bottomY, transform.position.z);
            verticalDirection = 1;
        }
        else if (transform.position.y >= topY)
        {
            transform.position = new Vector3(transform.position.x, topY, transform.position.z);
            verticalDirection = -1;
        }
    }

    private void UpdateScreenBounds()
    {
        if (Camera.main == null) return;
        float z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 bottomWorld = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, z));
        Vector3 topWorld = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, z));
        bottomY = bottomWorld.y + halfHeight + verticalPadding;
        topY = topWorld.y - halfHeight - verticalPadding;

        // 万一逆転していたら安全側に補正
        if (topY <= bottomY)
        {
            float mid = transform.position.y;
            bottomY = mid - 0.1f;
            topY = mid + 0.1f;
        }
    }

    void OnDrawGizmosSelected()
    {
        // エディタで上下境界を確認しやすくする（実行中以外でも描画）
        var rend = GetComponentInChildren<Renderer>();
        float hh = (rend != null) ? rend.bounds.extents.y : halfHeight;
        if (Camera.main == null) return;
        float z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 bottomWorld = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, z));
        Vector3 topWorld = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, z));
        float b = bottomWorld.y + hh + verticalPadding;
        float t = topWorld.y - hh - verticalPadding;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(transform.position.x - 0.5f, b, transform.position.z), new Vector3(transform.position.x + 0.5f, b, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x - 0.5f, t, transform.position.z), new Vector3(transform.position.x + 0.5f, t, transform.position.z));
    }
}