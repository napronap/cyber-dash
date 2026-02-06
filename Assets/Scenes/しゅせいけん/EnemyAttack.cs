using UnityEngine;

public class EnemyAttackZone : MonoBehaviour
{
    [SerializeField, Tooltip("�������ɵ��˺�")]
    public int damageToPlayer = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���������ж��󱾲��� PlayerHealth�����û�У������丸�����
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