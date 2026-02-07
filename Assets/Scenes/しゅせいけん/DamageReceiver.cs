using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [Header("��Ŀ�꣨��ѡ���������Զ��ڸ�����ң�")]
    [SerializeField] private FishUFO fishUFO;

    [Header("ͨ�û�ɱĿ�꣨��ѡ�������� KillByDamage ����Ϣ")]
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

        // ��δ��ʽָ�� killTarget����Ĭ��ʹ�������򸸼��ϵĿ���Ŀ��
        if (killTarget == null)
        {
            // ���԰󶨵������򸸼�������ű����Ա��� SendMessage
            killTarget = GetComponent<MonoBehaviour>();
            if (killTarget == null)
            {
                killTarget = GetComponentInParent<MonoBehaviour>();
            }
        }
    }

    // ��ͨ�˺����ɾ���Ŀ�궨������
    public void TakeDamage(int amount)
    {
        if (_dead) return;
        Kill();
    }

    // ��������������DashAttack���У�
    public void TakeFatalDamage()
    {
        if (_dead) return;
        Kill();
    }

    private void Kill()
    {
        _dead = true;

        // FishUFO �ػ���������������
        if (fishUFO != null)
        {
            fishUFO.PlayDeathAnimation();
            return;
        }

        // ͨ��Ŀ�꣺���� KillByDamage ��Ϣ�������ڣ�
        if (killTarget != null)
        {
            // ʹ�� SendMessage ����Ŀ���ϵ� KillByDamage ��������ʵ�֣�
            killTarget.SendMessage("KillByDamage", SendMessageOptions.DontRequireReceiver);
            return;
        }

        
        Destroy(gameObject);
    }
}
