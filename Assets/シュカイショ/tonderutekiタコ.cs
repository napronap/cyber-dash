using UnityEngine;

public class tonderutekiタコ : enemy
{
    [SerializeField, Tooltip("左方向へ進む移動速度（基底クラスに適用）")]
    private float speed = 2f;

    [Header("Despawn Conditions")]
    [SerializeField, Tooltip("プレイヤーと判定するタグ名。未設定ならレイヤーで判定")]
    private string playerTag = "Player";

    [SerializeField, Tooltip("プレイヤーのレイヤー（0 の場合は無視）")]
    private LayerMask playerLayers = 0;

    void Start()
    {
        SetMoveSpeed(speed);
    }

    // 物理は FixedUpdate で更新
    void FixedUpdate()
    {
        // 右から左へ等速移動
        Move(-1f);
    }

    void Update()
    {
        // 画面外に出たら消滅（カメラ前方のみ判定）
        var cam = Camera.main;
        if (cam != null)
        {
            var vp = cam.WorldToViewportPoint(transform.position);
            if (vp.z > 0f && (vp.x < 0f || vp.x > 1f || vp.y < 0f || vp.y > 1f))
            {
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
}
