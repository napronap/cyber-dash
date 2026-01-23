using UnityEngine;
using System.Collections;

public class PlayerControllerAlt : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;       // �ʏ�ړ����x

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 18f;      // �_�b�V�����x
    [SerializeField] private float dashDuration = 0.15f; // �_�b�V������
    [SerializeField] private float dashCooldown = 0.35f; // �N�[���_�E��

    [Header("Backward Slowdown")]
    [SerializeField] private float backwardSlowFactor = 0.4f;     // �������̌�����
    [SerializeField] private float backwardSlowDuration = 0.20f;  // ��������

    [Header("Components")]
    public Rigidbody2D rb;                 // Rigidbody2D
    public SpriteRenderer sr;              // �X�v���C�g���]�p
    public GameObject dashHitbox;          // �_�b�V���U���̓����蔻��

    private Vector2 moveInput;
    private bool isDashing = false;
    private bool canDash = true;

    private void Update()
    {
        // --- �ʏ�ړ� ---
        if (!isDashing)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            moveInput = new Vector2(x, y).normalized;

            // �E�����̎��������]
            if (moveInput.x > 0) sr.flipX = false;
        }

        // --- �_�b�V�����́iX�L�[�j ---
        if (Input.GetKeyDown(KeyCode.X) && canDash)
        {
            // �������̏ꍇ �� �_�b�V���s�� �� ���������K�p
            if (moveInput.x < 0)
            {
                StartCoroutine(BackwardSlowdown());
                return;
            }

            // �����͂̎� �� �f�t�H���g�͉E�����Ƀ_�b�V��
            if (moveInput == Vector2.zero)
                moveInput = Vector2.right;

            StartCoroutine(PerformDash(moveInput));
        }
    }
    private void FixedUpdate()
    {
        // �_�b�V�����͒ʏ�ړ����Ȃ�
        if (!isDashing)
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    // --- �_�b�V������ ---
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

    // --- �����͂Ń_�b�V�������݂����̌������� ---
    private IEnumerator BackwardSlowdown()
    {
        float originalSpeed = moveSpeed;

        // �����J�n
        moveSpeed *= backwardSlowFactor;

        yield return new WaitForSeconds(backwardSlowDuration);

        // �ʏ푬�x�ɖ߂�
        moveSpeed = originalSpeed;
    }
}