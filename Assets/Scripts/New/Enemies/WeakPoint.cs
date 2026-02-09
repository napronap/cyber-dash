using UnityEngine;

public class EnemyWeakPoint : MonoBehaviour
{
    private EnemyTako enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyTako>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            enemy.Die();
        }
    }
}
