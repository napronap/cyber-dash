using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class TESETO : MonoBehaviour
{
    [Header("Movement")]
<<<<<<< HEAD
    [SerializeField, Tooltip("・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ?・ｽ・ｽ・ｽx")]
    private float moveSpeed = 4f;
    [SerializeField, Tooltip("・ｽ・ｽ?・ｽﾍ度・ｽi?・ｽu??・ｽ・ｽ・ｽ・ｽ・ｽx・ｽj")]
    private float jumpForce = 6f;
    [SerializeField, Tooltip("・ｽ・・ｽ・ｽ・ｽI・ｽe?(0-1)")]
=======
    [SerializeField, Tooltip("Movement speed on the X-axis"), Min(0.01f)]
    private float moveSpeed = 6.5f;

    [SerializeField, Tooltip("Jump force")]
    private float jumpForce = 6f;

    [SerializeField, Tooltip("Air control (0-1)")]
>>>>>>> bcafac6eed285f17d0f550baf192ebe6ea97997c
    private float airControl = 0.6f;

    [Header("Back Hop (A)")]
    [SerializeField, Tooltip("Back hop horizontal distance (world units)"), Min(0.01f)]
    private float backHopDistance = 1.2f;

    [SerializeField, Tooltip("Back hop duration (seconds)"), Min(0.01f)]
    private float backHopDuration = 0.18f;

    [SerializeField, Tooltip("Back hop upward force (initial Y velocity)"), Min(0.01f)]
    private float backHopUpForce = 4.5f;

    [SerializeField, Tooltip("Duration of each frame for back hop animation (seconds); larger value = slower"), Min(0.01f)]
    private float backHopFrameDuration = 0.14f;

    [Header("Jump (K)")]
    [SerializeField, Tooltip("Cooldown time (seconds) for Jump (K)"), Min(0f)]
    private float jumpCooldown = 1f;

    [SerializeField, Tooltip("Jump (K) upward force (initial Y velocity)"), Min(0.01f)]
    private float jumpUpForceK = 6f;

    [SerializeField, Tooltip("Jump (K) gravity scale while in air (bigger = faster falling)"), Min(0.01f)]
    private float jumpGravityScaleK = 3f;

    [Header("Jump Attack (J)")]
    [SerializeField, Tooltip("Jump attack upward force (initial Y velocity)"), Min(0.01f)]
    private float jumpAttackUpForce = 6f;

    [SerializeField, Tooltip("Duration of each frame for jump attack animation (seconds); larger value = slower"), Min(0.01f)]
    private float jumpAttackFrameDuration = 0.08f;

    [SerializeField, Tooltip("Extra gravity scale while jump-attacking (bigger = faster falling)"), Min(1f)]
    private float jumpAttackGravityScale = 3f;

    [SerializeField, Tooltip("Cooldown time (seconds) for Jump Attack (J)"), Min(0f)]
    private float jumpAttackCooldown = 1f;

    [SerializeField, Header("Fall Tuning"), Tooltip("Gravity multiplier while falling (y < 0). Bigger = faster fall"), Min(1f)]
    private float fallGravityMultiplier = 8f;

    [SerializeField, Tooltip("Max downward speed (terminal velocity). More negative = faster fall"), Min(0.01f)]
    private float maxFallSpeed = 30f;

    [Header("Facing")]
<<<<<<< HEAD
    [SerializeField, Tooltip("・ｽ・ｽ?・ｽp SpriteRenderer・ｽi・ｽ・ｽ?・ｽu?・ｽ・ｽ??・ｽ・ｽj")]
=======
    [SerializeField, Tooltip("SpriteRenderer to be flipped for facing direction")]
>>>>>>> bcafac6eed285f17d0f550baf192ebe6ea97997c
    private SpriteRenderer spriteRenderer;

    [SerializeField, Tooltip("Fix facing direction: true=always right, false=always left")]
    private bool alwaysFaceRight = true;

    [Header("Ground Check")]
<<<<<<< HEAD
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
=======
    [SerializeField, Tooltip("Transform for ground detection position")]
    private Transform groundCheck;

    [SerializeField, Tooltip("Radius for ground detection"), Min(0.01f)]
    private float groundCheckRadius = 0.12f;

    [SerializeField, Tooltip("Layers considered as ground")]
    private LayerMask groundLayers = ~0;

    [Header("Damage / Body")]
    [SerializeField, Tooltip("Body collider used for taking damage. Only this collider will trigger PlayerHealth damage. If null, will use Collider2D on this GameObject.")]
    private Collider2D bodyCollider;

    [Header("Attack Hitbox")]
    [SerializeField, Tooltip("Trigger collider for attacks (child Collider2D). Disabled by default, enable briefly during attack.")]
    private Collider2D attackHitbox;

    [SerializeField, Tooltip("Duration (in seconds) for which the attack collider is active")]
    private float attackHitboxActiveTime = 0.08f;

    [Header("Frame Animation Slots")]
    [SerializeField, Tooltip("Idle (loop)")]
    private Sprite[] idleFrames;

    [SerializeField, Tooltip("Running to the right (loop)")]
    private Sprite[] runFrames;

    [SerializeField, Tooltip("Retreat/backward walk (loop). Used when holding A on ground.")]
    private Sprite[] retreatFrames;

    [SerializeField, Tooltip("Back hop/retreat frames (loop OK; will be used during back hop)")]
    private Sprite[] backFrames;

    [SerializeField, Tooltip("Jump (plays once)")]
    private Sprite[] jumpFrames;

    [SerializeField, Tooltip("Attack (plays once)")]
    private Sprite[] attackFrames;

    [SerializeField, Tooltip("Jump Attack (plays once)")]
    private Sprite[] jumpAttackFrames;

    [Header("Death")]
    [SerializeField, Tooltip("Death (plays once; keeps last frame)")]
    private Sprite[] deathFrames;

    [SerializeField, Tooltip("Duration of each frame for death animation (seconds)"), Min(0.01f)]
    private float deathFrameDuration = 0.08f;

    [SerializeField, Tooltip("Death scale (X=width, Y=height)")]
    private Vector2 deathSpriteScale = Vector2.one;

    [SerializeField, Tooltip("Death knockback X speed (world units/sec). Bigger = stronger"), Min(0f)]
    private float deathKnockbackSpeedX = 2.5f;

    [SerializeField, Tooltip("Optional upward speed on death (0 = none)"), Min(0f)]
    private float deathKnockbackUpSpeedY = 0.5f;

    [Header("Death Falling (Player)")]
    [SerializeField, Tooltip("If true, when player dies in air, let the body fall to ground"), Min(0f)]
    private bool fallToGroundOnDeath = true;

    [SerializeField, Tooltip("Extra gravity scale while falling after death (bigger = faster falling)"), Min(0f)]
    private float deathFallGravityScale = 3f;

    [SerializeField, Tooltip("Stop horizontal movement on death")]
    private bool stopHorizontalOnDeath = true;

    [Header("Frame Timing")]
    [SerializeField, Tooltip("Duration of each frame for normal actions (in seconds)")]
    private float frameDuration = 0.1f;

    [SerializeField, Tooltip("Duration of each frame for attacks (in seconds); smaller value = faster")]
    private float attackFrameDuration = 0.05f;

    [Header("Per-Animation Sprite Scale")]
    [SerializeField, Tooltip("Idle scale (X=width, Y=height)")]
    private Vector2 idleSpriteScale = Vector2.one;

    [SerializeField, Tooltip("Running scale (X=width, Y=height)")]
    private Vector2 runSpriteScale = Vector2.one;

    [SerializeField, Tooltip("Back hop scale (X=width, Y=height)")]
    private Vector2 backSpriteScale = Vector2.one;

    [SerializeField, Tooltip("Jump scale (X=width, Y=height)")]
    private Vector2 jumpSpriteScale = Vector2.one;

    [SerializeField, Tooltip("Attack scale (X=width, Y=height)")]
    private Vector2 attackSpriteScale = Vector2.one;

    [SerializeField, Tooltip("Jump Attack scale (X=width, Y=height)")]
    private Vector2 jumpAttackSpriteScale = Vector2.one;
>>>>>>> bcafac6eed285f17d0f550baf192ebe6ea97997c

    [Header("Back Hop Input")]
    [SerializeField, Tooltip("Max time (seconds) between two A key presses to trigger back hop"), Min(0.01f)]
    private float doubleTapTime = 0.25f;

    [SerializeField, Tooltip("Retreat speed multiplier when holding A (0-1 = slower, >1 = faster)"), Min(0f)]
    private float retreatSpeedMultiplier = 0.7f;

    private Rigidbody2D rb;
    private Collider2D bodyCollider;

    private float inputX;
<<<<<<< HEAD
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

=======
    private bool jumpPressed;
    private bool attackPressed;
    private bool isGrounded;

    private bool _isActionLocked;
    private System.Collections.IEnumerator _animCo;
    private System.Collections.IEnumerator _hitboxCo;

    private bool _airAnimFinished;

    private bool _jumpedThisAir;
    private float _lastAirY;

    private Vector3 _spriteBaseLocalScale;
    private System.Collections.IEnumerator _backHopCo;

    private bool _isJumpKActive;
    private bool _isJumpAttackActive;
    private float _baseGravityScale;

    private float _lastADownTime = -999f;
    private float _nextJumpAttackTime;

    private float _nextJumpTime;

    private bool _isDead;

    private enum LoopAnim
    {
        None,
        Idle,
        Run,
        Back
    }

    private enum OneShotAnim
    {
        None,
        Jump,
        Attack,
        JumpAttack,
        BackHop,
        Death
    }

    private LoopAnim _currentLoop = LoopAnim.None;

>>>>>>> bcafac6eed285f17d0f550baf192ebe6ea97997c
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
<<<<<<< HEAD
        bodyCollider = GetComponent<Collider2D>();
=======

        if (bodyCollider == null)
        {
            bodyCollider = GetComponent<Collider2D>();
        }
>>>>>>> 1f1912ee2bd7adf5a72e405c8011cda27e27f144

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
<<<<<<< HEAD

        if (spriteRenderer != null)
=======

        if (spriteRenderer != null)
        {
            _spriteBaseLocalScale = spriteRenderer.transform.localScale;
        }

        if (attackHitbox != null)
        {
            attackHitbox.isTrigger = true;
            attackHitbox.enabled = false;
        }

        if (rb != null)
        {
            _baseGravityScale = rb.gravityScale;
        }

        _nextJumpTime = Time.time;

        ApplyFixedFacing();
        PlayLoop(LoopAnim.Idle);
    }

    void Update()
    {
        if (_isDead)
        {
            return;
        }

        ReadMovementInput();

        UpdateGroundedState();

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Time.time - _lastADownTime <= doubleTapTime)
            {
                TryBackHop();
                _lastADownTime = -999f;
            }
            else
            {
                _lastADownTime = Time.time;
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
>>>>>>> bcafac6eed285f17d0f550baf192ebe6ea97997c
        {
<<<<<<< HEAD
            _baseVisualLocalScale = spriteRenderer.transform.localScale;
        }
        else
        {
            _baseVisualLocalScale = Vector3.one;
=======
            if (Time.time >= _nextJumpTime)
            {
                jumpPressed = true;
                _nextJumpTime = Time.time + Mathf.Max(0f, jumpCooldown);
            }
>>>>>>> 1f1912ee2bd7adf5a72e405c8011cda27e27f144
        }

<<<<<<< HEAD
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
=======
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
>>>>>>> bcafac6eed285f17d0f550baf192ebe6ea97997c
        {
            attackPressed = true;
        }

<<<<<<< HEAD
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
=======
        if (Input.GetKeyDown(KeyCode.J))
        {
            TryJumpAttack();
        }

        UpdateAnimationState();
>>>>>>> bcafac6eed285f17d0f550baf192ebe6ea97997c
    }

    void FixedUpdate()
    {
<<<<<<< HEAD
<<<<<<< HEAD
        if (isDead) return;

        isGrounded = IsGrounded();

        if (lockMoveWhileAttacking && isAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float targetVx = inputX * moveSpeed;
=======
=======
        if (_isDead)
        {
            return;
        }

>>>>>>> 1f1912ee2bd7adf5a72e405c8011cda27e27f144
        UpdateGroundedState();

        ApplyAirGravityTuning();

        if (_backHopCo == null)
        {
            ApplyHorizontalMovement();

            if (jumpPressed && isGrounded)
            {
                PerformJump();
            }
        }

        jumpPressed = false;

        if (!isGrounded && _jumpedThisAir)
        {
            _lastAirY = Mathf.Max(_lastAirY, transform.position.y);
        }
    }

    public void PlayDeath()
    {
        if (_isDead) return;

        _isDead = true;

        jumpPressed = false;
        attackPressed = false;
        inputX = 0f;

        if (_hitboxCo != null)
        {
            StopCoroutine(_hitboxCo);
            _hitboxCo = null;
        }

        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }

        StopCurrentAnimation();

        if (rb != null)
        {
            float faceSign = 1f;
            if (spriteRenderer != null)
            {
                faceSign = Mathf.Sign(spriteRenderer.transform.localScale.x);
                if (faceSign == 0f) faceSign = 1f;
            }

            float knockDir = -faceSign;
            var v = rb.linearVelocity;
            v.x = knockDir * Mathf.Abs(deathKnockbackSpeedX);
            v.y = Mathf.Max(v.y, deathKnockbackUpSpeedY);
            rb.linearVelocity = v;
        }

        _isActionLocked = true;
        _currentLoop = LoopAnim.None;

        ApplySpriteScale(deathSpriteScale);
        PlayOnce(deathFrames, keepLastFrame: true, customFrameDuration: deathFrameDuration, oneShot: OneShotAnim.Death);
    }

    private void ReadMovementInput()
    {
        float x = 0f;

        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;

        inputX = Mathf.Clamp(x, -1f, 1f);
        ApplyFixedFacing();
    }

    private void UpdateGroundedState()
    {
        isGrounded = IsGrounded();

        if (isGrounded)
        {
            _isJumpKActive = false;
            _isJumpAttackActive = false;
        }
    }

    private void ApplyAirGravityTuning()
    {
        if (rb == null) return;

        if (isGrounded)
        {
            rb.gravityScale = _baseGravityScale;
            return;
        }

        float targetGravity = _baseGravityScale;

        if (_isJumpAttackActive)
        {
            targetGravity = Mathf.Max(targetGravity, jumpAttackGravityScale);
        }

        if (_isJumpKActive)
        {
            targetGravity = Mathf.Max(targetGravity, jumpGravityScaleK);
        }

        if (rb.linearVelocity.y < 0f)
        {
            targetGravity = Mathf.Max(targetGravity, _baseGravityScale * fallGravityMultiplier);
        }

        rb.gravityScale = targetGravity;

        Vector2 v = rb.linearVelocity;
        if (v.y < -maxFallSpeed)
        {
            v.y = -maxFallSpeed;
            rb.linearVelocity = v;
        }
    }

    private void ApplyHorizontalMovement()
    {
        if (rb == null) return;

        float speed = moveSpeed;
        if (inputX < 0f)
        {
            speed *= Mathf.Max(0f, retreatSpeedMultiplier);
        }

        float targetVx = inputX * speed;
>>>>>>> bcafac6eed285f17d0f550baf192ebe6ea97997c
        float control = isGrounded ? 1f : Mathf.Clamp01(airControl);

        Vector2 v = rb.linearVelocity;
        float accel = moveSpeed * 5f;
        v.x = Mathf.MoveTowards(v.x, targetVx, accel * control * Time.fixedDeltaTime);
        rb.linearVelocity = v;
    }

<<<<<<< HEAD
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
=======
    private void PerformJump()
    {
        if (rb == null) return;

        Vector2 v = rb.linearVelocity;
        v.y = Mathf.Max(v.y, Mathf.Max(0.01f, jumpUpForceK));
        rb.linearVelocity = v;

        _isJumpKActive = true;

        _jumpedThisAir = true;
        _lastAirY = transform.position.y;
        _airAnimFinished = false;

        if (!_isActionLocked)
        {
            ApplySpriteScale(jumpSpriteScale);
            PlayOnce(jumpFrames, keepLastFrame: true, customFrameDuration: frameDuration, oneShot: OneShotAnim.Jump);
        }
    }

    private void UpdateAnimationState()
    {
        if (!_isActionLocked)
        {
            if (isGrounded)
            {
                if (attackPressed)
                {
                    attackPressed = false;
                    PlayOnce(attackFrames, keepLastFrame: false, customFrameDuration: attackFrameDuration, oneShot: OneShotAnim.Attack);
                    ActivateAttackHitboxOnce();
                }
                else if (Mathf.Abs(inputX) > 0.01f)
                {
                    if (inputX < 0f)
                    {
                        PlayLoop(LoopAnim.Back);
                    }
                    else
                    {
                        PlayLoop(LoopAnim.Run);
                    }
                }
                else
                {
                    PlayLoop(LoopAnim.Idle);
                }
            }
            else
            {
                EnsureAirAnim();
            }
        }
        else
        {
            attackPressed = false;
        }
    }

    private void TryJumpAttack()
    {
        if (Time.time < _nextJumpAttackTime) return;
        if (_backHopCo != null) return;
        if (jumpAttackFrames == null || jumpAttackFrames.Length == 0) return;
        if (rb == null) return;

        _nextJumpAttackTime = Time.time + Mathf.Max(0f, jumpAttackCooldown);

        StopCurrentAnimation();

        _isActionLocked = false;

        Vector2 v = rb.linearVelocity;
        v.y = Mathf.Max(v.y, jumpAttackUpForce);
        rb.linearVelocity = v;

        _isJumpAttackActive = true;
        rb.gravityScale = Mathf.Max(_baseGravityScale, jumpAttackGravityScale);

        _jumpedThisAir = true;
        _lastAirY = transform.position.y;

        PlayOnce(jumpAttackFrames, keepLastFrame: false, customFrameDuration: jumpAttackFrameDuration, oneShot: OneShotAnim.JumpAttack);
        ActivateAttackHitboxOnce();
    }

    private void TryBackHop()
    {
        if (_isActionLocked) return;

        if (!isGrounded) return;

        if (backFrames == null || backFrames.Length == 0) return;

        if (_backHopCo != null)
        {
            StopCoroutine(_backHopCo);
            _backHopCo = null;
        }

        _backHopCo = BackHopRoutine();
        StartCoroutine(_backHopCo);
    }

    private System.Collections.IEnumerator BackHopRoutine()
    {
        _isActionLocked = true;

        ApplySpriteScale(backSpriteScale);
        PlayOnce(backFrames, keepLastFrame: true, customFrameDuration: backHopFrameDuration, oneShot: OneShotAnim.BackHop);

        float totalAnimTime = backHopDuration;
        if (backFrames != null && backFrames.Length > 0)
        {
            totalAnimTime = Mathf.Max(backHopDuration, backFrames.Length * Mathf.Max(0.01f, backHopFrameDuration));
        }

        float vx = -(backHopDistance / Mathf.Max(0.01f, totalAnimTime));

        Vector2 v0 = rb.linearVelocity;
        v0.x = vx;
        v0.y = Mathf.Max(v0.y, backHopUpForce);
        rb.linearVelocity = v0;

        _jumpedThisAir = true;
        _lastAirY = transform.position.y;

        float t = 0f;
        while (t < totalAnimTime)
        {
            Vector2 v = rb.linearVelocity;
            v.x = vx;
            rb.linearVelocity = v;

            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _isActionLocked = false;
        _backHopCo = null;

        PlayLoop(LoopAnim.Idle);
    }

    private void StopCurrentAnimation()
    {
        if (_animCo != null)
        {
            StopCoroutine(_animCo);
            _animCo = null;
        }
    }

    private void EnsureAirAnim()
    {
        if (jumpFrames == null || jumpFrames.Length == 0) return;

        if (_animCo == null)
        {
            PlayOnce(jumpFrames, keepLastFrame: true, customFrameDuration: frameDuration, oneShot: OneShotAnim.Jump);
        }
    }

    private void ActivateAttackHitboxOnce()
    {
        if (attackHitbox == null) return;

        if (_hitboxCo != null)
        {
            StopCoroutine(_hitboxCo);
            _hitboxCo = null;
        }

        _hitboxCo = AttackHitboxRoutine();
        StartCoroutine(_hitboxCo);
    }

    private System.Collections.IEnumerator AttackHitboxRoutine()
    {
        attackHitbox.enabled = true;
        yield return new WaitForSeconds(Mathf.Max(0.01f, attackHitboxActiveTime));
        if (attackHitbox != null) attackHitbox.enabled = false;
        _hitboxCo = null;
    }

    private void ApplyFixedFacing()
    {
        if (spriteRenderer == null) return;

        var ls = spriteRenderer.transform.localScale;
        float absX = Mathf.Abs(ls.x);
        ls.x = alwaysFaceRight ? absX : -absX;
        spriteRenderer.transform.localScale = ls;
    }

    private void PlayLoop(LoopAnim loop)
    {
        if (_isActionLocked) return;
        if (_currentLoop == loop) return;

        _currentLoop = loop;

        Sprite[] frames = loop switch
        {
            LoopAnim.Run => runFrames,
            LoopAnim.Back => (retreatFrames != null && retreatFrames.Length > 0) ? retreatFrames : runFrames,
            _ => idleFrames,
        };

        Vector2 scale = loop switch
        {
            LoopAnim.Run => runSpriteScale,
            LoopAnim.Back => runSpriteScale,
            _ => idleSpriteScale,
        };

        ApplySpriteScale(scale);
        StartFrames(frames, loop: true, keepLastFrame: false, customFrameDuration: frameDuration);
    }

    private void PlayOnce(Sprite[] frames, bool keepLastFrame = false, float? customFrameDuration = null, OneShotAnim oneShot = OneShotAnim.None)
    {
        _currentLoop = LoopAnim.None;

        Vector2 scale = oneShot switch
        {
            OneShotAnim.Attack => attackSpriteScale,
            OneShotAnim.JumpAttack => jumpAttackSpriteScale,
            OneShotAnim.Jump => jumpSpriteScale,
            OneShotAnim.BackHop => backSpriteScale,
            OneShotAnim.Death => deathSpriteScale,
            _ => Vector2.one,
        };

        ApplySpriteScale(scale);
        StartFrames(frames, loop: false, keepLastFrame: keepLastFrame, customFrameDuration: customFrameDuration);
    }

    private void StartFrames(Sprite[] frames, bool loop, bool keepLastFrame, float? customFrameDuration)
    {
        if (spriteRenderer == null || frames == null || frames.Length == 0) return;

        StopCurrentAnimation();

        _animCo = FrameRoutine(frames, loop, keepLastFrame, customFrameDuration);
        StartCoroutine(_animCo);
    }

    private System.Collections.IEnumerator FrameRoutine(Sprite[] frames, bool loop, bool keepLastFrame, float? customFrameDuration)
    {
        float dur = Mathf.Max(0.01f, customFrameDuration ?? frameDuration);

        if (!loop)
        {
            _isActionLocked = true;
        }

        int i = 0;
        float t = 0f;

        while (true)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = frames[i];
            }

            t = 0f;
            while (t < dur)
            {
                t += Time.deltaTime;
                yield return null;
            }

            i++;
            if (i < frames.Length)
            {
                continue;
            }

            if (loop)
            {
                i = 0;
                continue;
            }

            if (keepLastFrame && spriteRenderer != null)
            {
                spriteRenderer.sprite = frames[frames.Length - 1];
            }

            if (_backHopCo != null)
            {
                _animCo = null;
                yield break;
            }

            if (_isDead)
            {
                _animCo = null;
                yield break;
            }

            ApplySpriteScale(idleSpriteScale);

            _isActionLocked = false;
            _animCo = null;
            yield break;
        }
    }

    private void ApplySpriteScale(Vector2 scale2)
    {
        if (spriteRenderer == null) return;

        float facingSign = Mathf.Sign(spriteRenderer.transform.localScale.x);
        if (facingSign == 0f) facingSign = 1f;

        scale2.x = Mathf.Max(0.01f, scale2.x);
        scale2.y = Mathf.Max(0.01f, scale2.y);

        var target = _spriteBaseLocalScale;
        target.x = Mathf.Abs(target.x) * facingSign * scale2.x;
        target.y = target.y * scale2.y;

        spriteRenderer.transform.localScale = target;
>>>>>>> bcafac6eed285f17d0f550baf192ebe6ea97997c
    }

    private bool IsGrounded()
    {
        Vector2 origin = (groundCheck != null)
            ? (Vector2)groundCheck.position
            : (Vector2)transform.position + Vector2.down * 0.1f;

        Collider2D hit = Physics2D.OverlapCircle(origin, groundCheckRadius, groundLayers);
        if (hit == null) return false;

        var myCol = GetComponent<Collider2D>();
        if (myCol != null && (hit == myCol || hit.transform.IsChildOf(transform)))
        {
            return false;
        }

        return true;
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

<<<<<<< HEAD
<<<<<<< Updated upstream
    // ｽｱﾍ貍ﾒﾖ蠻ｲﾌ衲・Tag=Enemy ｽﾓｴ･ﾊｱｿﾛ 1 ｵ翹愑・ｵ｣ｻｺﾔ PlayerAttack ﾗﾓﾅｲﾌ蟠･ｷ｢
=======
>>>>>>> Stashed changes
=======
>>>>>>> bcafac6eed285f17d0f550baf192ebe6ea97997c
    private void ApplyEnemyDamage(Collider2D col)
    {
        if (isDead) return;
        if (col == null) return;
        if (!col.CompareTag("Enemy")) return;

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< Updated upstream
        // ﾈ｡ﾍ貍ﾒﾖ蠻ｲﾌ螢ｨｹﾒﾔﾚﾍｬﾒｻ GameObject ﾉﾏｵﾄ Collider2D｣ｩ
        var bodyCollider = GetComponent<Collider2D>();
        if (bodyCollider == null) return;

        // ﾖｻﾓﾐﾖ蠻ｲﾌ衲・ﾐﾈﾋｷ｢ﾉ悅ﾓｴ･ﾊｱｲﾅｽ睛翹ﾋｺｦ
=======
        var bodyCollider = GetComponent<Collider2D>();
        if (bodyCollider == null) return;

>>>>>>> bcafac6eed285f17d0f550baf192ebe6ea97997c
        if (!bodyCollider.IsTouching(col)) return;
=======
        var body = GetComponent<Collider2D>();
        if (body == null) return;
        if (!body.IsTouching(col)) return;
>>>>>>> Stashed changes
=======
        if (bodyCollider == null)
        {
            bodyCollider = GetComponent<Collider2D>();
            if (bodyCollider == null) return;
        }

        if (attackHitbox != null && attackHitbox.IsTouching(col))
        {
            return;
        }

        if (!bodyCollider.IsTouching(col))
        {
            return;
        }
>>>>>>> 1f1912ee2bd7adf5a72e405c8011cda27e27f144

        var hp = GetComponent<PlayerHealth>();
        if (hp != null)
        {
            hp.TakeDamage(1);
        }
    }

<<<<<<< HEAD
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
=======
    [SerializeField, Header("Collision"), Tooltip("If true, ignore physical collisions between Player and Enemy colliders")]
    private bool ignoreEnemyPhysicalCollision = true;
<<<<<<< HEAD

    [SerializeField, Header("Back Hop Input"), Tooltip("Max time (seconds) between two A key presses to trigger back hop"), Min(0.01f)]
    private float doubleTapTime = 0.25f;

    private float _lastADownTime = -999f;

    [SerializeField, Tooltip("Retreat speed multiplier when holding A (0-1 = slower, >1 = faster)"), Min(0f)]
    private float retreatSpeedMultiplier = 0.7f;
>>>>>>> bcafac6eed285f17d0f550baf192ebe6ea97997c
=======
>>>>>>> 1f1912ee2bd7adf5a72e405c8011cda27e27f144
}