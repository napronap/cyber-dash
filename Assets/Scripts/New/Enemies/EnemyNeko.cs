using UnityEngine;

// doesn't work very well and I'm not sure how to fix it
public class EnemyNeko : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpInterval = 0.4f;
    [SerializeField] private float jumpPause = 0.15f;

    private Rigidbody2D rb;
    private Animator animator;
    private EnemyScore enemyScore;

    private bool isDead = false;
    private bool isGrounded = false;
    private bool isJumping = false;

    private float jumpTimer;
    private float pauseTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyScore = GetComponent<EnemyScore>();

        jumpTimer = jumpInterval;
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);

        // -----------------------------
        // Jump logic
        // -----------------------------
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;
        }
        else
        {
            jumpTimer -= Time.fixedDeltaTime;

            if (jumpTimer <= 0f && isGrounded)
            {
                Jump();
                jumpTimer = jumpInterval;
                pauseTimer = jumpPause;
            }
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float leftEdgeX = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x - sr.bounds.extents.x;

        if (transform.position.x < leftEdgeX)
            Destroy(gameObject);
    }

    private void Jump()
    {
        isGrounded = false;
        isJumping = true;
        animator.SetBool("IsJumping", true);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // --------------------------------------------------
    // Ground check
    // --------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
            animator.SetBool("IsJumping", false);
            Debug.Log("Neko grounded");
        }

        if (collision.collider.CompareTag("Player"))
        {
            WorkingPlayerController.Instance.TakeDamage();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // --------------------------------------------------
    // Death
    // --------------------------------------------------
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
