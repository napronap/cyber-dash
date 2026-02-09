using UnityEngine;

public class EnemyUFO : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waveAmplitude = 1.0f; // qué tan alto sube/baja
    [SerializeField] private float waveFrequency = 2.0f; // qué tan rápido oscila

    private Rigidbody2D rb;
    private bool isDead = false;
    private Animator animator;
    private EnemyScore enemyScore;

    private float startY;
    private float timeOffset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyScore = GetComponent<EnemyScore>();

        startY = transform.position.y;
        timeOffset = Random.Range(0f, 10f); // para que no todos sincronicen
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        // avanzar a la izquierda
        float x = rb.position.x - moveSpeed * Time.fixedDeltaTime;

        // movimiento sinusoidal en Y
        float y = startY + Mathf.Sin(Time.time * waveFrequency + timeOffset) * waveAmplitude;

        rb.MovePosition(new Vector2(x, y));

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

        int pts = (enemyScore != null) ? enemyScore.Points : 0;
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.Add(pts);

        animator.SetBool("IsDead", true);
    }
}
