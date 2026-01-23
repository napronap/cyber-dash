using UnityEngine;

public class FishUFO : MonoBehaviour
{
    [Header("上下移動の設定")]
    [SerializeField] private float verticalSpeed = 2f;

    [Header("画面上の余白")]
    [SerializeField] private float topPadding = 0.5f;

    [Header("下限（画面的に何割まで下がるか）")]
    [Range(0f, 1f)]
    [SerializeField] private float bottomPercent = 0.5f;

    [Header("開始方向 true=上 false=下")]
    [SerializeField] private bool startMovingUp = false;

    [Header("横移動の設定")]
    [SerializeField] private float horizontalSpeed = 2f;

    [Header("画面外に出たら消えるか？")]
    [SerializeField] private bool destroyWhenOffScreen = true;

    [Header("Frame Animation (帧序列)")]
    [SerializeField, Tooltip("移动帧序列（循环播放）")]
    private Sprite[] moveFrames;
    [SerializeField, Tooltip("死亡帧序列（顺序播放一次后销毁）")]
    private Sprite[] deathFrames;
    [SerializeField, Tooltip("每帧持续时间（秒）")]
    private float frameDuration = 0.1f;
    [SerializeField, Tooltip("播放动画的 SpriteRenderer")]
    private SpriteRenderer spriteRenderer;

    [SerializeField, Tooltip("若未配置死亡帧，则按此延时销毁（秒）")]
    private float deathDestroyDelay = 1.0f;

    private Rigidbody2D rb;
    private int direction; // 1=up, -1=down

    private float topLimit;
    private float bottomLimit;

    // 动画协程
    private System.Collections.IEnumerator _moveLoopCo;
    private System.Collections.IEnumerator _deathCo;
    private bool _isDead;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        direction = startMovingUp ? 1 : -1;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        // 初始显示一帧（优先移动第一帧）
        if (spriteRenderer != null)
        {
            if (moveFrames != null && moveFrames.Length > 0)
                spriteRenderer.sprite = moveFrames[0];
            else if (deathFrames != null && deathFrames.Length > 0)
                spriteRenderer.sprite = deathFrames[0];
        }
    }

    void OnEnable()
    {
        StartMoveLoop();
    }

    void OnDisable()
    {
        StopMoveLoop();
    }

    void Update()
    {
        if (_isDead) return;
        UpdateVerticalLimits();
        CheckOffScreen();
    }

    void FixedUpdate()
    {
        if (_isDead) return;

        // 垂直移動＋水平移動
        rb.linearVelocity = new Vector2(-horizontalSpeed, direction * verticalSpeed);

        // 上方向への反転
        if (transform.position.y >= topLimit)
        {
            transform.position = new Vector3(transform.position.x, topLimit, transform.position.z);
            direction = -1;
        }

        // 下方向への反転
        if (transform.position.y <= bottomLimit)
        {
            transform.position = new Vector3(transform.position.x, bottomLimit, transform.position.z);
            direction = 1;
        }
    }

    private void UpdateVerticalLimits()
    {
        var cam = Camera.main;
        if (cam == null) return;

        float z = Mathf.Abs(cam.transform.position.z - transform.position.z);

        Vector3 topWorld = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, z));
        topLimit = topWorld.y - topPadding;

        Vector3 bottomWorld = cam.ViewportToWorldPoint(new Vector3(0.5f, bottomPercent, z));
        bottomLimit = bottomWorld.y;
    }

    private void CheckOffScreen()
    {
        if (!destroyWhenOffScreen) return;

        var cam = Camera.main;
        if (cam == null) return;

        float z = Mathf.Abs(cam.transform.position.z - transform.position.z);
        Vector3 leftWorld = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, z));

        if (transform.position.x < leftWorld.x - 1f)
        {
            Destroy(gameObject);
        }
    }

    // 死亡仅通过 DamageZone -> DamageReceiver 路径触发
    public void PlayDeathAnimation()
    {
        if (_isDead) return;
        _isDead = true;

        rb.linearVelocity = Vector2.zero;
        StopMoveLoop();

        if (spriteRenderer != null && deathFrames != null && deathFrames.Length > 0)
        {
            if (_deathCo != null) StopCoroutine(_deathCo);
            _deathCo = DeathRoutine();
            StartCoroutine(_deathCo);
        }
        else
        {
            Destroy(gameObject, Mathf.Max(0.01f, deathDestroyDelay));
        }
    }

    // 移动动画：循环播放
    private void StartMoveLoop()
    {
        if (_isDead) return;
        if (spriteRenderer == null || moveFrames == null || moveFrames.Length == 0) return;
        StopMoveLoop();
        _moveLoopCo = MoveLoop();
        StartCoroutine(_moveLoopCo);
    }

    private void StopMoveLoop()
    {
        if (_moveLoopCo != null)
        {
            StopCoroutine(_moveLoopCo);
            _moveLoopCo = null;
        }
    }

    private System.Collections.IEnumerator MoveLoop()
    {
        float dur = Mathf.Max(0.01f, frameDuration);
        int i = 0;
        while (!_isDead)
        {
            if (spriteRenderer != null) spriteRenderer.sprite = moveFrames[i];
            i = (i + 1) % moveFrames.Length;
            yield return new WaitForSeconds(dur);
        }
    }

    private System.Collections.IEnumerator DeathRoutine()
    {
        float dur = Mathf.Max(0.01f, frameDuration);

        for (int i = 0; i < deathFrames.Length; i++)
        {
            if (spriteRenderer != null) spriteRenderer.sprite = deathFrames[i];
            yield return new WaitForSeconds(dur);
        }

        Destroy(gameObject);
    }
}