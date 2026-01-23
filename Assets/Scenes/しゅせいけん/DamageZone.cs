using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField, Tooltip("普通命中造成的伤害")]
    private int normalDamage = 1;

    [Header("Player Detection")]
    [SerializeField, Tooltip("使用玩家Tag判定（默认 Player）")]
    private bool usePlayerTag = true;
    [SerializeField, Tooltip("玩家Tag名称")]
    private string playerTag = "Player";
    [SerializeField, Tooltip("不使用Tag时，使用该LayerMask判定玩家")]
    private LayerMask playerLayers = 0;

    private DamageReceiver receiver;

    void Start()
    {
        receiver = GetComponentInParent<DamageReceiver>();
        if (receiver == null)
            Debug.LogWarning("DamageZone: 父层未找到 DamageReceiver。", this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (receiver == null || other == null) return;

        // 仅玩家触发生效
        if (IsPlayer(other))
        {
            receiver.TakeDamage(normalDamage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (receiver == null || collision == null) return;
        var col = collision.collider;
        if (col == null) return;

        // 仅玩家实碰生效
        if (IsPlayer(col))
        {
            receiver.TakeDamage(normalDamage);
        }
    }

    private bool IsPlayer(Collider2D col)
    {
        if (usePlayerTag)
        {
            return col.CompareTag(playerTag);
        }
        else
        {
            int mask = 1 << col.gameObject.layer;
            return (playerLayers.value & mask) != 0;
        }
    }
}