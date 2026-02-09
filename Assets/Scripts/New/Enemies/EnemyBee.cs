using UnityEngine;
using System.Collections;

public class EnemyBee : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float zigZagHeight = 1.5f;
    [SerializeField] private float zigZagLength = 1.5f;
    [SerializeField] private float descendPerZig = 0.75f;

    [Header("Vertical Speed")]
    [SerializeField] private float verticalSpeed = 2.0f;

    [Header("Attack")]
    [SerializeField] private float attackAnimTime = 0.15f;

    private Rigidbody2D rb;
    private Animator animator;
    private EnemyScore enemyScore;

    private bool isDead = false;

    private float baseY;
    private float traveledX = 0f;
    private int zigDir = 1;

    private Coroutine attackCo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyScore = GetComponent<EnemyScore>();

        baseY = transform.position.y;
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        // Movimiento horizontal constante
        float dx = -moveSpeed * Time.fixedDeltaTime;
        traveledX += Mathf.Abs(dx);

        // Cambio de zigzag + descenso progresivo
        if (traveledX >= zigZagLength)
        {
            traveledX = 0f;
            zigDir *= -1;
            baseY -= descendPerZig;

            TriggerAttackAnim(); // ⬅ ataque en cada quiebre
        }

        // Target vertical del zigzag
        float targetY = baseY + zigDir * zigZagHeight;

        // Movimiento vertical suave
        float newY = Mathf.MoveTowards(
            rb.position.y,
            targetY,
            verticalSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(new Vector2(rb.position.x + dx, newY));

        // Despawn fuera de pantalla
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float leftEdgeX =
            Camera.main.ViewportToWorldPoint(Vector3.zero).x - sr.bounds.extents.x;

        if (transform.position.x < leftEdgeX)
            Destroy(gameObject);
    }

    private void TriggerAttackAnim()
    {
        if (animator == null) return;

        if (attackCo != null)
            StopCoroutine(attackCo);

        attackCo = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        animator.SetBool("IsAttacking", true);
        yield return new WaitForSeconds(attackAnimTime);
        animator.SetBool("IsAttacking", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.collider.CompareTag("Player"))
        {
            WorkingPlayerController.Instance.TakeDamage();
        }
    }

    // -------------------------
    // Death
    // -------------------------
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;

        HitStop.Do(0.05f);

        int pts = enemyScore != null ? enemyScore.Points : 0;
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.Add(pts);

        animator.SetBool("IsDead", true);
    }

    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }
}
