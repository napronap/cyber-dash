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

    // ====== Swipe Attack ======
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
    [SerializeField, Tooltip("当たり判定時に与えるダメージ")]
    private int swipeDamage = 1;

    private bool _isSwiping;
    private float _weaponBaseZ;
    private bool _hasEnteredView;

    void Start()
    {
        SetMoveSpeed(speed);
        // 武器の基準角
        _weaponBaseZ = (weapon != null) ? GetLocalZ(weapon) : 0f;
        _hasEnteredView = false;
    }

    void FixedUpdate()
    {
        Move(Mathf.Clamp(initialDirection, -1f, 1f));
    }

    void Update()
    {
        // 画面外での自動破棄処理を削除しました
        // 必要に応じて他のロジックをここに追加できます

        // 画面入場検知（入場後のみ自動スワイプ）
        var cam = Camera.main;
        if (cam != null)
        {
            var vp = cam.WorldToViewportPoint(transform.position);
            bool inView = vp.z > 0f && vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f;
            if (inView) _hasEnteredView = true;
        }

        if (autoSwipe && _hasEnteredView && !_isSwiping)
        {
            TriggerSwipe();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsPlayerCollider(collision.collider))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayerCollider(other))
        {
            Destroy(gameObject);
        }
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

    // Swipe Attack 
    public void TriggerSwipe()
    {
        if (_isSwiping || weapon == null) return;
        _isSwiping = true;
        StartCoroutine(SwipeRoutine());
    }

    private System.Collections.IEnumerator SwipeRoutine()
    {
        float baseZ = _weaponBaseZ;

        // 予備動作：少し引く
        yield return RotateTo(weapon, baseZ - swipeAngle * 0.3f);

        // 本振り：途中でヒット適用
        yield return RotateWithHitTo(weapon, baseZ + swipeAngle);

        // 復帰
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
        if (weapon == null) return;

        // 武器に付与された Collider2D のみを使用（簡易円当たり判定は削除）
        if (weapon.TryGetComponent<Collider2D>(out var col))
        {
            var contactFilter = new ContactFilter2D();
            contactFilter.useLayerMask = playerLayers != 0;
            contactFilter.layerMask = playerLayers;
            contactFilter.useTriggers = true;

            Collider2D[] results = new Collider2D[8];
            int count = Physics2D.OverlapCollider(col, contactFilter, results);
            for (int i = 0; i < count; i++)
            {
                var hitCol = results[i];
                if (hitCol == null) continue;
                if (!string.IsNullOrEmpty(playerTag) && !hitCol.CompareTag(playerTag)) continue;

                // プレイヤー側にダメージ API がある場合はここで呼ぶ。
                // ここでは簡易的に破壊のみ（必要に応じて置き換え）
                Destroy(hitCol.gameObject);
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
