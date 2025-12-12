using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 3;
    public int currentHP;

    private SpriteRenderer sr;
    private bool invulnerable = false;

    void Start()
    {
        currentHP = maxHP;
        sr = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int amount)
    {
        if (invulnerable) return;

        currentHP -= amount;
        StartCoroutine(Invulnerability());
        StartCoroutine(FlashHit());

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator Invulnerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(1f);   // 1秒間の無敵時間
        invulnerable = false;
    }

    private IEnumerator FlashHit()
    {
        Color original = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = original;
    }

    private void Die()
    {
        Debug.Log("Player died");
        // ここでリスポーン処理やアニメーションを再生する
    }
}