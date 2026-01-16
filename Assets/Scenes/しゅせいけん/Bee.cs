using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bee : MonoBehaviour
{
    public enum State { Wander, PrepareDive, Dive, Recover }
    public enum DeathFallMode { Gravity, ConstantVelocity }

    [Header("Wander")]
    [SerializeField, Tooltip("左右游荡的水平幅度（世界单位）")] private float wanderAmplitude = 0.5f;
    [SerializeField, Tooltip("游荡时垂直微振幅（世界单位）")] private float wanderVerticalAmplitude = 0.1f;
    [SerializeField, Tooltip("游荡速度（影响正弦频率）")] private float wanderSpeed = 1f;
    [SerializeField, Tooltip("游荡持续时间（秒）")] private float wanderDuration = 2f;

    [Header("Prepare Dive")]
    [SerializeField, Tooltip("准备下冲的静默时间（秒）")] private float prepareDuration = 1.5f;

    [Header("Dive")]
    [SerializeField, Tooltip("下冲速度（世界单位/秒）")] private float diveSpeed = 6f;
    [SerializeField, Tooltip("攻击持续时间（秒）")] private float diveDuration = 1f;
    [SerializeField, Tooltip("碰撞时对玩家造成的伤害")] private int attackDamage = 10;

    [Header("Recover")]
    [SerializeField, Tooltip("下冲后静默时间（秒）")] private float recoverDuration = 1.5f;

    [Header("Attack Collider")]
    [SerializeField, Tooltip("用于实际造成伤害的碰撞体（建议设置为子对象的 Collider2D 并设为 isTrigger）")]
    private Collider2D attackCollider;

    [Header("Life Collider (致命部位)")]
    [SerializeField, Tooltip("玩家触碰此碰撞体时敌人立刻死亡（建议子对象 Collider2D，isTrigger=true）")]
    private Collider2D lifeCollider;

    [Header("Roam Range")]
    [SerializeField, Tooltip("游荡时允许的水平/垂直范围（以开始位置为中心的半径）")]
    private Vector2 roamRange = new Vector2(1f, 0.5f);

    [Header("画面外に出たら消えるか？")]
    [SerializeField] private bool destroyWhenOffScreen = true;

    [Header("Frame Animation (帧序列)")]
    [SerializeField, Tooltip("移动帧序列（游荡/准备/恢复状态下循环播放）")]
    private Sprite[] moveFrames;
    [SerializeField, Tooltip("攻击帧序列（俯冲时完整播放一遍）")]
    private Sprite[] attackFrames;
    [SerializeField, Tooltip("死亡帧序列（顺序播放一次后销毁）")]
    private Sprite[] deathFrames;
    [SerializeField, Tooltip("每帧持续时间（秒）")]
    private float frameDuration = 0.1f;
    [SerializeField, Tooltip("播放动画的 SpriteRenderer")]
    private SpriteRenderer spriteRenderer;

    [SerializeField, Tooltip("死亡销毁延时（秒，若有死亡帧则忽略，由帧序列决定销毁时机）")]
    private float deathDestroyDelay = 2f;

    [Header("Death Falling (死亡下落效果)")]
    [SerializeField, Tooltip("死亡下落模式：Gravity=开启重力；ConstantVelocity=常速向下")]
    private DeathFallMode deathFallMode = DeathFallMode.Gravity;
    [SerializeField, Tooltip("死亡时的重力缩放（仅 Gravity 模式）")]
    private float deathGravityScale = 2.0f;
    [SerializeField, Tooltip("死亡时常量下落速度（仅 ConstantVelocity 模式，负值向下）")]
    private float deathFallSpeedY = -4.0f;

    [Header("Ground (地面碰撞)")]
    [SerializeField, Tooltip("主体碰撞体（非触发，用于与地面发生物理碰撞）。若为空将自动取自身 Collider2D")]
    private Collider2D bodyCollider;
    [SerializeField, Tooltip("地面图层（用于限制攻击/死亡时穿地）")]
    private LayerMask groundLayers = ~0;

    private State state = State.Wander;
    private Rigidbody2D rb;
    private float stateTime = 0f;
    private Vector2 startPos;
    private bool isDead = false;

    // 帧动画内部状态
    private System.Collections.IEnumerator _currentAttackCo;
    private System.Collections.IEnumerator _currentMoveCo;
    private bool _isPlayingAttackCycle;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // 主体物理碰撞体（非触发），用于与地面阻挡
        if (bodyCollider == null) bodyCollider = GetComponent<Collider2D>();
        if (bodyCollider != null) bodyCollider.isTrigger = false;

        startPos = transform.position;

        if (attackCollider != null)
        {
            attackCollider.isTrigger = true;
            attackCollider.enabled = false;
        }

        if (lifeCollider != null)
        {
            lifeCollider.isTrigger = true;
            lifeCollider.enabled = true;
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            if (moveFrames != null && moveFrames.Length > 0)
                spriteRenderer.sprite = moveFrames[0];
            else if (attackFrames != null && attackFrames.Length > 0)
                spriteRenderer.sprite = attackFrames[0];
        }
    }

    void Start()
    {
        EnterState(State.Wander);
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            if (deathFallMode == DeathFallMode.ConstantVelocity && rb != null)
            {
                rb.linearVelocity = new Vector2(0f, deathFallSpeedY);
            }
            return;
        }

        float dt = Time.fixedDeltaTime;
        stateTime += dt;

        switch (state)
        {
            case State.Wander:
                DoWander(dt);
                if (stateTime >= wanderDuration) EnterState(State.PrepareDive);
                break;
            case State.PrepareDive:
                rb.linearVelocity = Vector2.zero;
                if (stateTime >= prepareDuration) EnterState(State.Dive);
                break;
            case State.Dive:
                DoDive(dt);
                // 仅在时间到并且攻击帧已完整播放后，才进入恢复
                if (stateTime >= diveDuration && !_isPlayingAttackCycle)
                {
                    EnterState(State.Recover);
                }
                break;
            case State.Recover:
                rb.linearVelocity = Vector2.zero;
                if (attackCollider != null) attackCollider.enabled = false;
                if (stateTime >= recoverDuration) EnterState(State.Wander);
                break;
        }
    }

    private void EnterState(State newState)
    {
        if (isDead) return;

        state = newState;
        stateTime = 0f;

        switch (state)
        {
            case State.Wander:
                if (attackCollider != null) attackCollider.enabled = false;
                startPos = transform.position;
                StartMoveLoop();
                break;
            case State.PrepareDive:
                if (attackCollider != null) attackCollider.enabled = false;
                StartMoveLoop();
                break;
            case State.Dive:
                if (attackCollider != null) attackCollider.enabled = true;
                StopMoveLoop();

                // 确保俯冲持续时间至少覆盖攻击帧总时长，避免中途切换
                float attackTotal = (attackFrames != null) ? attackFrames.Length * Mathf.Max(0.01f, frameDuration) : 0f;
                if (attackTotal > 0f)
                {
                    diveDuration = Mathf.Max(diveDuration, attackTotal);
                }

                StartAttackCycleOnce();
                break;
            case State.Recover:
                if (attackCollider != null) attackCollider.enabled = false;
                StartMoveLoop();
                break;
        }
    }

    private void DoWander(float dt)
    {
        float t = Time.time * wanderSpeed;
        float targetX = startPos.x + Mathf.Sin(t) * Mathf.Min(wanderAmplitude, roamRange.x);
        float targetY = startPos.y + Mathf.Sin(t * 2f) * Mathf.Min(wanderVerticalAmplitude, roamRange.y);

        targetX = Mathf.Clamp(targetX, startPos.x - roamRange.x, startPos.x + roamRange.x);
        targetY = Mathf.Clamp(targetY, startPos.y - roamRange.y, startPos.y + roamRange.y);

        Vector2 targetPos = new Vector2(targetX, targetY);
        Vector2 toTarget = targetPos - (Vector2)transform.position;

        Vector2 desiredVel = toTarget / Mathf.Max(dt, 0.0001f);
        float maxVel = wanderSpeed * 2f;
        if (desiredVel.magnitude > maxVel) desiredVel = desiredVel.normalized * maxVel;
        rb.linearVelocity = desiredVel;
    }

    private void DoDive(float dt)
    {
        float half = diveDuration * 0.5f;
        Vector2 dirDownLeft = new Vector2(-1f, -1f).normalized;
        Vector2 dirUpLeft = new Vector2(-1f, 1f).normalized;

        if (stateTime < half)
        {
            rb.linearVelocity = dirDownLeft * Mathf.Abs(diveSpeed);
        }
        else
        {
            rb.linearVelocity = dirUpLeft * Mathf.Abs(diveSpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || isDead) return;

        // 关键致命部位：玩家触碰即死亡
        if (lifeCollider != null && lifeCollider.IsTouching(other))
        {
            if (other.CompareTag("Player"))
            {
                Die();
            }
            return;
        }

        // 攻击碰撞：只有在攻击碰撞箱触发且命中玩家时造成伤害
        if (attackCollider != null)
        {
            if (!attackCollider.IsTouching(other)) return;
        }

        if (other.CompareTag("Player"))
        {
            other.gameObject.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
        }
    }

    // 与地面发生物理碰撞时的处理
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsGround(collision.collider)) return;

        if (isDead)
        {
            // 死亡落地后停住
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        if (state == State.Dive)
        {
            // 俯冲触地后切换到恢复状态
            EnterState(State.Recover);
        }
    }

    private bool IsGround(Collider2D col)
    {
        int mask = 1 << col.gameObject.layer;
        return (groundLayers.value & mask) != 0;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (attackCollider != null) attackCollider.enabled = false;
        if (lifeCollider != null) lifeCollider.enabled = false;
        if (bodyCollider != null) bodyCollider.enabled = true; // 保证主体碰撞还在

        // 启用死亡下落效果
        if (rb != null)
        {
            switch (deathFallMode)
            {
                case DeathFallMode.Gravity:
                    rb.gravityScale = Mathf.Max(0f, deathGravityScale);
                    rb.linearVelocity = new Vector2(0f, 0f);
                    break;
                case DeathFallMode.ConstantVelocity:
                    rb.gravityScale = 0f;
                    rb.linearVelocity = new Vector2(0f, deathFallSpeedY);
                    break;
            }
        }

        // 停止所有动画协程
        StopMoveLoop();
        StopAttackAnimIfAny();

        // 播放死亡帧序列后销毁；若未配置死亡帧，则按延时销毁
        if (spriteRenderer != null && deathFrames != null && deathFrames.Length > 0)
        {
            StartCoroutine(DeathRoutine());
        }
        else
        {
            Destroy(gameObject, Mathf.Max(0.01f, deathDestroyDelay));
        }
    }

    // 移动动画：循环播放
    private void StartMoveLoop()
    {
        if (spriteRenderer == null || moveFrames == null || moveFrames.Length == 0) return;
        StopMoveLoop();
        _currentMoveCo = MoveLoop();
        StartCoroutine(_currentMoveCo);
    }

    private void StopMoveLoop()
    {
        if (_currentMoveCo != null)
        {
            StopCoroutine(_currentMoveCo);
            _currentMoveCo = null;
        }
    }

    private System.Collections.IEnumerator MoveLoop()
    {
        float dur = Mathf.Max(0.01f, frameDuration);
        int i = 0;
        while (!isDead && state != State.Dive)
        {
            if (spriteRenderer != null) spriteRenderer.sprite = moveFrames[i];
            i = (i + 1) % moveFrames.Length;
            yield return new WaitForSeconds(dur);
        }
    }

    // 攻击动画：一次性播放
    private void StartAttackCycleOnce()
    {
        if (spriteRenderer == null || attackFrames == null || attackFrames.Length == 0) return;

        StopAttackAnimIfAny();
        _currentAttackCo = AttackCycleOnce();
        _isPlayingAttackCycle = true;
        StartCoroutine(_currentAttackCo);
    }

    private void StopAttackAnimIfAny()
    {
        if (_currentAttackCo != null)
        {
            StopCoroutine(_currentAttackCo);
            _currentAttackCo = null;
        }
        _isPlayingAttackCycle = false;
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
        _currentAttackCo = null;

        // 回到移动循环（若仍处于非俯冲状态）
        StartMoveLoop();
    }

    // 死亡动画：一次性播放后销毁
    private System.Collections.IEnumerator DeathRoutine()
    {
        float dur = Mathf.Max(0.01f, frameDuration);

        for (int i = 0; i < deathFrames.Length; i++)
        {
            if (spriteRenderer != null) spriteRenderer.sprite = deathFrames[i];
            yield return new WaitForSeconds(dur);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = Application.isPlaying ? (Vector3)startPos : transform.position;
        Gizmos.DrawWireCube(center, new Vector3(roamRange.x * 2f, roamRange.y * 2f, 0.1f));
    }
}