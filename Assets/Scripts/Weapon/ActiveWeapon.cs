using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] private Sword sword;

    public Sword getActiveWeapon()
    {
        return sword;
    }

    
}
