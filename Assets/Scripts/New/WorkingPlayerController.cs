using UnityEngine;
using System.Collections;

public class WorkingPlayerController : MonoBehaviour
{
    public static WorkingPlayerController Instance { get; private set; }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Jump (soft)")]
    [SerializeField] private float jumpVelocity = 13f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashTime = 0.12f;
    [SerializeField] private float dashLongMultiplier = 1f;
    [SerializeField] private float dashShortMultiplier = 0.6f;

    [Header("Damage")]
    [SerializeField] private int maxHp = 4;
    [SerializeField] private float invulTime = 0.5f;
    [SerializeField] private float hitKnockbackSpeed = 10f;
    [SerializeField] private float hitKnockbackTime = 0.15f;

    private Rigidbody2D rb;

    private bool isGrounded = false;
    private bool isDashing = false;
    private bool airDashAvailable = true;
    private bool isInvulnerable = false;
    private bool isDead = false;
    private bool isAttacking = false;

    private int currentHp;

    public enum DashType
    {
        None = 0,
        Forward = 1,
        Back = 2,
        Up = 3,
    }

    private DashType currentDash = DashType.None;

    // --------------------------------------------------
    // HUD HP GETTERS
    // --------------------------------------------------
    public int CurrentHP => currentHp;
    public int MaxHP => maxHp;


    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        currentHp = maxHp;
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerJump += (_, __) => Jump();
        GameInput.Instance.OnPlayerDash += (_, __) => TryDash();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (!isDashing)
        {
            float x = GameInput.Instance.GetMovementVector().x;
            rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);
        }
    }

    // --------------------------------------------------
    // JUMP
    // --------------------------------------------------
    private void Jump()
    {
        if (isDead) return;
        if (!isGrounded || isDashing) return;

        isGrounded = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
    }

    // --------------------------------------------------
    // DASH
    // --------------------------------------------------
    private void TryDash()
    {
        if (isDead) return;
        if (isDashing) return;
        if (!isGrounded && !airDashAvailable) return;

        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        isAttacking = true;

        if (!isGrounded)
            airDashAvailable = false;

        Vector2 input = GameInput.Instance.GetMovementVector();
        if (input.sqrMagnitude < 0.001f)
            input = Vector2.right;

        Vector2 dir = input.normalized;
        bool isBackDash = dir.x < 0f;
        float mult = isBackDash ? dashShortMultiplier : dashLongMultiplier;

        if (dir.y > 0.1f)
            currentDash = DashType.Up;
        else if (dir.x < 0f)
            currentDash = DashType.Back;
        else
            currentDash = DashType.Forward;

        rb.linearVelocity = dashSpeed * mult * dir;

        yield return new WaitForSeconds(dashTime);

        currentDash = DashType.None;
        isAttacking = false;
        isDashing = false;
    }

    // --------------------------------------------------
    // DAMAGE
    // --------------------------------------------------
    public void TakeDamage()
    {
        if (isDead) return;
        if (isInvulnerable) return;

        HitStop.Do(0.05f);
        ScreenShake.Shake(0.15f, 0.08f);

        currentHp--;

        if (currentHp <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageRoutine());
        }
    }

    private IEnumerator DamageRoutine()
    {
        isInvulnerable = true;

        isDashing = true;
        currentDash = DashType.Back;
        rb.linearVelocity = Vector2.left * hitKnockbackSpeed;

        yield return new WaitForSeconds(hitKnockbackTime);

        isDashing = false;
        currentDash = DashType.None;

        yield return new WaitForSeconds(invulTime - hitKnockbackTime);

        isInvulnerable = false;
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;

        // this is supposed to stop enemies' movement etc but it doesn't seem to be working correctly?
        Time.timeScale = 0f;

        // absolutely disgusting way of freezing enemies and buildings and the enemy spawner but whatever I don't want to do it in a smart way
        foreach (EnemyTako e in FindObjectsByType<EnemyTako>(FindObjectsSortMode.None))
            e.Freeze();

        foreach (EnemyBee e in FindObjectsByType<EnemyBee>(FindObjectsSortMode.None))
            e.Freeze();

        foreach (EnemyUFO e in FindObjectsByType<EnemyUFO>(FindObjectsSortMode.None))
            e.Freeze();

        foreach (ParallaxLayerBase p in FindObjectsByType<ParallaxLayerBase>(FindObjectsSortMode.None))
            p.toggleActive();
        
        foreach (NewEnemySpawner es in FindObjectsByType<NewEnemySpawner>(FindObjectsSortMode.None))
            es.setActive(false);


        GameOverUI.Instance.Show();
    }

    // --------------------------------------------------
    // GROUND CHECK
    // --------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Ground")) return;

        isGrounded = true;
        airDashAvailable = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Ground")) return;

        isGrounded = false;
    }

    // --------------------------------------------------
    // ANIMATOR FLAGS
    // --------------------------------------------------
    public bool IsJumping() => !isGrounded && !isDead;
    public bool IsDashing() => isDashing;
    public bool IsAttacking() => isAttacking;
    public bool IsDead() => isDead;
    public bool IsInvulnerable() => isInvulnerable;
    public DashType GetDashType() => currentDash;
}
