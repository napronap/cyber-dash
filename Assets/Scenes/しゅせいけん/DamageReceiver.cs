using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [Header("°ó¶¨Ä¿±ê£¨¿ÉÑ¡£¬²»Ìûğò×Ô¶¯ÔÚ¸¸²ã²éÕÒ£©")]
    [SerializeField] private FishUFO fishUFO;

    [Header("Í¨ÓÃ»÷É±Ä¿±ê£¨¿ÉÑ¡£©£º·¢ËÍ KillByDamage ÏûÏûÏ¢")]
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

        // ÈôÎ´ÏÔÊ½Ö¸¶¨ killTarget£¬ÔòÄ¬ÈÏÊ¹ÓÃ×ÔÉú×ò¸¸¼¶ÉÏµÄ¿ÉÓÃÄ¿±E
        if (killTarget == null)
        {
            // ³¢ÊÔ°ó¶¨µ½×ÔÉú×ò¸¸¼¶µÄÈÎÒâ½Å±¾£¬ÒÔ±¸ÓÃ SendMessage
            killTarget = GetComponent<MonoBehaviour>();
            if (killTarget == null)
            {
                killTarget = GetComponentInParent<MonoBehaviour>();
            }
        }
    }

    // ÆÕÍ¨ÉËº¦£ºÓÉ¾ßÌåÄ¿±E¨ÒåËÀÍE
    public void TakeDamage(int amount)
    {
        if (_dead) return;
        Kill();
    }

    // ¾ø¶ÔÖÂÃE¨ÀıÈçDashAttackÃEĞ£©
    public void TakeFatalDamage()
    {
        if (_dead) return;
        Kill();
    }

    private void Kill()
    {
        _dead = true;

        // FishUFO ÌØ»¯£º²¥·ÅËÀÍö¶¯»­
        if (fishUFO != null)
        {
            fishUFO.PlayDeathAnimation();
            return;
        }

        // Í¨ÓÃÄ¿±ê£º·¢ËÍ KillByDamage ÏûÏ¢£¨Èô´æÔÚ£©
        if (killTarget != null)
        {
            // Ê¹ÓÃ SendMessage µ÷ÓÃÄ¿±EÏµÄ KillByDamage ·½·¨£¨ÈôÊµÏÖ£©
            killTarget.SendMessage("KillByDamage", SendMessageOptions.DontRequireReceiver);
            return;
        }

        // ÎŞ°ó¶¨Ä¿±ê£ºÖ±½ÓÏú»Ù×ÔÉE
        Destroy(gameObject);
    }
}