using UnityEngine;

public class EnemyWeakPoint : MonoBehaviour
{
    private IDamageable enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<IDamageable>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enemy == null) return;

        if (other.CompareTag("PlayerAttack"))
        {
            enemy.Die();
        }
    }
}
