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

        // ¹Ø¼ü£ºäÖÈ¾Æ÷Í¨³£ÔÚ×ÓÎïÌåÉÏ£¬Ê¹ÓÃ InChildren ²ÅÄÜÄÃµ½
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (invulnerable) return;
        if (amount <= 0) return;

        currentHP = Mathf.Max(0, currentHP - amount);

        // ÃEĞ·´À¡£¨¿ÉÔÚËÀÍöÊ±Ìø¹ı£©
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
    /// ½öµ±ÃüÖĞµÄ Collider ÊÇÍæ¼Ò¡°ÉíÌåÅö×²Ìå¡±Ê±²Å»á¿ÛÑª¡£
    /// </summary>
    public void TryTakeDamage(Collider2D hitCollider, int amount)
    {
        if (hitCollider == null) return;

        var teseto = GetComponent<TESETO>();
        if (teseto == null)
        {
            // Ã»ÓĞ TESETO Ê±ÍË»¯ÎªÆÕÍ¨¿ÛÑª£¬±ÜÃâÖ±½ÓÊ§Ğ§
            TakeDamage(amount);
            return;
        }

        // TESETO ÉÏÅäÖÃµÄ bodyCollider£¨»òÄ¬ÈÏ¸ù Collider£©²ÅÔÊĞíÊÜÉË
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
        // Í¨¹ı·´Éä¶ÁÈ¡ TESETO µÄË½ÓĞ×Ö¶Î bodyCollider£¨²»¸Ä TESETO£¬±£³Ö¡°×îĞ¡ÇÖÈë¡±£©
        // Èç¹ûÄãÔ¸Òâ¸Ä TESETO£¬¸Ä³É¹«¿ªÊôĞÔ»á¸ü¸É¾»¡£
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

<<<<<<< HEAD
        // Í£Ö¹ÊäÈEEÆ¶¯
=======
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
            Debug.LogWarning("PlayerHealth: Î´ÕÒµ½ TESETO ×é¼ş£¬ÎŞ·¨²¥·ÅËÀÍö¶¯»­¡£");
        }

>>>>>>> 1f1912ee2bd7adf5a72e405c8011cda27e27f144
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            // ²»Òª rb.isKinematic = true; ·ñÔòÎŞ·¨ÏÂÂä
        }

<<<<<<< HEAD
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // ¿ÉÑ¡£ºÒş²Ø½ÇÉ«Íâ¹Û
        if (sr != null)
        {
            sr.enabled = false;
        }

        // Á¢¼´Ïú»ÙÍæ¼Ò£¨»ò¸ù¾İĞèÒª¸ÄÎªÖØÔØ³¡¾°/¸´»ûİ÷³Ì£©
        Destroy(gameObject);
        GetComponent<TESETO>()?.Die();
=======
        // ²»Òª½ûÓÃ Collider£¬·ñÔò¿ÉÄÜ´©¹ıµØÃæ/ÎŞ·¨ÂäµØ½Ó´¥
        // var col = GetComponent<Collider2D>();
        // if (col != null) col.enabled = false;
>>>>>>> 1f1912ee2bd7adf5a72e405c8011cda27e27f144
    }
}