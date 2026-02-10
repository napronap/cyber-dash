using UnityEngine;

public class EnemyAttackZone : MonoBehaviour
{
    [SerializeField, Tooltip("对玩家造成的伤害")]
    public int damageToPlayer = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerHealth>();
        if (player != null)
        {
            player.TryTakeDamage(other, damageToPlayer);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null || collision.collider == null) return;

        var hit = collision.collider;
        var player = hit.GetComponentInParent<PlayerHealth>();
        if (player != null)
        {
            player.TryTakeDamage(hit, damageToPlayer);
        }
    }
}