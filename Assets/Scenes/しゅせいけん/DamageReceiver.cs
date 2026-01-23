using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [Header("绑定目标（可选，不填则自动在父层查找）")]
    [SerializeField] private FishUFO fishUFO;

    [Header("通用击杀目标（可选）：发送 KillByDamage 消消息")]
    [SerializeField] private MonoBehaviour killTarget;

    private bool _dead;

    private void Awake()
    {
        if (fishUFO == null)
        {
            fishUFO = GetComponent<FishUFO>();
            if (fishUFO == null)
            {
                fishUFO = GetComponentInParent<FishUFO>();
            }
        }

        // 若未显式指定 killTarget，则默认使用自身或父级上的可用目标
        if (killTarget == null)
        {
            // 尝试绑定到自身或父级的任意脚本，以备用 SendMessage
            killTarget = GetComponent<MonoBehaviour>();
            if (killTarget == null)
            {
                killTarget = GetComponentInParent<MonoBehaviour>();
            }
        }
    }

    // 普通伤害：由具体目标定义死亡
    public void TakeDamage(int amount)
    {
        if (_dead) return;
        Kill();
    }

    // 绝对致命（例如DashAttack命中）
    public void TakeFatalDamage()
    {
        if (_dead) return;
        Kill();
    }

    private void Kill()
    {
        _dead = true;

        // FishUFO 特化：播放死亡动画
        if (fishUFO != null)
        {
            fishUFO.PlayDeathAnimation();
            return;
        }

        // 通用目标：发送 KillByDamage 消息（若存在）
        if (killTarget != null)
        {
            // 使用 SendMessage 调用目标上的 KillByDamage 方法（若实现）
            killTarget.SendMessage("KillByDamage", SendMessageOptions.DontRequireReceiver);
            return;
        }

        // 无绑定目标：直接销毁自身
        Destroy(gameObject);
    }
}