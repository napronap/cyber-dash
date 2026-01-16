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
    private Animator anim; // 动画组件

    private float topLimit;
    private float bottomLimit;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        direction = startMovingUp ? 1 : -1;

        // 获取动画组件（走路/死亡动画用）
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("Animatorコンポーネントがアタッチされていません！", this);
        }
    }

    void Update()
    {
        UpdateVerticalLimits();
        CheckOffScreen();

        // 控制走路动画：有移動速度 → 走路アニメ再生
        if (anim != null)
        {
            bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f || Mathf.Abs(rb.linearVelocity.y) > 0.1f;
            anim.SetBool("IsWalking", isMoving);
        }
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

    // 死亡动画触发（DamageReceiverから呼び出す）
    public void PlayDeathAnimation()
    {
        if (anim != null)
        {
            anim.SetTrigger("IsDead"); // 死亡アニメ触发
            anim.SetBool("IsWalking", false); // 走路アニメ停止
        }
    }
}