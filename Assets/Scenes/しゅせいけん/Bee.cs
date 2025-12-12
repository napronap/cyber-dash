using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bee : MonoBehaviour
{
    public enum State { Wander, PrepareDive, Dive, Recover }

    [Header("Stats")]
    [SerializeField] private int maxHp = 10;

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

    [Header("Life Collider")]
    [SerializeField, Tooltip("玩家触碰此碰撞体时会对当前敌人造成伤害（建议子对象 Collider2D，isTrigger=true）")]
    private Collider2D lifeCollider;

    [SerializeField, Tooltip("玩家触碰生命碰撞箱时，敌人失去的生命值")] private int selfDamageOnHit = 10;

    [Header("Roam Range")]
    [SerializeField, Tooltip("游荡时允许的水平/垂直范围（以开始位置为中心的半径）")]
    private Vector2 roamRange = new Vector2(1f, 0.5f);

    [Header("画面外に出たら消えるか？")]
    [SerializeField] private bool destroyWhenOffScreen = true;

    private State state = State.Wander;
    private Rigidbody2D rb;
    private float stateTime = 0f;
    private Vector2 startPos;
    private int currentHp;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        currentHp = maxHp;
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
    }

    void Start()
    {
        EnterState(State.Wander);
    }

    void FixedUpdate()
    {
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
                if (stateTime >= diveDuration) EnterState(State.Recover);
                break;
            case State.Recover:
                rb.linearVelocity = Vector2.zero;
                if (stateTime >= recoverDuration) EnterState(State.Wander);
                break;
        }
    }

    private void EnterState(State newState)
    {
        state = newState;
        stateTime = 0f;

        switch (state)
        {
            case State.Wander:
                if (attackCollider != null) attackCollider.enabled = false;
                startPos = transform.position;
                break;
            case State.PrepareDive:
                if (attackCollider != null) attackCollider.enabled = false;
                break;
            case State.Dive:
                if (attackCollider != null) attackCollider.enabled = true;
                break;
            case State.Recover:
                if (attackCollider != null) attackCollider.enabled = false;
                break;
        }
    }

    private void DoWander(float dt)
    {
        // 基于 startPos 与 roamRange 限制小范围游荡
        float t = Time.time * wanderSpeed;
        float targetX = startPos.x + Mathf.Sin(t) * Mathf.Min(wanderAmplitude, roamRange.x);
        float targetY = startPos.y + Mathf.Sin(t * 2f) * Mathf.Min(wanderVerticalAmplitude, roamRange.y);

        // 保证目标在 roamRange 内
        targetX = Mathf.Clamp(targetX, startPos.x - roamRange.x, startPos.x + roamRange.x);
        targetY = Mathf.Clamp(targetY, startPos.y - roamRange.y, startPos.y + roamRange.y);

        Vector2 targetPos = new Vector2(targetX, targetY);
        Vector2 toTarget = targetPos - (Vector2)transform.position;

        // 以较平滑的方式移动：设置速度为差值 / dt，但限制最大速率防止抖动
        Vector2 desiredVel = toTarget / Mathf.Max(dt, 0.0001f);
        float maxVel = wanderSpeed * 2f; // 限制一个合理的最大速度
        if (desiredVel.magnitude > maxVel) desiredVel = desiredVel.normalized * maxVel;
        rb.linearVelocity = desiredVel;
    }

    private void DoDive(float dt)
    {
        // V-shaped dive: first down-left, then up-left
        float half = diveDuration * 0.5f;
        Vector2 dirDownLeft = new Vector2(-1f, -1f).normalized;
        Vector2 dirUpLeft = new Vector2(-1f, 1f).normalized;

        if (stateTime < half)
        {
            // phase 0: down-left
            rb.linearVelocity = dirDownLeft * Mathf.Abs(diveSpeed);
        }
        else
        {
            // phase 1: up-left
            rb.linearVelocity = dirUpLeft * Mathf.Abs(diveSpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // lifeCollider check: player touching lifeCollider damages this enemy
        if (lifeCollider != null)
        {
            if (lifeCollider.IsTouching(other))
            {
                if (other.CompareTag("Player"))
                {
                    TakeDamage(selfDamageOnHit);
                }
                return;
            }
        }

        // attack collider check: only apply if attackCollider touched (or none assigned)
        if (attackCollider != null)
        {
            if (!attackCollider.IsTouching(other)) return;
        }

        if (other.CompareTag("Player"))
        {
            other.gameObject.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void TakeDamage(int dmg)
    {
        if (dmg <= 0) return;
        currentHp -= dmg;
        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }
    }

    // 调试时在编辑器中显示 roam 范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = Application.isPlaying ? (Vector3)startPos : transform.position;
        Gizmos.DrawWireCube(center, new Vector3(roamRange.x * 2f, roamRange.y * 2f, 0.1f));
    }
   
}
