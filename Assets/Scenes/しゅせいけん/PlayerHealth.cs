using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 3;
    public int currentHP;

    private SpriteRenderer sr;
    private bool invulnerable = false;
    private bool isDead = false;

    void Start()
    {
        currentHP = Mathf.Max(0, maxHP);

        // 关键：渲染器通常在子物体上，使用 InChildren 才能拿到
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (invulnerable) return;
        if (amount <= 0) return;

        currentHP = Mathf.Max(0, currentHP - amount);

        // 命中反馈（可在死亡时跳过）
        if (currentHP > 0)
        {
            StartCoroutine(Invulnerability());
            StartCoroutine(FlashHit());
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 仅当命中的 Collider 是玩家“身体碰撞体”时才会扣血。
    /// </summary>
    public void TryTakeDamage(Collider2D hitCollider, int amount)
    {
        if (hitCollider == null) return;

        var teseto = GetComponent<TESETO>();
        if (teseto == null)
        {
            // 没有 TESETO 时退化为普通扣血，避免直接失效
            TakeDamage(amount);
            return;
        }

        // TESETO 上配置的 bodyCollider（或默认根 Collider）才允许受伤
        var body = GetBodyColliderFallback(teseto);
        if (body == null) return;

        if (hitCollider != body)
        {
            return;
        }

        TakeDamage(amount);
    }

    private static Collider2D GetBodyColliderFallback(TESETO teseto)
    {
        // 通过反射读取 TESETO 的私有字段 bodyCollider（不改 TESETO，保持“最小侵入”）
        // 如果你愿意改 TESETO，改成公开属性会更干净。
        var f = typeof(TESETO).GetField("bodyCollider", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var body = f?.GetValue(teseto) as Collider2D;
        if (body != null) return body;

        return teseto.GetComponent<Collider2D>();
    }

    private IEnumerator Invulnerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(1f);
        invulnerable = false;
    }

    private IEnumerator FlashHit()
    {
        if (sr == null) yield break;
        Color original = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = original;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player died");

        if (sr != null)
        {
            sr.enabled = true;
        }

        var teseto = GetComponent<TESETO>();
        if (teseto != null)
        {
            teseto.PlayDeath();
        }
        else
        {
            Debug.LogWarning("PlayerHealth: 未找到 TESETO 组件，无法播放死亡动画。");
        }

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            // 不要 rb.isKinematic = true; 否则无法下落
        }

        // 不要禁用 Collider，否则可能穿过地面/无法落地接触
        // var col = GetComponent<Collider2D>();
        // if (col != null) col.enabled = false;
    }
}