using UnityEngine;

// this script is probably unnecessary but I was too lazy to directly add a points field to each enemy script
public class EnemyScore : MonoBehaviour
{
    [SerializeField] private int points = 100;
    public int Points => points;
}
