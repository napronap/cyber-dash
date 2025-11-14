using UnityEngine;

/// <summary>
/// 2D �p�̃��W���[�������ꂽ�G�R���|�[�l���g
/// - Rigidbody2D �� Collider2D ��K�{��
/// - �C���X�y�N�^�Œ����\�ȃp�����[�^�iHP / �ړ� / �W�����v / ��s / �p�g���[���j
/// - Move/Jump/TakeDamage/Heal ���̌��J���\�b�h���
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField, Tooltip("�ő�HP")]
    private int maxHealth = 10;
    public int CurrentHealth { get; private set; }

    [Header("Movement")]
    [SerializeField, Tooltip("�ړ����x")]
    private float moveSpeed = 2f;

    [SerializeField, Tooltip("�W�����v�́i�C���p���X�j")]
    private float jumpForce = 5f;

    [Header("AI / Patrol")]
    [SerializeField, Tooltip("飛行可能（true で重力を無効化）")]
    private bool canFly = false;

    [Header("AI / Patrol")]
    [SerializeField, Tooltip("�����p�g���[�����s����")]
    private bool patrol = true;

    [SerializeField, Tooltip("�p�g���[���̉��������i�������j")]
    private float patrolDistance = 3f;

    [Header("Ground Check")]
    [SerializeField, Tooltip("�n�ʔ���Ɏg�� Transform�i���ݒ莞�̓I�u�W�F�N�g���S���牺�Ƀ`�F�b�N�j")]
    private Transform groundCheck;

    [SerializeField, Tooltip("�n�ʔ���̔��a"), Min(0.01f)]
    private float groundCheckRadius = 0.1f;

    [SerializeField, Tooltip("�n�ʃ��C���[")]
    private LayerMask groundLayers = ~0;

    // ����
    private Rigidbody2D rb;
    private Vector2 patrolOrigin;
    private int patrolDirection = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentHealth = maxHealth;
        patrolOrigin = transform.position;
        rb.gravityScale = canFly ? 0f : 1f;
    }

    void Update()
    {
        if (patrol)
        {
            float offset = transform.position.x - patrolOrigin.x;
            if (Mathf.Abs(offset) >= patrolDistance)
            {
                patrolDirection *= -1;
            }

            Move(patrolDirection);
        }
    }

    #region Movement API
    /// <summary>
    /// �����ړ��Bdirection �� -1..1 ��z��
    /// </summary>
    public void Move(float direction)
    {
        float clamped = Mathf.Clamp(direction, -1f, 1f);
        rb.linearVelocity = new Vector2(clamped * moveSpeed, rb.linearVelocity.y);
        // �����ύX�i�K�v�Ȃ�X�v���C�g�̔��]�Ȃǂ������ōs���j
        if (clamped != 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(clamped) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    /// <summary>
    /// �W�����v�B��s�\�Ȃ琂�����x�𒼐ڐݒ肵�ď㏸������i�K�v�ɉ����Ē����j
    /// </summary>
    public void Jump()
    {
        if (canFly)
        {
            // ��s���͏�����ɑ��x��^���č����𒲐��ł���悤�ɂ���
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            return;
        }

        if (IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// ��s���[�h�ؑ�
    /// </summary>
    public void SetCanFly(bool value)
    {
        canFly = value;
        rb.gravityScale = canFly ? 0f : 1f;
    }

    public void SetMoveSpeed(float speed) => moveSpeed = Mathf.Max(0f, speed);
    public void SetJumpForce(float value) => jumpForce = Mathf.Max(0f, value);
    public void SetMaxHealth(int value)
    {
        maxHealth = Mathf.Max(1, value);
        CurrentHealth = Mathf.Min(CurrentHealth, maxHealth);
    }
    #endregion

    #region Health API
    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
    }

    private void Die()
    {
        // �V���v���ɔj���B�v�[����G�t�F�N�g������΂����������ւ��Ă��������B
        Destroy(gameObject);
    }
    #endregion

    #region Utilities
    public bool IsGrounded()
    {
        Vector2 origin;
        if (groundCheck != null)
            origin = groundCheck.position;
        else
            origin = transform.position + Vector3.down * 0.1f;

        Collider2D hit = Physics2D.OverlapCircle(origin, groundCheckRadius, groundLayers);
        return hit != null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = (groundCheck != null) ? groundCheck.position : transform.position + Vector3.down * 0.1f;
        Gizmos.DrawWireSphere(origin, groundCheckRadius);
    }
    #endregion
}
