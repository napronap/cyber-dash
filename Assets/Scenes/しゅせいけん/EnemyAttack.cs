using UnityEngine;

public class EnemyAttackZone : MonoBehaviour
{
    public int damageToPlayer = 1;     // 触手が与えるダメージ

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 侵入したオブジェクトが PlayerHealth を持っている場合 → ダメージを与える
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damageToPlayer);
        }
    }
}