using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class toutekisakana : enemy
{
    //[Header("移動")]
    //[SerializeField, Tooltip("右から左への移動速度")]
    //private float moveSpeed = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // 毎物理フレーム、左方向へ移動（Rigidbody2D を使う）
        rb.linearVelocity  = Vector2.left * moveSpeed;
    }

    // 画面外（レンダラーが見えなくなった）で自動破棄
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    // プレイヤーにぶつかったら消える（Collider が trigger の場合）
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }

        if (other.CompareTag(""))
        {
            Destroy(gameObject);
        }
    }

    // プレイヤーにぶつかったら消える（通常の衝突の場合）
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}