using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [Header("Damage Settings")]
    public int normalDamage = 1;               // ダッシュ以外の場合の通常ダメージ
    public bool instaKillFromDash = true;      // true の場合、ダッシュ攻撃で即死させる

    private DamageReceiver receiver;

    void Start()
    {
        receiver = GetComponentInParent<DamageReceiver>();

        if (receiver == null)
            Debug.LogWarning("DamageZone: No se encontró DamageReceiver en el enemigo.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (receiver == null) return;

        // 1. ダッシュ攻撃が当たった場合 → 即死
        if (instaKillFromDash && other.CompareTag("DashAttack"))
        {
            receiver.TakeFatalDamage();
            return;
        }

        // 2. ダッシュでない場合は通常ダメージ
        receiver.TakeDamage(normalDamage);
    }
}