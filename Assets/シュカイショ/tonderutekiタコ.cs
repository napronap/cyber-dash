using UnityEngine;

public class tonderutekiタコ : enemyKaisho
{
    [SerializeField, Tooltip("移動速度（基底クラスへ適用）")]
    private float speed = 2f;

    [Header("Player 判定")]
    [SerializeField, Tooltip("プレイヤータグ")]
    private string playerTag = "Player";
    [SerializeField, Tooltip("プレイヤーレイヤー（0 の場合は無視）")]
    private LayerMask playerLayers = 0;

    [Header("初期方向 (-1=左, +1=右)")]
    [SerializeField] private float initialDirection = -1f;

    [Header("Swipe Attack")]
    [SerializeField, Tooltip("挥击用の武器 `Transform`（長方体の代替）")]
    private Transform weapon;
    [SerializeField, Tooltip("1回の振り幅（度）")]
    private float swipeAngle = 60f;
    [SerializeField, Tooltip("振りの速度（度/秒）")]
    private float swipeSpeed = 360f;
    [SerializeField, Tooltip("次の振りまでの待機時間（秒）")]
    private float swipeCooldown = 1.0f;
    [SerializeField, Tooltip("入場後に自動で定期的に振る")]
    private bool autoSwipe = true;

    // フレームアニメーション
    [Header("Frame Animation (画像を挿入して使用)")]
    [SerializeField, Tooltip("通常/攻撃時に再生するフレーム画像列")]
    private Sprite[] attackFrames;
    [SerializeField, Tooltip("死亡時に再生するフレーム画像列")]
    private Sprite[] deathFrames;
    [SerializeField, Tooltip("1フレームの表示時間（秒）")]
    private float frameDuration = 0.1f;
    [SerializeField, Tooltip("アニメ再生対象の `SpriteRenderer`")]
    private SpriteRenderer spriteRenderer;

    private bool _isSwiping;
    private float _weaponBaseZ;
    private bool _isDying;

    // フレーム再生内部状態
    private System.Collections.IEnumerator _currentAnimCo;
    private bool _isAttackLoopPlaying; // 攻撃アニメ常時ループ開始済み

    void Start()
    {
        SetMoveSpeed(speed);
        _weaponBaseZ = (weapon != null) ? GetLocalZ(weapon) : 0f;
        _isDying = false;
        _isAttackLoopPlaying = false;

        // 参照が未設定なら自動取得
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        // 初期表示
        if (spriteRenderer != null && attackFrames != null && attackFrames.Length > 0)
        {
            spriteRenderer.sprite = attackFrames[0];
        }
    }

    void FixedUpdate()
    {
        if (_isDying) return;
        Move(Mathf.Clamp(initialDirection, -1f, 1f));
    }

    void Update()
    {
        if (_isDying) return;

        // 画面に映ったら（SpriteRenderer が可視になったら）攻撃アニメをループ開始
        if (!_isAttackLoopPlaying
            && spriteRenderer != null
            && spriteRenderer.isVisible
            && attackFrames != null
            && attackFrames.Length > 0)
        {
            PlayFrames(attackFrames, loop: true);
            _isAttackLoopPlaying = true;
        }

        // スワイプ攻撃はアニメ開始後に回す
        if (autoSwipe && _isAttackLoopPlaying && !_isSwiping && weapon != null)
        {
            TriggerSwipe();
        }
    }

    // 仅在 DamageReceiver -> KillByDamage 调用时死亡
    public void KillByDamage()
    {
        HandleDeath();
    }

    // 以往对玩家碰撞即死亡的逻辑移除，避免非头部命中导致死亡
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 不再因与玩家碰撞自动死亡
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 不再因与玩家触发器接触自动死亡
    }

    private bool IsPlayerCollider(Collider2D col)
    {
        if (col == null) return false;
        if (!string.IsNullOrEmpty(playerTag) && col.CompareTag(playerTag)) return true;
        if (playerLayers != 0 && ((1 << col.gameObject.layer) & playerLayers) != 0) return true;
        return false;
    }

    public void SetInitialDirection(float dir)
    {
        initialDirection = Mathf.Clamp(dir, -1f, 1f);
    }

    // 攻撃（スワイプのみ。見た目のアニメは常時ループ）
    public void TriggerSwipe()
    {
        if (_isSwiping || weapon == null) return;
        _isSwiping = true;
        StartCoroutine(SwipeRoutine());
    }

    private System.Collections.IEnumerator SwipeRoutine()
    {
        float baseZ = _weaponBaseZ;
        yield return RotateTo(weapon, baseZ - swipeAngle * 0.3f);
        yield return RotateWithHitTo(weapon, baseZ + swipeAngle);
        yield return RotateTo(weapon, baseZ);
        yield return new WaitForSeconds(swipeCooldown);
        _isSwiping = false;
    }

    private System.Collections.IEnumerator RotateTo(Transform t, float targetZ)
    {
        float timeout = 2f;
        float endTime = Time.time + timeout;
        float current = GetLocalZ(t);

        while (!Mathf.Approximately(current, targetZ))
        {
            current = Mathf.MoveTowards(current, targetZ, swipeSpeed * Time.deltaTime);
            SetLocalZ(t, current);
            if (Time.time > endTime) break;
            yield return null;
        }
        SetLocalZ(t, targetZ);
    }

    private System.Collections.IEnumerator RotateWithHitTo(Transform t, float targetZ)
    {
        float current = GetLocalZ(t);
        float startZ = current;
        float total = Mathf.Abs(targetZ - startZ);
        bool hitApplied = false;

        float timeout = 2f;
        float endTime = Time.time + timeout;

        while (!Mathf.Approximately(current, targetZ))
        {
            current = Mathf.MoveTowards(current, targetZ, swipeSpeed * Time.deltaTime);
            SetLocalZ(t, current);

            float traveled = Mathf.Abs(current - startZ);
            float ratio = total > 0f ? traveled / total : 1f;

            if (!hitApplied && ratio > 0.35f && ratio < 0.75f)
            {
                ApplySwipeHit();
                hitApplied = true;
            }

            if (Time.time > endTime) break;
            yield return null;
        }
        SetLocalZ(t, targetZ);
    }

    private void ApplySwipeHit()
    {
        if (weapon == null || _isDying) return;

        if (weapon.TryGetComponent<Collider2D>(out var col))
        {
            var filter = new ContactFilter2D
            {
                useLayerMask = playerLayers != 0,
                layerMask = playerLayers,
                useTriggers = true
            };

            Collider2D[] results = new Collider2D[8];
            int count = Physics2D.OverlapCollider(col, filter, results);
            for (int i = 0; i < count; i++)
            {
                var hitCol = results[i];
                if (hitCol == null) continue;
                if (!string.IsNullOrEmpty(playerTag) && !hitCol.CompareTag(playerTag)) continue;

                Destroy(hitCol.gameObject);
            }
        }
    }

    // 死亡処理：アニメ再生後に破棄
    private void HandleDeath()
    {
        if (_isDying) return;
        _isDying = true;

        // 移動/攻撃/アニメの全コルーチン停止
        StopAllCoroutines();

        // 物理と当たり判定を停止
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

        // 武器角度を初期化
        if (weapon != null)
        {
            SetLocalZ(weapon, _weaponBaseZ);
        }

        // 死亡アニメがあれば再生、なければ即時破棄
        if (spriteRenderer != null && deathFrames != null && deathFrames.Length > 0)
        {
            StartCoroutine(DeathRoutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 死亡アニメを順再生してから破棄
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

    // フレーム再生の共通処理
    private void PlayFrames(Sprite[] frames, bool loop)
    {
        if (spriteRenderer == null || frames == null || frames.Length == 0)
            return;

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
            spriteRenderer.sprite = frames[index];
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
