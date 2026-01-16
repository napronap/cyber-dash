using UnityEngine;
using System.Collections;

public class DamageReceiver : MonoBehaviour
{
    [Header("Enemy HP")]
    public int maxHP = 5;
    public int currentHP;

    private SpriteRenderer sr;
    private FishUFO fishUFO; // 移动脚本引用
    private bool isDead = false; // 死亡判定（重複実行防止）

    void Start()
    {
        currentHP = maxHP;
        sr = GetComponent<SpriteRenderer>();
        fishUFO = GetComponent<FishUFO>(); // 获取移动脚本
    }

    // 通常ダメージ
    public void TakeDamage(int amount)
    {
        if (isDead) return; // 死亡後は無効

        currentHP -= amount;

        if (sr != null)
            StartCoroutine(FlashHit());

        if (currentHP <= 0)
            Die();
    }

    // 即死ダメージ（ダッシュ攻撃用）
    public void TakeFatalDamage()
    {
        if (isDead) return; // 死亡後は無効

        currentHP = 0;

        if (sr != null)
            StartCoroutine(FlashHit());

        Die();
    }

    private IEnumerator FlashHit()
    {
        if (sr != null)
        {
            Color originalColor = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;
        }
    }

    private void Die()
    {
        isDead = true;

        // 死亡アニメ再生
        if (fishUFO != null)
        {
            fishUFO.PlayDeathAnimation();
        }

        // 死亡アニメが終わってからオブジェクトを削除（アニメ長さに合わせて調整）
        Destroy(gameObject, 0.5f); // 0.5fはアニメの長さに合わせて変更

        // 補足：移動停止（必要な場合）
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}