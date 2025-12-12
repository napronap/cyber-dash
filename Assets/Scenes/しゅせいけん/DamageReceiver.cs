using UnityEngine;
using System.Collections;

public class DamageReceiver : MonoBehaviour
{
    [Header("Enemy HP")]
    public int maxHP = 5;
    public int currentHP;

    private SpriteRenderer sr;

    void Start()
    {
        currentHP = maxHP;
        sr = GetComponent<SpriteRenderer>();
    }

    // 通常ダメージ
    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        if (sr != null)
            StartCoroutine(FlashHit());

        if (currentHP <= 0)
            Die();
    }

    // 即死ダメージ（ダッシュ攻撃用）
    public void TakeFatalDamage()
    {
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
        // ここに後でアニメーション、パーティクル、サウンドなどを追加する
        Destroy(gameObject);
    }
    }