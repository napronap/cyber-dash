using UnityEngine;

public class FishUFO : MonoBehaviour
{
    [Header("上下移動の設定")]
    [SerializeField] private float verticalSpeed = 2f;

    [Header("画面上の余白")]
    [SerializeField] private float topPadding = 0.5f;

    [Header("下限（画面の何割まで下がるか）")]
    [Range(0f, 1f)]
    [SerializeField] private float bottomPercent = 0.5f;

    [Header("開始方向 true=上 false=下")]
    [SerializeField] private bool startMovingUp = false;

    [Header("横移動の設定")]
    [SerializeField] private float horizontalSpeed = 2f;

    [Header("画面外に出たら消えるか？")]
    [SerializeField] private bool destroyWhenOffScreen = true;

    private Rigidbody2D rb;
    private int direction; // 1=up, -1=down

    private float topLimit;
    private float bottomLimit;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        direction = startMovingUp ? 1 : -1;
    }

    void Update()
    {
        UpdateVerticalLimits();
        CheckOffScreen();
    }

    void FixedUpdate()
    {
        // 垂直移動＋水平移動
        rb.linearVelocity = new Vector2(-horizontalSpeed, direction * verticalSpeed);

        // 上方向への反転
        if (transform.position.y >= topLimit)
        {
            transform.position = new Vector3(transform.position.x, topLimit, transform.position.z);
            direction = -1;
        }

        //下方向への反転
        if (transform.position.y <= bottomLimit)
        {
            transform.position = new Vector3(transform.position.x, bottomLimit, transform.position.z);
            direction = 1;
        }
    }
    private void UpdateVerticalLimits()
    {
        float z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

        Vector3 topWorld = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1f, z));
        topLimit = topWorld.y - topPadding;

        Vector3 bottomWorld = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, bottomPercent, z));
        bottomLimit = bottomWorld.y;
    }

    private void CheckOffScreen()
    {
        if (!destroyWhenOffScreen) return;

        float z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 leftWorld = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.5f, z));

        if (transform.position.x < leftWorld.x - 1f)
        {
            Destroy(gameObject);
        }
    }
}