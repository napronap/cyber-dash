using UnityEngine;

public class Enemyneko : enemyKaisho
{
    [Header("Left Hand")]
    [SerializeField] private Transform leftHand;

    [Header("Swipe")]
    [SerializeField, Tooltip("挥动角度（度）")]
    private float swipeAngle = 35f;
    [SerializeField, Tooltip("挥动速度（度/秒）")]
    private float swipeSpeed = 240f;
    [SerializeField, Tooltip("每次挥动后的冷却时间（秒）")]
    private float swipeCooldown = 1.0f;
    [SerializeField, Tooltip("自动挥击的时间间隔（秒）")]
    private float swipeInterval = 1.5f;

    [Header("Hit")]
    [SerializeField, Tooltip("伤害数值")]
    private float damage = 10f;
    [SerializeField, Tooltip("命中检测半径")]
    private float hitRadius = 0.4f;
    [SerializeField, Tooltip("命中掩码")]
    private LayerMask hitMask;

    [Header("Jump (Forward to Left)")]
    [SerializeField, Tooltip("向前（左）跳的水平速度")]
    private float forwardJumpSpeed = 3.5f;
    [SerializeField, Tooltip("跳跃的向上速度")]
    private float jumpUpVelocity = 6.0f;
    [SerializeField, Tooltip("起跳冷却（秒）")]
    private float jumpCooldown = 0.6f;

    [Header("Auto Jump")]
    [SerializeField, Tooltip("是否自动跳跃（落地后冷却完成自动再跳）")]
    private bool autoJump = true;

    [Header("Frame Animation (插入图片使用)")]
    [SerializeField, Tooltip("通常/攻击帧序列（完整播一遍）")]
    private Sprite[] attackFrames;
    [SerializeField, Tooltip("死亡帧序列（顺序播放一次）")]
    private Sprite[] deathFrames;
    [SerializeField, Tooltip("每帧持续时间（秒）")]
    private float frameDuration = 0.1f;
    [SerializeField, Tooltip("播放动画的 SpriteRenderer")]
    private SpriteRenderer spriteRenderer;

    // 状态
    private bool _isSwiping;
    private float _baseZ;
    private float _nextSwipeTime;

    // 跳跃状态
    private Rigidbody2D _rb;
    private float _lastJumpTime;
    private bool _isJumping;
    private float _jumpGraceUntil;

    // 动画内部状态
    private System.Collections.IEnumerator _currentAnimCo;
    private bool _isPlayingAttackCycle;
    private bool _isDying;

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody2D>();
        _isDying = false;
        _isJumping = false;
        _jumpGraceUntil = 0f;
        _isPlayingAttackCycle = false;

        if (leftHand == null)
        {
            var trs = GetComponentsInChildren<Transform>(true);
            foreach (var t in trs)
            {
                var name = t.name.ToLowerInvariant();
                if (name.Contains("left") || name.Contains("左")) { leftHand = t; break; }
            }
        }
        if (leftHand == null)
        {
            Debug.LogWarning($"{nameof(Enemyneko)}: 请在 Inspector 指定左手 Transform。");
            return;
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null && attackFrames != null && attackFrames.Length > 0)
        {
            spriteRenderer.sprite = attackFrames[0];
        }

        _baseZ = GetLocalZ(leftHand);
        _nextSwipeTime = Time.time + swipeInterval;
    }

    private void OnValidate()
    {
        swipeAngle = Mathf.Clamp(swipeAngle, 0f, 180f);
        swipeSpeed = Mathf.Max(1f, swipeSpeed);
        swipeCooldown = Mathf.Max(0f, swipeCooldown);
        swipeInterval = Mathf.Max(0.1f, swipeInterval);
        hitRadius = Mathf.Max(0.01f, hitRadius);
        damage = Mathf.Max(0f, damage);
        forwardJumpSpeed = Mathf.Max(0f, forwardJumpSpeed);
        jumpUpVelocity = Mathf.Max(0f, jumpUpVelocity);
        jumpCooldown = Mathf.Max(0.05f, jumpCooldown);
        frameDuration = Mathf.Max(0.01f, frameDuration);
    }

    private void Update()
    {
        if (_isDying) return;

        if (_isJumping && Time.time >= _jumpGraceUntil && IsGrounded())
        {
            _isJumping = false;
        }

        if (_isJumping && !_isSwiping && Time.time >= _nextSwipeTime)
        {
            TriggerSwipe();
            _nextSwipeTime = Time.time + swipeInterval;
        }

        if (autoJump && IsGrounded() && Time.time - _lastJumpTime >= jumpCooldown)
        {
            PerformForwardJump();
        }
    }

    public void PerformForwardJump()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        if (_rb == null) return;
        if (!IsGrounded()) return;
        if (Time.time - _lastJumpTime < jumpCooldown) return;

        _lastJumpTime = Time.time;

        _isJumping = true;
        _jumpGraceUntil = Time.time + 0.2f;

        var v = _rb.linearVelocity;
        v.x = -forwardJumpSpeed;
        v.y = jumpUpVelocity;
        _rb.linearVelocity = v;

        if (!_isPlayingAttackCycle && spriteRenderer != null && attackFrames != null && attackFrames.Length > 0)
        {
            StartAttackCycleOnce();
        }
    }

    private void StartAttackCycleOnce()
    {
        if (_currentAnimCo != null)
        {
            StopCoroutine(_currentAnimCo);
            _currentAnimCo = null;
        }
        _currentAnimCo = AttackCycleOnce();
        _isPlayingAttackCycle = true;
        StartCoroutine(_currentAnimCo);
    }

    private System.Collections.IEnumerator AttackCycleOnce()
    {
        float dur = Mathf.Max(0.01f, frameDuration);

        for (int i = 0; i < attackFrames.Length; i++)
        {
            if (spriteRenderer != null) spriteRenderer.sprite = attackFrames[i];
            yield return new WaitForSeconds(dur);
        }
            _isPlayingAttackCycle = false;
        _currentAnimCo = null;

        if (!_isJumping || _isDying)
        {
            if (spriteRenderer != null && attackFrames != null && attackFrames.Length > 0)
            {
                spriteRenderer.sprite = attackFrames[0];
            }
        }
    }

    private void TriggerSwipe()
    {
        if (_isSwiping || leftHand == null) return;
        _isSwiping = true;
        StartCoroutine(SwipeRoutine());
    }

    private System.Collections.IEnumerator SwipeRoutine()
    {
        yield return RotateHandTo(_baseZ - swipeAngle * 0.4f);
        yield return RotateHandWithHitTo(_baseZ + swipeAngle);
        yield return RotateHandTo(_baseZ);
        yield return new WaitForSeconds(swipeCooldown);
        _isSwiping = false;
    }

    private System.Collections.IEnumerator RotateHandTo(float targetZ)
    {
        const float timeout = 2f;
        float end = Time.time + timeout;
        float current = GetLocalZ(leftHand);
        while (!Mathf.Approximately(current, targetZ))
        {
            current = Mathf.MoveTowards(current, targetZ, swipeSpeed * Time.deltaTime);
            SetLocalZ(leftHand, current);
            if (Time.time > end) break;
            yield return null;
        }
        SetLocalZ(leftHand, targetZ);
    }

    private System.Collections.IEnumerator RotateHandWithHitTo(float targetZ)
    {
        const float timeout = 2f;
        float end = Time.time + timeout;
        float startZ = GetLocalZ(leftHand);
        float total = Mathf.Abs(targetZ - startZ);
        bool hitApplied = false;

        while (!Mathf.Approximately(GetLocalZ(leftHand), targetZ))
        {
            float current = Mathf.MoveTowards(GetLocalZ(leftHand), targetZ, swipeSpeed * Time.deltaTime);
            SetLocalZ(leftHand, current);

            float traveled = Mathf.Abs(current - startZ);
            float t = total > 0f ? traveled / total : 1f;

            if (!hitApplied && t > 0.35f && t < 0.75f)
            {
                ApplySwipeHit();
                hitApplied = true;
            }

            if (Time.time > end) break;
            yield return null;
        }
        SetLocalZ(leftHand, targetZ);
    }

    private void ApplySwipeHit()
    {
        var pos = leftHand.position;
        var hits = Physics2D.OverlapCircleAll(pos, hitRadius, hitMask);
        for (int i = 0; i < hits.Length; i++)
        {
            var dmg = hits[i].GetComponent<IDamageABLE>();
            if (dmg != null) dmg.TakeDamage(damage);
        }
    }

    // 供 DamageReceiver 调用的死亡入口
    public void KillByDamage()
    {
        StartDeath();
    }

    public void StartDeath()
    {
        if (_isDying) return;
        _isDying = true;

        StopAllCoroutines();
        _isSwiping = false;
        _isJumping = false;
        _isPlayingAttackCycle = false;
        _currentAnimCo = null;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
        }
        var cols = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].enabled = false;
        }

        if (leftHand != null)
        {
            SetLocalZ(leftHand, _baseZ);
        }

        if (spriteRenderer != null && deathFrames != null && deathFrames.Length > 0)
        {
            StartCoroutine(DeathRoutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator DeathRoutine()
    {
        float dur = Mathf.Max(0.01f, frameDuration);

        if (spriteRenderer != null && deathFrames != null && deathFrames.Length > 0)
        {
            for (int i = 0; i < deathFrames.Length; i++)
            {
                spriteRenderer.sprite = deathFrames[i];
                yield return new WaitForSeconds(dur);
            }
        }

        Destroy(gameObject);
    }

    // 删除原来的“直接碰到就死亡”判定，改由 DamageZone 触发
    private void OnCollisionEnter2D(Collision2D collision) { }
    private void OnTriggerEnter2D(Collider2D other) { }

    private static float GetLocalZ(Transform t)
    {
        var e = t.localEulerAngles;
        float z = e.z;
        if (z > 180f) z -= 360f;
        return z;
    }

    private static void SetLocalZ(Transform t, float z)
    {
        var e = t.localEulerAngles;
        e.z = (z < 0f) ? z + 360f : z;
        t.localEulerAngles = e;
    }
}

public interface IDamageABLE
{
    void TakeDamage(float amount);
}
