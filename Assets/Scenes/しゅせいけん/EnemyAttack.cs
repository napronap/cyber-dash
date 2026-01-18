using UnityEngine;

public class EnemyAttackZone : MonoBehaviour
{
    [SerializeField, Tooltip("对玩家造成的伤害")]
    public int damageToPlayer = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 优先在命中对象本层找 PlayerHealth；如果没有，则在其父层查找
        var player = other.GetComponent<PlayerHealth>();
        if (player == null)
        {
            player = other.GetComponentInParent<PlayerHealth>();
        }

        if (player != null)
        {
            player.TakeDamage(damageToPlayer);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null || collision.collider == null) return;

        var player = collision.collider.GetComponent<PlayerHealth>();
        if (player == null)
        {
            player = collision.collider.GetComponentInParent<PlayerHealth>();
        }

        if (player != null)
        {
            player.TakeDamage(damageToPlayer);
        }
    }
}