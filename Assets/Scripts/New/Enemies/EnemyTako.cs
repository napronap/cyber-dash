using UnityEngine;

public class EnemyTako : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private bool isDead = false;
    private Animator animator;
    private EnemyScore enemyScore;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyScore = GetComponent<EnemyScore>();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        rb.linearVelocity = new Vector2(-moveSpeed, 0f);

        // despawn on left edge
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float leftEdgeX = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x - sr.bounds.extents.x;

        if (transform.position.x < leftEdgeX)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.collider.CompareTag("Player"))
        {
            WorkingPlayerController.Instance.TakeDamage();
        }
    }


    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }


    public void Die()
    {
        if (isDead) return;

        isDead = true;

        rb.linearVelocity = Vector2.zero;

        HitStop.Do(0.05f);
        // screen shake on enemy death AND player damage feels too much
        // so I'm commenting this one here
        // ScreenShake.Shake(0.12f, 0.06f);
        
        int pts = (enemyScore != null) ? enemyScore.Points : 0;
        if (ScoreManager.Instance != null) ScoreManager.Instance.Add(pts);

        animator.SetBool("IsDead", true);
    }

}
