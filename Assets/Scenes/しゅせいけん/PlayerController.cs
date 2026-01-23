using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;       // 通常移動速度

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 18f;      // ダッシュ速度
    [SerializeField] private float dashDuration = 0.15f; // ダッシュ時間
    [SerializeField] private float dashCooldown = 0.35f; // クールダウン

    [Header("Backward Slowdown")]
    [SerializeField] private float backwardSlowFactor = 0.4f;     // 後ろ方向の減速率
    [SerializeField] private float backwardSlowDuration = 0.20f;  // 減速時間

    [Header("Components")]
    public Rigidbody2D rb;                 // Rigidbody2D
    public SpriteRenderer sr;              // スプライト反転用
    public GameObject dashHitbox;          // ダッシュ攻撃の当たり判定

    private Vector2 moveInput;
    private bool isDashing = false;
    private bool canDash = true;

    private void Update()
    {
        // --- 通常移動 ---
        if (!isDashing)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            moveInput = new Vector2(x, y).normalized;

            // 右向きの時だけ反転
            if (moveInput.x > 0) sr.flipX = false;
        }

        // --- ダッシュ入力（Xキー） ---
        if (Input.GetKeyDown(KeyCode.X) && canDash)
        {
            // 左方向の場合 → ダッシュ不可 → 減速だけ適用
            if (moveInput.x < 0)
            {
                StartCoroutine(BackwardSlowdown());
                return;
            }

            // 無入力の時 → デフォルトは右方向にダッシュ
            if (moveInput == Vector2.zero)
                moveInput = Vector2.right;

            StartCoroutine(PerformDash(moveInput));
        }
    }
    private void FixedUpdate()
    {
        // ダッシュ中は通常移動しない
        if (!isDashing)
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    // --- ダッシュ処理 ---
    private IEnumerator PerformDash(Vector2 direction)
    {
        isDashing = true;
        canDash = false;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        dashHitbox.SetActive(true);
        rb.linearVelocity = direction * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        dashHitbox.SetActive(false);
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // --- 左入力でダッシュを試みた時の減速処理 ---
    private IEnumerator BackwardSlowdown()
    {
        float originalSpeed = moveSpeed;

        // 減速開始
        moveSpeed *= backwardSlowFactor;

        yield return new WaitForSeconds(backwardSlowDuration);

        // 通常速度に戻す
        moveSpeed = originalSpeed;
    }
}