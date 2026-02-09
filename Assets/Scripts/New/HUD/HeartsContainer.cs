using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeartsContainer : MonoBehaviour
{
    [SerializeField] private GameObject heartPrefab;

    private readonly List<Image> hearts = new();
    private int lastMaxHp = -1;

    private void Update()
    {
        var player = WorkingPlayerController.Instance;
        if (player == null) return;

        if (player.MaxHP != lastMaxHp)
        {
            BuildHearts(player.MaxHP);
            lastMaxHp = player.MaxHP;
        }

        int hp = player.CurrentHP;
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].enabled = i < hp;
        }
    }

    private void BuildHearts(int maxHp)
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        hearts.Clear();

        for (int i = 0; i < maxHp; i++)
        {
            GameObject heart = Instantiate(heartPrefab, transform);
            Image img = heart.GetComponent<Image>();
            hearts.Add(img);
        }
    }
}
