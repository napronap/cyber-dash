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

    // 状态
    private bool _isSwiping;
    private float _baseZ;
    private float _nextSwipeTime;

    // 跳跃状态
    private Rigidbody2D _rb;
    private float _lastJumpTime;

    private void OnEnable()
    {
       
        _rb = GetComponent<Rigidbody2D>();

        // 自动查找左手（若未在 Inspector 指定）
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

        _baseZ = GetLocalZ(leftHand);
        _nextSwipeTime = Time.time + swipeInterval; // 定时开始
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
    }

    private void Update()
    {
        // 定时自动挥击：到时且未在挥击中则触发
        if (!_isSwiping && Time.time >= _nextSwipeTime)
        {
            TriggerSwipe();
            _nextSwipeTime = Time.time + swipeInterval;
        }

        // 自动跳跃：接地且冷却完成时起跳（不依赖落地挥击）
        if (autoJump && IsGrounded() && Time.time - _lastJumpTime >= jumpCooldown)
        {
            PerformForwardJump();
        }
    }

    private void LateUpdate()
    {
       
    }

    // 向前（左）跳跃（如需向右，把 v.x 改为 +forwardJumpSpeed）
    public void PerformForwardJump()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        if (_rb == null) return;
        if (!IsGrounded()) return; // 若需要空中连跳，移除此判断
        if (Time.time - _lastJumpTime < jumpCooldown) return;

        _lastJumpTime = Time.time;

        var v = _rb.linearVelocity; // 使用正确 API
        v.x = -forwardJumpSpeed;
        v.y = jumpUpVelocity;
        _rb.linearVelocity = v;
    }

    private void TriggerSwipe()
    {
        if (_isSwiping || leftHand == null) return;
        _isSwiping = true;
        StartCoroutine(SwipeRoutine());
    }

    private System.Collections.IEnumerator SwipeRoutine()
    {
        // 预备动作
        yield return RotateHandTo(_baseZ - swipeAngle * 0.4f);
        // 主挥动（中段命中）
        yield return RotateHandWithHitTo(_baseZ + swipeAngle);
        // 复位
        yield return RotateHandTo(_baseZ);
        // 冷却
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

            // 进度（0..1）
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
