using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class TESETO : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Tooltip("Movement speed on the X-axis"), Min(0.01f)]
    private float moveSpeed = 6.5f;

    [SerializeField, Tooltip("Jump force")]
    private float jumpForce = 6f;

    [SerializeField, Tooltip("Air control (0-1)")]
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

    [SerializeField, Header("Back Hop Input"), Tooltip("Cooldown time (seconds) for Back Hop (A)"), Min(0f)]
    private float backHopCooldown = 0.3f;

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
    [SerializeField, Tooltip("SpriteRenderer to be flipped for facing direction")]
    private SpriteRenderer spriteRenderer;

    [SerializeField, Tooltip("Fix facing direction: true=always right, false=always left")]
    private bool alwaysFaceRight = true;

    [Header("Ground Check")]
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

    [SerializeField, Tooltip("Running (loop)")]
    private Sprite[] runFrames;

    [SerializeField, Tooltip("Back hop frames (loop OK; will be used during back hop)")]
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

    private Rigidbody2D rb;
    private float inputX;
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

    private float _nextBackHopTime;
    private float _nextJumpAttackTime;

    private float _nextJumpTime;

    private bool _isDead;

    private enum LoopAnim
    {
        None,
        Idle,
        Run
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (bodyCollider == null)
        {
            bodyCollider = GetComponent<Collider2D>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

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

        _nextBackHopTime = Time.time;
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
            if (Time.time >= _nextBackHopTime)
            {
                TryBackHop();
                _nextBackHopTime = Time.time + Mathf.Max(0f, backHopCooldown);
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (Time.time >= _nextJumpTime)
            {
                jumpPressed = true;
                _nextJumpTime = Time.time + Mathf.Max(0f, jumpCooldown);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            attackPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            TryJumpAttack();
        }

        UpdateAnimationState();
    }

    void FixedUpdate()
    {
        if (_isDead)
        {
            return;
        }

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

            if (fallToGroundOnDeath)
            {
                rb.isKinematic = false;
                rb.gravityScale = Mathf.Max(_baseGravityScale, deathFallGravityScale);

                if (stopHorizontalOnDeath)
                {
                    v = rb.linearVelocity;
                    v.x = 0f;
                    rb.linearVelocity = v;
                }
            }
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

        float targetVx = inputX * moveSpeed;
        float control = isGrounded ? 1f : Mathf.Clamp01(airControl);

        Vector2 v = rb.linearVelocity;
        float accel = moveSpeed * 5f;
        v.x = Mathf.MoveTowards(v.x, targetVx, accel * control * Time.fixedDeltaTime);
        rb.linearVelocity = v;
    }

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
                    PlayLoop(LoopAnim.Run);
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
            _ => idleFrames,
        };

        Vector2 scale = loop switch
        {
            LoopAnim.Run => runSpriteScale,
            _ => idleSpriteScale,
        };

        ApplySpriteScale(scale);
        StartFrames(frames, loop: true, keepLastFrame: false, customFrameDuration: frameDuration, oneShot: OneShotAnim.None);
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
        StartFrames(frames, loop: false, keepLastFrame: keepLastFrame, customFrameDuration: customFrameDuration, oneShot: oneShot);
    }

    private void StartFrames(Sprite[] frames, bool loop, bool keepLastFrame, float? customFrameDuration, OneShotAnim oneShot)
    {
        if (spriteRenderer == null || frames == null || frames.Length == 0) return;

        StopCurrentAnimation();

        _animCo = FrameRoutine(frames, loop, keepLastFrame, customFrameDuration, oneShot);
        StartCoroutine(_animCo);
    }

    private System.Collections.IEnumerator FrameRoutine(Sprite[] frames, bool loop, bool keepLastFrame, float? customFrameDuration, OneShotAnim oneShot)
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

            if (oneShot == OneShotAnim.Death)
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
    }

    private bool IsGrounded()
    {
        Vector2 origin = (groundCheck != null)
            ? groundCheck.position
            : transform.position + Vector3.down * 0.1f;

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

    private void ApplyEnemyDamage(Collider2D col)
    {
        if (col == null) return;
        if (!col.CompareTag("Enemy")) return;

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

        var hp = GetComponent<PlayerHealth>();
        if (hp != null)
        {
            hp.TakeDamage(1);
        }
    }

    [SerializeField, Header("Collision"), Tooltip("If true, ignore physical collisions between Player and Enemy colliders")]
    private bool ignoreEnemyPhysicalCollision = true;
}