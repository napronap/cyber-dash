using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [Header("绑定目标（可选，不填则自动在父层查找）")]
    [SerializeField] private FishUFO fishUFO;

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
    }

    // 普通伤害：UFO设计为一次攻击即死亡
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

        // 调用UFO死亡帧动画
        if (fishUFO != null)
        {
            fishUFO.PlayDeathAnimation();
        }
        else
        {
            // 若未绑定到UFO，可直接销毁或扩展为其他敌人类型
            Destroy(gameObject);
        }
    }
}