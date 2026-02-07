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

    [Header("Jump Attack (J)")]
    [SerializeField, Tooltip("Jump attack upward force (initial Y velocity), Min(0.01f)")]
    private float jumpAttackUpForce = 6f;

    [SerializeField, Tooltip("Duration of each frame for jump attack animation (seconds); larger value = slower"), Min(0.01f)]
    private float jumpAttackFrameDuration = 0.08f;

    [SerializeField, Tooltip("Extra gravity scale while jump-attacking (bigger = faster falling)"), Min(1f)]
    private float jumpAttackGravityScale = 3f;

    [SerializeField, Header("Fall Tuning"), Tooltip("Gravity multiplier while falling (y < 0). Bigger = faster fall"), Min(1f)]
    private float fallGravityMultiplier = 2.5f;

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

    [SerializeField, Tooltip("Fall (loop)")]
    private Sprite[] fallFrames;

    [SerializeField, Tooltip("Attack (plays once)")]
    private Sprite[] attackFrames;

    [SerializeField, Tooltip("Jump Attack (plays once)")]
    private Sprite[] jumpAttackFrames;

    [Header("Frame Timing")]
    [SerializeField, Tooltip("Duration of each frame for normal actions (in seconds)")]
    private float frameDuration = 0.1f;

    [SerializeField, Tooltip("Duration of each frame for attacks (in seconds); smaller value = faster")]
    private float attackFrameDuration = 0.05f;

    [SerializeField, Tooltip("Duration of each frame for fall animation (in seconds)")]
    private float fallFrameDuration = 0.1f;

    [Header("Air State (No Velocity)")]
    [SerializeField, Tooltip("Tolerance for detecting no more upward movement while in air (world units)")]
    private float fallDetectEpsilon = 0.001f;

    [SerializeField, Tooltip("Velocity threshold to enter fall pose (y < -value). Bigger = later fall pose"), Min(0f)]
    private float fallEnterVelocity = 0.05f;

    [Header("Per-Animation Sprite Scale")]
    [SerializeField, Tooltip("Idle scale (X=width, Y=height)")]
    private Vector2 idleSpriteScale = Vector2.one;

    [SerializeField, Tooltip("Running scale (X=width, Y=height)")]
    private Vector2 runSpriteScale = Vector2.one;

    [SerializeField, Tooltip("Back hop scale (X=width, Y=height)")]
    private Vector2 backSpriteScale = Vector2.one;

    [SerializeField, Tooltip("Jump scale (X=width, Y=height)")]
    private Vector2 jumpSpriteScale = Vector2.one;

    [SerializeField, Tooltip("Fall scale (X=width, Y=height)")]
    private Vector2 fallSpriteScale = Vector2.one;

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

    private bool _isFallingPose;
    private bool _jumpedThisAir;
    private float _lastAirY;

    private Vector3 _spriteBaseLocalScale;
    private System.Collections.IEnumerator _backHopCo;

    private bool _isJumpAttackActive;
    private float _baseGravityScale;

    private enum LoopAnim
    {
        None,
        Idle,
        Run,
        Back,
        Fall
    }

    private enum OneShotAnim
    {
        None,
        Jump,
        Attack,
        JumpAttack,
        BackHop
    }

    private LoopAnim _currentLoop = LoopAnim.None;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

        ApplyFixedFacing();
        PlayLoop(LoopAnim.Idle);
    }

    void Update()
    {
        ReadMovementInput();

        UpdateGroundedState();

        if (isGrounded)
        {
            ResetAirStateOnGround();
        }

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
        {
            jumpPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            attackPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            TryJumpAttack();
        }

        if (!isGrounded && !_isActionLocked)
        {
            TryEnterFallingPose_ByVelocity();
        }

        UpdateAnimationState();
    }

    void FixedUpdate()
    {
        UpdateGroundedState();

        ApplyAirGravityTuning();

        if (_backHopCo == null)
        {
            ApplyHorizontalMovement();

            if (jumpPressed && isGrounded)
            {
                PerformJump();
            }

            rb.linearVelocity = rb.linearVelocity;
        }

        jumpPressed = false;

        if (!isGrounded && _jumpedThisAir)
        {
            _lastAirY = Mathf.Max(_lastAirY, transform.position.y);
        }
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
    }

    private void ResetAirStateOnGround()
    {
        _isFallingPose = false;
        _jumpedThisAir = false;

        if (_isJumpAttackActive && rb != null)
        {
            rb.gravityScale = _baseGravityScale;
            _isJumpAttackActive = false;
        }

        if (rb != null)
        {
            rb.gravityScale = _baseGravityScale;
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

        if (rb.linearVelocity.y < 0f)
        {
            targetGravity = Mathf.Max(targetGravity, _baseGravityScale * fallGravityMultiplier);
        }

        rb.gravityScale = targetGravity;
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
        v.y = jumpForce;
        rb.linearVelocity = v;

        _jumpedThisAir = true;
        _lastAirY = transform.position.y;

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
        if (_backHopCo != null) return;
        if (jumpAttackFrames == null || jumpAttackFrames.Length == 0) return;
        if (rb == null) return;

        StopCurrentAnimation();

        _isFallingPose = false;
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

    private bool TryEnterFallingPose_NoVelocity()
    {
        if (_isFallingPose) return true;
        if (fallFrames == null || fallFrames.Length == 0 || spriteRenderer == null) return false;

        float y = transform.position.y;
        if (y > _lastAirY + fallDetectEpsilon)
        {
            _lastAirY = y;
            return false;
        }

        _isFallingPose = true;

        StopCurrentAnimation();

        _currentLoop = LoopAnim.None;
        spriteRenderer.sprite = fallFrames[0];

        ApplySpriteScale(fallSpriteScale);

        return true;
    }

    private void TryEnterFallingPose_ByVelocity()
    {
        if (isGrounded) return;
        if (fallFrames == null || fallFrames.Length == 0 || spriteRenderer == null) return;
        if (rb == null) return;

        if (rb.linearVelocity.y >= -fallEnterVelocity)
        {
            return;
        }

        PlayLoop(LoopAnim.Fall);
    }

    private void EnsureAirAnim()
    {
        if (_isFallingPose) return;
        if (jumpFrames == null || jumpFrames.Length == 0) return;

        if (_currentLoop != LoopAnim.None)
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
            LoopAnim.Fall => fallFrames,
            _ => idleFrames,
        };

        Vector2 scale = loop switch
        {
            LoopAnim.Run => runSpriteScale,
            LoopAnim.Back => runSpriteScale,
            LoopAnim.Fall => fallSpriteScale,
            _ => idleSpriteScale,
        };

        float dur = loop switch
        {
            LoopAnim.Fall => fallFrameDuration,
            _ => frameDuration,
        };

        ApplySpriteScale(scale);
        StartFrames(frames, loop: true, keepLastFrame: false, customFrameDuration: dur);
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
            _ => Vector2.one,
        };

        ApplySpriteScale(scale);
        StartFrames(frames, loop: false, keepLastFrame: keepLastFrame, customFrameDuration: customFrameDuration);
    }

    private void StartFrames(Sprite[] frames, bool loop, bool keepLastFrame, float? customFrameDuration)
    {
        if (spriteRenderer == null || frames == null || frames.Length == 0) return;

        StopCurrentAnimation();

        _isFallingPose = false;

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
        while (true)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = frames[i];
            }

            yield return new WaitForSeconds(dur);

            i++;
            if (i >= frames.Length)
            {
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

                ApplySpriteScale(idleSpriteScale);

                _isActionLocked = false;
                _animCo = null;
                yield break;
            }
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

        var bodyCollider = GetComponent<Collider2D>();
        if (bodyCollider == null) return;

        if (!bodyCollider.IsTouching(col)) return;

        var hp = GetComponent<PlayerHealth>();
        if (hp != null)
        {
            hp.TakeDamage(1);
        }
    }

    [SerializeField, Header("Collision"), Tooltip("If true, ignore physical collisions between Player and Enemy colliders")]
    private bool ignoreEnemyPhysicalCollision = true;

    [SerializeField, Header("Back Hop Input"), Tooltip("Max time (seconds) between two A key presses to trigger back hop"), Min(0.01f)]
    private float doubleTapTime = 0.25f;

    private float _lastADownTime = -999f;

    [SerializeField, Tooltip("Retreat speed multiplier when holding A (0-1 = slower, >1 = faster)"), Min(0f)]
    private float retreatSpeedMultiplier = 0.7f;
}