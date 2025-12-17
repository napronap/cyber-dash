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
        // 画面外での自動破棄処理を削除しました
        // 必要に応じて他のロジックをここに追加できます
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
