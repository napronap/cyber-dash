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

    private Rigidbody2D rb;

    private bool isGrounded = false;
    private bool isDashing = false;
    private bool airDashAvailable = true;

    public enum DashType
    {
        None = 0,
        Forward = 1,
        Back = 2,
        Up = 3,
    }

    private DashType currentDash = DashType.None;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerJump += (_, __) => Jump();
        GameInput.Instance.OnPlayerDash += (_, __) => TryDash();
    }

    private void FixedUpdate()
    {
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
        if (!isGrounded || isDashing)
            return;

        isGrounded = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
    }

    // --------------------------------------------------
    // DASH
    // --------------------------------------------------
    private void TryDash()
    {
        if (isDashing)
            return;

        if (!isGrounded && !airDashAvailable)
            return;

        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;

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
        isDashing = false;
    }

    // --------------------------------------------------
    // GROUND CHECK
    // --------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Ground"))
            return;

        isGrounded = true;
        airDashAvailable = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Ground"))
            return;

        isGrounded = false;
    }

    // --------------------------------------------------
    // ANIMATOR FLAGS
    // --------------------------------------------------
    public bool IsJumping() => !isGrounded;
    public bool IsDashing() => isDashing;
    public DashType GetDashType() => currentDash;
}
