using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour, IDamageABLE
{
    public int maxHP = 3;
    public int currentHP;

    private SpriteRenderer sr;
    private bool invulnerable = false;
    private bool isDead = false;

    public UnityEvent<int, int> OnHealthChanged = new UnityEvent<int, int>();

    // ここをInspectorで設定
    [SerializeField] private Transform heartsRoot; // 任意: ハート親
    [SerializeField] private List<GameObject> heartObjects = new List<GameObject>(); // 任意: ハート個別参照（後ろが最後に消える）

    void Start()
    {
        currentHP = Mathf.Max(0, maxHP);
        sr = GetComponent<SpriteRenderer>();

        // heartObjects未指定ならheartsRoot直下の子を自動収集
        if (heartObjects.Count == 0 && heartsRoot != null)
        {
            heartObjects = new List<GameObject>(heartsRoot.childCount);
            for (int i = 0; i < heartsRoot.childCount; i++)
            {
                heartObjects.Add(heartsRoot.GetChild(i).gameObject);
            }
        }

        // 実際のハート数に合わせて初期HPをクランプ
        if (heartObjects.Count > 0)
        {
            currentHP = Mathf.Min(currentHP, heartObjects.Count);
        }

        OnHealthChanged.Invoke(currentHP, maxHP);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (invulnerable) return;
        if (amount <= 0) return;

        int oldHP = currentHP;
        currentHP = Mathf.Max(0, currentHP - amount);

        // 減少分だけハートを末尾から削除
        int lost = Mathf.Max(0, oldHP - currentHP);
        if (lost > 0)
        {
            RemoveHearts(lost);
        }

        if (currentHP > 0 && currentHP < oldHP)
        {
            StartCoroutine(Invulnerability());
            StartCoroutine(FlashHit());
        }

        OnHealthChanged.Invoke(currentHP, maxHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    // Enemyneko 用（float）
    void IDamageABLE.TakeDamage(float amount)
    {
        int dmg = Mathf.FloorToInt(amount);
        TakeDamage(dmg);
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

        if (sr != null)
        {
            sr.enabled = false;
        }

        Destroy(gameObject);
    }

    // 末尾（リスト後方）からアクティブなハートを優先的に削除
    private void RemoveHearts(int count)
    {
        if (count <= 0) return;

        int removed = 0;
        for (int i = heartObjects.Count - 1; i >= 0 && removed < count; i--)
        {
            var h = heartObjects[i];
            if (h == null)
            {
                heartObjects.RemoveAt(i);
                continue;
            }

            if (h.activeInHierarchy)
            {
                Destroy(h);
                heartObjects.RemoveAt(i);
                removed++;
            }
        }
    }
}