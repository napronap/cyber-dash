using UnityEngine;

public class EnemyUFO : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waveAmplitude = 1.0f;
    [SerializeField] private float waveFrequency = 2.0f;

    private Rigidbody2D rb;
    private bool isDead = false;
    private Animator animator;
    private EnemyScore enemyScore;

    private float startY;
    private float timeOffset;
    private bool active = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyScore = GetComponent<EnemyScore>();

        startY = transform.position.y;
        timeOffset = Random.Range(0f, 10f);
    }

    private void FixedUpdate()
    {
        if (!active || isDead) return;

        float x = rb.position.x - moveSpeed * Time.fixedDeltaTime;

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

    public void Freeze()
    {
        active = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
}
