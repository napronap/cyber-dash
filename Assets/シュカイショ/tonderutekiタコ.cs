using UnityEngine;

public class tonderutekiタコ : enemyKaisho
{
    [SerializeField, Tooltip("移動速度（基底クラスへ適用）")]
    private float speed = 2f;

    [Header("Player 判定")]
    [SerializeField, Tooltip("プレイヤータグ")]
    private string playerTag = "Player";
    [SerializeField, Tooltip("プレイヤーレイヤー（0 の場合は無視）")]
    private LayerMask playerLayers = 0;

    [Header("初期方向 (-1=左, +1=右)")]
    [SerializeField] private float initialDirection = -1f;

    // 一度でも画面内に入ったか
    private bool hasEnteredView = false;

    void Start()
    {
        SetMoveSpeed(speed);
    }

    void FixedUpdate()
    {
        Move(Mathf.Clamp(initialDirection, -1f, 1f));
    }

    void Update()
    {
        var cam = Camera.main;
        if (cam == null) return;

        Vector3 vp = cam.WorldToViewportPoint(transform.position);

        bool inView = vp.z > 0f && vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f;

        if (inView)
        {
            hasEnteredView = true;
        }
        else
        {
            // まだ画面に入っていない間は破棄しない
            if (hasEnteredView)
            {
                // 一度入ってから外れたので破棄
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsPlayerCollider(collision.collider))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayerCollider(other))
        {
            Destroy(gameObject);
        }
    }

    private bool IsPlayerCollider(Collider2D col)
    {
        if (col == null) return false;
        if (!string.IsNullOrEmpty(playerTag) && col.CompareTag(playerTag)) return true;
        if (playerLayers != 0 && ((1 << col.gameObject.layer) & playerLayers) != 0) return true;
        return false;
    }

    public void SetInitialDirection(float dir)
    {
        initialDirection = Mathf.Clamp(dir, -1f, 1f);
    }
}
