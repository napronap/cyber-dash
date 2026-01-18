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
        sr = GetComponent<SpriteRenderer>();
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

        // 停止输入与移动
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
        }

        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 可选：隐藏角色外观
        if (sr != null)
        {
            sr.enabled = false;
        }

        // 立即销毁玩家（或根据需要改为重载场景/复活流程）
        Destroy(gameObject);
    }
}