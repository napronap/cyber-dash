using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class TESETO : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Tooltip("・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ?・ｽ・ｽ・ｽx")]
    private float moveSpeed = 4f;
    [SerializeField, Tooltip("・ｽ・ｽ?・ｽﾍ度・ｽi?・ｽu??・ｽ・ｽ・ｽ・ｽ・ｽx・ｽj")]
    private float jumpForce = 6f;
    [SerializeField, Tooltip("・ｽ・・ｽ・ｽ・ｽI・ｽe?(0-1)")]
    private float airControl = 0.6f;

    [Header("Facing")]
    [SerializeField, Tooltip("・ｽ・ｽ?・ｽp SpriteRenderer・ｽi・ｽ・ｽ?・ｽu?・ｽ・ｽ??・ｽ・ｽj")]
    private SpriteRenderer spriteRenderer;

    [Header("Ground Check")]
    [SerializeField, Tooltip("・ｽn・ｽﾊ費ｿｽ・ｽ・ｽp Transform・ｽi・ｽ・ｽ?・ｽu?・ｽp・ｽ{・ｽ・ｽ・ｽc・ｽ・ｽ・ｽ・ｽ・ｽj")]
    private Transform groundCheck;
    [SerializeField, Tooltip("・ｽn・ｽﾊ費ｿｽ・ｽ阡ｼ・ｽa"), Min(0.01f)]
    private float groundCheckRadius = 0.12f;
    [SerializeField, Tooltip("・ｽn・ｽ・ｽ?")]
    private LayerMask groundLayers = ~0;

    [Header("Frame Animation (・ｽ・ｽ・ｽ・ｽ?・ｽ・ｽ?・ｽ・ｽ?)")]
    [SerializeField, Tooltip("・ｽz??・ｽi・ｽz?・ｽj")]
    private Sprite[] runFrames;
    [SerializeField, Tooltip("・ｽ@・ｽ・ｽ?・ｽi・ｽz?・ｽj")]
    private Sprite[] backFrames;
    [SerializeField, Tooltip("・ｽU??・ｽi・ｽd・ｽ・ｽ・ｽ齊滂ｿｽj")]
    private Sprite[] attackFrames;
    [SerializeField, Tooltip("・ｽ・ｽ?・ｽU??・ｽi・ｽd・ｽ・ｽ・ｽ齊滂ｿｽj")]
    private Sprite[] jumpAttackFrames;
    [SerializeField, Tooltip("・ｽ・ｽ・ｽS?・ｽi・ｽd・ｽ・ｽ・ｽ齊滂ｿｽj")]
    private Sprite[] deathFrames;

    [Header("Frame Duration (sec) - Per Animation")]
    [SerializeField, Tooltip("螂碑ｷ大勘逕ｻ豈丞ｸｧ譌ｶ髣ｴ・育ｧ抵ｼ・)]
    private float runFrameDuration = 0.1f;
    [SerializeField, Tooltip("蜷朱蜉ｨ逕ｻ豈丞ｸｧ譌ｶ髣ｴ・育ｧ抵ｼ・)]
    private float backFrameDuration = 0.1f;
    [SerializeField, Tooltip("謾ｻ蜃ｻ蜉ｨ逕ｻ豈丞ｸｧ譌ｶ髣ｴ・育ｧ抵ｼ・)]
    private float attackFrameDuration = 0.06f;
    [SerializeField, Tooltip("霍ｳ霍・判蜃ｻ蜉ｨ逕ｻ豈丞ｸｧ譌ｶ髣ｴ・育ｧ抵ｼ・)]
    private float jumpAttackFrameDuration = 0.06f;
    [SerializeField, Tooltip("豁ｻ莠｡蜉ｨ逕ｻ豈丞ｸｧ譌ｶ髣ｴ・育ｧ抵ｼ・)]
    private float deathFrameDuration = 0.08f;

    [Header("Visual Size Normalize")]
    [SerializeField, Tooltip("繝輔Ξ繝ｼ繝縺斐→縺ｮ隕九◆逶ｮ繧ｵ繧､繧ｺ蟾ｮ繧偵せ繧ｱ繝ｼ繝ｫ陬懈ｭ｣縺ｧ蜷ｸ蜿弱☆繧・)]
    private bool normalizeVisualSize = true;

    [SerializeField, Tooltip("蝓ｺ貅悶せ繝励Λ繧､繝茨ｼ域悴謖・ｮ壹↑繧・runFrames[0] 縺ｪ縺ｩ縺九ｉ閾ｪ蜍募叙蠕暦ｼ・)]
    private Sprite referenceSprite;

    [Header("Action Settings")]
    [SerializeField, Tooltip("・ｽU??・ｽ・ｽ・ｽ・ｽ?・ｽ・ｽ・ｽ?")]
    private bool lockMoveWhileAttacking = true;

    [SerializeField, Tooltip("・ｽU?・ｽ・ｽ・ｽ・ｽp・ｽG?・ｽ・ｽi・ｽ・ｽ?・ｽC・ｽs?・ｽu?・ｽ・ｽ・ｽd?・ｽ・ｽj")]
    private Collider2D attackHitbox;
    [SerializeField, Tooltip("・ｽU?・ｽ・ｽ???・ｽi・ｽb・ｽj・ｽC0 ?・ｽg・ｽp?・ｽ・ｽ??")]
    private float attackActiveTime = 0f;

    private Rigidbody2D rb;
    private Collider2D bodyCollider;

    private float inputX;
    private bool isGrounded;

    private bool isAttacking;
    private bool isDead;

    private IEnumerator _animCo;
    private AnimState _animState = AnimState.Idle;

    // 隕九◆逶ｮ縺縺代ｒ陬懈ｭ｣縺吶ｋ縺溘ａ縲ヾpriteRenderer 蛛ｴ縺ｮ蝓ｺ貅悶せ繧ｱ繝ｼ繝ｫ繧剃ｿ晄戟
    private Vector3 _baseVisualLocalScale;
    private Vector2 _refWorldSize;

    private enum AnimState
    {
        Idle,
        Run,
        Back,
        Attack,
        JumpAttack,
        Die
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            _baseVisualLocalScale = spriteRenderer.transform.localScale;
        }
        else
        {
            _baseVisualLocalScale = Vector3.one;
        }

        if (referenceSprite == null)
        {
            referenceSprite = GetFirstAvailableFrame();
        }

        if (referenceSprite != null)
        {
            var s = referenceSprite.bounds.size;
            _refWorldSize = new Vector2(Mathf.Max(0.0001f, s.x), Mathf.Max(0.0001f, s.y));
        }
        else
        {
            _refWorldSize = new Vector2(1f, 1f);
        }

        if (spriteRenderer != null)
        {
            var first = GetFirstAvailableFrame();
            if (first != null)
            {
                SetSprite(first);
            }
        }

        if (attackHitbox != null)
        {
            attackHitbox.isTrigger = true;
            attackHitbox.enabled = false;
        }
    }

    void Update()
    {
        if (isDead) return;

        bool pressA = Input.GetKey(KeyCode.A);
        bool pressD = Input.GetKey(KeyCode.D);

        inputX = 0f;
        if (pressA) inputX = -1f;
        else if (pressD) inputX = 1f;

        // 隕九◆逶ｮ縺ｮ蜷代″蜿崎ｻ｢・・priteRenderer 蛛ｴ縺ｮ縺ｿ・・
        if (spriteRenderer != null && Mathf.Abs(inputX) > 0.01f)
        {
            var ls = spriteRenderer.transform.localScale;
            ls.x = Mathf.Abs(ls.x) * (inputX >= 0 ? 1f : -1f);
            spriteRenderer.transform.localScale = ls;
        }

        if (!isAttacking)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartAttack();
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                StartJumpAttack();
            }
        }

        if (!isAttacking)
        {
            if (inputX > 0.01f)
                SetLoopAnim(AnimState.Run, runFrames, runFrameDuration);
            else if (inputX < -0.01f)
                SetLoopAnim(AnimState.Back, backFrames, backFrameDuration);
            else
                SetIdle();
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        isGrounded = IsGrounded();

        if (lockMoveWhileAttacking && isAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float targetVx = inputX * moveSpeed;
        float control = isGrounded ? 1f : Mathf.Clamp01(airControl);

        Vector2 v = rb.linearVelocity;
        float accel = moveSpeed * 5f;
        v.x = Mathf.MoveTowards(v.x, targetVx, accel * control * Time.fixedDeltaTime);
        rb.linearVelocity = v;
    }

    private void StartAttack()
    {
        isAttacking = true;
        SetOneShotAnim(AnimState.Attack, attackFrames, attackFrameDuration, onDone: () =>
        {
            isAttacking = false;
            _animState = AnimState.Idle;
        });

        StartCoroutine(AttackHitboxRoutine());
    }

    private void StartJumpAttack()
    {
        isAttacking = true;

        if (isGrounded)
        {
            var v = rb.linearVelocity;
            v.y = jumpForce;
            rb.linearVelocity = v;
        }

        SetOneShotAnim(AnimState.JumpAttack, jumpAttackFrames, jumpAttackFrameDuration, onDone: () =>
        {
            isAttacking = false;
            _animState = AnimState.Idle;
        });

        StartCoroutine(AttackHitboxRoutine());
    }

    private IEnumerator AttackHitboxRoutine()
    {
        if (attackHitbox == null) yield break;

        float duration = attackActiveTime;
        if (duration <= 0f)
        {
            Sprite[] frames = (_animState == AnimState.JumpAttack) ? jumpAttackFrames : attackFrames;
            float durPerFrame = (_animState == AnimState.JumpAttack) ? jumpAttackFrameDuration : attackFrameDuration;
            duration = GetFramesDuration(frames, durPerFrame);
        }
        duration = Mathf.Max(0.05f, duration);

        attackHitbox.enabled = true;
        yield return new WaitForSeconds(duration);
        attackHitbox.enabled = false;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        isAttacking = false;

        StopAllCoroutines();
        StopAnimCoroutine();

        if (attackHitbox != null) attackHitbox.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
        }

        if (bodyCollider != null) bodyCollider.enabled = false;

        SetOneShotAnim(AnimState.Die, deathFrames, deathFrameDuration, onDone: () => { });
    }

    private bool IsGrounded()
    {
        Vector2 origin = (groundCheck != null)
            ? (Vector2)groundCheck.position
            : (Vector2)transform.position + Vector2.down * 0.1f;

        Collider2D hit = Physics2D.OverlapCircle(origin, groundCheckRadius, groundLayers);
        return hit != null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 origin = (groundCheck != null) ? groundCheck.position : transform.position + Vector3.down * 0.1f;
        Gizmos.DrawWireSphere(origin, groundCheckRadius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ApplyEnemyDamage(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null || collision.collider == null) return;
        ApplyEnemyDamage(collision.collider);
    }

<<<<<<< Updated upstream
    // ｽｱﾍ貍ﾒﾖ蠻ｲﾌ衲・Tag=Enemy ｽﾓｴ･ﾊｱｿﾛ 1 ｵ翹愑・ｵ｣ｻｺﾔ PlayerAttack ﾗﾓﾅｲﾌ蟠･ｷ｢
=======
>>>>>>> Stashed changes
    private void ApplyEnemyDamage(Collider2D col)
    {
        if (isDead) return;
        if (col == null) return;
        if (!col.CompareTag("Enemy")) return;

<<<<<<< Updated upstream
        // ﾈ｡ﾍ貍ﾒﾖ蠻ｲﾌ螢ｨｹﾒﾔﾚﾍｬﾒｻ GameObject ﾉﾏｵﾄ Collider2D｣ｩ
        var bodyCollider = GetComponent<Collider2D>();
        if (bodyCollider == null) return;

        // ﾖｻﾓﾐﾖ蠻ｲﾌ衲・ﾐﾈﾋｷ｢ﾉ悅ﾓｴ･ﾊｱｲﾅｽ睛翹ﾋｺｦ
        if (!bodyCollider.IsTouching(col)) return;
=======
        var body = GetComponent<Collider2D>();
        if (body == null) return;
        if (!body.IsTouching(col)) return;
>>>>>>> Stashed changes

        var hp = GetComponent<PlayerHealth>();
        if (hp != null)
        {
            hp.TakeDamage(1);
        }
    }

    private void SetIdle()
    {
        if (_animState == AnimState.Idle) return;
        _animState = AnimState.Idle;
        StopAnimCoroutine();

        if (spriteRenderer != null)
        {
            var first = GetFirstAvailableFrame();
            if (first != null) SetSprite(first);
        }
    }

    private void SetLoopAnim(AnimState state, Sprite[] frames, float frameDur)
    {
        if (spriteRenderer == null) return;
        if (frames == null || frames.Length == 0) return;
        if (_animState == state) return;

        _animState = state;
        PlayFrames(frames, loop: true, frameDur: frameDur, onDone: null);
    }

    private void SetOneShotAnim(AnimState state, Sprite[] frames, float frameDur, System.Action onDone)
    {
        if (spriteRenderer == null)
        {
            onDone?.Invoke();
            return;
        }

        _animState = state;

        if (frames == null || frames.Length == 0)
        {
            onDone?.Invoke();
            return;
        }

        PlayFrames(frames, loop: false, frameDur: frameDur, onDone: onDone);
    }

    private void PlayFrames(Sprite[] frames, bool loop, float frameDur, System.Action onDone)
    {
        if (spriteRenderer == null || frames == null || frames.Length == 0)
        {
            onDone?.Invoke();
            return;
        }

        StopAnimCoroutine();
        _animCo = FramePlayer(frames, loop, frameDur, onDone);
        StartCoroutine(_animCo);
    }

    private IEnumerator FramePlayer(Sprite[] frames, bool loop, float frameDur, System.Action onDone)
    {
        int index = 0;
        float dur = Mathf.Max(0.01f, frameDur);

        while (true)
        {
            if (spriteRenderer != null)
            {
                SetSprite(frames[index]);
            }

            yield return new WaitForSeconds(dur);

            index++;
            if (index >= frames.Length)
            {
                if (loop)
                {
                    index = 0;
                }
                else
                {
                    break;
                }
            }
        }

        onDone?.Invoke();
    }

    private void StopAnimCoroutine()
    {
        if (_animCo != null)
        {
            StopCoroutine(_animCo);
            _animCo = null;
        }
    }

    private Sprite GetFirstAvailableFrame()
    {
        if (runFrames != null && runFrames.Length > 0) return runFrames[0];
        if (backFrames != null && backFrames.Length > 0) return backFrames[0];
        if (attackFrames != null && attackFrames.Length > 0) return attackFrames[0];
        if (jumpAttackFrames != null && jumpAttackFrames.Length > 0) return jumpAttackFrames[0];
        if (deathFrames != null && deathFrames.Length > 0) return deathFrames[0];
        return null;
    }

    private static float GetFramesDuration(Sprite[] frames, float frameDur)
    {
        if (frames == null || frames.Length == 0) return 0f;
        return frames.Length * Mathf.Max(0.01f, frameDur);
    }

    private void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        ApplyVisualScaleForSprite(sprite);
    }

    private void ApplyVisualScaleForSprite(Sprite sprite)
    {
        if (!normalizeVisualSize) return;
        if (spriteRenderer == null) return;
        if (sprite == null) return;

        var size = sprite.bounds.size;
        float sy = Mathf.Max(0.0001f, size.y);
        float mul = _refWorldSize.y / sy;

        // 蜿崎ｻ｢・育ｬｦ蜿ｷ・峨・莉翫・隕九◆逶ｮ繧ｹ繧ｱ繝ｼ繝ｫ縺ｮ x 繧堤ｶｭ謖√＠縺溘＞縺ｮ縺ｧ縲∫ｬｦ蜿ｷ縺縺第ｮ九＠縺ｦ邨ｶ蟇ｾ蛟､繧定｣懈ｭ｣
        float signX = Mathf.Sign(spriteRenderer.transform.localScale.x);
        if (signX == 0f) signX = 1f;

        spriteRenderer.transform.localScale = new Vector3(
            Mathf.Abs(_baseVisualLocalScale.x) * mul * signX,
            _baseVisualLocalScale.y * mul,
            _baseVisualLocalScale.z);
    }
}