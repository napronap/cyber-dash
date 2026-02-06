using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Gorilla : MonoBehaviour
{
    public enum State { Wander, Attack, Recover, Dead }

    [Header("Stats")]
    [SerializeField] private int maxHp = 2; // 需要两次击中才会死亡

    [Header("Movement")]
    [SerializeField, Tooltip("持续向左的移动速度（世界单位/秒）")] private float moveSpeed = 2.2f; // 稍微提升移动速度
    [SerializeField, Tooltip("游荡水平幅度（世界单位，已弃用）")] private float roamX = 1.5f;
    [SerializeField, Tooltip("游荡垂直幅度（世界单位）")] private float roamY = 0.3f;
    [SerializeField, Tooltip("游荡速度（影响频率，已弃用）")] private float wanderSpeed = 1f;
    [SerializeField, Tooltip("游荡时直线移动最大速度（已弃用）")]
    private float wanderMaxSpeed = 2f;

    [Header("Attack")]
    [SerializeField, Tooltip("攻击造成的伤害（身体触碰玩家时）")] private int attackDamage = 20;
    [SerializeField, Tooltip("攻击触发的时间间隔（秒）——每隔几秒播放一次攻击动画）")] private float attackInterval = 3f;

    [Header("Colliders")]
    [SerializeField, Tooltip("头部弱点碰撞体（玩家碰触头部会对敌人造成伤害）")] private Collider2D headCollider;
    [SerializeField, Tooltip("身体碰撞体（玩家接触身体会受到伤害）")] private Collider2D bodyCollider;
    [SerializeField, Tooltip("攻击判定碰撞体（攻击时启用以造成伤害）")] private Collider2D attackCollider;

    [Header("Animation frames")]
    [SerializeField, Tooltip("走路帧数组（4 帧）")] private Sprite[] walkFrames;
    [SerializeField, Tooltip("攻击帧数组（7 帧）")] private Sprite[] attackFrames;
    [SerializeField, Tooltip("死亡帧数组（9 帧）")] private Sprite[] deathFrames;
    [SerializeField, Tooltip("每帧时长（秒）")] private float frameDuration = 0.08f;
    [SerializeField, Tooltip("用于显示帧动画的 SpriteRenderer")] private SpriteRenderer spriteRenderer;

    [Header("Player filter")]
    [SerializeField, Tooltip("玩家标签（优先）")] private string playerTag = "Player";
    [SerializeField, Tooltip("玩家层掩码（如果不使用标签可设置此项）")] private LayerMask playerLayers = 0;

    [Header("Offscreen")]
    [SerializeField, Tooltip("离开屏幕左侧多少距离后销毁（世界单位）")] private float offscreenMargin = 0.1f;

    [Header("Audio")]
    [SerializeField, Tooltip("行动/攻击 音效（可循环用于走路）")] private AudioClip sfxAction;
    [SerializeField, Tooltip("受击 音效")] private AudioClip sfxHit;
    [SerializeField, Tooltip("死亡 音效")] private AudioClip sfxDeath;
    [SerializeField, Tooltip("是否将 action 音效循环播放（用于走路）")] private bool loopActionSound = false;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private int currentHp;
    private State state = State.Wander;
    private float stateTime = 0f;

    private float attackTimer = 0f;

    private bool _hasBeenVisible = false;

    private AudioSource _sfxSource;
    private System.Collections.IEnumerator _currentAnimCo;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        currentHp = maxHp;
        startPos = transform.position;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (headCollider != null) { headCollider.isTrigger = true; }
        if (bodyCollider != null) { bodyCollider.isTrigger = true; }
        if (attackCollider != null) { attackCollider.isTrigger = true; attackCollider.enabled = false; }

        attackTimer = attackInterval;

        if (!TryGetComponent<AudioSource>(out _sfxSource))
        {
            // Add one only if none exists
            _sfxSource = gameObject.AddComponent<AudioSource>();
        }
        if (_sfxSource != null)
        {
            _sfxSource.playOnAwake = false;
            _sfxSource.loop = false;
        }
    }

    void Start()
    {
        if (spriteRenderer != null && walkFrames != null && walkFrames.Length > 0)
        {
            spriteRenderer.sprite = walkFrames[0];
            PlayFrames(walkFrames, loop: true);
        }

        if (loopActionSound && sfxAction != null)
        {
            if (_sfxSource != null)
            {
                _sfxSource.clip = sfxAction;
                _sfxSource.loop = true;
                _sfxSource.Play();
            }
        }
    }

    void Update()
    {
        // Track visibility: only consider destroying after it has been visible once
        if (spriteRenderer != null)
        {
            if (spriteRenderer.isVisible)
            {
                _hasBeenVisible = true;
            }
            else if (_hasBeenVisible && state != State.Dead)
            {
                // check left-side exit
                if (Camera.main != null)
                {
                    float z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
                    Vector3 leftWorld = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.5f, z));
                    if (transform.position.x < leftWorld.x - offscreenMargin)
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    // fallback: destroy if not visible after being visible
                    Destroy(gameObject);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (state == State.Dead) return;

        stateTime += Time.fixedDeltaTime;

        if (state == State.Wander)
        {
            // 持续向左移动
            rb.linearVelocity = new Vector2(-Mathf.Abs(moveSpeed), rb.linearVelocity.y);

            // 定时触发攻击动画
            attackTimer -= Time.fixedDeltaTime;
            if (attackTimer <= 0f)
            {
                attackTimer = attackInterval;
                StartCoroutine(AttackRoutine());
            }
        }
        else if (state == State.Attack || state == State.Recover)
        {
            // 固定期间不自己改变速度，攻击协程会设置 velocity
        }
    }

    // DetectPlayerInRange() 保留但不再用于定时攻击逻辑，供可能的扩展用途
    private Collider2D DetectPlayerInRange()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, 0f);
        foreach (var c in hits)
        {
            if (c == null) continue;
            if (!string.IsNullOrEmpty(playerTag) && c.CompareTag(playerTag)) return c;
            if (playerLayers != 0 && ((1 << c.gameObject.layer) & playerLayers) != 0) return c;
        }
        return null;
    }

    // 无参数的攻击协程（仅播放动画并在中间启用攻击判定）
    private System.Collections.IEnumerator AttackRoutine()
    {
        state = State.Attack;
        stateTime = 0f;

        // 停止移动
        rb.linearVelocity = Vector2.zero;

        // 播放攻击音效
        if (_sfxSource != null && sfxAction != null && !loopActionSound)
        {
            _sfxSource.PlayOneShot(sfxAction);
        }

        // 播放攻击帧（非循环）并在中间启用攻击碰撞
        if (attackFrames != null && attackFrames.Length > 0)
        {
            // stop walk animation
            if (_currentAnimCo != null)
            {
                StopCoroutine(_currentAnimCo);
                _currentAnimCo = null;
            }

            float dur = Mathf.Max(0.01f, frameDuration);
            int midFrame = attackFrames.Length / 2;

            for (int i = 0; i < attackFrames.Length; i++)
            {
                if (spriteRenderer != null) spriteRenderer.sprite = attackFrames[i];

                // 在中间几帧打开攻击判定
                if (i == midFrame)
                {
                    if (attackCollider != null) attackCollider.enabled = true;
                }
                if (i == midFrame + 1)
                {
                    if (attackCollider != null) attackCollider.enabled = false;
                }

                yield return new WaitForSeconds(dur);
            }
        }

        // 恢复到行走动画
        if (walkFrames != null && walkFrames.Length > 0)
        {
            PlayFrames(walkFrames, loop: true);
        }

        state = State.Recover;
        // 短暂恢复期
        yield return new WaitForSeconds(0.1f);
        state = State.Wander;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // 玩家触碰头部 -> 对敌人造成伤害
        if (headCollider != null && headCollider.IsTouching(other))
        {
            TakeDamage(1); // 玩家踩头造成的伤害量，改为 1
            return;
        }

        // 身体触碰玩家 -> 对玩家造成伤害
        if (bodyCollider != null && bodyCollider.IsTouching(other))
        {
            if (other.CompareTag(playerTag) || (playerLayers != 0 && ((1 << other.gameObject.layer) & playerLayers) != 0))
            {
                other.gameObject.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
            }
            return;
        }

        // 攻击判定碰撞体命中玩家（攻击时造成伤害）
        if (attackCollider != null && attackCollider.IsTouching(other))
        {
            if (other.CompareTag(playerTag) || (playerLayers != 0 && ((1 << other.gameObject.layer) & playerLayers) != 0))
            {
                other.gameObject.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        if (dmg <= 0) return;
        currentHp -= dmg;

        // 受击音效
        if (_sfxSource != null && sfxHit != null)
        {
            _sfxSource.PlayOneShot(sfxHit);
        }

        if (currentHp <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        if (state == State.Dead) return;
        state = State.Dead;

        StopAllCoroutines();

        // 停止物理和碰撞
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            // stop any looping action sound
            if (_sfxSource != null && _sfxSource.isPlaying && loopActionSound)
                _sfxSource.Stop();

            // play death sfx
            if (_sfxSource != null && sfxDeath != null)
                _sfxSource.PlayOneShot(sfxDeath);

            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        var cols = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < cols.Length; i++) cols[i].enabled = false;

        // 如果有死亡帧则逐帧播放后销毁
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
        for (int i = 0; i < deathFrames.Length; i++)
        {
            if (spriteRenderer != null) spriteRenderer.sprite = deathFrames[i];
            yield return new WaitForSeconds(dur);
        }
        Destroy(gameObject);
    }

    // 共通的帧动画播放（可循环）
    private void PlayFrames(Sprite[] frames, bool loop)
    {
        if (spriteRenderer == null || frames == null || frames.Length == 0) return;
        if (_currentAnimCo != null)
        {
            StopCoroutine(_currentAnimCo);
            _currentAnimCo = null;
        }
        _currentAnimCo = FramePlayer(frames, loop);
        StartCoroutine(_currentAnimCo);
    }

    private System.Collections.IEnumerator FramePlayer(Sprite[] frames, bool loop)
    {
        int index = 0;
        float dur = Mathf.Max(0.01f, frameDuration);
        while (true)
        {
            if (spriteRenderer != null) spriteRenderer.sprite = frames[index];
            yield return new WaitForSeconds(dur);
            index++;
            if (index >= frames.Length)
            {
                if (loop) index = 0; else break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.0f);

        Gizmos.color = Color.yellow;
        Vector3 center = Application.isPlaying ? (Vector3)startPos : transform.position;
        Gizmos.DrawWireCube(center, new Vector3(roamX * 2f, roamY * 2f, 0.1f));
    }
}
