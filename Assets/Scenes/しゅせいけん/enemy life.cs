using UnityEngine;

public class enemylife : MonoBehaviour
{
    [Header("HP")]
    [SerializeField, Tooltip("最大HP")]
    private int maxHp = 30;

    [SerializeField, Tooltip("プレイヤーと接触したときに失うHP")]
    private int damageOnPlayerCollision = 10;

    [SerializeField, Tooltip("判定するプレイヤータグ（空欄でタグ判定なし）")]
    private string playerTag = "Player";

    private int currentHp;
    private Renderer rend;
    private bool pendingDestroy = false;
    private bool noRenderer = false;

    void Awake()
    {
        currentHp = Mathf.Max(0, maxHp);
        // 子を含めて Renderer を探す
        rend = GetComponentInChildren<Renderer>() ?? GetComponent<Renderer>();
        if (rend == null)
        {
            noRenderer = true;
            Debug.LogWarning($"{nameof(enemylife)}: Renderer が見つかりません。可視判定ができないため HP<=0 で即時破棄されます。");
        }
    }

    // トリガー用（isTrigger = true の Collider2D）
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        if (!string.IsNullOrEmpty(playerTag) && !other.CompareTag(playerTag)) return;
        ApplyCollisionDamage();
    }

    // 衝突用（isTrigger = false の Collider2D）
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;
        var otherObj = collision.collider != null ? collision.collider.gameObject : collision.gameObject;
        if (otherObj == null) return;
        if (!string.IsNullOrEmpty(playerTag) && !otherObj.CompareTag(playerTag)) return;
        ApplyCollisionDamage();
    }

    private void ApplyCollisionDamage()
    {
        TakeDamage(damageOnPlayerCollision);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        if (currentHp <= 0) return; // 既に死んでいるなら無視

        currentHp -= amount;

        if (currentHp <= 0)
        {
            currentHp = 0;
            if (noRenderer || IsVisible())
            {
                Destroy(gameObject);
            }
            else
            {
                // 画面外なら表示されるまで待って破棄
                pendingDestroy = true;
                // 重複ダメージを防ぐため当たり判定を無効化
                foreach (var c in GetComponents<Collider2D>()) c.enabled = false;
            }
        }
    }

    private bool IsVisible()
    {
        return rend != null && rend.isVisible;
    }

    // 画面に現れたときに pendingDestroy をチェックして破棄
    void OnBecameVisible()
    {
        if (pendingDestroy) Destroy(gameObject);
    }

    // 現在HPを外部参照可能に
    public int CurrentHp => currentHp;
}
